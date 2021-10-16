import {AfterViewInit, Component, ElementRef, ViewChild} from '@angular/core';
import {FormValidationService, ValidationFormGroup} from '../../../shared/services/form-validation/form-validation.service';
import {Observable} from 'rxjs';
import {OrderDto, OrderService, StringTypeaheadDto, TypeaheadService} from '../../../shared/services/api';
import {Location} from '@angular/common';
import {ActivatedRoute} from '@angular/router';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {single} from 'rxjs/operators';
import {Modal} from 'bootstrap';
import {GuidService} from '../../../shared/services/state-management/guid.service';

@Component({
  selector: 'ts-master-data-orders-edit',
  templateUrl: './master-data-orders-edit.component.html',
  styleUrls: ['./master-data-orders-edit.component.scss']
})
export class MasterDataOrdersEditComponent implements AfterViewInit {
  @ViewChild('orderEdit') private orderEdit?: ElementRef;
  @ViewChild('title') private title?: ElementRef;

  public orderForm: ValidationFormGroup;
  public isNewRecord: boolean;
  public customers$: Observable<StringTypeaheadDto[]>;
  private modal!: Modal;

  constructor(
    private location: Location,
    private route: ActivatedRoute,
    private orderService: OrderService,
    private entityService: EntityService,
    private formValidationService: FormValidationService,
    typeaheadService: TypeaheadService,
  ) {
    this.isNewRecord = this.route.snapshot.params.id === GuidService.guidEmpty;
    this.orderForm = this.formValidationService.getFormGroup<OrderDto>('OrderDto', {id: GuidService.guidEmpty, hidden: false});

    if (!this.isNewRecord)
      this.orderService
        .get({id: this.route.snapshot.params.id})
        .pipe(single())
        .subscribe(order => this.orderForm.patchValue(order));

    this.customers$ = typeaheadService.getCustomers({showHidden: true});
  }

  public ngAfterViewInit(): void {
    this.modal = new bootstrap.Modal(this.orderEdit?.nativeElement);
    this.orderEdit?.nativeElement.addEventListener('hide.bs.modal', () => this.location.back());
    this.orderEdit?.nativeElement.addEventListener('shown.bs.modal', () => this.title?.nativeElement.focus());
    this.modal.show();
  }

  public save(): void {
    if (!this.orderForm.valid)
      return;

    const apiAction = this.isNewRecord
      ? this.orderService.create({orderDto: this.orderForm.value})
      : this.orderService.update({orderDto: this.orderForm.value});

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
    this.modal?.hide();
  }
}
