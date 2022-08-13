import {Component, Input} from '@angular/core';
import {ApexChart, ApexDataLabels, ApexLegend, ApexNonAxisChartSeries, ApexPlotOptions, ApexStates, ApexStroke, ApexTooltip} from 'ng-apexcharts';
import {UtilityService} from '../../../../../core/app/services/utility.service';
import {ChartService} from '../../services/chart.service';
import {FormatService} from '../../../../../core/app/services/format.service';

export interface TotalRow {
  customerTitle: string;
  orderTitle?: string;
  daysWorked: number;
  daysPlanned: number;
  totalWorkedPercentage: number;
  totalPlannedPercentage: number;
  daysDifference: number;
  color: string;
}

type DonutChartOptions = {
  chart: ApexChart;
  source: TotalRow[],
  series: ApexNonAxisChartSeries;
  labels: string[];
  colors: string[];
  dataLabels: ApexDataLabels;
  tooltip: ApexTooltip;
  stroke: ApexStroke;
  plotOptions: ApexPlotOptions
  states: ApexStates
  legend: ApexLegend,
}

@Component({
  selector: 'ts-chart-totals-overview',
  templateUrl: './chart-totals-overview.component.html',
  styleUrls: ['./chart-totals-overview.component.scss']
})
export class ChartTotalsOverviewComponent<TTotalRow extends TotalRow> {
  private readonly MISC_COLOR = '#CFCFCF';
  public chartPlanned?: DonutChartOptions;
  public chartWorked?: DonutChartOptions;
  public chartLeft?: DonutChartOptions;

  @Input()
  public set rows(rows: TTotalRow[]) {
    this.chartPlanned = this.createChartPlannedWorkedOptions(rows, 'planned');
    this.chartWorked = this.createChartPlannedWorkedOptions(rows, 'worked');
  }

  @Input()
  public set selectedRow(selectedRows: TTotalRow[] | undefined) {
    this.chartLeft = this.createChartLeftOptions(selectedRows);
  }

  constructor(
    private utilityService: UtilityService,
    private formatService: FormatService,
  ) { }

  private createChartPlannedWorkedOptions(rows: TTotalRow[], type: 'planned' | 'worked'): DonutChartOptions {
    const valueSelector: (row: TTotalRow) => number = type == 'planned'
      ? row => row.daysPlanned
      : row => row.daysWorked;

    const label = type == 'planned'
      ? $localize`:@@Component.ChartTotals.Planned:[i18n] Planned`
      : $localize`:@@Component.ChartTotals.Worked:[i18n] Worked`;

    const chartOptions = this.getDefaultChartOptions(label, seriesIndex => this.getToolTipChartPlannedWorked(seriesIndex, type));
    const reducedRows = this.reduceSeries(rows, valueSelector);
    chartOptions.source = reducedRows;
    chartOptions.series = reducedRows.map(row => valueSelector(row));
    chartOptions.labels = reducedRows.map(row => row.orderTitle ?? row.customerTitle);
    chartOptions.colors = reducedRows.map(row => row.color);
    return chartOptions;
  }

  private reduceSeries(rows: TTotalRow[], valueSelector: (row: TTotalRow) => number): TTotalRow[] {
    const sortedRows = [...rows];
    sortedRows.sort((rowA, rowB) => valueSelector(rowB) - valueSelector(rowA));
    const total = this.utilityService.sum(sortedRows.map(valueSelector));
    const limit = total * .03;
    const series = sortedRows.filter(row => valueSelector(row) > limit);
    const sharedSeries = sortedRows.filter(row => valueSelector(row) <= limit);
    const daysWorked = this.utilityService.sum(sharedSeries.map(row => row.daysWorked));
    const daysPlanned = this.utilityService.sum(sharedSeries.map(row => row.daysPlanned));
    const totalWorkedPercentage = this.utilityService.sum(sharedSeries.map(row => row.totalWorkedPercentage));
    const totalPlannedPercentage = this.utilityService.sum(sharedSeries.map(row => row.totalPlannedPercentage));
    series.push({
      customerTitle: $localize`:@@Component.ChartTotals.Other:[i18n] Other`,
      daysWorked,
      daysPlanned,
      totalWorkedPercentage,
      totalPlannedPercentage,
      color: this.MISC_COLOR,
    } as TTotalRow);
    return series;
  }

