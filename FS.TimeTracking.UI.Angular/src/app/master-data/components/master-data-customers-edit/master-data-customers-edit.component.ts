import {AfterViewInit, Component, ElementRef, ViewChild} from '@angular/core';
import {CustomerDto, CustomerService} from '../../../shared/services/api';
import {ActivatedRoute} from '@angular/router';
import {single} from 'rxjs/operators';
import {FormValidationService, ValidationFormGroup} from '../../../shared/services/form-validation/form-validation.service';
import {Modal} from 'bootstrap';
import {Location} from '@angular/common';
import {EntityService} from '../../../shared/services/state-management/entity.service';

@Component({
  selector: 'ts-master-data-customers-edit',
  templateUrl: './master-data-customers-edit.component.html',
  styleUrls: ['./master-data-customers-edit.component.scss']
})
export class MasterDataCustomersEditComponent implements AfterViewInit {
  @ViewChild('customerEdit') private customerEdit?: ElementRef;
  @ViewChild('shortName') private shortName?: ElementRef;

  public customerForm: ValidationFormGroup;
  public isNewRecord: boolean;
  private modal!: Modal;

  constructor(
    private location: Location,
    private route: ActivatedRoute,
    private customerService: CustomerService,
    private entityService: EntityService,
    private formValidationService: FormValidationService,
  ) {
    this.isNewRecord = this.route.snapshot.params.id === this.entityService.guidEmpty;
    this.customerForm = this.formValidationService.getFormGroup<CustomerDto>('CustomerDto', {id: this.entityService.guidEmpty, hidden: false});

    if (!this.isNewRecord)
      this.customerService
        .get(this.route.snapshot.params.id)
        .pipe(single())
        .subscribe(customer => this.customerForm.patchValue(customer));
  }

  public ngAfterViewInit(): void {
    this.modal = new bootstrap.Modal(this.customerEdit?.nativeElement);
    this.customerEdit?.nativeElement.addEventListener('hide.bs.modal', () => this.location.back());
    this.customerEdit?.nativeElement.addEventListener('shown.bs.modal', () => this.shortName?.nativeElement.focus());
    this.modal.show();
  }

  public save(): void {
    if (!this.customerForm.valid)
      return;

    const apiAction = this.isNewRecord
      ? this.customerService.create(this.customerForm.value)
      : this.customerService.update(this.customerForm.value);

    const customerChangedAction = this.isNewRecord
      ? 'created'
      : 'updated';

    apiAction
      .pipe(single())
      .subscribe(customer => {
        this.close();
        this.entityService.customerChanged.next({entity: customer, action: customerChangedAction});
      });
  }

  public delete(): void {
    this.customerService
      .delete(this.customerForm.value.id)
      .pipe(single())
      .subscribe(() => {
        this.close();
        this.entityService.customerChanged.next({entity: this.customerForm.value, action: 'deleted'});
      });
  }

  public close(): void {
    this.modal.hide();
  }
}
