import {AfterViewInit, Component, TemplateRef, ViewChild} from '@angular/core';
import {FormValidationService, ValidationFormGroup} from '../../../../../core/app/services/form-validation/form-validation.service';
import {ActivatedRoute, Router} from '@angular/router';
import {ProjectDto, ProjectService, GuidStringTypeaheadDto, TypeaheadService} from '../../../../../api/timetracking';
import {EntityService} from '../../../../../core/app/services/state-management/entity.service';
import {single} from 'rxjs/operators';
import {Observable} from 'rxjs';
import {GuidService} from '../../../../../core/app/services/state-management/guid.service';
import {NgbModal, NgbModalRef} from '@ng-bootstrap/ng-bootstrap';
import {AuthenticationService} from '../../../../../core/app/services/authentication.service';

@Component({
  selector: 'ts-master-data-projects-edit',
  templateUrl: './master-data-projects-edit.component.html',
  styleUrls: ['./master-data-projects-edit.component.scss']
})
export class MasterDataProjectsEditComponent implements AfterViewInit {
  public projectForm: ValidationFormGroup;
  public isNewRecord: boolean;
  public isReadOnly: boolean;
  public customers$: Observable<GuidStringTypeaheadDto[]>;

  @ViewChild('projectEdit') private projectEdit?: TemplateRef<any>;

  private modal?: NgbModalRef

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private projectService: ProjectService,
    private entityService: EntityService,
    private formValidationService: FormValidationService,
    typeaheadService: TypeaheadService,
    authenticationService: AuthenticationService,
    private modalService: NgbModal
  ) {
    this.projectForm = this.formValidationService.getFormGroup<ProjectDto>('ProjectDto', {id: GuidService.guidEmpty, hidden: false});

    this.customers$ = typeaheadService.getCustomers({showHidden: true});

    this.isNewRecord = this.route.snapshot.params['id'] === GuidService.guidEmpty;
    this.isReadOnly = !authenticationService.currentUser.hasRole.masterDataProjectsManage;

    if (this.isNewRecord)
      return;

    this.projectService
      .get({id: this.route.snapshot.params['id']})
      .pipe(single())
      .subscribe(project => {
        this.isReadOnly = project.isReadonly ?? false;
        this.projectForm.patchValue(project);
      });
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
