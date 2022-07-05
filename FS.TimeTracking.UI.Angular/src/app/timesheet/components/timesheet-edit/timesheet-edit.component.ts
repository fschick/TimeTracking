import {AfterViewInit, Component, OnDestroy, TemplateRef, ViewChild} from '@angular/core';
import {FormValidationService, ValidationFormGroup} from '../../../shared/services/form-validation/form-validation.service';
import {ActivatedRoute, Router} from '@angular/router';
import {StringTypeaheadDto, TimeSheetDto, TimeSheetService, TypeaheadService} from '../../../../api/timetracking';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {GuidService} from '../../../shared/services/state-management/guid.service';
import {filter, map, pairwise, single, startWith} from 'rxjs/operators';
import {BehaviorSubject, combineLatest, Observable, Subscription} from 'rxjs';
import {DateTime} from 'luxon';
import {FormControl} from '@angular/forms';
import {NgbModal, NgbModalRef} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'ts-timesheet-edit',
  templateUrl: './timesheet-edit.component.html',
  styleUrls: ['./timesheet-edit.component.scss']
})
export class TimesheetEditComponent implements AfterViewInit, OnDestroy {
  public projects$: Observable<StringTypeaheadDto[]>;
  public selectedProject$: BehaviorSubject<StringTypeaheadDto | undefined>;
  public activities$: Observable<StringTypeaheadDto[]>;
  public selectedActivity$: BehaviorSubject<StringTypeaheadDto | undefined>;
  public orders$: Observable<StringTypeaheadDto[]>;
  public selectedOrder$: BehaviorSubject<StringTypeaheadDto | undefined>;
  public timesheetForm: ValidationFormGroup;
  public isNewRecord: boolean;

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
    private modalService: NgbModal
  ) {
    this.timesheetForm = this.createTimesheetForm();

    this.isNewRecord = this.route.snapshot.params['id'] === GuidService.guidEmpty;
    if (this.isNewRecord)
      this.setTimeToNowWhenEmptyEndDateIsFilled(null);
    else
      this.timesheetService
        .get({id: this.route.snapshot.params['id']})
        .pipe(single())
        .subscribe(timesheet => {
          this.timesheetForm.patchValue(timesheet);
          this.setTimeToNowWhenEmptyEndDateIsFilled(timesheet.endDate);
        });

    this.projects$ = typeaheadService.getProjects({});
    this.selectedProject$ = new BehaviorSubject<StringTypeaheadDto | undefined>(undefined);

    this.activities$ = combineLatest([typeaheadService.getActivities({}), this.selectedProject$])
      .pipe(map(([activities, project]) =>
        activities.filter(activity => !project || !activity.extended.projectId || activity.extended.projectId === project.extended.id)
      ));
    this.selectedActivity$ = new BehaviorSubject<StringTypeaheadDto | undefined>(undefined);

    this.orders$ = combineLatest([this.typeaheadService.getOrders({}), this.selectedProject$])
      .pipe(map(([orders, project]) =>
        orders.filter(order => !project || order.extended.customerId === project.extended.customerId)
      ));
    this.selectedOrder$ = new BehaviorSubject<StringTypeaheadDto | undefined>(undefined);
  }

  public ngAfterViewInit(): void {
    this.modal = this.modalService.open(this.timesheetEdit, {size: 'lg', scrollable: true});
    this.modal.hidden.pipe(single()).subscribe(() => this.router.navigate(['..'], {relativeTo: this.route}));
  }

  public selectedProjectChanged(project?: StringTypeaheadDto): void {
    const activityProjectId = this.selectedActivity$.value?.extended.projectId;
    if (activityProjectId && activityProjectId !== project?.id)
      this.timesheetForm.controls['activityId'].patchValue(undefined);

    const orderCustomerId = this.selectedOrder$.value?.extended.customerId;
    if (orderCustomerId !== project?.extended.customerId)
      this.timesheetForm.controls['orderId'].patchValue(undefined);

    this.selectedProject$.next(project);
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
          billable: true
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

  private setTimeToNowWhenEmptyEndDateIsFilled(initialValue: DateTime | null | undefined): void {
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
}
