import {AfterViewInit, ChangeDetectorRef, Component, EventEmitter, Input, OnDestroy, Output, TemplateRef, ViewChild} from '@angular/core';
import {map, Observable, shareReplay, Subscription, tap} from 'rxjs';
import {StringTypeaheadDto, TimeSheetGetListFilteredRequestParams, TypeaheadService} from '../../services/api';
import {AbstractControl, FormBuilder, FormGroup} from '@angular/forms';
import {DateTime} from 'luxon';
import {ActivatedRoute, ParamMap} from '@angular/router';
import {StorageService} from '../../services/storage/storage.service';
import {DateParserService} from '../../services/date-parser.service';

export type FilteredRequestParams = TimeSheetGetListFilteredRequestParams;
export type FilterName = keyof FilteredRequestParams;

export interface Filter {
  name: FilterName;
  showHidden?: boolean;
  resettable?: boolean;
  defaultValue?: any;
}

type FilterTemplates = Record<FilterName, TemplateRef<any>>;
type FilterControlName = keyof FilterControls;
type FilterControls =
  Record<keyof FilteredRequestParams, AbstractControl>
  & {
  timeSheetStartMonth: AbstractControl;
  timeSheetEndMonth: AbstractControl,
  orderStartMonth: AbstractControl,
  orderDueMonth: AbstractControl,
  holidayStartMonth: AbstractControl,
  holidayEndMonth: AbstractControl,
};

@Component({
  selector: 'ts-filter',
  templateUrl: './filter.component.html',
  styleUrls: ['./filter.component.scss']
})
export class TimesheetFilterComponent implements AfterViewInit, OnDestroy {
  @Input()
  public set filters(filters: (FilterName | Filter)[]) {
    this._filters = filters.map(x => typeof x === 'string' ? {name: x} : x);
    this.updateFilterSources(this._filters);
    this.applyFilterValues(this._filters);
  }

  @Output() public filterChanged = new EventEmitter<FilteredRequestParams>();

  @ViewChild('timeSheetStartDate') private timeSheetStartDate!: TemplateRef<any>;
  @ViewChild('timeSheetEndDate') private timeSheetEndDate!: TemplateRef<any>;
  @ViewChild('customerId') private customerId!: TemplateRef<any>;
  @ViewChild('customerNumber') private customerNumber!: TemplateRef<any>;
  @ViewChild('customerCompanyName') private customerCompanyName!: TemplateRef<any>;
  @ViewChild('customerHidden') private customerHidden!: TemplateRef<any>;
  @ViewChild('projectId') private projectId!: TemplateRef<any>;
  @ViewChild('projectHidden') private projectHidden!: TemplateRef<any>;
  @ViewChild('activityId') private activityId!: TemplateRef<any>;
  @ViewChild('orderStartDate') private orderStartDate!: TemplateRef<any>;
  @ViewChild('orderDueDate') private orderDueDate!: TemplateRef<any>;
  @ViewChild('orderId') private orderId!: TemplateRef<any>;
  @ViewChild('timeSheetIssue') private timeSheetIssue!: TemplateRef<any>;
  @ViewChild('timeSheetComment') private timeSheetComment!: TemplateRef<any>;
  @ViewChild('activityHidden') private activityHidden!: TemplateRef<any>;
  @ViewChild('holidayStartDate') private holidayStartDate!: TemplateRef<any>;
  @ViewChild('holidayEndDate') private holidayEndDate!: TemplateRef<any>;
  @ViewChild('holidayTitle') private holidayTitle!: TemplateRef<any>;
  @ViewChild('holidayType') private holidayType!: TemplateRef<any>;
  @ViewChild('filterNotImplemented') private filterNotImplemented!: TemplateRef<any>;

  public get primaryFilters(): Filter[] | undefined { return this._filters?.slice(0, 4); }

  public get secondaryFilters(): Filter[] | undefined { return this._filters?.slice(4); }

  public _filters?: Filter[];
  public customers$?: Observable<StringTypeaheadDto[]>;
  public projects$?: Observable<StringTypeaheadDto[]>;
  public orders$?: Observable<StringTypeaheadDto[]>;
  public activities$?: Observable<StringTypeaheadDto[]>;
  public isFiltered$: Observable<boolean>;
  public filterForm: FormGroup;
  public filterTemplates?: FilterTemplates;

  private readonly booleanTrueRegex = /^\s*(true|1|on)\s*$/i;
  private readonly filterStorageKey = 'timeSheetFilter';
  private readonly onFilterChanged: Observable<any>;
  private readonly subscriptions = new Subscription();

