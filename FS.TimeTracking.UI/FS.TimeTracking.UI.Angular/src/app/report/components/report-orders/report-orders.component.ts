import {ChangeDetectorRef, Component, OnDestroy, OnInit, TemplateRef, ViewChild} from '@angular/core';
import {Observable, Subject, Subscription} from 'rxjs';
import {ReportService, WorkTimeDto} from '../../../shared/services/api';
import {single, switchMap} from 'rxjs/operators';
import {Column, Configuration, DataCellTemplate} from '../../../shared/components/simple-table/simple-table.component';
import {LocalizationService} from '../../../shared/services/internationalization/localization.service';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {FormatService} from '../../../shared/services/format.service';
import {Filter, FilteredRequestParams, FilterName} from '../../../shared/components/filter/filter.component';
import {ApexAxisChartSeries,} from "ng-apexcharts";
import {DateTime} from 'luxon';
import {ChartOptions, ReportChartService} from '../../services/report-chart.service';

@Component({
  selector: 'ts-report-orders',
  templateUrl: './report-orders.component.html',
  styleUrls: ['./report-orders.component.scss']
})
export class ReportOrdersComponent implements OnInit, OnDestroy {
  @ViewChild('infoCellTemplate', {static: true}) private infoCellTemplate?: DataCellTemplate<WorkTimeDto>;
  @ViewChild('orderPeriodHeadTemplate', {static: true}) private orderPeriodHeadTemplate?: DataCellTemplate<WorkTimeDto>;
  @ViewChild('orderPeriodDataTemplate', {static: true}) private orderPeriodDataTemplate?: DataCellTemplate<WorkTimeDto>;

  public filterChanged = new Subject<FilteredRequestParams>();
  public filters: (Filter | FilterName)[];
  public chartOptions: ChartOptions;
  public chartSeries?: ApexAxisChartSeries;
  public plannedArePartial: boolean = false;

  public tableConfiguration: Partial<Configuration<WorkTimeDto>>;
  public tableColumns?: Column<WorkTimeDto>[];
  public tableRows: WorkTimeDto[] = [];
  private readonly subscriptions = new Subscription();

  public localizedDays = $localize`:@@Abbreviations.Days:[i18n] days`;
  public localizedHours = $localize`:@@Abbreviations.Hours:[i18n] h`;

  constructor(
    public formatService: FormatService,
    private reportService: ReportService,
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
      {name: 'activityId'},
      {name: 'orderId'},
      {name: 'timeSheetIssue'},
      {name: 'timeSheetComment'}
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

  public tableRowsChanged(rows: Array<WorkTimeDto>): void {
    this.chartSeries = this.createSeries(rows);
    this.plannedArePartial = rows.some(r => r.plannedIsPartial);
    this.changeDetector.detectChanges();
  }

  public openInfoDetail(infoDetailDialog: TemplateRef<any>) {
    this.modalService.open(infoDetailDialog, {
      centered: true,
      size: 'lg',
    });
  }

  private loadData(filter: FilteredRequestParams): Observable<WorkTimeDto[]> {
    return this.reportService.getWorkTimesPerOrder(filter).pipe(single());
  }

  private createSeries(workTimes: WorkTimeDto[]): ApexAxisChartSeries {
    return [
      {
        name: $localize`:@@Page.Report.Common.Worked:[i18n] Worked`,
        data: workTimes.map(workTime => ({
          x: workTime.orderTitle,
          y: workTime.daysWorked,
          meta: {time: workTime.timeWorked}
        }))
      },
      {
        name: $localize`:@@Page.Report.Common.Planned:[i18n] Planned`,
        data: workTimes.map(workTime => ({
          x: workTime.orderTitle,
          y: workTime.daysPlanned,
          meta: {time: workTime.timePlanned}
        }))
      },
    ];
  }

  private createTableConfiguration(): Partial<Configuration<WorkTimeDto>> {
    return {
      cssWrapper: 'table-responsive',
      cssTable: 'table table-card table-sm align-middle text-break border',
      glyphSortAsc: '',
      glyphSortDesc: '',
      locale: this.localizationService.language,
    };
  }

  private createTableColumns(): Column<WorkTimeDto>[] {
    const cssHeadCell = 'border-0 text-nowrap';
    const cssHeadCellMd = 'd-none d-md-table-cell';
    const cssDataCellMd = cssHeadCellMd;

    return [
      {
        title: $localize`:@@DTO.WorkTimeDto.OrderTitle:[i18n] Order`,
        prop: 'orderTitle',
        cssHeadCell: cssHeadCell,
      }, {
        title: $localize`:@@DTO.WorkTimeDto.OrderPeriod:[i18n] Order period`,
        cssHeadCell: `${cssHeadCell}`,
        prop: 'plannedStart',
        headCellTemplate: this.orderPeriodHeadTemplate,
        dataCellTemplate: this.orderPeriodDataTemplate,
      }, {
        title: $localize`:@@Page.Report.Common.Worked:[i18n] Worked`,
        prop: 'daysWorked',
        cssHeadCell: `${cssHeadCell} text-nowrap text-end`,
        cssDataCell: 'text-nowrap text-end',
        format: row => `${this.formatService.formatDays(row.daysWorked)} ${this.localizedDays}`,
      }, {
        title: $localize`:@@Page.Report.Common.Planned:[i18n] Planned`,
        prop: 'daysPlanned',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd} text-end`,
        cssDataCell: `${cssDataCellMd} text-nowrap text-end`,
        format: row => `${this.formatService.formatDays(row.daysPlanned)} ${this.localizedDays}`,
      }, {
        title: $localize`:@@Page.Report.Common.Done:[i18n] Done`,
        prop: 'ratioFinished',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd} text-end`,
        cssDataCell: `${cssDataCellMd} text-nowrap text-end`,
        format: row => `${this.formatService.formatRatio(row.ratioFinished)} %`,
      }, {
        title: $localize`:@@Page.Report.Common.Remain:[i18n] Remain`,
        prop: 'daysDifference',
        cssHeadCell: `${cssHeadCell} text-end`,
        cssDataCell: 'text-nowrap text-end',
        format: row => `${this.formatService.formatDays(row.daysDifference)} ${this.localizedDays}`,
      }, {
        title: $localize`:@@Page.Report.Common.Ratio:[i18n] Ratio`,
        prop: 'ratioTotalPlanned',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd} text-end`,
        cssDataCell: `${cssDataCellMd} text-nowrap text-end`,
        format: row => `${this.formatService.formatRatio(row.ratioTotalPlanned)} %`,
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
