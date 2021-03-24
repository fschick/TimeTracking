import {AfterViewInit, Component, ElementRef, TemplateRef, ViewChild} from '@angular/core';
import {CustomerDto, CustomerService} from '../../../shared/services/api';
import {ActivatedRoute} from '@angular/router';
import {single} from 'rxjs/operators';
import {FormValidationService, ValidationFormGroup} from '../../../shared/services/form-validation/form-validation.service';
import {Location} from '@angular/common';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {DialogRef, DialogService} from '@ngneat/dialog';

@Component({
  selector: 'ts-master-data-customers-edit',
  templateUrl: './master-data-customers-edit.component.html',
  styleUrls: ['./master-data-customers-edit.component.scss']
})
export class MasterDataCustomersEditComponent implements AfterViewInit {
  @ViewChild('customerEdit') private customerEdit?: TemplateRef<any>;
  @ViewChild('shortName') private shortName?: ElementRef;

  public customerForm: ValidationFormGroup;
  public isNewRecord: boolean;
  private dialog?: DialogRef<unknown, any, TemplateRef<any>>;

  constructor(
    private location: Location,
    private route: ActivatedRoute,
    private customerService: CustomerService,
    private entityService: EntityService,
    private formValidationService: FormValidationService,
    private dialogService: DialogService,
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
    if (!this.customerEdit)
      return;

    this.dialog = this.dialogService.open(this.customerEdit, {draggable: true, size: 'inherit'});
    this.dialog.afterClosed$.pipe(single()).subscribe(_ => this.location.back());
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

  public close(): void {
    this.dialog?.close();
  }
}
