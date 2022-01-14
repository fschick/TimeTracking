import {ChangeDetectorRef, Component, OnDestroy, OnInit, TemplateRef, ViewChild} from '@angular/core';
import {Column, Configuration, DataCellTemplate} from '../../../shared/components/simple-table/simple-table.component';
import {IssueReportService, IssueWorkTimeDto} from '../../../shared/services/api';
import {Observable, Subject, Subscription} from 'rxjs';
import {Filter, FilteredRequestParams, FilterName} from '../../../shared/components/filter/filter.component';
import {ChartOptions, ReportChartService} from '../../services/report-chart.service';
import {ApexAxisChartSeries} from 'ng-apexcharts';
import {FormatService} from '../../../shared/services/format.service';
import {LocalizationService} from '../../../shared/services/internationalization/localization.service';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {DateTime} from 'luxon';
import {single, switchMap} from 'rxjs/operators';

@Component({
  selector: 'ts-report-issues',
  templateUrl: './report-issues.component.html',
  styleUrls: ['./report-issues.component.scss']
})
export class ReportIssuesComponent implements OnInit, OnDestroy {
  @ViewChild('infoCellTemplate', {static: true}) private infoCellTemplate?: DataCellTemplate<IssueWorkTimeDto>;
  @ViewChild('orderPeriodHeadTemplate', {static: true}) private orderPeriodHeadTemplate?: DataCellTemplate<IssueWorkTimeDto>;
  @ViewChild('orderPeriodDataTemplate', {static: true}) private orderPeriodDataTemplate?: DataCellTemplate<IssueWorkTimeDto>;

  public filterChanged = new Subject<FilteredRequestParams>();
  public filters: (Filter | FilterName)[];
  public chartOptions: ChartOptions;
  public chartSeries?: ApexAxisChartSeries;

  public tableConfiguration: Partial<Configuration<IssueWorkTimeDto>>;
  public tableColumns?: Column<IssueWorkTimeDto>[];
  public tableRows: IssueWorkTimeDto[] = [];
  private readonly subscriptions = new Subscription();

  public localizedDays = $localize`:@@Abbreviations.Days:[i18n] days`;
  public localizedHours = $localize`:@@Abbreviations.Hours:[i18n] h`;

  constructor(
    public formatService: FormatService,
    private reportService: IssueReportService,
    private localizationService: LocalizationService,
    private modalService: NgbModal,
    private reportChartService: ReportChartService,
    private changeDetector: ChangeDetectorRef,
  ) {
    const defaultStartDate = DateTime.now().startOf('year');
    const defaultEndDate = DateTime.now().endOf('year');

    this.filters = [
      {name: 'timeSheetStartDate', resettable: false, defaultValue: defaultStartDate},
      {name: 'timeSheetEndDate', resettable: false, defaultValue: defaultEndDate},
      {name: 'timeSheetBillable', defaultValue: true},
      {name: 'customerId'},
      {name: 'projectId'},
      {name: 'activityId'},
      {name: 'orderId'},
      {name: 'timeSheetIssue'}
    ];

    const filterChanged = this.filterChanged
      .pipe(switchMap(filter => this.loadData(filter)))
      .subscribe(x => this.tableRows = x);

    this.subscriptions.add(filterChanged);

    this.chartOptions = this.reportChartService.createChartOptions();
    this.tableConfiguration = this.createTableConfiguration();
  }

  public ngOnInit(): void {
    this.tableColumns = this.createTableColumns();
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  public tableRowsChanged(rows: Array<IssueWorkTimeDto>): void {
    this.chartSeries = this.createSeries(rows);
    this.changeDetector.detectChanges();
  }

  public openInfoDetail(infoDetailDialog: TemplateRef<any>) {
    this.modalService.open(infoDetailDialog, {
      centered: true,
      size: 'lg',
    });
  }

  private loadData(filter: FilteredRequestParams): Observable<IssueWorkTimeDto[]> {
    return this.reportService.getWorkTimesPerIssue(filter).pipe(single());
  }

  private createSeries(workTimes: IssueWorkTimeDto[]): ApexAxisChartSeries {
    return [
      {
        name: $localize`:@@Page.Report.Common.Worked:[i18n] Worked`,
        data: workTimes.map(workTime => ({
          x: workTime.issue,
          y: workTime.daysWorked,
          meta: {time: workTime.timeWorked}
        }))
      }
    ];
  }

  private createTableConfiguration(): Partial<Configuration<IssueWorkTimeDto>> {
    return {
      cssWrapper: 'table-responsive',
      cssTable: 'table table-card table-sm align-middle text-break border',
      glyphSortAsc: '',
      glyphSortDesc: '',
      locale: this.localizationService.language,
    };
  }

  private createTableColumns(): Column<IssueWorkTimeDto>[] {
    const cssHeadCell = 'border-0 text-nowrap';
    const cssHeadCellMd = 'd-none d-md-table-cell';
    const cssDataCellMd = cssHeadCellMd;

    return [
      {
        title: $localize`:@@DTO.WorkTimeDto.Issue:[i18n] Issue`,
        prop: 'issue',
        cssHeadCell: cssHeadCell,
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
        format: row => `${this.formatService.formatDays(row.daysWorked)} ${this.localizedDays}`,
      }, {
        title: $localize`:@@Page.Report.Common.Ratio:[i18n] %`,
        prop: 'ratioTotalWorked',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd} text-end`,
        cssDataCell: `${cssDataCellMd} text-nowrap text-end`,
        format: row => `${this.formatService.formatRatio(row.ratioTotalWorked)} %`,
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
