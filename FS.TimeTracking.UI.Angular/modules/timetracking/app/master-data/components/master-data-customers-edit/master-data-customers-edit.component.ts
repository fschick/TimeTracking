import {AfterViewInit, Component, TemplateRef, ViewChild} from '@angular/core';
import {CustomerDto, CustomerService} from '../../../../../api/timetracking';
import {ActivatedRoute, Router} from '@angular/router';
import {single} from 'rxjs/operators';
import {FormValidationService, ValidationFormGroup} from '../../../../../core/app/services/form-validation/form-validation.service';
import {EntityService} from '../../../../../core/app/services/state-management/entity.service';
import {GuidService} from '../../../../../core/app/services/state-management/guid.service';
import {NgbModal, NgbModalRef} from '@ng-bootstrap/ng-bootstrap';
import {AuthenticationService} from '../../../../../core/app/services/authentication.service';

@Component({
  selector: 'ts-master-data-customers-edit',
  templateUrl: './master-data-customers-edit.component.html',
  styleUrls: ['./master-data-customers-edit.component.scss']
})
export class MasterDataCustomersEditComponent implements AfterViewInit {
  public customerForm: ValidationFormGroup;
  public isNewRecord: boolean;
  public isReadOnly: boolean;

  @ViewChild('customerEdit') private customerEdit?: TemplateRef<any>;

  private modal?: NgbModalRef

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private customerService: CustomerService,
    private entityService: EntityService,
    private formValidationService: FormValidationService,
    private modalService: NgbModal,
    authenticationService: AuthenticationService,
  ) {
    this.customerForm = this.formValidationService
      .getFormGroup<CustomerDto>('CustomerDto', {
        id: GuidService.guidEmpty,
        hourlyRate: 0,
        hidden: false
      });

    this.isNewRecord = this.route.snapshot.params['id'] === GuidService.guidEmpty;
    this.isReadOnly = !authenticationService.currentUser.hasRole.masterDataCustomersManage;

    if (this.isNewRecord)
      return;

    this.customerService
      .get({id: this.route.snapshot.params['id']})
      .pipe(single())
      .subscribe(customer => {
        this.isReadOnly = customer.isReadonly ?? false;
        this.customerForm.patchValue(customer);
      });
  }

  public ngAfterViewInit(): void {
    this.modal = this.modalService.open(this.customerEdit, {size: 'lg', scrollable: true});
    this.modal.hidden.pipe(single()).subscribe(() => this.router.navigate(['..'], {relativeTo: this.route}));
  }

  public save(): void {
    if (!this.customerForm.valid)
      return;

    const apiAction = this.isNewRecord
      ? this.customerService.create({customerDto: this.customerForm.value})
      : this.customerService.update({customerDto: this.customerForm.value});

    const customerChangedAction = this.isNewRecord
      ? 'created'
      : 'updated';

    apiAction
      .pipe(single())
      .subscribe(customer => {
        this.modal?.close();
        this.entityService.customerChanged.next({entity: customer, action: customerChangedAction});
      });
  }
}
