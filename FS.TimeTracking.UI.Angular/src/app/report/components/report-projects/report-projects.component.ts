import {ChangeDetectorRef, Component, OnDestroy, OnInit, TemplateRef, ViewChild} from '@angular/core';
import {Column, Configuration, DataCellTemplate} from '../../../shared/components/simple-table/simple-table.component';
import {ProjectReportService, ProjectWorkTimeDto} from '../../../shared/services/api';
import {Observable, Subject, Subscription} from 'rxjs';
import {Filter, FilteredRequestParams, FilterName} from '../../../shared/components/filter/filter.component';
import {ChartOptions, ReportChartService} from '../../services/report-chart.service';
import {ApexAxisChartSeries} from 'ng-apexcharts';
import {FormatService} from '../../../shared/services/format.service';
import {LocalizationService} from '../../../shared/services/internationalization/localization.service';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {DateTime} from 'luxon';
import {single, switchMap} from 'rxjs/operators';
import {UtilityService} from '../../../shared/services/utility.service';

@Component({
  selector: 'ts-report-projects',
  templateUrl: './report-projects.component.html',
  styleUrls: ['./report-projects.component.scss']
})
export class ReportProjectsComponent implements OnInit, OnDestroy {
  @ViewChild('infoCellTemplate', {static: true}) private infoCellTemplate?: DataCellTemplate<ProjectWorkTimeDto>;
  @ViewChild('orderPeriodHeadTemplate', {static: true}) private orderPeriodHeadTemplate?: DataCellTemplate<ProjectWorkTimeDto>;
  @ViewChild('orderPeriodDataTemplate', {static: true}) private orderPeriodDataTemplate?: DataCellTemplate<ProjectWorkTimeDto>;

  public filterChanged = new Subject<FilteredRequestParams>();
  public filters: (Filter | FilterName)[];
  public chartOptions: ChartOptions;
  public chartSeries?: ApexAxisChartSeries;

  public tableConfiguration: Partial<Configuration<ProjectWorkTimeDto>>;
  public tableColumns?: Column<ProjectWorkTimeDto>[];
  public tableRows: ProjectWorkTimeDto[] = [];
  private readonly subscriptions = new Subscription();

  public localizedDays = $localize`:@@Abbreviations.Days:[i18n] days`;
  public localizedHours = $localize`:@@Abbreviations.Hours:[i18n] h`;

  constructor(
    public formatService: FormatService,
    private utilityService: UtilityService,
    private reportService: ProjectReportService,
    private localizationService: LocalizationService,
    private modalService: NgbModal,
    private reportChartService: ReportChartService,
    private changeDetector: ChangeDetectorRef,
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

    this.chartOptions = this.reportChartService.createChartOptions();
    this.tableConfiguration = this.createTableConfiguration();
  }

  public ngOnInit(): void {
    const filterChanged = this.filterChanged
      .pipe(switchMap(filter => this.loadData(filter)))
      .subscribe(x => this.tableRows = x);
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

  public openInfoDetail(infoDetailDialog: TemplateRef<any>) {
    this.modalService.open(infoDetailDialog, {
      centered: true,
      size: 'lg',
    });
  }

  private loadData(filter: FilteredRequestParams): Observable<ProjectWorkTimeDto[]> {
    return this.reportService.getWorkTimesPerProject(filter).pipe(single());
  }

  private createSeries(workTimes: ProjectWorkTimeDto[]): ApexAxisChartSeries {
    return [
      {
        name: $localize`:@@Page.Report.Common.Worked:[i18n] Worked`,
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
        footer: $localize`:@@Common.Sum:[i18n] Sum`,
      }, {
        title: $localize`:@@DTO.WorkTimeDto.CustomerTitle:[i18n] Customer`,
        prop: 'customerTitle',
        cssHeadCell: cssHeadCellMd,
        cssDataCell: cssDataCellMd,
      }, {
        title: $localize`:@@Page.Report.Common.Worked:[i18n] Worked`,
        prop: 'daysWorked',
        cssHeadCell: `${cssHeadCell} text-nowrap text-end`,
        cssDataCell: 'text-nowrap text-end',
        cssFooterCell: 'text-nowrap text-end',
        format: row => `${this.formatService.formatDays(row.daysWorked)} ${this.localizedDays}`,
        footer: () => `${this.formatService.formatDays(this.utilityService.sum(this.tableRows.map(row => row.daysWorked)))} ${this.localizedDays}`,
      }, {
        title: $localize`:@@Page.Report.Common.Ratio:[i18n] %`,
        prop: 'ratioTotalWorked',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd} text-end`,
        cssDataCell: `${cssDataCellMd} text-nowrap text-end`,
        cssFooterCell: `${cssDataCellMd} text-nowrap text-end`,
        format: row => `${this.formatService.formatRatio(row.ratioTotalWorked)} %`,
        footer: '100 %',
      }, {
        title: $localize`:@@Common.Details:[i18n] Details`,
        customId: 'info',
        cssHeadCell: `${cssHeadCell} text-end`,
        cssDataCell: 'text-end',
        dataCellTemplate: this.infoCellTemplate,
        sortable: false
      }
    ];
  }
}
