import {AfterViewInit, Component, EventEmitter, OnDestroy, Output} from '@angular/core';
import {Observable, Subscription} from 'rxjs';
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
        shareReplay(1),
      );

    this.isFiltered$ = this.onFilterChanged
      .pipe(map((timeSheetFilter: TimeSheetFilterDto) =>
        Object.entries(timeSheetFilter).some(([key, value]) => key !== 'startDate' && key !== 'endDate' && value && value?.length !== 0)
      ));
  }

  ngAfterViewInit(): void {
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
        endDate: [endDate, Validators.required],
        projectCustomerId: [timeSheetFilter.projectCustomerId],
        projectId: [timeSheetFilter.projectId],
        activityId: [timeSheetFilter.activityId],
        orderId: [timeSheetFilter.orderId],
        issue: [timeSheetFilter.issue, {updateOn: 'blur'}],
        comment: [timeSheetFilter.comment, {updateOn: 'blur'}],
      },
    );

    const startDateChanged = filterForm.controls['startDate'].valueChanges.subscribe(newStartDate => {
      if (filterForm.value.endDate < newStartDate)
        filterForm.controls['endDate'].setValue(newStartDate);
    });

    const endDateChanged = filterForm.controls['endDate'].valueChanges.subscribe(newEndDate => {
      if (filterForm.value.startDate > newEndDate)
        filterForm.controls['startDate'].setValue(newEndDate);
    });

    this.subscriptions.add(startDateChanged);
    this.subscriptions.add(endDateChanged);

    return filterForm;
  }
}
