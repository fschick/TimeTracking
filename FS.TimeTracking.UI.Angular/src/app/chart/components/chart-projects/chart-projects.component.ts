import {ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {Column, Configuration, DataCellTemplate, FooterCellTemplate} from '../../../shared/components/simple-table/simple-table.component';
import {ProjectChartService, ProjectWorkTimeDto} from '../../../shared/services/api';
import {Observable, Subscription} from 'rxjs';
import {Filter, FilteredRequestParams, FilterName} from '../../../shared/components/filter/filter.component';
import {ChartOptions, ChartService} from '../../services/chart.service';
import {ApexAxisChartSeries} from 'ng-apexcharts';
import {FormatService} from '../../../shared/services/format.service';
import {LocalizationService} from '../../../shared/services/internationalization/localization.service';
import {DateTime} from 'luxon';
import {single, switchMap} from 'rxjs/operators';
import {UtilityService} from '../../../shared/services/utility.service';
import {EntityService} from '../../../shared/services/state-management/entity.service';

@Component({
  selector: 'ts-chart-projects',
  templateUrl: './chart-projects.component.html',
  styleUrls: ['./chart-projects.component.scss']
})
export class ChartProjectsComponent implements OnInit, OnDestroy {
  @ViewChild('infoCellTemplate', {static: true}) private infoCellTemplate?: DataCellTemplate<ProjectWorkTimeDto>;
  @ViewChild('orderPeriodHeadTemplate', {static: true}) private orderPeriodHeadTemplate?: DataCellTemplate<ProjectWorkTimeDto>;
  @ViewChild('orderPeriodDataTemplate', {static: true}) private orderPeriodDataTemplate?: DataCellTemplate<ProjectWorkTimeDto>;
  @ViewChild('infoFooterTemplate', {static: true}) private infoFooterTemplate?: FooterCellTemplate<ProjectWorkTimeDto>;

  public filters: (Filter | FilterName)[];
  public chartOptions: ChartOptions;
  public chartSeries?: ApexAxisChartSeries;

  public tableConfiguration: Partial<Configuration<ProjectWorkTimeDto>>;
  public tableColumns?: Column<ProjectWorkTimeDto>[];
  public tableRows: ProjectWorkTimeDto[] = [];
  public tableFooter: Partial<ProjectWorkTimeDto> = {};
  private readonly subscriptions = new Subscription();

  public localizedDays = $localize`:@@Abbreviations.Days:[i18n] days`;
  public localizedHours = $localize`:@@Abbreviations.Hours:[i18n] h`;

  constructor(
    public formatService: FormatService,
    private utilityService: UtilityService,
    private projectChartService: ProjectChartService,
    private localizationService: LocalizationService,
    private chartService: ChartService,
    private changeDetector: ChangeDetectorRef,
    private entityService: EntityService,
  ) {
    const defaultStartDate = DateTime.now().startOf('year');
    const defaultEndDate = DateTime.now().endOf('year');

    this.filters = [
      {name: 'timeSheetStartDate', defaultValue: defaultStartDate},
      {name: 'timeSheetEndDate', defaultValue: defaultEndDate},
      {name: 'customerId'},
      {name: 'orderId'},
      {name: 'projectId'},
      {name: 'activityId'},
      {name: 'timeSheetIssue'},
      {name: 'timeSheetBillable', defaultValue: true},
    ];

    this.chartOptions = this.chartService.createChartOptions();
    this.tableConfiguration = this.createTableConfiguration();
  }

  public ngOnInit(): void {
    const filterChanged = this.entityService.filterChanged
      .pipe(switchMap(filter => this.loadData(filter)))
      .subscribe(rows => this.setTableData(rows));
    this.subscriptions.add(filterChanged);

    this.tableColumns = this.createTableColumns();
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  public tableRowsChanged(rows: Array<ProjectWorkTimeDto>): void {
    this.chartSeries = this.createSeries(rows);
    this.changeDetector.detectChanges();
  }

  private loadData(filter: FilteredRequestParams): Observable<ProjectWorkTimeDto[]> {
    return this.projectChartService.getWorkTimesPerProject(filter).pipe(single());
  }

  private setTableData(rows: ProjectWorkTimeDto[]) {
    this.tableRows = rows;
    this.tableFooter = {
      daysWorked: this.utilityService.sum(rows.map(row => row.daysWorked)),
      timeWorked: this.utilityService.durationSum(rows.map(row => row.timeWorked)),
      budgetWorked: this.utilityService.sum(rows.map(row => row.budgetWorked)),
      totalWorkedPercentage: 1,
      currency: rows[0]?.currency,
    };
  }

  private createSeries(workTimes: ProjectWorkTimeDto[]): ApexAxisChartSeries {
    return [
      {
        name: $localize`:@@Page.Chart.Common.Worked:[i18n] Worked`,
        data: workTimes.map(workTime => ({
          x: workTime.projectTitle,
          y: workTime.daysWorked,
          meta: {time: workTime.timeWorked}
        }))
      }
    ];
  }

  private createTableConfiguration(): Partial<Configuration<ProjectWorkTimeDto>> {
    return {
      cssWrapper: 'table-responsive',
      cssTable: 'table table-card table-sm align-middle text-break border',
      glyphSortAsc: '',
      glyphSortDesc: '',
      locale: this.localizationService.language,
      cssFooterRow: 'text-strong',
    };
  }

  private createTableColumns(): Column<ProjectWorkTimeDto>[] {
    const cssHeadCell = 'border-0 text-nowrap';
    const cssHeadCellMd = 'd-none d-md-table-cell';
    const cssDataCellMd = cssHeadCellMd;

    return [
      {
        title: $localize`:@@DTO.WorkTimeDto.ProjectTitle:[i18n] Project`,
        prop: 'projectTitle',
        cssHeadCell: cssHeadCell,
        footer: $localize`:@@Common.Summary:[i18n] Summary`,
      }, {
        title: $localize`:@@DTO.WorkTimeDto.CustomerTitle:[i18n] Customer`,
        prop: 'customerTitle',
        cssHeadCell: cssHeadCellMd,
        cssDataCell: cssDataCellMd,
      }, {
        title: $localize`:@@Page.Chart.Common.Worked:[i18n] Worked`,
        prop: 'daysWorked',
        cssHeadCell: `${cssHeadCell} text-nowrap text-end`,
        cssDataCell: 'text-nowrap text-end',
        cssFooterCell: 'text-nowrap text-end',
        format: row => `${this.formatService.formatDays(row.daysWorked)} ${this.localizedDays}`,
        footer: () => `${this.formatService.formatDays(this.tableFooter.daysWorked)} ${this.localizedDays}`,
      }, {
        title: $localize`:@@Page.Chart.Common.TotalWorkedPercentage:[i18n] worked %`,
        prop: 'totalWorkedPercentage',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd} text-end`,
        cssDataCell: `${cssDataCellMd} text-nowrap text-end`,
        cssFooterCell: `${cssDataCellMd} text-nowrap text-end`,
        format: row => `${this.formatService.formatRatio(row.totalWorkedPercentage)} %`,
        footer: () => `${this.formatService.formatRatio(this.tableFooter.totalWorkedPercentage)} %`,
      }, {
        title: $localize`:@@Common.Details:[i18n] Details`,
        customId: 'info',
        cssHeadCell: `${cssHeadCell} ps-3 text-center`,
        cssDataCell: 'ps-3 text-center',
        cssFooterCell: 'ps-3 text-center',
        dataCellTemplate: this.infoCellTemplate,
        footerCellTemplate: this.infoFooterTemplate,
        sortable: false,
        width: '1%',
      }
    ];
  }
}
