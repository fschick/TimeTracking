import {AfterViewInit, Component, ElementRef, ViewChild} from '@angular/core';
import {FormValidationService, ValidationFormGroup} from '../../../shared/services/form-validation/form-validation.service';
import {Modal} from 'bootstrap';
import {Location} from '@angular/common';
import {ActivatedRoute} from '@angular/router';
import {ProjectDto, ProjectService, StringTypeaheadDto, TypeaheadService} from '../../../shared/services/api';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {single} from 'rxjs/operators';
import {Observable} from 'rxjs';
import {GuidService} from '../../../shared/services/state-management/guid.service';

@Component({
  selector: 'ts-master-data-projects-edit',
  templateUrl: './master-data-projects-edit.component.html',
  styleUrls: ['./master-data-projects-edit.component.scss']
})
export class MasterDataProjectsEditComponent implements AfterViewInit {
  @ViewChild('projectEdit') private projectEdit?: ElementRef;
  @ViewChild('title') private title?: ElementRef;

  public projectForm: ValidationFormGroup;
  public isNewRecord: boolean;
  public customers$: Observable<StringTypeaheadDto[]>;
  private modal!: Modal;

  constructor(
    private location: Location,
    private route: ActivatedRoute,
    private projectService: ProjectService,
    private entityService: EntityService,
    private formValidationService: FormValidationService,
    typeaheadService: TypeaheadService,
  ) {
    this.isNewRecord = this.route.snapshot.params['id'] === GuidService.guidEmpty;
    this.projectForm = this.formValidationService.getFormGroup<ProjectDto>('ProjectDto', {id: GuidService.guidEmpty, hidden: false});

    if (!this.isNewRecord)
      this.projectService
        .get({id: this.route.snapshot.params['id']})
        .pipe(single())
        .subscribe(project => this.projectForm.patchValue(project));

    this.customers$ = typeaheadService.getCustomers({showHidden: true});
  }

  public ngAfterViewInit(): void {
    this.modal = new bootstrap.Modal(this.projectEdit?.nativeElement);
    this.projectEdit?.nativeElement.addEventListener('hide.bs.modal', () => this.location.back());
    this.projectEdit?.nativeElement.addEventListener('shown.bs.modal', () => this.title?.nativeElement.focus());
    this.modal.show();
  }

  public save(): void {
    if (!this.projectForm.valid)
      return;

    const apiAction = this.isNewRecord
      ? this.projectService.create({projectDto: this.projectForm.value})
      : this.projectService.update({projectDto: this.projectForm.value});

    const projectChangedAction = this.isNewRecord
      ? 'created'
      : 'updated';

    apiAction
      .pipe(single())
      .subscribe(project => {
        this.close();
        this.entityService.projectChanged.next({entity: project, action: projectChangedAction});
      });
  }

  public close(): void {
    this.modal?.hide();
  }
}
