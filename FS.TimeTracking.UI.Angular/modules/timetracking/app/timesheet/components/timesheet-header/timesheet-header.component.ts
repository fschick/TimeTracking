import {Component, OnDestroy, OnInit} from '@angular/core';
import {FilteredRequestParams} from '../../../../../core/app/components/filter/filter.component';
import {filter, map, single, switchMap} from 'rxjs/operators';
import {TimeSheetService, WorkdayAggregationUnit, WorkedDaysInfoDto} from '../../../../../api/timetracking';
import {DateTime, Duration} from 'luxon';
import {Observable, Subscription} from 'rxjs';
import {EntityService} from '../../../../../core/app/services/state-management/entity.service';
import {FormatService} from '../../../../../core/app/services/format.service';
import {ApexAxisChartSeries, ApexChart, ApexPlotOptions, ApexStates, ApexTooltip, ApexXAxis} from 'ng-apexcharts';
import {$localize} from '@angular/localize/init';
import {DecimalPipe} from '@angular/common';
import {NavigationEnd, Router} from '@angular/router';

class WorkTimeOverviewDto {
  workedDays = 0;
  workedDaysDuration = Duration.fromMillis(0);
  workdays = 0;
  workdaysDuration = Duration.fromMillis(0);
  holidays = 0;
  holidaysDuration = Duration.fromMillis(0);
}

type ThumbChartOptions = {
  chart: ApexChart;
  plotOptions: ApexPlotOptions;
  colors: string[],
  tooltip: ApexTooltip;
  states: ApexStates;
  xaxis: ApexXAxis;
};

@Component({
  selector: 'ts-timesheet-header',
  templateUrl: './timesheet-header.component.html',
  styleUrls: ['./timesheet-header.component.scss']
})
export class TimeSheetHeaderComponent implements OnInit, OnDestroy {
  private readonly subscriptions = new Subscription();
  private readonly hoursAbbr: string;
  private readonly daysAbbr: string;
  private readonly calendarWeekAbbr: string;

  public chartSeries: ApexAxisChartSeries = [];
  public readonly chartOptions: ThumbChartOptions = {} as ThumbChartOptions;
  public chartDescription: string = '';

  public overview?: WorkTimeOverviewDto;
  public visible = true;
  public daysOnlyFormat: string;

  constructor(
    private router: Router,
    private entityService: EntityService,
    private timeSheetService: TimeSheetService,
    private decimalPipe: DecimalPipe,
    private formatService: FormatService,
  ) {
    this.hoursAbbr = $localize`:@@Abbreviations.Hours:[i18n] h`;
    this.daysAbbr = $localize`:@@Abbreviations.Days:[i18n] days`;
    this.calendarWeekAbbr = $localize`:@@Abbreviations.CalendarWeek:[i18n] CW`;
    const escapedHoursAbbr = formatService.escapeDurationFormat(this.hoursAbbr);
    this.daysOnlyFormat = `hh${escapedHoursAbbr}`;
    this.chartOptions = this.creatChartOptions();

    const visible = router.events
      .pipe(filter(x => x instanceof NavigationEnd), map(x => x as NavigationEnd),)
      .subscribe(x => this.visible = x.url.match(/^(\/|\/[0-9a-f]{8}-.*)$/) != null);
    this.subscriptions.add(visible);
  }

