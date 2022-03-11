import {Injectable} from '@angular/core';
import {ApexChart, ApexDataLabels, ApexLegend, ApexPlotOptions, ApexStates, ApexStroke, ApexTooltip, ApexXAxis, ApexYAxis} from 'ng-apexcharts';
import {FormatService} from '../../shared/services/format.service';

export type ChartOptions = {
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

@Injectable({
  providedIn: 'root'
})
export class ChartService {
  private localizedDays = $localize`:@@Abbreviations.Days:[i18n] days`;
  private localizedHours = $localize`:@@Abbreviations.Hours:[i18n] h`;

  constructor(
    public formatService: FormatService
  ) {
  }

  public createChartOptions(): ChartOptions {
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
          text: $localize`:@@Page.Chart.Common.Days:[i18n] Days`,
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
}