  private getToolTipChartPlannedWorked(seriesIndex: number, type: 'planned' | 'worked'): string {
    const chart = type === 'planned' ? this.chartPlanned : this.chartWorked;
    if (chart === undefined)
      throw Error('Chart tooltip cannot be rendered before chart is fully initialized.');

    const label = type === 'planned'
      ? $localize`:@@Component.ChartTotals.DaysPlanned:[i18n] days planned`
      : $localize`:@@Component.ChartTotals.DaysWorked:[i18n] days worked`;

    const color = chart.colors[seriesIndex];
    const title = chart.labels[seriesIndex];
    const days = chart.series[seriesIndex];
    const totalPercentage = type === 'planned'
      ? chart.source[seriesIndex].totalPlannedPercentage
      : chart.source[seriesIndex].totalWorkedPercentage;
    return `
      <div class="d-flex">
        <div class="me-2">
          <span class="chart-legend" style="background-color: ${color}"></span>
        </div>
        <div>
          <div class="fw-bold">${title}</div>
          <div>${this.formatService.formatDays(days)} ${label}</div>
          <div>(${this.formatService.formatRatio(totalPercentage)} ${$localize`:@@Component.ChartTotals.Percentage:[i18n] %`} ${$localize`:@@Component.ChartTotals.OfTotalTime:[i18n] of total time`})</div>
        </div>
      </div>
    `;
  }

  private createChartLeftOptions(rows: TTotalRow[] | undefined): DonutChartOptions | undefined {
    if (!rows?.length)
      return undefined;

    const daysWorked = this.utilityService.sum(rows.map(row => row.daysWorked));
    const daysPlanned = this.utilityService.sum(rows.map(row => row.daysPlanned));
    const leftPercentage = Math.max(1 - (daysWorked / daysPlanned), 0);

    const rowLeft = {
      color: rows.length === 1 ? `${rows[0].color}30` : '#DFDFDF',
      customerTitle: '',
      daysWorked: Math.max(daysPlanned - daysWorked, 0),
    } as TTotalRow;

    const chartRows = [...rows, rowLeft];
    const chartOptions = this.getDefaultChartOptions($localize`:@@Component.ChartTotals.Left:[i18n] left`, seriesIndex => this.getToolTipChartLeft(seriesIndex));
    chartOptions.source = chartRows;
    chartOptions.series = chartRows.map(row => row.daysWorked);
    chartOptions.labels = chartRows.map(row => row.orderTitle ?? row.customerTitle);
    chartOptions.colors = chartRows.map(row => row.color);

    chartOptions.plotOptions.pie!.donut!.labels!.value!.show = true;
    chartOptions.plotOptions.pie!.donut!.labels!.name!.offsetY = 20;
    chartOptions.plotOptions.pie!.donut!.labels!.value!.offsetY = -18;
    chartOptions.plotOptions.pie!.donut!.labels!.total!.color = '#989898';
    chartOptions.plotOptions.pie!.donut!.labels!.total!.formatter = () => `${this.formatService.formatRatio(leftPercentage)}${$localize`:@@Component.ChartTotals.Percentage:[i18n] %`}`;

    return chartOptions;
  }

  private getToolTipChartLeft(seriesIndex: number): string {
    const chart = this.chartLeft;
    if (chart === undefined)
      throw Error('Chart tooltip cannot be rendered before chart is fully initialized.');

    const color = chart.colors[seriesIndex];
    const title = chart.labels[seriesIndex];
    const days = chart.series[seriesIndex];
    const label = title
      ? $localize`:@@Component.ChartTotals.DaysWorked:[i18n] days worked`
      : $localize`:@@Component.ChartTotals.WorkdaysLeft:[i18n] workdays left`;

    return `
      <div class="d-flex">
        <div class="me-2">
          <span class="chart-legend" style="background-color: ${color}"></span>
        </div>
        <div>
          <div class="fw-bold">${title}</div>
          <div>${this.formatService.formatDays(days)} ${label}</div>
        </div>
      </div>
    `;
  }

  private getDefaultChartOptions(label: string, tooltip?: (seriesIndex: number) => string): DonutChartOptions {
    return {
      chart: {
        type: 'donut',
        height: '180px',
        animations: {
          enabled: false
        },
      },
      source: [],
      series: [],
      labels: [],
      dataLabels: {
        enabled: false
      },
      colors: [],
      stroke: {
        width: 0
      },
      tooltip: {
        theme: 'light',
        custom: tooltip
          ? ({seriesIndex}) => { return tooltip(seriesIndex); }
          : undefined,
      },
      plotOptions: {
        pie: {
          donut: {
            size: '70%',
            labels: {
              show: true,
              name: {
                show: true,
                offsetY: 5,
              },
              value: {
                show: false,
                fontWeight: ChartService.FONT_WEIGHT_BOLD,
                fontSize: '1.5rem',
              },
              total: {
                show: true,
                showAlways: true,
                label: label,
                fontSize: '.9rem',
                fontWeight: ChartService.FONT_WEIGHT_BOLD,
                fontFamily: 'Sarabun',
                color: '#292929',
              }
            }
          },
        }
      },
      states: {
        hover: {
          filter: {
            type: "none"
          }
        }
      },
      legend: {
        show: false
      }
    };
  }
}
