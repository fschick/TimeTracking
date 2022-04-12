import {ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {Column, Configuration, DataCellTemplate, FooterCellTemplate} from '../../../shared/components/simple-table/simple-table.component';
import {ActivityChartService, ActivityWorkTimeDto} from '../../../shared/services/api';
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
  selector: 'ts-chart-activities',
  templateUrl: './chart-activities.component.html',
  styleUrls: ['./chart-activities.component.scss']
})
export class ChartActivitiesComponent implements OnInit, OnDestroy {
  @ViewChild('infoCellTemplate', {static: true}) private infoCellTemplate?: DataCellTemplate<ActivityWorkTimeDto>;
  @ViewChild('orderPeriodHeadTemplate', {static: true}) private orderPeriodHeadTemplate?: DataCellTemplate<ActivityWorkTimeDto>;
  @ViewChild('orderPeriodDataTemplate', {static: true}) private orderPeriodDataTemplate?: DataCellTemplate<ActivityWorkTimeDto>;
  @ViewChild('infoFooterTemplate', {static: true}) private infoFooterTemplate?: FooterCellTemplate<ActivityWorkTimeDto>;

  public filters: (Filter | FilterName)[];
  public chartOptions: ChartOptions;
  public chartSeries?: ApexAxisChartSeries;

  public tableConfiguration: Partial<Configuration<ActivityWorkTimeDto>>;
  public tableColumns?: Column<ActivityWorkTimeDto>[];
  public tableRows: ActivityWorkTimeDto[] = [];
  public tableFooter: Partial<ActivityWorkTimeDto> = {};
  private readonly subscriptions = new Subscription();

  public localizedDays = $localize`:@@Abbreviations.Days:[i18n] days`;
  public localizedHours = $localize`:@@Abbreviations.Hours:[i18n] h`;

  constructor(
    public formatService: FormatService,
    private utilityService: UtilityService,
    private activityChartService: ActivityChartService,
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

  public tableRowsChanged(rows: Array<ActivityWorkTimeDto>): void {
    this.chartSeries = this.createSeries(rows);
    this.changeDetector.detectChanges();
  }

  private loadData(filter: FilteredRequestParams): Observable<ActivityWorkTimeDto[]> {
    return this.activityChartService.getWorkTimesPerActivity(filter).pipe(single());
  }

  private setTableData(rows: ActivityWorkTimeDto[]) {
    this.tableRows = rows;
    this.tableFooter = {
      daysWorked: this.utilityService.sum(rows.map(row => row.daysWorked)),
      timeWorked: this.utilityService.durationSum(rows.map(row => row.timeWorked)),
      budgetWorked: this.utilityService.sum(rows.map(row => row.budgetWorked)),
      totalWorkedPercentage: 1,
      currency: rows[0]?.currency,
    };
  }

  private createSeries(workTimes: ActivityWorkTimeDto[]): ApexAxisChartSeries {
    return [
      {
        name: $localize`:@@Page.Chart.Common.Worked:[i18n] Worked`,
        data: workTimes.map(workTime => ({
          x: workTime.activityTitle,
          y: workTime.daysWorked,
          meta: {time: workTime.timeWorked}
        }))
      }
    ];
  }

  private createTableConfiguration(): Partial<Configuration<ActivityWorkTimeDto>> {
    return {
      cssWrapper: 'table-responsive',
      cssTable: 'table table-card table-sm align-middle text-break border',
      glyphSortAsc: '',
      glyphSortDesc: '',
      locale: this.localizationService.language,
      cssFooterRow: 'text-strong',
    };
  }

  private createTableColumns(): Column<ActivityWorkTimeDto>[] {
    const cssHeadCell = 'border-0 text-nowrap';
    const cssHeadCellMd = 'd-none d-md-table-cell';
    const cssDataCellMd = cssHeadCellMd;

    return [
      {
        title: $localize`:@@DTO.WorkTimeDto.ActivityTitle:[i18n] Activity`,
        prop: 'activityTitle',
        cssHeadCell: cssHeadCell,
        footer: $localize`:@@Common.Summary:[i18n] Summary`,
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