  public ngOnInit(): void {
    const loadWorkTimeOverview = this.entityService.reloadRequested
      .pipe(
        filter(() => this.visible),
        switchMap(filter => this.loadOverviewData(filter))
      )
      .subscribe(overview => this.overview = overview);
    this.subscriptions.add(loadWorkTimeOverview);
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private loadOverviewData(filter: FilteredRequestParams): Observable<WorkTimeOverviewDto> {
    return this.timeSheetService
      .getWorkedDaysOverview(filter)
      .pipe(
        single(),
        map(workedTimeInfo => this.createWorkTimeOverview(workedTimeInfo))
      );
  };

  private createWorkTimeOverview(workedTimeInfo: WorkedDaysInfoDto): WorkTimeOverviewDto {
    const workedDays = workedTimeInfo.totalTimeWorked.as('hours') / workedTimeInfo.workHoursPerWorkday.hours;
    const workedDaysDuration = workedTimeInfo.totalTimeWorked;
    const workdays = workedTimeInfo.personalWorkdays;
    const workdaysDuration = Duration.fromDurationLike({hour: workdays * workedTimeInfo.workHoursPerWorkday.hours});
    const holidays = workedTimeInfo.personalHolidays;
    const holidaysDuration = Duration.fromDurationLike({hour: holidays * workedTimeInfo.workHoursPerWorkday.hours});

    this.chartSeries = [
      {
        name: $localize`:@@Page.Chart.Common.Worked:[i18n] Worked`,
        data: workedTimeInfo.lastWorkedTimes.map(workTime => ({
          x: this.getDateFormat(workTime.date, workedTimeInfo.lastWorkedTimesAggregationUnit), //.toJSDate().getTime(),
          y: workTime.timeWorked.as('hours') / (workedTimeInfo.lastWorkedTimesAggregationUnit === 'day' ? 1 : workedTimeInfo.workHoursPerWorkday.hours),
          meta: {aggregationUnit: workedTimeInfo.lastWorkedTimesAggregationUnit}
        }))
      }
    ];

    this.chartDescription = this.getChartDescription(workedTimeInfo.lastWorkedTimes.length, workedTimeInfo.lastWorkedTimesAggregationUnit);

    return {
      workedDays,
      workedDaysDuration,
      workdays,
      workdaysDuration,
      holidays,
      holidaysDuration
    }
  }

  private getDateFormat(date: DateTime, aggregationUnit: WorkdayAggregationUnit): string {
    switch (aggregationUnit) {
      case 'invalid':
        return '';
      case 'day':
        return this.formatService.formatDate(date);
      case 'week':
        return this.formatService.formatDate(date);
      // return this.formatService.formatDate(date, `${this.formatService.escapeDateTimeFormat(this.calendarWeekAbbr)} W yyyy`);
      case 'month':
        return this.formatService.formatDate(date, 'MMM yyyy');
      case 'year':
        return this.formatService.formatDate(date, 'yyyy');
    }
  }

  private getChartDescription(count: number, aggregationUnit: WorkdayAggregationUnit): string {
    switch (aggregationUnit) {
      case 'invalid':
        return $localize`:@@Component.Header.DaysWorked:[i18n] days worked`;
      case 'day':
        return $localize`:@@Component.Header.PastDays:[i18n] past ${count}:COUNT: days`;
      case 'week':
        return $localize`:@@Component.Header.PastWeeks:[i18n] past ${count}:COUNT: weeks`;
      case 'month':
        return $localize`:@@Component.Header.PastMonths:[i18n] past ${count}:COUNT: months`;
      case 'year':
        return $localize`:@@Component.Header.PastYears:[i18n] past ${count}:COUNT: years`;
    }
  }

  private creatChartOptions(): ThumbChartOptions {
    return {
      chart: {
        type: 'bar',
        height: 30,
        sparkline: {
          enabled: true
        },
        offsetY: 13
      },
      plotOptions: {
        bar: {
          borderRadius: 5,
        }
      },
      xaxis: {
        type: 'category',
      },
      colors: ['#14B655'],
      tooltip: {
        intersect: false,
        custom: ({_, seriesIndex, dataPointIndex, w}) => {
          const data = w.config.series[seriesIndex].data[dataPointIndex];
          const unit = data.meta.aggregationUnit === 'day' ? this.hoursAbbr : this.daysAbbr;
          return `<span class="m-1 text-center">${this.decimalPipe.transform(data.y, '1.0-1')} ${unit}<br>${data.x}</span>`;
        },
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
