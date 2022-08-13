import {AfterViewInit, Component, TemplateRef, ViewChild} from '@angular/core';
import {FormValidationService, ValidationFormGroup} from '../../../shared/services/form-validation/form-validation.service';
import {ActivatedRoute, Router} from '@angular/router';
import {ProjectDto, ProjectService, StringTypeaheadDto, TypeaheadService} from '../../../../../api/timetracking';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {single} from 'rxjs/operators';
import {Observable} from 'rxjs';
import {GuidService} from '../../../shared/services/state-management/guid.service';
import {NgbModal, NgbModalRef} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'ts-master-data-projects-edit',
  templateUrl: './master-data-projects-edit.component.html',
  styleUrls: ['./master-data-projects-edit.component.scss']
})
export class MasterDataProjectsEditComponent implements AfterViewInit {
  public projectForm: ValidationFormGroup;
  public isNewRecord: boolean;
  public customers$: Observable<StringTypeaheadDto[]>;

  @ViewChild('projectEdit') private projectEdit?: TemplateRef<any>;

  private modal?: NgbModalRef

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private projectService: ProjectService,
    private entityService: EntityService,
    private formValidationService: FormValidationService,
    typeaheadService: TypeaheadService,
    private modalService: NgbModal
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
    this.modal = this.modalService.open(this.projectEdit, {size: 'lg', scrollable: true});
    this.modal.hidden.pipe(single()).subscribe(() => this.router.navigate(['..'], {relativeTo: this.route}));
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
        this.modal?.close();
        this.entityService.projectChanged.next({entity: project, action: projectChangedAction});
      });
  }
}
