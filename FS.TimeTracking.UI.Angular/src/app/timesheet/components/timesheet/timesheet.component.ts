import {Component, OnDestroy} from '@angular/core';
import {TimeSheetDto, TimeSheetListDto, TimeSheetService, WorkdayService, WorkedTimeInfoDto} from '../../../shared/services/api';
import {map, single, switchMap} from 'rxjs/operators';
import {DateTime, Duration} from 'luxon';
import {LocalizationService} from '../../../shared/services/internationalization/localization.service';
import {ActivatedRoute, Router} from '@angular/router';
import {HttpClient} from '@angular/common/http';
import {combineLatest, Subject, Subscription, timer} from 'rxjs';
import {StorageService} from '../../../shared/services/storage/storage.service';
import {UtilityService} from '../../../shared/services/utility.service';
import {GuidService} from '../../../shared/services/state-management/guid.service';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {Filter, FilteredRequestParams, FilterName} from '../../../shared/components/filter/filter.component';

// import {Validators as CustomValidators} from '../../../shared/services/form-validation/validators';

interface TimeSheetDayGroupDto {
  date: DateTime;
  workTime: Duration;
  timeSheets: TimeSheetListDto[];
}

class TimeSheetOverviewDto {
  workdays = 0;
  workdaysDuration = Duration.fromMillis(0);
  holidays = 0;
  holidaysDuration = Duration.fromMillis(0);
  workedDays = 0;
  workedTime = Duration.fromMillis(0);
  workDayTimeSheets: TimeSheetDayGroupDto[] = [];
  omittedTimeSheets = 0;
}

@Component({
  selector: 'ts-timesheet',
  templateUrl: './timesheet.component.html',
  styleUrls: ['./timesheet.component.scss']
})
export class TimesheetComponent implements OnDestroy {
  public guidService = GuidService;
  public overview?: TimeSheetOverviewDto;
  public filterChanged = new Subject<FilteredRequestParams>();
  public filters: (Filter | FilterName)[];

  public get showDetails(): boolean {
    return this.storageService.get(this.timeSheetShowDetailsStorageKey, 'false') === 'true';
  };

  public set showDetails(value: boolean) {
    this.storageService.set(this.timeSheetShowDetailsStorageKey, value ? 'true' : 'false');
  };

  private readonly timeSheetShowDetailsStorageKey = 'timeSheetShowDetails';
  private readonly subscriptions = new Subscription();

