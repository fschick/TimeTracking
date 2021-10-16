import {AfterViewInit, Component, ElementRef, ViewChild} from '@angular/core';
import {FormValidationService, ValidationFormGroup} from '../../../shared/services/form-validation/form-validation.service';
import {Observable} from 'rxjs';
import {ActivityDto, ActivityService, StringTypeaheadDto, TypeaheadService} from '../../../shared/services/api';
import {Location} from '@angular/common';
import {ActivatedRoute} from '@angular/router';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {single} from 'rxjs/operators';
import {Modal} from 'bootstrap';
import {GuidService} from '../../../shared/services/state-management/guid.service';

@Component({
  selector: 'ts-master-data-activities-edit',
  templateUrl: './master-data-activities-edit.component.html',
  styleUrls: ['./master-data-activities-edit.component.scss']
})
export class MasterDataActivitiesEditComponent implements AfterViewInit {
  @ViewChild('activityEdit') private activityEdit?: ElementRef;
  @ViewChild('title') private title?: ElementRef;

  public activityForm: ValidationFormGroup;
  public isNewRecord: boolean;
  public customers$: Observable<StringTypeaheadDto[]>;
  public projects$: Observable<StringTypeaheadDto[]>;
  private modal!: Modal;

  constructor(
    private location: Location,
    private route: ActivatedRoute,
    private activityService: ActivityService,
    private entityService: EntityService,
    private formValidationService: FormValidationService,
    typeaheadService: TypeaheadService,
  ) {
    this.isNewRecord = this.route.snapshot.params.id === GuidService.guidEmpty;
    this.activityForm = this.formValidationService.getFormGroup<ActivityDto>('ActivityDto', {id: GuidService.guidEmpty, hidden: false});

    if (!this.isNewRecord)
      this.activityService
        .get({id: this.route.snapshot.params.id})
        .pipe(single())
        .subscribe(activity => this.activityForm.patchValue(activity));

    this.customers$ = typeaheadService.getCustomers({showHidden: true});
    this.projects$ = typeaheadService.getProjects({showHidden: true});
  }

  public ngAfterViewInit(): void {
    this.modal = new bootstrap.Modal(this.activityEdit?.nativeElement);
    this.activityEdit?.nativeElement.addEventListener('hide.bs.modal', () => this.location.back());
    this.activityEdit?.nativeElement.addEventListener('shown.bs.modal', () => this.title?.nativeElement.focus());
    this.modal.show();
  }

  public save(): void {
    if (!this.activityForm.valid)
      return;

    const apiAction = this.isNewRecord
      ? this.activityService.create({activityDto: this.activityForm.value})
      : this.activityService.update({activityDto: this.activityForm.value});

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
    this.modal?.hide();
  }
}
