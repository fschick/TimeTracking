import {AfterViewInit, Component, OnDestroy, TemplateRef, ViewChild} from '@angular/core';
import {FormValidationService, ValidationFormGroup} from '../../../../../core/app/services/form-validation/form-validation.service';
import {ActivatedRoute, Router} from '@angular/router';
import {GuidStringTypeaheadDto, TimeSheetDto, TimeSheetService, TypeaheadService} from '../../../../../api/timetracking';
import {EntityService} from '../../../../../core/app/services/state-management/entity.service';
import {GuidService} from '../../../../../core/app/services/state-management/guid.service';
import {filter, map, pairwise, single, startWith} from 'rxjs/operators';
import {forkJoin, of, Subscription} from 'rxjs';
import {DateTime} from 'luxon';
import {FormControl} from '@angular/forms';
import {NgbModal, NgbModalRef} from '@ng-bootstrap/ng-bootstrap';
import {ConfigurationService} from '../../../../../core/app/services/configuration.service';
import {AuthenticationService} from '../../../../../core/app/services/authentication.service';

@Component({
  selector: 'ts-timesheet-edit',
  templateUrl: './timesheet-edit.component.html',
  styleUrls: ['./timesheet-edit.component.scss']
})
export class TimesheetEditComponent implements AfterViewInit, OnDestroy {
  public allCustomers: GuidStringTypeaheadDto[] = [];
  public selectableProjects: GuidStringTypeaheadDto[] = [];
  public selectableActivities: GuidStringTypeaheadDto[] = [];
  public selectableOrders: GuidStringTypeaheadDto[] = [];
  public allUsers: GuidStringTypeaheadDto[] = [];
  public timesheetForm: ValidationFormGroup;
  public isNewRecord: boolean;
  public isReadOnly: boolean;
  public isUserReadonly: boolean;

  public authorizationEnabled: boolean;

  private allProjects: GuidStringTypeaheadDto[] = [];
  private allActivities: GuidStringTypeaheadDto[] = [];
  private allOrders: GuidStringTypeaheadDto[] = [];

  @ViewChild('timesheetEdit') private timesheetEdit?: TemplateRef<any>;

