import {AfterViewInit, Component, TemplateRef, ViewChild} from '@angular/core';
import {FormValidationService, ValidationFormGroup} from '../../../shared/services/form-validation/form-validation.service';
import {Observable} from 'rxjs';
import {ActivityDto, ActivityService, StringTypeaheadDto, TypeaheadService} from '../../../../../api/timetracking';
import {ActivatedRoute, Router} from '@angular/router';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {single} from 'rxjs/operators';
import {GuidService} from '../../../shared/services/state-management/guid.service';
import {NgbModal, NgbModalRef} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'ts-master-data-activities-edit',
  templateUrl: './master-data-activities-edit.component.html',
  styleUrls: ['./master-data-activities-edit.component.scss']
})
export class MasterDataActivitiesEditComponent implements AfterViewInit {
  public activityForm: ValidationFormGroup;
  public isNewRecord: boolean;
  public customers$: Observable<StringTypeaheadDto[]>;
  public projects$: Observable<StringTypeaheadDto[]>;

  @ViewChild('activityEdit') private activityEdit?: TemplateRef<any>;

  private modal?: NgbModalRef

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private activityService: ActivityService,
    private entityService: EntityService,
    private formValidationService: FormValidationService,
    private modalService: NgbModal,
    typeaheadService: TypeaheadService,
  ) {
    this.isNewRecord = this.route.snapshot.params['id'] === GuidService.guidEmpty;
    this.activityForm = this.formValidationService.getFormGroup<ActivityDto>('ActivityDto', {id: GuidService.guidEmpty, hidden: false});

    if (!this.isNewRecord)
      this.activityService
        .get({id: this.route.snapshot.params['id']})
        .pipe(single())
        .subscribe(activity => this.activityForm.patchValue(activity));

    this.customers$ = typeaheadService.getCustomers({showHidden: true});
    this.projects$ = typeaheadService.getProjects({showHidden: true});
  }

  public ngAfterViewInit(): void {
    this.modal = this.modalService.open(this.activityEdit, {size: 'lg', scrollable: true});
    this.modal.hidden.pipe(single()).subscribe(() => this.router.navigate(['..'], {relativeTo: this.route}));
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
        this.modal?.close();
        this.entityService.activityChanged.next({entity: activity, action: activityChangedAction});
      });
  }
}
