import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {
  Column,
  Configuration,
  DataCellTemplate,
  SimpleTableComponent
} from '../../../shared/components/simple-table/simple-table.component';
import {ActivityListDto, OrderListDto, OrderService} from '../../../shared/services/api';
import {Observable, Subject, Subscription} from 'rxjs';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {ActivatedRoute, Router} from '@angular/router';
import {LocalizationService} from '../../../shared/services/internationalization/localization.service';
import {single, switchMap} from 'rxjs/operators';
import {GuidService} from '../../../shared/services/state-management/guid.service';
import {Filter, FilteredRequestParams, FilterName} from '../../../shared/components/filter/filter.component';

@Component({
  selector: 'ts-master-data-orders',
  templateUrl: './master-data-orders.component.html',
  styleUrls: ['./master-data-orders.component.scss']
})
export class MasterDataOrdersComponent implements OnInit, OnDestroy {

  @ViewChild(SimpleTableComponent) private orderTable?: SimpleTableComponent<OrderListDto>;
  @ViewChild('dataCellTemplate', {static: true}) private dataCellTemplate?: DataCellTemplate<OrderListDto>;
  @ViewChild('actionCellTemplate', {static: true}) private actionCellTemplate?: DataCellTemplate<OrderListDto>;

  public guidService = GuidService;
  public rows?: OrderListDto[];
  public columns!: Column<OrderListDto>[];
  public configuration?: Partial<Configuration<OrderListDto>>;
  public filters: (Filter | FilterName)[];
  public filterChanged = new Subject<FilteredRequestParams>();
  private readonly subscriptions = new Subscription();

  constructor(
    private entityService: EntityService,
    private router: Router,
    private route: ActivatedRoute,
    private orderService: OrderService,
    private localizationService: LocalizationService,
  ) {
    this.filters = [
      {name: 'orderStartDate'},
      {name: 'orderDueDate'},
      {name: 'orderId', showHidden: true},
      {name: 'customerId', showHidden: true},
    ];

    const filterChanged = this.filterChanged
      .pipe(
        switchMap(filter => this.loadData(filter)),
        this.entityService.withUpdatesFrom(this.entityService.orderChanged, this.orderService),
      )
      .subscribe(rows => this.rows = rows);

    this.subscriptions.add(filterChanged);
  }

  public ngOnInit(): void {
    this.configuration = {
      cssWrapper: 'table-responsive',
      cssTable: 'table table-card table-sm align-middle text-break border',
      glyphSortAsc: '',
      glyphSortDesc: '',
      locale: this.localizationService.language,
    };

    const cssHeadCell = 'border-0 text-nowrap';
    const cssHeadCellMd = 'd-none d-md-table-cell';
    const cssDataCellMd = cssHeadCellMd;
    this.columns = [
      {
        title: $localize`:@@DTO.OrderListDto.Title:[i18n] Order`,
        cssHeadCell: cssHeadCell,
        prop: 'title',
        dataCellTemplate: this.dataCellTemplate
      }, {
        title: $localize`:@@DTO.OrderListDto.CustomerTitle:[i18n] Customer`,
        prop: 'customerTitle',
        cssHeadCell: cssHeadCell,
        dataCellTemplate: this.dataCellTemplate
      }, {
        title: $localize`:@@DTO.OrderListDto.StartDate:[i18n] Start date`,
        prop: 'startDate',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd}`,
        cssDataCell: cssDataCellMd,
        dataCellTemplate: this.dataCellTemplate,
        format: (row) => row.startDate.toFormat(this.localizationService.dateTime.dateFormat)
      }, {
        title: $localize`:@@DTO.OrderListDto.DueDate:[i18n] Due date`,
        prop: 'dueDate',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd}`,
        cssDataCell: cssDataCellMd,
        dataCellTemplate: this.dataCellTemplate,
        format: (row) => row.dueDate.toFormat(this.localizationService.dateTime.dateFormat)
      }, {
        title: $localize`:@@Common.Action:[i18n] Action`,
        customId: 'delete',
        dataCellTemplate: this.actionCellTemplate,
        cssHeadCell: cssHeadCell,
        cssDataCell: 'text-nowrap action-cell',
        width: '1px',
        sortable: false
      },
    ];
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private loadData(filter: FilteredRequestParams): Observable<OrderListDto[]> {
    return this.orderService.getListFiltered(filter)
      .pipe(single());
  }

  public getDataCellValue(row: OrderListDto, column: Column<OrderListDto>): string {
    return this.orderTable?.getCellValue(row, column) ?? '';
  }

  public deleteItem(id: string): void {
    this.orderService
      .delete({id})
      .pipe(single())
      .subscribe(() => {
        this.entityService.orderChanged.next({entity: {id} as OrderListDto, action: 'deleted'});
      });
  }
}
