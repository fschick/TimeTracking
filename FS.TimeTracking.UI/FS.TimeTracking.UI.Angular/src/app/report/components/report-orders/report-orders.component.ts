import {Component, OnInit, TemplateRef, ViewChild} from '@angular/core';
import {BehaviorSubject, Observable, share, Subject} from 'rxjs';
import {ReportService, WorkTimeDto} from '../../../shared/services/api';
import {map, single, switchMap} from 'rxjs/operators';
import {Column, Configuration, DataCellTemplate} from '../../../shared/components/simple-table/simple-table.component';
import {LocalizationService} from '../../../shared/services/internationalization/localization.service';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {FormatService} from '../../../shared/services/format.service';
import {Filter, FilteredRequestParams, FilterName} from '../../../shared/components/filter/filter.component';
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexDataLabels,
  ApexPlotOptions,
  ApexXAxis,
  ApexYAxis,
  ApexStroke,
  ApexTooltip,
  ApexStates,
  ApexLegend
} from "ng-apexcharts";

type ChartOptions = {
  chart: ApexChart;
  colors: string[],
  dataLabels: ApexDataLabels;
  plotOptions: ApexPlotOptions;
  xAxis: ApexXAxis;
  yAxis: ApexYAxis;
  tooltip: ApexTooltip;
  stroke: ApexStroke;
  states: ApexStates;
  legend: ApexLegend;
};

@Component({
  selector: 'ts-report-orders',
  templateUrl: './report-orders.component.html',
  styleUrls: ['./report-orders.component.scss']
})
export class ReportOrdersComponent implements OnInit {
  @ViewChild('infoCellTemplate', {static: true}) private infoCellTemplate?: DataCellTemplate<WorkTimeDto>;
  @ViewChild('orderPeriodHeadTemplate', {static: true}) private orderPeriodHeadTemplate?: DataCellTemplate<WorkTimeDto>;
  @ViewChild('orderPeriodDataTemplate', {static: true}) private orderPeriodDataTemplate?: DataCellTemplate<WorkTimeDto>;

  public filterChanged = new Subject<FilteredRequestParams>();
  public filters: (Filter | FilterName)[];
  public chartOptions: ChartOptions;
  public chartSeries$: Observable<ApexAxisChartSeries>;
  public plannedArePartial$: Observable<boolean>;

  public tableConfiguration: Partial<Configuration<WorkTimeDto>>;
  public tableColumns?: Column<WorkTimeDto>[];
  public tableRows$: Observable<WorkTimeDto[]>;
  private tableSortedRows$: BehaviorSubject<WorkTimeDto[]>;

  public localizedDays = $localize`:@@Abbreviations.Days:[i18n] days`;
  public localizedHours = $localize`:@@Abbreviations.Hours:[i18n] h`;

  constructor(
    public formatService: FormatService,
    private reportService: ReportService,
    private localizationService: LocalizationService,
    private modalService: NgbModal,
  ) {
    this.filters = [
      {name: 'timeSheetStartDate'},
      {name: 'timeSheetEndDate'},
      {name: 'customerId'},
      {name: 'projectId'},
      {name: 'activityId'},
      {name: 'orderId'},
      {name: 'timeSheetIssue'},
      {name: 'timeSheetComment'}
    ];

    this.tableRows$ = this.filterChanged
      .pipe(
        switchMap(filter => this.loadData(filter)),
        share()
      );

    this.tableSortedRows$ = new BehaviorSubject<WorkTimeDto[]>([]);

    this.plannedArePartial$ = this.tableSortedRows$
      .pipe(
        map(rows => rows.some(r => r.plannedIsPartial))
      );

    this.chartOptions = this.createChartOptions();
    this.chartSeries$ = this.tableSortedRows$
      .pipe(
        map((workTimes: WorkTimeDto[]) => this.createSeries(workTimes))
      );

    this.tableConfiguration = this.createTableConfiguration();
  }

  public ngOnInit(): void {
    this.tableColumns = this.createTableColumns();
  }

  public tableRowsChanged(rows: Array<WorkTimeDto>): void {
    this.tableSortedRows$.next(rows);
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

  private createChartOptions(): ChartOptions {
    return {
      chart: {
        type: 'bar',
        height: 350
      },
      legend: {
        position: 'top',
      },
      xAxis: {
        crosshairs: {
          fill: {
            type: 'solid',
            color: '#F2F2F2'
          },
        },
      },
      yAxis: {
        title: {
          text: $localize`:@@Page.Report.Common.Days:[i18n] Days`,
        },
        labels: {
          formatter: (value: number) => this.formatService.formatDays(value)
        }
      },
      colors: ['#0D3B66', '#93B7BE'],
      plotOptions: {
        bar: {
          dataLabels: {},
        }
      },
      dataLabels: {
        enabled: true,
        formatter: (value: number) => this.formatService.formatDays(value)
      },
      stroke: {
        show: true,
        width: 10,
        colors: ['transparent']
      },
      tooltip: {
        followCursor: true,
        shared: true,
        intersect: false,
        onDatasetHover: {highlightDataSeries: false},
        y: {
          formatter: (value, {dataPointIndex, seriesIndex, w}) => {
            const days = this.formatService.formatDays(value);
            const time = this.formatService.formatDuration(w.config.series[seriesIndex].data[dataPointIndex].meta.time);
            return `${days} ${this.localizedDays} (${time} ${this.localizedHours})`;
          }
        }
      },
      states: {
        hover: {
          filter: {
            type: 'none',
          }
        }
      },
    };
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