  constructor(
    private route: ActivatedRoute,
    private storageService: StorageService,
    private formBuilder: FormBuilder,
    private changeDetector: ChangeDetectorRef,
    private typeaheadService: TypeaheadService,
    private dateParserService: DateParserService,
  ) {
    this.filterForm = this.createFilterForm();

    this.onFilterChanged = this.filterForm.valueChanges
      .pipe(
        map(filter => this.unifyEmptyValues(filter)),
        tap(filter => this.saveFilterFormValue(filter)),
        shareReplay(1),
      );

    this.isFiltered$ = this.onFilterChanged.pipe(map(filter => this.isFiltered(filter)));

    const filterChangedEmitter = this.onFilterChanged
      .pipe(map(timeSheetFilter => this.convertToFilteredRequestParams(timeSheetFilter)))
      .subscribe(timeSheetFilter => this.filterChanged.emit(timeSheetFilter));
    this.subscriptions.add(filterChangedEmitter);
  }

  public ngAfterViewInit(): void {
    this.filterTemplates = {
      timeSheetId: this.filterNotImplemented,
      timeSheetIssue: this.timeSheetIssue,
      timeSheetStartDate: this.timeSheetStartDate,
      timeSheetEndDate: this.timeSheetEndDate,
      timeSheetBillable: this.filterNotImplemented,
      timeSheetComment: this.timeSheetComment,
      projectId: this.projectId,
      projectTitle: this.filterNotImplemented,
      projectComment: this.filterNotImplemented,
      projectHidden: this.projectHidden,
      customerId: this.customerId,
      customerTitle: this.filterNotImplemented,
      customerNumber: this.customerNumber,
      customerDepartment: this.filterNotImplemented,
      customerCompanyName: this.customerCompanyName,
      customerContactName: this.filterNotImplemented,
      customerHourlyRate: this.filterNotImplemented,
      customerStreet: this.filterNotImplemented,
      customerZipCode: this.filterNotImplemented,
      customerCity: this.filterNotImplemented,
      customerCountry: this.filterNotImplemented,
      customerComment: this.filterNotImplemented,
      customerHidden: this.customerHidden,
      activityId: this.activityId,
      activityTitle: this.filterNotImplemented,
      activityComment: this.filterNotImplemented,
      activityHidden: this.activityHidden,
      orderId: this.orderId,
      orderTitle: this.filterNotImplemented,
      orderDescription: this.filterNotImplemented,
      orderNumber: this.filterNotImplemented,
      orderStartDate: this.orderStartDate,
      orderDueDate: this.orderDueDate,
      orderHourlyRate: this.filterNotImplemented,
      orderBudget: this.filterNotImplemented,
      orderComment: this.filterNotImplemented,
      orderHidden: this.filterNotImplemented,
      holidayId: this.filterNotImplemented,
      holidayTitle: this.holidayTitle,
      holidayStartDate: this.holidayStartDate,
      holidayEndDate: this.holidayEndDate,
      holidayType: this.holidayType,
    };

    this.changeDetector.detectChanges();
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  public resetFilterForm(): void {
    if (!this._filters)
      return;

    const nonResettableFilter: FilterControlName[] = ['timeSheetStartMonth', 'timeSheetEndMonth', 'orderStartMonth', 'orderDueMonth', 'holidayStartMonth', 'holidayEndMonth'];
    const resettableFilterNames = this._filters
      .filter(x => x.resettable !== false || nonResettableFilter.includes(x.name))
      .map(x => x.name as string);

    const filterToClear = this._filters
      .filter(filter => resettableFilterNames.includes(filter.name))
      .map(filter => [filter.name, filter.defaultValue]);

    const defaultFilter = Object.fromEntries(filterToClear);
    this.filterForm.patchValue(defaultFilter);
  }

  public isResettable(filterName: FilterName): boolean {
    return this._filters?.filter(x => x.name === filterName)[0]?.resettable !== false;
  }

  public setFormValue($event: Event, formControlName: FilterName): void {
    this.filterForm.controls[formControlName].setValue(($event.target as HTMLInputElement).value);
  }

  private createFilterForm(): FormGroup {
    const filter = this.getInitialFilterValues();

    const updateOnBlurFilter: FilterName[] = ['timeSheetIssue', 'timeSheetComment', 'projectTitle', 'projectComment', 'customerTitle', 'customerNumber', 'customerDepartment', 'customerCompanyName', 'customerContactName', 'customerStreet', 'customerZipCode', 'customerCity', 'customerCountry', 'activityTitle', 'activityComment', 'orderTitle', 'orderDescription', 'orderNumber', 'orderHourlyRate', 'orderBudget', 'orderComment', 'holidayTitle'];
    const formControlsConfigMap = Object.entries(filter)
      .map(([filterName, value]) => [filterName, [value, {updateOn: updateOnBlurFilter.includes(filterName as FilterName) ? 'blur' : 'change'}]]);
    const formControlsConfig = Object.fromEntries(formControlsConfigMap);
    const filterForm = this.formBuilder.group(formControlsConfig);

    const formControls = filterForm.controls as FilterControls;
    this.registerDateSync(formControls, 'timeSheetStartDate', 'timeSheetEndDate', 'timeSheetStartMonth', 'timeSheetEndMonth');
    this.registerDateSync(formControls, 'orderStartDate', 'orderDueDate', 'orderStartMonth', 'orderDueMonth');
    this.registerDateSync(formControls, 'holidayStartDate', 'holidayEndDate', 'holidayStartMonth', 'holidayEndMonth');

    return filterForm;
  }

  private getInitialFilterValues(): Record<FilterControlName, any> {
    const emptyFilter = this.getEmptyFilter();
    const savedFilter = this.loadSavedFilter();
    const queryFilter = this.getFilterFromQuery(emptyFilter);
    return {...emptyFilter, ...savedFilter, ...queryFilter};
  }

  private getEmptyFilter(): Record<FilterControlName, undefined> {
    return {
      timeSheetId: undefined,
      timeSheetIssue: undefined,
      timeSheetStartDate: undefined,
      timeSheetStartMonth: undefined,
      timeSheetEndDate: undefined,
      timeSheetEndMonth: undefined,
      timeSheetBillable: undefined,
      timeSheetComment: undefined,
      projectId: undefined,
      projectTitle: undefined,
      projectComment: undefined,
      projectHidden: undefined,
      customerId: undefined,
      customerTitle: undefined,
      customerNumber: undefined,
      customerDepartment: undefined,
      customerCompanyName: undefined,
      customerContactName: undefined,
      customerHourlyRate: undefined,
      customerStreet: undefined,
      customerZipCode: undefined,
      customerCity: undefined,
      customerCountry: undefined,
      customerComment: undefined,
      customerHidden: undefined,
      activityId: undefined,
      activityTitle: undefined,
      activityComment: undefined,
      activityHidden: undefined,
      orderId: undefined,
      orderTitle: undefined,
      orderDescription: undefined,
      orderNumber: undefined,
      orderStartDate: undefined,
      orderStartMonth: undefined,
      orderDueDate: undefined,
      orderDueMonth: undefined,
      orderHourlyRate: undefined,
      orderBudget: undefined,
      orderComment: undefined,
      orderHidden: undefined,
      holidayId: undefined,
      holidayTitle: undefined,
      holidayStartDate: undefined,
      holidayStartMonth: undefined,
      holidayEndDate: undefined,
      holidayEndMonth: undefined,
      holidayType: undefined,
    }
  }

  private loadSavedFilter(): Record<FilterControlName, any> {
    const rawSavedFilter = JSON.parse(this.storageService.get(this.filterStorageKey, '{}'));
    return this.dateParserService.convertJsStringsToLuxon(rawSavedFilter);
  }

  private getFilterFromQuery(emptyFilter: Record<FilterControlName, undefined>): Partial<Record<FilterControlName, any>> {
    const queryParameterMap: ParamMap = this.route.snapshot.queryParamMap;
    const booleanFilters: FilterName[] = ['timeSheetBillable', 'projectHidden', 'customerHidden', 'activityHidden', 'orderHidden'];
    const numericFilters: FilterName[] = ['customerHourlyRate', 'orderHourlyRate', 'orderBudget'];
    const dateTimeFilters: FilterName[] = ['timeSheetStartDate', 'timeSheetEndDate', 'orderStartDate', 'orderDueDate', 'holidayStartDate', 'holidayEndDate'];

    const filterableQueryParams = Object
      .entries(emptyFilter)
      .filter(([key]) => queryParameterMap.has(key))
      .map(([key]) => ({key: key as FilterName, value: queryParameterMap.get(key) as string}))
      .map(filter => {
        if (booleanFilters.includes(filter.key))
          return [filter.key, this.booleanTrueRegex.test(filter.value)];
        else if (numericFilters.includes(filter.key))
          return [filter.key, parseFloat(filter.value)];
        else if (dateTimeFilters.includes(filter.key))
          return [filter.key, DateTime.fromISO(filter.value)];
        else
          return [filter.key, filter.value];
      });

    return Object.fromEntries(filterableQueryParams);
  }

  private unifyEmptyValues(filter: Record<FilterControlName, any>): Record<FilterControlName, any> {
    for (const name of Object.keys(filter)) {
      const filterName = name as FilterControlName;
      const hasEmptyValue =
        filter[filterName] === undefined ||
        filter[filterName] === null ||
        filter[filterName] === '' ||
        (Array.isArray(filter[filterName]) && filter[filterName].length === 0);

      if (hasEmptyValue)
        filter[filterName] = this._filters?.find(x => x.name === filterName)?.defaultValue === undefined ? undefined : null;
    }

    return filter;
  }

  private saveFilterFormValue(filter: any) {
    this.storageService.set(this.filterStorageKey, JSON.stringify(filter))
  }

  private convertToFilteredRequestParams(filter: Record<FilterControlName, any>): FilteredRequestParams {
    if (!this._filters)
      return {};

    const filterToUse = this._filters.map(x => x.name) as any;
    const filterRequestParams = Object.entries(filter)
      .filter((([name]) => filterToUse.includes(name)))
      .map(([name, value]) => {
        if (value === '' || value == null)
          return [name, undefined];
        if (Array.isArray(value))
          return [name, value.length > 0 ? value.join() : undefined];
        if (typeof value === 'boolean')
          return [name, value ? 'true' : 'false'];
        if (typeof value === 'number')
          return [name, value.toFixed(15)];
        if (name === 'timeSheetStartDate')
          return ['timeSheetEndDate', `>=${filter.timeSheetStartDate.toISO()},ISNULL`];
        if (name === 'timeSheetEndDate')
          return ['timeSheetStartDate', `<${filter.timeSheetEndDate.toISO()}`];
        if (name === 'orderStartDate')
          return ['orderDueDate', `>=${filter.orderStartDate.toISO()}`];
        if (name === 'orderDueDate')
          return ['orderStartDate', `<${filter.orderDueDate.toISO()}`];
        if (name === 'holidayStartDate')
          return ['holidayEndDate', `>=${filter.holidayStartDate.toISO()}`];
        if (name === 'holidayEndDate')
          return ['holidayStartDate', `<${filter.holidayEndDate.toISO()}`];
        return [name, value];
      })
      .filter(([, value]) => value !== undefined);

    return Object.fromEntries(filterRequestParams);
  }

  private isFiltered(filter: any): boolean {
    if (!this._filters)
      return false;

    return this._filters
      .filter(x => x.resettable !== false && filter[x.name] !== x.defaultValue)
      .length > 0;
  }

  private registerDateSync(formControls: FilterControls, fromDate: FilterControlName, toDate: FilterControlName, fromMonth: FilterControlName, toMonth: FilterControlName) {
    const startDateChanged = formControls[fromDate].valueChanges.subscribe((newStartDate: DateTime) => {
      formControls[fromMonth].setValue(newStartDate, {emitEvent: false})
      if (formControls[toDate].value && formControls[toDate].value < newStartDate)
        formControls[toDate].setValue(newStartDate);
    });

    const startMonthChanged = formControls[fromMonth].valueChanges.subscribe((newStartDate: DateTime) => {
      formControls[fromDate].setValue(newStartDate, {emitEvent: false})
      if (formControls[toDate].value && formControls[toDate].value < newStartDate)
        formControls[toDate].setValue(newStartDate.endOf('month'));
    });

    const endDateChanged = formControls[toDate].valueChanges.subscribe((newEndDate: DateTime) => {
      formControls[toMonth].setValue(newEndDate, {emitEvent: false})
      if (formControls[fromDate].value && formControls[fromDate].value > newEndDate)
        formControls[fromDate].setValue(newEndDate);
    });

    const endMonthChanged = formControls[toMonth].valueChanges.subscribe((newEndDate: DateTime) => {
      formControls[toDate].setValue(newEndDate, {emitEvent: false})
      if (formControls[fromDate].value && formControls[fromDate].value > newEndDate)
        formControls[fromDate].setValue(newEndDate.startOf('month'));
    });

    this.subscriptions.add(startDateChanged);
    this.subscriptions.add(startMonthChanged);
    this.subscriptions.add(endDateChanged);
    this.subscriptions.add(endMonthChanged);
  }

  private updateFilterSources(filters: Filter[]) {
    for (const filter of filters) {
      switch (filter.name) {
        case 'customerId':
          this.customers$ = this.typeaheadService.getCustomers({showHidden: filter.showHidden});
          break;
        case 'projectId':
          this.projects$ = this.typeaheadService.getProjects({showHidden: filter.showHidden});
          break;
        case 'orderId':
          this.orders$ = this.typeaheadService.getOrders({showHidden: filter.showHidden});
          break;
        case 'activityId':
          this.activities$ = this.typeaheadService.getActivities({showHidden: filter.showHidden});
          break;
      }
    }
  }

  private applyFilterValues(filters: Filter[]) {
    const initialFilterValues = this.getInitialFilterValues();
    const defaultValues = filters
      .filter(filter => filter.defaultValue !== undefined) // Filters having default value
      .filter(filter => initialFilterValues[filter.name] === undefined)
      .map(filter => [filter.name, filter.defaultValue]);

    this.filterForm.patchValue(Object.fromEntries(defaultValues));
  }
}