  private modal?: NgbModalRef
  private subscriptions = new Subscription();

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private timesheetService: TimeSheetService,
    private entityService: EntityService,
    private formValidationService: FormValidationService,
    private typeaheadService: TypeaheadService,
    private configurationService: ConfigurationService,
    private authenticationService: AuthenticationService,
    private modalService: NgbModal,
  ) {
    this.isNewRecord = this.route.snapshot.params['id'] === GuidService.guidEmpty;
    this.isReadOnly = !authenticationService.currentUser.hasRole.timeSheetManage;
    this.authorizationEnabled = this.configurationService.clientConfiguration.features.authorization;
    this.isUserReadonly = !authenticationService.currentUser.hasRole.foreignDataManage;
    this.timesheetForm = this.createTimesheetForm();

    const getTimeSheet = !this.isNewRecord ? timesheetService.get({id: this.route.snapshot.params['id']}) : of(undefined);
    const getCustomers = typeaheadService.getCustomers({});
    const getProjects = typeaheadService.getProjects({});
    const getActivities = typeaheadService.getActivities({});
    const getOrders = typeaheadService.getOrders({});
    const getUsers = this.authorizationEnabled ? typeaheadService.getUsers({}) : of([]);

    forkJoin([getTimeSheet, getCustomers, getProjects, getActivities, getOrders, getUsers])
      .subscribe(([timesheet, customers, projects, activities, orders, users]) => {
        this.allCustomers = customers;
        this.allActivities = activities;
        this.allProjects = projects;
        this.allOrders = orders;
        this.allUsers = users;

        if (timesheet) {
          this.isReadOnly = timesheet.isReadonly ?? false;
          this.timesheetForm.patchValue(timesheet);
        }

        this.setSelectable(timesheet?.customerId);
        this.setTimeEndToNowAfterEndDateWasSet(timesheet?.endDate);
      });
  }

  public ngAfterViewInit(): void {
    this.modal = this.modalService.open(this.timesheetEdit, {size: 'lg', scrollable: true});
    this.modal.hidden.pipe(single()).subscribe(() => this.router.navigate(['..'], {relativeTo: this.route}));
  }

  public selectedCustomerChanged(customer?: GuidStringTypeaheadDto): void {
    this.setSelectable(customer?.id);
  }

  public selectedActivityChanged(activity?: GuidStringTypeaheadDto): void {
    if (!activity?.extended.customerId)
      return;

    this.timesheetForm.patchValue({customerId: activity.extended.customerId});
    this.setSelectable(activity.extended.customerId);
  }

  public selectedProjectChanged(project?: GuidStringTypeaheadDto): void {
    if (!project?.extended.customerId)
      return;

    this.timesheetForm.patchValue({customerId: project.extended.customerId});
    this.setSelectable(project.extended.customerId);
  }

  public selectedOrderChanged(order?: GuidStringTypeaheadDto): void {
    if (!order)
      return;

    this.timesheetForm.patchValue({customerId: order.extended.customerId});
    this.setSelectable(order.extended.customerId);
  }

  public save(): void {
    if (!this.timesheetForm.valid)
      return;

    const apiAction = this.isNewRecord
      ? this.timesheetService.create({timeSheetDto: this.timesheetForm.value})
      : this.timesheetService.update({timeSheetDto: this.timesheetForm.value});

    const timesheetChangedAction = this.isNewRecord
      ? 'created'
      : 'updated';

    apiAction
      .pipe(single())
      .subscribe(timesheet => {
        this.modal?.close();
        this.entityService.timesheetChanged.next({entity: timesheet, action: timesheetChangedAction});
      });
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private createTimesheetForm(): ValidationFormGroup {
    const timesheetForm = this.formValidationService
      .getFormGroup<TimeSheetDto>(
        'TimeSheetDto',
        {
          startDate: DateTime.now(),
          id: GuidService.guidEmpty,
          billable: true,
          userId: this.authorizationEnabled ? this.authenticationService.currentUser.id : GuidService.guidEmpty,
        },
        {
          startTime: new FormControl(DateTime.now()),
          endTime: new FormControl(),
        }
      );

    const formControls = timesheetForm.controls;
    const startDateToTimeSync = formControls['startDate'].valueChanges.subscribe(x => formControls['startTime'].patchValue(x, {emitEvent: false}));
    const startTimeToDateSync = formControls['startTime'].valueChanges.subscribe(x => formControls['startDate'].patchValue(x, {emitEvent: false}));
    const endDateToTimeSync = formControls['endDate'].valueChanges.subscribe(x => formControls['endTime'].patchValue(x, {emitEvent: false}));
    const endTimeToDateSync = formControls['endTime'].valueChanges.subscribe(x => formControls['endDate'].patchValue(x, {emitEvent: false}));
    this.subscriptions.add(startDateToTimeSync);
    this.subscriptions.add(startTimeToDateSync);
    this.subscriptions.add(endDateToTimeSync);
    this.subscriptions.add(endTimeToDateSync);

    return timesheetForm;
  }

  private setTimeEndToNowAfterEndDateWasSet(initialValue: DateTime | null | undefined): void {
    const endDateChanged = this.timesheetForm.controls['endDate'].valueChanges
      .pipe(
        startWith(initialValue),
        pairwise(),
        filter(([prev, current]) => prev == null && current != null),
        map(([, current]) => current)
      )
      .subscribe((newValue: DateTime) => {
        const now = DateTime.now();
        const startOfDay = now.startOf('day');
        const todayTimePart = now.toMillis() - startOfDay.toMillis();
        newValue = newValue.startOf('day').plus(todayTimePart);
        this.timesheetForm.controls['endDate'].patchValue(newValue);
      });

    this.subscriptions.add(endDateChanged);
  }

  private setSelectable(customerId: string | undefined): void {
    if (customerId) {
      this.selectableActivities = this.allActivities.filter(activity => activity.extended.customerId == null || activity.extended.customerId === customerId)
      this.selectableProjects = this.allProjects.filter(project => project.extended.customerId == null || project.extended.customerId === customerId)
      this.selectableOrders = this.allOrders.filter(order => order.extended.customerId === customerId)
    } else {
      this.selectableActivities = this.allActivities;
      this.selectableProjects = this.allProjects;
      this.selectableOrders = this.allOrders;
    }

    const selectedActivityIsSelectable = this.selectableActivities.some(activity => activity.id === this.timesheetForm.value.activityId);
    if (!selectedActivityIsSelectable)
      this.timesheetForm.patchValue({activityId: null});

    const selectedProjectIsSelectable = this.selectableProjects.some(project => project.id === this.timesheetForm.value.projectId);
    if (!selectedProjectIsSelectable)
      this.timesheetForm.patchValue({projectId: null});

    const selectedOrderIsSelectable = this.selectableOrders.some(order => order.id === this.timesheetForm.value.orderId);
    if (!selectedOrderIsSelectable)
      this.timesheetForm.patchValue({orderId: null});
  }
}
