import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {
  Column,
  Configuration,
  DataCellClickEvent,
  DataCellTemplate,
  SimpleTableComponent
} from '../../../shared/components/simple-table/simple-table.component';
import {OrderListDto, OrderService} from '../../../shared/services/api';
import {Subscription} from 'rxjs';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {ActivatedRoute, Router} from '@angular/router';
import {StorageService} from '../../../shared/services/storage/storage.service';
import {single} from 'rxjs/operators';

@Component({
  selector: 'ts-master-data-orders',
  templateUrl: './master-data-orders.component.html',
  styleUrls: ['./master-data-orders.component.scss']
})
export class MasterDataOrdersComponent implements OnInit, OnDestroy {

  @ViewChild(SimpleTableComponent) private orderTable?: SimpleTableComponent<OrderListDto>;
  @ViewChild('dataCellTemplate', {static: true}) private dataCellTemplate?: DataCellTemplate<OrderListDto>;
  @ViewChild('actionCellTemplate', {static: true}) private actionCellTemplate?: DataCellTemplate<OrderListDto>;

  public rows: OrderListDto[];
  public columns!: Column<OrderListDto>[];
  public configuration?: Partial<Configuration<OrderListDto>>;

  private subscriptions = new Subscription();

  constructor(
    public entityService: EntityService,
    private router: Router,
    private route: ActivatedRoute,
    private orderService: OrderService,
    private storageService: StorageService,
  ) {
    this.rows = [];
  }

  public ngOnInit(): void {
    this.orderService.list().pipe(single()).subscribe(x => this.rows = x);

    const orderChanged = this.entityService.orderChanged
      .pipe(this.entityService.replaceEntityWithListDto(this.orderService))
      .subscribe(changedEvent => {
          const updatedRows = this.entityService.updateCollection(this.rows, 'id', changedEvent);
          return this.rows = [...updatedRows];
        }
      );
    this.subscriptions.add(orderChanged);

    this.configuration = {
      cssWrapper: 'table-responsive',
      cssTable: 'table table-borderless table-hover small',
      glyphSortAsc: '',
      glyphSortDesc: '',
      locale: this.storageService.language,
    };

    const dataCellCss = (row: OrderListDto) => row.hidden ? 'text-secondary text-decoration-line-through' : '';
    this.columns = [
      {title: $localize`:@@DTO.OrderListDto.Title:[i18n] Order`, prop: 'title', cssDataCell: dataCellCss, dataCellTemplate: this.dataCellTemplate},
      {
        title: $localize`:@@DTO.OrderListDto.CustomerTitle:[i18n] Customer`,
        prop: 'customerTitle',
        cssDataCell: dataCellCss,
        dataCellTemplate: this.dataCellTemplate
      },
      {
        title: $localize`:@@DTO.OrderListDto.CustomerCompanyName:[i18n] Company`,
        prop: 'customerCompanyName',
        cssDataCell: dataCellCss,
        dataCellTemplate: this.dataCellTemplate
      },
      {
        title: $localize`:@@Common.Action:[i18n] Action`,
        customId: 'delete',
        dataCellTemplate: this.actionCellTemplate,
        cssDataCell: 'text-nowrap align-middle action-cell',
        width: '1px',
        sortable: false
      },
    ];
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  public getDataCellValue(row: OrderListDto, column: Column<OrderListDto>): string {
    return this.orderTable?.getCellValue(row, column) ?? '';
  }

  public dataCellClick($event: DataCellClickEvent<OrderListDto>): void {
    if ($event.column.customId !== 'delete')
      this.router.navigate([$event.row.id], {relativeTo: this.route});
  }

  public deleteItem(id: string): void {
    this.orderService
      .delete(id)
      .pipe(single())
      .subscribe(() => {
        this.entityService.orderChanged.next({entity: {id} as OrderListDto, action: 'deleted'});
      });
  }
}
