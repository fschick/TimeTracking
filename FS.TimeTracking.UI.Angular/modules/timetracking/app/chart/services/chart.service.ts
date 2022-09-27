import {Injectable} from '@angular/core';
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexDataLabels,
  ApexFill,
  ApexGrid,
  ApexLegend,
  ApexPlotOptions,
  ApexStates,
  ApexTooltip,
  ApexXAxis,
  ApexYAxis
} from 'ng-apexcharts';
import {FormatService} from '../../../../core/app/services/format.service';
import {DurationPipe} from '../../../../core/app/pipes/duration.pipe';
import {SortOrder} from '../../../../core/app/components/simple-table/simple-table.component';

export type ChartOptions = {
  chart: ApexChart;
  xAxis: ApexXAxis;
  yAxis: ApexYAxis;
  colors: string[],
  plotOptions: ApexPlotOptions;
  fill: ApexFill;
  grid: ApexGrid;
  states: ApexStates;
  dataLabels: ApexDataLabels;
  tooltip: ApexTooltip;
  legend: ApexLegend;
};

@Injectable({
  providedIn: 'root'
})
export class ChartService {
  private readonly LOCALIZED_DAYS = $localize`:@@Abbreviations.Days:[i18n] days`;
  // private readonly chartColors = ['#6E58FF', '#F753FF', '#FF3C69', '#FF9C3C', '#F9ED20', '#3AD827', '#30B79D', '#002B8C', '#C03CFF', '#FF0098', '#FF563C', '#FFCB3C', '#BBE348', '#2AD19A', '#12838E'];
  // private readonly chartColors = ['#6E58FF', '#FF3C69', '#F9ED20', '#30B79D', '#C03CFF', '#FF563C', '#BBE348', '#F753FF', '#FF9C3C', '#3AD827', '#002B8C', '#FF0098', '#FFCB3C', '#2AD19A', '#12838E'];
  private readonly chartColors = ['#FF3C69', '#F753FF', '#6E58FF', '#FF9C3C', '#F9ED20', '#30B79D', '#C03CFF', '#FF563C', '#BBE348', '#3AD827', '#002B8C', '#FF0098', '#FFCB3C', '#2AD19A', '#12838E'];
  public static FONT_WEIGHT_BOLD = 700;

  constructor(
    private formatService: FormatService,
    private durationPipe: DurationPipe,
  ) {
  }

  public addColors<T>(rows: T[]): (T & { color: string, completed: number })[] {
    return rows.map((row, index) => {
      const colorIndex = index % this.chartColors.length;
      return ({...row, color: this.chartColors[colorIndex], completed: 0});
    });
  }

  public createChartOptions(columnCount = 6, maxYValue = 40): ChartOptions {
    let yAxisTickMinStep: number;
    let yAxisTickAmount: number;

    if (maxYValue > 60) {
      yAxisTickMinStep = 30;
      yAxisTickAmount = 3;
    } else if (maxYValue > 10) {
      yAxisTickMinStep = 10;
      yAxisTickAmount = 2;
    } else {
      yAxisTickMinStep = 1;
      yAxisTickAmount = 1;
    }

    return {
      chart: {
        type: 'bar',
        height: 350,
        stacked: true,
        toolbar: {
          show: false
        },
        selection: {
          enabled: false,
        },
        animations: {
          enabled: false
        }
      },
      xAxis: {
        axisBorder: {
          show: true,
          color: '#CFCFCF',
          offsetY: -.5,
        },
        axisTicks: {
          show: false
        },
        labels: {
          offsetY: 3,
          style: {
            fontSize: '.8rem',
            colors: '#5F5F5F',
            fontWeight: ChartService.FONT_WEIGHT_BOLD
          }
        },
        crosshairs: {
          fill: {
            type: 'solid',
            color: '#F2F2F2',
          },
        },
      },
      yAxis: {
        title: {
          // text: $localize`:@@Page.Chart.Common.Days:[i18n] Days`,
        },
        max: Math.ceil(maxYValue / yAxisTickMinStep) * yAxisTickMinStep,
        tickAmount: yAxisTickAmount,
        labels: {
          style: {
            fontSize: '.8rem',
            colors: '#989898',
            fontWeight: ChartService.FONT_WEIGHT_BOLD
          },
          formatter: (value: number) => this.formatService.formatDays(value, '1.0-0')
        }
      },
      colors: ['#14B655', '#C8E5D3'],
      plotOptions: {
        bar: {
          dataLabels: {},
          columnWidth: this.getChartColumnWidth(columnCount),
          // borderRadius: 5, // Activate after https://github.com/apexcharts/apexcharts.js/issues/2676 is solved.
        }
      },
      fill: {
        opacity: 1
      },
      grid: {
        borderColor: '#F3F3F3',
      },
      states: {
        hover: {
          filter: {
            type: 'none',
          }
        }
      },
      dataLabels: {
        enabled: false,
        formatter: (value: number) => this.formatService.formatDays(value)
      },
      tooltip: {
        enabled: true,
        followCursor: true,
        shared: false,
        intersect: false,
        onDatasetHover: {highlightDataSeries: false},
        custom: ({dataPointIndex, w}) => {
          return this.getToolTip(w.config.series, dataPointIndex);
        }
      },
      legend: {
        show: false,
        position: 'top',
      },
    };
  }

  public sortByDaysDifference<T extends { daysPlanned: number | undefined, daysDifference: number }>(rowA: T, rowB: T, direction: SortOrder) {
    const keepPlannedOnTop = direction === 'asc' ? -1 : 1;
    if (rowA.daysPlanned && !rowB.daysPlanned)
      return 1 * keepPlannedOnTop;
    if (!rowA.daysPlanned && rowB.daysPlanned)
      return -1 * keepPlannedOnTop;
    return rowA.daysDifference - rowB.daysDifference;
  }

  private getToolTip(series: ApexAxisChartSeries, dataPointIndex: number): string {
    const workedTitle = series[0].name;
    const worked: any = series[0].data[dataPointIndex];
    const workedDays = worked.meta.days;
    const workedTime = worked.meta.time;
    const workedHtml = `
      <div class='d-flex'>
        <div class='me-2'>
          <span class='chart-legend worked'></span>
        </div>
        <div>
          <div class='fw-bold'>${workedTitle}</div>
          <div>${this.formatService.formatDays(workedDays)} ${this.LOCALIZED_DAYS} (${this.durationPipe.transform(workedTime)})</div>
        </div>
      </div>
    `;

    if (series.length === 1)
      return workedHtml;

    const plannedTitle = series[1].name;
    const planned: any = series[1].data[dataPointIndex];
    const plannedDays = planned.meta.days;
    const plannedTime = planned.meta.time;
    const plannedHtml = `
      <div class='d-flex mt-2'>
        <div class='me-2'>
          <span class='chart-legend planned'></span>
        </div>
        <div>
          <div class='fw-bold'>${plannedTitle}</div>
          <div>${this.formatService.formatDays(plannedDays)} ${this.LOCALIZED_DAYS} (${this.durationPipe.transform(plannedTime)})</div>
        </div>
      </div>
    `;

    return workedHtml + plannedHtml;
  }

  private getChartColumnWidth(dataCount: number): string {
    if (dataCount <= 6)
      return '40%';
    if (dataCount <= 12)
      return '60%';
    // if (dataCount <= 24)
    //   return '80%';
    return '80%';
  }
}
