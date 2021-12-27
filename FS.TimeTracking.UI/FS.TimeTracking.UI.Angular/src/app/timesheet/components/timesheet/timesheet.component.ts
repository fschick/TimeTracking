import {Component, OnDestroy} from '@angular/core';
import {TimeSheetDto, TimeSheetListDto, TimeSheetService} from '../../../shared/services/api';
import {filter, map, single, switchMap} from 'rxjs/operators';
import {DateTime, Duration} from 'luxon';
import {LocalizationService} from '../../../shared/services/internationalization/localization.service';
import {ActivatedRoute, Router} from '@angular/router';
import {HttpClient} from '@angular/common/http';
import {Observable, Subject, Subscription, timer} from 'rxjs';
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
  workTime = 0;
  workDays: TimeSheetDayGroupDto[] = [];
  omittedEntities = 0;
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
  public allowUpdate = true;

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
    private localizationService: LocalizationService,
    private route: ActivatedRoute,
    private router: Router,
    private httpClient: HttpClient,
    private storageService: StorageService,
    private utilityService: UtilityService,
  ) {
    const defaultEndDate = DateTime.now().startOf('day');
    const defaultStartDate = defaultEndDate.startOf('month');

    const filterChanged = this.filterChanged
      .pipe(
        filter(() => this.allowUpdate),
        switchMap(filter => this.loadData(filter)),
        this.entityService.withUpdatesFrom(this.entityService.timesheetChanged, this.timeSheetService),
        switchMap(timeSheets => (timer(0, 5000)).pipe(map(() => timeSheets))),
        map(timeSheets => this.createTimeSheetOverview(timeSheets)),
      )
      .subscribe(timeSheets => this.overview = timeSheets);

    this.subscriptions.add(filterChanged);

    this.filters = [
      {name: 'timeSheetStartDate', resettable: false, defaultValue: defaultStartDate},
      {name: 'timeSheetEndDate', resettable: false, defaultValue: defaultEndDate},
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

  private loadData(filter: FilteredRequestParams): Observable<TimeSheetListDto[]> {
    return this.timeSheetService.getListFiltered(filter)
      .pipe(single());
  }

  private createTimeSheetOverview(timeSheets: TimeSheetListDto[]): TimeSheetOverviewDto {
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

    const overallWorkTime = timeSheetWorkDayGroups.reduce((duration, timeSheet) => duration.plus(timeSheet.workTime), Duration.fromMillis(0));
    const workDaysToDisplay = this.limitWorkDaysToDisplay(timeSheetWorkDayGroups, 50);
    const omittedEntities = timeSheetWorkDayGroups.length - workDaysToDisplay.length;

    return {
      workTime: overallWorkTime.as('hours') / 8,
      workDays: workDaysToDisplay,
      omittedEntities: omittedEntities
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
