import {Component, OnDestroy, OnInit} from '@angular/core';
import {TimeSheetDto, TimeSheetGridDto, TimeSheetService} from '../../../../../api/timetracking';
import {map, single, switchMap} from 'rxjs/operators';
import {DateTime, Duration} from 'luxon';
import {LocalizationService} from '../../../../../core/app/services/internationalization/localization.service';
import {ActivatedRoute, Router} from '@angular/router';
import {HttpClient} from '@angular/common/http';
import {Observable, Subscription, timer} from 'rxjs';
import {StorageService} from '../../../../../core/app/services/storage.service';
import {UtilityService} from '../../../../../core/app/services/utility.service';
import {GuidService} from '../../../../../core/app/services/state-management/guid.service';
import {EntityService} from '../../../../../core/app/services/state-management/entity.service';
import {Filter, FilteredRequestParams, FilterName} from '../../../../../core/app/components/filter/filter.component';
import {AuthenticationService} from '../../../../../core/app/services/authentication.service';

// import {Validators as CustomValidators} from '../../../../../core/app/services/form-validation/validators';

interface TimeSheetDayGroupDto {
  date: DateTime;
  workTime: Duration;
  timeSheets: TimeSheetGridDto[];
}

class TimeSheetOverviewDto {
  workDayTimeSheets: TimeSheetDayGroupDto[] = [];
  omittedTimeSheets = 0;
  runningTimeSheets: TimeSheetGridDto[] = [];
}

@Component({
  selector: 'ts-timesheet',
  templateUrl: './timesheet.component.html',
  styleUrls: ['./timesheet.component.scss']
})
export class TimesheetComponent implements OnInit, OnDestroy {
  public guidService = GuidService;
  public overview?: TimeSheetOverviewDto;
  public filters: (Filter | FilterName)[];
  public showDetails = false;
  public createNewDisabled: boolean;

  private readonly subscriptions = new Subscription();

  constructor(
    private entityService: EntityService,
    private timeSheetService: TimeSheetService,
    private localizationService: LocalizationService,
    private route: ActivatedRoute,
    private router: Router,
    private httpClient: HttpClient,
    private storageService: StorageService,
    private utilityService: UtilityService,
    authenticationService: AuthenticationService,
  ) {
    this.createNewDisabled = !authenticationService.currentUser.hasRole.timeSheetManage;

    const defaultStartDate = DateTime.now().minus({month: 1}).startOf('month');
    const defaultEndDate = DateTime.now().endOf('month');

    this.filters = [
      {name: 'showDetails', defaultValue: false, isPrimary: true},
      {name: 'timeSheetStartDate', defaultValue: defaultStartDate, isPrimary: true},
      {name: 'timeSheetEndDate', defaultValue: defaultEndDate, isPrimary: true},
      {name: 'customerId', isPrimary: true},
      {name: 'orderId'},
      {name: 'projectId', isPrimary: true},
      {name: 'activityId'},
      {name: 'timeSheetIssue'},
      {name: 'timeSheetComment'},
      {name: 'timeSheetBillable'},
      {name: 'userId'},
    ];
  }

  public ngOnInit(): void {
    const loadTimeSheets = this.entityService.reloadRequested
      .pipe(switchMap(filter => this.loadData(filter)))
      .subscribe(overview => this.overview = overview);
    this.subscriptions.add(loadTimeSheets);

    const showDetails = this.entityService.filterChanged
      .pipe(map(filter => filter.showDetails === 'true'))
      .subscribe(showDetails => this.showDetails = showDetails);
    this.subscriptions.add(showDetails);
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  public deleteItem(id: string) {
    this.timeSheetService
      .delete({id})
      .pipe(single())
      .subscribe(() => {
        this.entityService.timesheetChanged.next({entity: {id} as TimeSheetGridDto, action: 'deleted'});
      });
  }

  public restartTimeSheet(copyFromTimesheetId: string): void {
    this.timeSheetService.startSimilarTimeSheetEntry({copyFromTimesheetId, startDateTime: DateTime.now()})
      .pipe(single())
      .subscribe((timeSheetDto: TimeSheetDto) => {
        this.entityService.timesheetChanged.next({entity: {id: timeSheetDto.id} as TimeSheetGridDto, action: 'created'});
      });
  }

  public stopTimeSheet(timesheetId: string): void {
    this.timeSheetService.stopTimeSheetEntry({timesheetId: timesheetId, endDateTime: DateTime.now()})
      .pipe(single())
      .subscribe((timeSheetDto: TimeSheetDto) => {
        this.entityService.timesheetChanged.next({entity: {id: timeSheetDto.id} as TimeSheetGridDto, action: 'updated'});
      });
  }

  public stopAllTimeSheets() {
    if (!this.overview)
      return;

    for (const runningTimeSheet of this.overview?.runningTimeSheets)
      this.stopTimeSheet(runningTimeSheet.id);
  }

  public timeSheetDayGroupKey(index: number, item: TimeSheetDayGroupDto) {
    // see https://moment.github.io/luxon/api-docs/index.html#datetimeequals
    return +item.date;
  }

  public timeSheetRowKey(index: number, item: TimeSheetGridDto) {
    return item.id;
  }

  private loadData(filter: FilteredRequestParams): Observable<TimeSheetOverviewDto> {
    return this.timeSheetService
      .getGridFiltered(filter)
      .pipe(
        single(),
        this.entityService.withUpdatesFrom(this.entityService.timesheetChanged, this.timeSheetService),
        // Updates active tasks (e.g. duration display) every 50 seconds.
        switchMap(timeSheets => (timer(0, 500000)).pipe(map(() => timeSheets))),
        map(timeSheets => this.createTimeSheetOverview(timeSheets))
      );
  };

  private createTimeSheetOverview(timeSheets: TimeSheetGridDto[]): TimeSheetOverviewDto {
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

    const workDaysToDisplay = this.limitWorkDaysToDisplay(timeSheetWorkDayGroups, 50);
    const omittedTimeSheets = timeSheetWorkDayGroups.length - workDaysToDisplay.length;
    const runningTimeSheets = timeSheets.filter(x => !x.endDate);

    return {
      workDayTimeSheets: workDaysToDisplay,
      omittedTimeSheets: omittedTimeSheets,
      runningTimeSheets: runningTimeSheets,
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
