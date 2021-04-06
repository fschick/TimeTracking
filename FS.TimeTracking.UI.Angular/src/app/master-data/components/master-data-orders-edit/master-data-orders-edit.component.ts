import {AfterViewInit, Component, ElementRef, OnInit, TemplateRef, ViewChild} from '@angular/core';
import {FormValidationService, ValidationFormGroup} from '../../../shared/services/form-validation/form-validation.service';
import {Observable} from 'rxjs';
import {OrderDto, OrderService, StringTypeaheadDto, TypeaheadService} from '../../../shared/services/api';
import {DialogRef, DialogService} from '@ngneat/dialog';
import {Location} from '@angular/common';
import {ActivatedRoute} from '@angular/router';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {single} from 'rxjs/operators';

@Component({
  selector: 'ts-master-data-orders-edit',
  templateUrl: './master-data-orders-edit.component.html',
  styleUrls: ['./master-data-orders-edit.component.scss']
})
export class MasterDataOrdersEditComponent implements AfterViewInit {
  @ViewChild('orderEdit') private orderEdit?: TemplateRef<any>;
  @ViewChild('shortName') private shortName?: ElementRef;

  public orderForm: ValidationFormGroup;
  public isNewRecord: boolean;
  public customers$: Observable<StringTypeaheadDto[]>;
  private dialog?: DialogRef<unknown, any, TemplateRef<any>>;

  constructor(
    private location: Location,
    private route: ActivatedRoute,
    private orderService: OrderService,
    private entityService: EntityService,
    private formValidationService: FormValidationService,
    typeaheadService: TypeaheadService,
    private dialogService: DialogService,
  ) {
    this.isNewRecord = this.route.snapshot.params.id === this.entityService.guidEmpty;
    this.orderForm = this.formValidationService.getFormGroup<OrderDto>('OrderDto', {id: this.entityService.guidEmpty, hidden: false});

    if (!this.isNewRecord)
      this.orderService
        .get(this.route.snapshot.params.id)
        .pipe(single())
        .subscribe(order => this.orderForm.patchValue(order));

    this.customers$ = typeaheadService.getCustomers();
  }

  public ngAfterViewInit(): void {
    if (!this.orderEdit)
      return;

    this.dialog = this.dialogService.open(this.orderEdit, {draggable: true, size: 'inherit'});
    this.dialog.afterClosed$.pipe(single()).subscribe(_ => this.location.back());
  }

  public save(): void {
    if (!this.orderForm.valid)
      return;

    const apiAction = this.isNewRecord
      ? this.orderService.create(this.orderForm.value)
      : this.orderService.update(this.orderForm.value);

    const orderChangedAction = this.isNewRecord
      ? 'created'
      : 'updated';

    apiAction
      .pipe(single())
      .subscribe(order => {
        this.close();
        this.entityService.orderChanged.next({entity: order, action: orderChangedAction});
      });
  }

  public close(): void {
    this.dialog?.close();
  }
}
