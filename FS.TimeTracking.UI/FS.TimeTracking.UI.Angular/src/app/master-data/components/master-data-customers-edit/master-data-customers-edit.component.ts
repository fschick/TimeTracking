import {AfterViewInit, Component, ElementRef, ViewChild} from '@angular/core';
import {CustomerDto, CustomerService} from '../../../shared/services/api';
import {ActivatedRoute} from '@angular/router';
import {single} from 'rxjs/operators';
import {FormValidationService, ValidationFormGroup} from '../../../shared/services/form-validation/form-validation.service';
import {Location} from '@angular/common';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {Modal} from 'bootstrap';
import {GuidService} from '../../../shared/services/state-management/guid.service';

@Component({
  selector: 'ts-master-data-customers-edit',
  templateUrl: './master-data-customers-edit.component.html',
  styleUrls: ['./master-data-customers-edit.component.scss']
})
export class MasterDataCustomersEditComponent implements AfterViewInit {
  @ViewChild('customerEdit') private customerEdit?: ElementRef;
  @ViewChild('title') private title?: ElementRef;

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
    this.isNewRecord = this.route.snapshot.params['id'] === GuidService.guidEmpty;
    this.customerForm = this.formValidationService.getFormGroup<CustomerDto>('CustomerDto', {id: GuidService.guidEmpty, hidden: false});

    if (!this.isNewRecord)
      this.customerService
        .get({id: this.route.snapshot.params['id']})
        .pipe(single())
        .subscribe(customer => this.customerForm.patchValue(customer));
  }

  public ngAfterViewInit(): void {
    this.modal = new bootstrap.Modal(this.customerEdit?.nativeElement);
    this.customerEdit?.nativeElement.addEventListener('hide.bs.modal', () => this.location.back());
    this.customerEdit?.nativeElement.addEventListener('shown.bs.modal', () => this.title?.nativeElement.focus());
    this.modal.show();
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
        this.close();
        this.entityService.customerChanged.next({entity: customer, action: customerChangedAction});
      });
  }

  public close(): void {
    this.modal?.hide();
  }
}
