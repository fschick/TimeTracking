import {AfterViewInit, ChangeDetectorRef, Component, EventEmitter, Input, OnDestroy, Output, TemplateRef, ViewChild} from '@angular/core';
import {Observable, shareReplay, Subscription, map, tap} from 'rxjs';
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
  required?: boolean;
  defaultValue?: any;
}

type FilterTemplates = { [key in FilterName]: TemplateRef<any> };
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
  private readonly onFilterChanged: Observable<FilteredRequestParams>;
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
    this.updateFilterFromQueryParams();

    this.onFilterChanged = this.filterForm.valueChanges
      .pipe(
        tap(filter => this.saveFilterFormValue(filter)),
        map(timeSheetFilter => this.convertToFilteredRequestParams(timeSheetFilter)),
        shareReplay(1),
      );

    const filterChangedEmitter = this.onFilterChanged.subscribe(timeSheetFilter => this.filterChanged.emit(timeSheetFilter));
    this.subscriptions.add(filterChangedEmitter);

    this.isFiltered$ = this.onFilterChanged.pipe(map(filter => this.isFiltered(filter)));
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
      customerStreet: this.filterNotImplemented,
      customerZipCode: this.filterNotImplemented,
      customerCity: this.filterNotImplemented,
      customerCountry: this.filterNotImplemented,
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

  private createFilterForm(): FormGroup {
    const filter = this.loadFilterFormValue();

    const updateOnBlurFilter: FilterName[] = ['timeSheetIssue', 'timeSheetComment', 'projectTitle', 'projectComment', 'customerTitle', 'customerNumber', 'customerDepartment', 'customerCompanyName', 'customerContactName', 'customerStreet', 'customerZipCode', 'customerCity', 'customerCountry', 'activityTitle', 'activityComment', 'orderTitle', 'orderDescription', 'orderNumber', 'orderHourlyRate', 'orderBudget', 'orderComment', 'holidayTitle'];
    const formControlsConfigMap = Object.entries(filter)
      .map(([key, value]) => [key, [value, {updateOn: updateOnBlurFilter.some(x => x === key) ? 'blur' : 'change'}]]);
    const formControlsConfig: Record<FilterControlName, [0]> = Object.fromEntries(formControlsConfigMap);

    const filterForm = this.formBuilder.group(formControlsConfig);
    const formControls = filterForm.controls as FilterControls;

    this.registerDateSync(formControls, 'timeSheetStartDate', 'timeSheetEndDate', 'timeSheetStartMonth', 'timeSheetEndMonth');
    this.registerDateSync(formControls, 'orderStartDate', 'orderDueDate', 'orderStartMonth', 'orderDueMonth');
    this.registerDateSync(formControls, 'holidayStartDate', 'holidayEndDate', 'holidayStartMonth', 'holidayEndMonth');

    return filterForm;
  }

  public clearFilterForm(): void {
    if (!this._filters)
      return;

    const nonClearableFilter: FilterControlName[] = ['timeSheetStartMonth', 'timeSheetEndMonth', 'orderStartMonth', 'orderDueMonth', 'holidayStartMonth', 'holidayEndMonth'];
    const requiredFilters = this._filters.filter(x => x.required).map(x => x.name as string);
    const filterToClear = Object.keys(this.filterForm.value).filter(name => !requiredFilters.includes(name) && !nonClearableFilter.includes(name as FilterControlName));

    const emptyFilter: { [name: string]: any } = {};
    for (const key of filterToClear)
      emptyFilter[key] = null;
    this.filterForm.patchValue(emptyFilter);
  }

  public isRequired(filterName: FilterName): boolean {
    return !this._filters?.filter(x => x.name === filterName)[0]?.required;
  }

  public changeValue($event: Event, formControlName: FilterName): void {
    this.filterForm.controls[formControlName].setValue(($event.target as HTMLInputElement).value);
  }

  private convertToFilteredRequestParams(filter: any): FilteredRequestParams {
    if (!this._filters)
      return {};

    const filterToUse = this._filters.map(x => x.name) as any;
    const filterRequestParams = Object.entries(filter)
      .filter((([name]) => filterToUse.includes(name)))
      .map(([name, value]) => {
        if (value === '')
          return [name, undefined];
        if (Array.isArray(value))
          return [name, value.length > 0 ? value.join() : undefined];
        if (typeof value === 'boolean')
          return [name, value ? 'true' : 'false'];
        if (typeof value === 'number')
          return [name, value.toFixed(15)];
        if (name === 'timeSheetStartDate')
          return this.createDateFilterParam(filter, name, 'timeSheetStartDate', 'timeSheetEndDate');
        if (name === 'timeSheetEndDate')
          return [name, undefined];
        if (name === 'orderStartDate')
          return this.createDateFilterParam(filter, name, 'orderStartDate', 'orderDueDate');
        if (name === 'orderDueDate')
          return [name, undefined];
        if (name === 'holidayStartDate')
          return this.createDateFilterParam(filter, name, 'holidayStartDate', 'holidayEndDate');
        if (name === 'holidayEndDate')
          return [name, undefined];
        return [name, value];
      });

    return Object.fromEntries(filterRequestParams);
  }

  private createDateFilterParam(filter: any, name: string, from: string, to: string): [string, string | undefined] {
    if (filter[from] && filter[to])
      return [name, `${filter[from].toFormat('yyyy-MM-ddZZ')}_${filter[to].toFormat('yyyy-MM-ddZZ')}`];
    if (filter[from])
      return [name, `>=${filter[from].toFormat('yyyy-MM-ddZZ')}`];
    if (filter[to])
      return [name, `<${filter[to].toFormat('yyyy-MM-ddZZ')}`];
    return [name, undefined];
  }

  private saveFilterFormValue(filter: any) {
    this.storageService.set(this.filterStorageKey, JSON.stringify(filter))
  }

  private loadFilterFormValue(): Record<FilterControlName, any> {
    const emptyFilter = this.getEmptyFilter();
    const rawFilter = JSON.parse(this.storageService.get(this.filterStorageKey, '{}'));
    const savedFilter = this.dateParserService.convertJsStringsToLuxon(rawFilter);
    return {...emptyFilter, ...savedFilter};
  }

  private isFiltered(filter: FilteredRequestParams): boolean {
    if (!this._filters)
      return false;

    const requiredFilters = this._filters.filter(x => x.required).map(x => x.name);
    return Object.entries(filter)
      .some(([name, value]: [any, string]) =>
        !requiredFilters.includes(name) &&
        value && value?.length !== 0
      );
  }

  private updateFilterFromQueryParams(): void {
    const queryParameterMap: ParamMap = this.route.snapshot.queryParamMap;
    const formControls = this.filterForm.controls as FilterControls;

    for (const property of Object.entries(formControls)) {
      const key = property[0] as FilterControlName;
      if (!queryParameterMap.has(key))
        continue;

      const queryParamValue = queryParameterMap.get(key);
      const filterValue = property[1].value;

      if (queryParamValue === '')
        formControls[key]?.patchValue(null);
      else if (typeof filterValue === 'number' && queryParamValue)
        formControls[key]?.patchValue(parseFloat(queryParamValue));
      else if (typeof filterValue === 'boolean' && queryParamValue)
        formControls[key]?.patchValue(this.booleanTrueRegex.test(queryParamValue));
      else if (filterValue instanceof DateTime && queryParamValue)
        formControls[key]?.patchValue(DateTime.fromISO(queryParamValue));
    }
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

  private getEmptyFilter(): Record<FilterControlName, null> {
    return {
      timeSheetId: null,
      timeSheetIssue: null,
      timeSheetStartDate: null,
      timeSheetStartMonth: null,
      timeSheetEndDate: null,
      timeSheetEndMonth: null,
      timeSheetBillable: null,
      timeSheetComment: null,
      projectId: null,
      projectTitle: null,
      projectComment: null,
      projectHidden: null,
      customerId: null,
      customerTitle: null,
      customerNumber: null,
      customerDepartment: null,
      customerCompanyName: null,
      customerContactName: null,
      customerStreet: null,
      customerZipCode: null,
      customerCity: null,
      customerCountry: null,
      customerHidden: null,
      activityId: null,
      activityTitle: null,
      activityComment: null,
      activityHidden: null,
      orderId: null,
      orderTitle: null,
      orderDescription: null,
      orderNumber: null,
      orderStartDate: null,
      orderStartMonth: null,
      orderDueDate: null,
      orderDueMonth: null,
      orderHourlyRate: null,
      orderBudget: null,
      orderComment: null,
      orderHidden: null,
      holidayId: null,
      holidayTitle: null,
      holidayStartDate: null,
      holidayStartMonth: null,
      holidayEndDate: null,
      holidayEndMonth: null,
      holidayType: null,
    }
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
    const formControls = this.filterForm.controls;
    const defaultValues = filters
      .filter(filter => filter.defaultValue !== undefined) // Filters having default value
      .filter(filter => {
        const currentValue = formControls[filter.name]?.value;
        return currentValue === '' || currentValue === undefined || currentValue === null
      })
      .map(filter => [filter.name, filter.defaultValue]);

    this.filterForm.patchValue(Object.fromEntries(defaultValues));
  }
}
