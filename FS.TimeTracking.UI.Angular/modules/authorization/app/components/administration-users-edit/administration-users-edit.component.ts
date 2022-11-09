import {AfterViewInit, Component, OnInit, TemplateRef, ViewChild} from '@angular/core';
import {FormValidationService, ValidationFormGroup} from '../../../../core/app/services/form-validation/form-validation.service';
import {NgbModal, NgbModalRef} from '@ng-bootstrap/ng-bootstrap';
import {ActivatedRoute, Router} from '@angular/router';
import {GuidStringTypeaheadDto, TypeaheadService, UserDto, UserService} from '../../../../api/timetracking';
import {EntityService} from '../../../../core/app/services/state-management/entity.service';
import {GuidService} from '../../../../core/app/services/state-management/guid.service';
import {single} from 'rxjs/operators';
import {AbstractControl, FormArray, FormControl} from '@angular/forms';
import {Validators} from '../../../../core/app/services/form-validation/validators';
import {ConfigurationService} from '../../../../core/app/services/configuration.service';
import {$localizeId} from '../../../../core/app/services/internationalization/localizeId';
import {EMPTY, Observable} from 'rxjs';


@Component({
  selector: 'ts-administration-users-edit',
  templateUrl: './administration-users-edit.component.html',
  styleUrls: ['./administration-users-edit.component.scss']
})
export class AdministrationUsersEditComponent implements AfterViewInit {
  public userForm: ValidationFormGroup;
  public isNewRecord: boolean;
  public customers$: Observable<GuidStringTypeaheadDto[]> = EMPTY;

  @ViewChild('userEdit') private userEdit?: TemplateRef<any>;

  private modal?: NgbModalRef;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private userService: UserService,
    private entityService: EntityService,
    private formValidationService: FormValidationService,
    private modalService: NgbModal,
    configurationService: ConfigurationService,
    typeaheadService: TypeaheadService,
  ) {
    this.customers$ = typeaheadService.getCustomers({showHidden: true});

    const defaultPermissions = configurationService.clientConfiguration.defaultPermissions;
    this.userForm = this.formValidationService
      .getFormGroup<UserDto>(
        'UserDto',
        {id: GuidService.guidEmpty, enabled: true, permissions: defaultPermissions},
        {confirmPassword: new FormControl()},
        [Validators.compare('password', 'confirmPassword')]
      );

    this.isNewRecord = this.route.snapshot.params['id'] === GuidService.guidEmpty;
    if (this.isNewRecord)
      return;

    this.userService
      .get({id: this.route.snapshot.params['id']})
      .pipe(single())
      .subscribe(user => {
        this.userForm.patchValue(user);
      });
  }

  public getPermissionControls(): AbstractControl[] {
    return (this.userForm.get('permissions') as FormArray).controls;
  }

  public translatePermissionName(permissionName: string): string {
    const transUnitId = `@@Page.Administration.Users.Permissions.${permissionName}`;
    return $localizeId`${transUnitId}:TRANSUNITID:`;
  }

  public ngAfterViewInit(): void {
    this.modal = this.modalService.open(this.userEdit, {size: 'lg', scrollable: true});
    this.modal.hidden.pipe(single()).subscribe(() => this.router.navigate(['..'], {relativeTo: this.route}));
  }

  public save(): void {
    if (!this.userForm.valid)
      return;

    const apiAction = this.isNewRecord
      ? this.userService.create({userDto: this.userForm.value})
      : this.userService.update({userDto: this.userForm.value});

    const userChangedAction = this.isNewRecord
      ? 'created'
      : 'updated';

    apiAction
      .pipe(single())
      .subscribe(user => {
        this.modal?.close();
        this.entityService.userChanged.next({entity: user, action: userChangedAction});
      });
  }
}
