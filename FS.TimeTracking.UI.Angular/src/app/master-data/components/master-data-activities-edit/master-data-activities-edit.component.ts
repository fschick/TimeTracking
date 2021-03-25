import {AfterViewInit, Component, ElementRef, TemplateRef, ViewChild} from '@angular/core';
import {FormValidationService, ValidationFormGroup} from '../../../shared/services/form-validation/form-validation.service';
import {Observable} from 'rxjs';
import {ActivityDto, ActivityService, StringTypeaheadDto, TypeaheadService} from '../../../shared/services/api';
import {DialogRef, DialogService} from '@ngneat/dialog';
import {Location} from '@angular/common';
import {ActivatedRoute} from '@angular/router';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {single} from 'rxjs/operators';

@Component({
  selector: 'ts-master-data-activities-edit',
  templateUrl: './master-data-activities-edit.component.html',
  styleUrls: ['./master-data-activities-edit.component.scss']
})
export class MasterDataActivitiesEditComponent implements AfterViewInit {
  @ViewChild('activityEdit') private activityEdit?: TemplateRef<any>;
  @ViewChild('shortName') private shortName?: ElementRef;

  public activityForm: ValidationFormGroup;
  public isNewRecord: boolean;
  public customers$: Observable<StringTypeaheadDto[]>;
  public projects$: Observable<StringTypeaheadDto[]>;
  private dialog?: DialogRef<unknown, any, TemplateRef<any>>;

  constructor(
    private location: Location,
    private route: ActivatedRoute,
    private activityService: ActivityService,
    private entityService: EntityService,
    private formValidationService: FormValidationService,
    typeaheadService: TypeaheadService,
    private dialogService: DialogService,
  ) {
    this.isNewRecord = this.route.snapshot.params.id === this.entityService.guidEmpty;
    this.activityForm = this.formValidationService.getFormGroup<ActivityDto>('ActivityDto', {id: this.entityService.guidEmpty, hidden: false});

    if (!this.isNewRecord)
      this.activityService
        .get(this.route.snapshot.params.id)
        .pipe(single())
        .subscribe(activity => this.activityForm.patchValue(activity));

    this.customers$ = typeaheadService.getCustomers();
    this.projects$ = typeaheadService.getProjects();
  }

  public ngAfterViewInit(): void {
    if (!this.activityEdit)
      return;

    this.dialog = this.dialogService.open(this.activityEdit, {draggable: true, size: 'inherit'});
    this.dialog.afterClosed$.pipe(single()).subscribe(_ => this.location.back());
  }

  public save(): void {
    if (!this.activityForm.valid)
      return;

    const apiAction = this.isNewRecord
      ? this.activityService.create(this.activityForm.value)
      : this.activityService.update(this.activityForm.value);

    const activityChangedAction = this.isNewRecord
      ? 'created'
      : 'updated';

    apiAction
      .pipe(single())
      .subscribe(activity => {
        this.close();
        this.entityService.activityChanged.next({entity: activity, action: activityChangedAction});
      });
  }

  public close(): void {
    this.dialog?.close();
  }
}
