import {AfterViewInit, Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {CustomerDto, CustomerService} from '../../../shared/services/api';
import {ActivatedRoute, Router} from '@angular/router';
import {take} from 'rxjs/operators';
import {FormValidationService, ValidationFormGroup} from '../../../shared/services/form-validation/form-validation.service';
import {Modal} from 'bootstrap';
import {EntityChangedService} from '../../../shared/services/state-management/entity-changed.service';
import {Location} from '@angular/common';

@Component({
  selector: 'ts-master-data-customers-edit',
  templateUrl: './master-data-customers-edit.component.html',
  styleUrls: ['./master-data-customers-edit.component.scss']
})
export class MasterDataCustomersEditComponent implements AfterViewInit {
  @ViewChild('customerEdit') private customerEdit?: ElementRef;

  public customerForm: ValidationFormGroup;
  private modal!: Modal;

  constructor(
    private location: Location,
    private route: ActivatedRoute,
    private customerService: CustomerService,
    private entityChangedService: EntityChangedService,
    private formValidationService: FormValidationService,
  ) {
    this.customerForm = this.formValidationService.getFormGroup<CustomerDto>('CustomerDto');
    this.customerService
      .get(this.route.snapshot.params.id)
      .pipe(take(1))
      .subscribe(customer => this.customerForm.patchValue(customer));
  }

  public ngAfterViewInit(): void {
    this.modal = new bootstrap.Modal(this.customerEdit?.nativeElement);
    this.customerEdit?.nativeElement.addEventListener('hide.bs.modal', () => this.location.back());
    this.modal.show();
  }

  public save(): void {
    if (!this.customerForm.valid)
      return;

    this.customerService.update(this.customerForm.value)
      .pipe(take(1))
      .subscribe(customer => {
        this.close();
        this.entityChangedService.customerChanged.next({action: 'updated', entity: customer});
      });
  }

  public close(): void {
    this.modal.hide();
  }
}
