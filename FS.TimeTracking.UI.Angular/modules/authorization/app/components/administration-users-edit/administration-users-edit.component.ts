import {AfterViewInit, Component, TemplateRef, ViewChild} from '@angular/core';
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
import {UtilityService} from '../../../../core/app/services/utility.service';
import {AuthenticationService} from '../../../../core/app/services/authentication.service';

type IndexedControl = { index: number } & AbstractControl;
type PermissionGroup = { name: string, controls: IndexedControl[] };

@Component({
  selector: 'ts-administration-users-edit',
  templateUrl: './administration-users-edit.component.html',
  styleUrls: ['./administration-users-edit.component.scss']
})
export class AdministrationUsersEditComponent implements AfterViewInit {
  public userForm: ValidationFormGroup;
  public isNewRecord: boolean;
  public isReadOnly: boolean;
  public customers$: Observable<GuidStringTypeaheadDto[]> = EMPTY;
  public permissionGroups: PermissionGroup[];

  @ViewChild('userEdit') private userEdit?: TemplateRef<any>;

  private modal?: NgbModalRef;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private userService: UserService,
    private entityService: EntityService,
    private formValidationService: FormValidationService,
    private utilityService: UtilityService,
    private modalService: NgbModal,
    private authenticationService: AuthenticationService,
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

    const permissionControls = (this.userForm.get('permissions') as FormArray).controls.map((control, index) => ({index, ...control} as IndexedControl));
    const permissionControlGroups = this.utilityService.groupBy(permissionControls, permission => permission.value.group as string);
    this.permissionGroups = Array.from(permissionControlGroups).map(([groupName, controls]) => ({name: groupName, controls}));

    this.isNewRecord = this.route.snapshot.params['id'] === GuidService.guidEmpty;
    this.isReadOnly = !authenticationService.currentUser.hasRole.administrationUsersManage || !authenticationService.currentUser.hasRole.foreignDataManage;

    if (this.isNewRecord)
      return;

    this.userService
      .get({id: this.route.snapshot.params['id']})
      .pipe(single())
      .subscribe(user => {
        this.isReadOnly = user.isReadonly ?? false;
        if (user.id === this.authenticationService.currentUser.id)
          this.disableAdministrateUsersControl();
        this.userForm.patchValue(user);
      });
  }

  public translatePermissionGroup(permissionGroup: string): string {
    const transUnitId = `@@Page.Administration.Users.PermissionGroups.${permissionGroup}`;
    return $localizeId`${transUnitId}:TRANSUNITID:`;
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

    // https://stackoverflow.com/a/40148168
    const userDto = this.userForm.getRawValue();
    const apiAction = this.isNewRecord
      ? this.userService.create({userDto})
      : this.userService.update({userDto});

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

  private disableAdministrateUsersControl() {
    const permissions = (this.userForm.get('permissions') as FormArray);
    const administrateUserPermissions = permissions.controls.find(control => control.value.name === 'administration-users');
    administrateUserPermissions?.disable();
  }
}
