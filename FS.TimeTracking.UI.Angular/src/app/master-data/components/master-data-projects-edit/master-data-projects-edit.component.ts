import {AfterViewInit, Component, ElementRef, TemplateRef, ViewChild} from '@angular/core';
import {FormValidationService, ValidationFormGroup} from '../../../shared/services/form-validation/form-validation.service';
import {Modal} from 'bootstrap';
import {Location} from '@angular/common';
import {ActivatedRoute} from '@angular/router';
import {ProjectDto, ProjectService, StringTypeaheadDto, TypeaheadService} from '../../../shared/services/api';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {map, single} from 'rxjs/operators';
import {Observable} from 'rxjs';
import {DialogRef, DialogService} from '@ngneat/dialog';

@Component({
  selector: 'ts-master-data-projects-edit',
  templateUrl: './master-data-projects-edit.component.html',
  styleUrls: ['./master-data-projects-edit.component.scss']
})
export class MasterDataProjectsEditComponent implements AfterViewInit {
  @ViewChild('projectEdit') private projectEdit?: TemplateRef<any>;

  public projectForm: ValidationFormGroup;
  public isNewRecord: boolean;
  public customers$: Observable<StringTypeaheadDto[]>;
  private dialog?: DialogRef<unknown, any, TemplateRef<any>>;

  constructor(
    private location: Location,
    private route: ActivatedRoute,
    private projectService: ProjectService,
    private entityService: EntityService,
    private formValidationService: FormValidationService,
    typeaheadService: TypeaheadService,
    private dialogService: DialogService,
  ) {
    this.isNewRecord = this.route.snapshot.params.id === this.entityService.guidEmpty;
    this.projectForm = this.formValidationService.getFormGroup<ProjectDto>('ProjectDto', {id: this.entityService.guidEmpty, hidden: false});

    if (!this.isNewRecord)
      this.projectService
        .get(this.route.snapshot.params.id)
        .pipe(single())
        .subscribe(project => this.projectForm.patchValue(project));

    this.customers$ = typeaheadService.getCustomers();
  }

  public ngAfterViewInit(): void {
    if (!this.projectEdit)
      return;

    this.dialog = this.dialogService.open(this.projectEdit, {draggable: true, size: 'inherit'});
    this.dialog.afterClosed$.pipe(single()).subscribe(_ => this.location.back());
  }

  public save(): void {
    if (!this.projectForm.valid)
      return;

    const apiAction = this.isNewRecord
      ? this.projectService.create(this.projectForm.value)
      : this.projectService.update(this.projectForm.value);

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
    this.dialog?.close();
  }
}