  constructor(
    private entityService: EntityService,
    private timeSheetService: TimeSheetService,
    private workdayService: WorkdayService,
    private localizationService: LocalizationService,
    private route: ActivatedRoute,
    private router: Router,
    private httpClient: HttpClient,
    private storageService: StorageService,
    private utilityService: UtilityService,
  ) {
    const defaultStartDate = DateTime.now().startOf('month');
    const defaultEndDate = DateTime.now().endOf('month');

    const loadTimeSheets = this.filterChanged
      .pipe(switchMap(filter => this.loadData(filter)))
      .subscribe((overview) => this.overview = overview);
    this.subscriptions.add(loadTimeSheets);

    this.filters = [
      {name: 'timeSheetStartDate', defaultValue: defaultStartDate},
      {name: 'timeSheetEndDate', defaultValue: defaultEndDate},
      {name: 'customerId'},
      {name: 'projectId'},
      {name: 'activityId'},
      {name: 'orderId'},
      {name: 'timeSheetIssue'},
      {name: 'timeSheetComment'}
    ];
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  public deleteItem(id: string) {
    this.timeSheetService
      .delete({id})
      .pipe(single())
      .subscribe(() => {
        this.entityService.timesheetChanged.next({entity: {id} as TimeSheetListDto, action: 'deleted'});
      });
  }

  public restartTimeSheet(copyFromTimesheetId: string): void {
    this.timeSheetService.startSimilarTimeSheetEntry({copyFromTimesheetId, startDateTime: DateTime.now()})
      .pipe(single())
      .subscribe((timeSheetDto: TimeSheetDto) => {
        this.entityService.timesheetChanged.next({entity: {id: timeSheetDto.id} as TimeSheetListDto, action: 'created'});
      });
  }

  public stopTimeSheet(copyFromTimesheetId: string): void {
    this.timeSheetService.stopTimeSheetEntry({timesheetId: copyFromTimesheetId, endDateTime: DateTime.now()})
      .pipe(single())
      .subscribe((timeSheetDto: TimeSheetDto) => {
        this.entityService.timesheetChanged.next({entity: {id: timeSheetDto.id} as TimeSheetListDto, action: 'updated'});
      });
  }

  public timeSheetDayGroupKey(index: number, item: TimeSheetDayGroupDto) {
    // see https://moment.github.io/luxon/api-docs/index.html#datetimeequals
    return +item.date;
  }

  public timeSheetListKey(index: number, item: TimeSheetListDto) {
    return item.id;
  }

  private loadData(filter: FilteredRequestParams) {
    return combineLatest([this.loadTimeSheets(filter), this.loadWorkDayInfo(filter)])
      .pipe(
        map(([timeSheets, workedTimeInfo]) => this.createTimeSheetOverview(timeSheets, workedTimeInfo))
      )
  };

  private loadTimeSheets(filter: FilteredRequestParams) {
    return this.timeSheetService
      .getListFiltered(filter)
      .pipe(
        single(),
        this.entityService.withUpdatesFrom(this.entityService.timesheetChanged, this.timeSheetService),
        switchMap(timeSheets => (timer(0, 5000)).pipe(map(() => timeSheets)))
      );
  };

  private loadWorkDayInfo(filter: FilteredRequestParams) {
    return this.workdayService
      .getWorkedDaysInfo(filter)
      .pipe(single());
  };

  private createTimeSheetOverview(timeSheets: TimeSheetListDto[], workedTimeInfo: WorkedTimeInfoDto): TimeSheetOverviewDto {
    timeSheets.sort((a, b) => a.startDate.equals(b.startDate) ? 0 : a.startDate > b.startDate ? -1 : 1);

    for (const timeSheet of timeSheets) {
      if (!timeSheet.endDate)
        timeSheet.duration = DateTime.now().diff(timeSheet.startDate);
    }

    const timeSheetsByWorkDay = this.utilityService.groupBy(timeSheets, timeSheet => timeSheet.startDate.startOf('day').toMillis());

    const timeSheetWorkDayGroups: TimeSheetDayGroupDto[] = Array
      .from(timeSheetsByWorkDay)
      .map(([date, timeSheetDayGroup]) => ({
        date: DateTime.fromMillis(date),
        workTime: timeSheetDayGroup.reduce((duration, timeSheet) => duration.plus(timeSheet.duration ?? 0), Duration.fromMillis(0)),
        timeSheets: timeSheetDayGroup
      }));

    if (timeSheetWorkDayGroups.length === 0)
      return new TimeSheetOverviewDto();

    const workedTime = timeSheetWorkDayGroups.reduce((duration, timeSheet) => duration.plus(timeSheet.workTime), Duration.fromMillis(0));
    const workedDays = workedTime.as('hours') / workedTimeInfo.workHoursPerWorkday.hours;

    const workDaysToDisplay = this.limitWorkDaysToDisplay(timeSheetWorkDayGroups, 50);
    const omittedTimeSheets = timeSheetWorkDayGroups.length - workDaysToDisplay.length;

    const workdays = workedTimeInfo.personalWorkdays;
    const workdaysDuration = Duration.fromDurationLike({hour: workdays * workedTimeInfo.workHoursPerWorkday.hours});
    const holidays = workedTimeInfo.personalHolidays;
    const holidaysDuration = Duration.fromDurationLike({hour: holidays * workedTimeInfo.workHoursPerWorkday.hours});

    return {
      workdays: workdays,
      workdaysDuration: workdaysDuration,
      holidays: holidays,
      holidaysDuration: holidaysDuration,
      workedTime: workedTime,
      workedDays: workedDays,
      workDayTimeSheets: workDaysToDisplay,
      omittedTimeSheets: omittedTimeSheets
    };
  }

  private limitWorkDaysToDisplay(timeSheetDayGroups: TimeSheetDayGroupDto[], maxRowsToDisplay: number) {
    let currentRowsToDisplay = 0;
    const workDaysToDisplay: TimeSheetDayGroupDto[] = [];

    for (const timeSheetDayGroup of timeSheetDayGroups) {
      workDaysToDisplay.push(timeSheetDayGroup);
      currentRowsToDisplay += timeSheetDayGroup.timeSheets.length;
      if (currentRowsToDisplay > maxRowsToDisplay)
        break;
    }

    return workDaysToDisplay;
  }
}
