import {AfterViewInit, Component, EventEmitter, OnDestroy, Output} from '@angular/core';
import {combineLatest, merge, Observable, Subscription} from 'rxjs';
import {StringTypeaheadDto, TypeaheadService} from '../../../shared/services/api';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {DateTime} from 'luxon';
import {ActivatedRoute} from '@angular/router';
import {StorageService} from '../../../shared/services/storage/storage.service';
import {map, shareReplay, startWith, tap} from 'rxjs/operators';

export class TimeSheetFilterDto {
  constructor(
    public startDate: DateTime,
    public endDate: DateTime,
    public projectCustomerId?: string,
    public projectId?: string,
    public activityId?: string,
    public orderId?: string,
    public issue?: string,
    public comment?: string,
  ) {}
}

@Component({
  selector: 'ts-timesheet-filter',
  templateUrl: './timesheet-filter.component.html',
  styleUrls: ['./timesheet-filter.component.scss']
})
export class TimesheetFilterComponent implements AfterViewInit, OnDestroy {
  @Output() filterChanged = new EventEmitter<TimeSheetFilterDto>();

  public customers$: Observable<StringTypeaheadDto[]>;
  public projects$: Observable<StringTypeaheadDto[]>;
  public orders$: Observable<StringTypeaheadDto[]>;
  public activities$: Observable<StringTypeaheadDto[]>;
  public isFiltered$: Observable<boolean>;
  public filterForm: FormGroup;

  private readonly timeSheetFilterStorageKey = 'timeSheetFilter';
  private readonly onFilterChanged: Observable<TimeSheetFilterDto>;
  private readonly subscriptions = new Subscription();

  constructor(
    private route: ActivatedRoute,
    private storageService: StorageService,
    private formBuilder: FormBuilder,
    typeaheadService: TypeaheadService,
  ) {
    this.customers$ = typeaheadService.getCustomers({});
    this.projects$ = typeaheadService.getProjects({});
    this.orders$ = typeaheadService.getOrders({});
    this.activities$ = typeaheadService.getActivities({});

    this.filterForm = this.createFilterForm();

    this.onFilterChanged = this.filterForm.valueChanges
      .pipe(
        tap(timeSheetFilter => this.storageService.set(this.timeSheetFilterStorageKey, JSON.stringify(timeSheetFilter))),
        startWith(this.filterForm.value as TimeSheetFilterDto),
        map(timeSheetFilter => this.replaceArraysByJoinedValues(timeSheetFilter)),
        map(timeSheetFilter => this.replaceEmptyByNull(timeSheetFilter)),
        shareReplay(1),
      );

    const requiredFilters = ['startDate', 'startMonth', 'endDate', 'endMonth'];
    this.isFiltered$ = this.onFilterChanged
      .pipe(map((timeSheetFilter: TimeSheetFilterDto) =>
        Object.entries(timeSheetFilter).some(([key, value]) =>
          !requiredFilters.includes(key) &&
          value &&
          value?.length !== 0
        )
      ));
  }

  public ngAfterViewInit(): void {
    const filterChangedEmitter = this.onFilterChanged.subscribe(timeSheetFilter => this.filterChanged.emit(timeSheetFilter));
    this.subscriptions.add(filterChangedEmitter);
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  public clearFilterForm(): void {
    this.filterForm.patchValue({
      projectCustomerId: undefined,
      projectId: undefined,
      activityId: undefined,
      orderId: undefined,
      issue: undefined,
      comment: undefined,
    });
  }

  public changeValue($event: Event, formControlName: string): void {
    this.filterForm.controls[formControlName].setValue(($event.target as HTMLInputElement).value);
  }

  private createFilterForm(): FormGroup {
    const queryParameterMap = this.route.snapshot.queryParamMap;
    const timeSheetFilter = JSON.parse(this.storageService.get(this.timeSheetFilterStorageKey, '{}'));

    let endDate = DateTime.fromISO(queryParameterMap.get('endDate') ?? timeSheetFilter.endDate ?? 'INVALID');
    if (!endDate.isValid)
      endDate = DateTime.now().startOf('day');

    let startDate = DateTime.fromISO(queryParameterMap.get('startDate') ?? timeSheetFilter.startDate ?? 'INVALID');
    if (!startDate.isValid)
      startDate = endDate.startOf('month');

    const filterForm = this.formBuilder.group(
      {
        startDate: [startDate, Validators.required],
        startMonth: [startDate, Validators.required],
        endDate: [endDate, Validators.required],
        endMonth: [endDate, Validators.required],
        projectCustomerId: [timeSheetFilter.projectCustomerId],
        projectId: [timeSheetFilter.projectId],
        activityId: [timeSheetFilter.activityId],
        orderId: [timeSheetFilter.orderId],
        issue: [timeSheetFilter.issue, {updateOn: 'blur'}],
        comment: [timeSheetFilter.comment, {updateOn: 'blur'}],
      },
    );

    const formControls = filterForm.controls;

    const startDateChanged = formControls['startDate'].valueChanges.subscribe((newStartDate: DateTime) => {
      formControls['startMonth'].setValue(newStartDate, {emitEvent: false})
      if (filterForm.value.endDate < newStartDate)
        formControls['endDate'].setValue(newStartDate);
    });

    const startMonthChanged = formControls['startMonth'].valueChanges.subscribe((newStartDate: DateTime) => {
      formControls['startDate'].setValue(newStartDate, {emitEvent: false})
      if (filterForm.value.endDate < newStartDate)
        formControls['endDate'].setValue(newStartDate.endOf('month'));
    });

    const endDateChanged = formControls['endDate'].valueChanges.subscribe((newEndDate: DateTime) => {
      formControls['endMonth'].setValue(newEndDate, {emitEvent: false})
      if (filterForm.value.startDate > newEndDate)
        formControls['startDate'].setValue(newEndDate);
    });

    const endMonthChanged = formControls['endMonth'].valueChanges.subscribe((newEndDate: DateTime) => {
      formControls['endDate'].setValue(newEndDate, {emitEvent: false})
      if (filterForm.value.startDate > newEndDate)
        formControls['startDate'].setValue(newEndDate.startOf('month'));
    });

    this.subscriptions.add(startDateChanged);
    this.subscriptions.add(startMonthChanged);
    this.subscriptions.add(endDateChanged);
    this.subscriptions.add(endMonthChanged);

    return filterForm;
  }

  private replaceArraysByJoinedValues(obj: any) {
    const arrayProperties = Object.entries(obj)
      .filter(([key]) => Array.isArray(obj[key]));

    for (const [key, value] of arrayProperties)
      obj[key] = (value as []).join();

    return obj;
  }

  private replaceEmptyByNull(obj: any) {
    const emptyStringProperies = Object.entries(obj)
      .filter(([key, value]) => value === "");

    for (const [key, value] of emptyStringProperies)
      obj[key] = null;

    return obj;
  }
}
