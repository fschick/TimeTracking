import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {Column, Configuration, DataCellTemplate, SimpleTableComponent} from '../../../../../core/app/components/simple-table/simple-table.component';
import {OrderGridDto, OrderService} from '../../../../../api/timetracking';
import {Observable, Subscription} from 'rxjs';
import {EntityService} from '../../../../../core/app/services/state-management/entity.service';
import {ActivatedRoute, Router} from '@angular/router';
import {LocalizationService} from '../../../../../core/app/services/internationalization/localization.service';
import {single, switchMap} from 'rxjs/operators';
import {GuidService} from '../../../../../core/app/services/state-management/guid.service';
import {Filter, FilteredRequestParams, FilterName} from '../../../../../core/app/components/filter/filter.component';
import {DateTime} from 'luxon';
import {AuthenticationService} from '../../../../../core/app/services/authentication.service';

@Component({
  selector: 'ts-master-data-orders',
  templateUrl: './master-data-orders.component.html',
  styleUrls: ['./master-data-orders.component.scss']
})
export class MasterDataOrdersComponent implements OnInit, OnDestroy {

  @ViewChild(SimpleTableComponent) private orderTable?: SimpleTableComponent<OrderGridDto>;
  @ViewChild('dataCellTemplate', {static: true}) private dataCellTemplate?: DataCellTemplate<OrderGridDto>;
  @ViewChild('actionCellTemplate', {static: true}) private actionCellTemplate?: DataCellTemplate<OrderGridDto>;

  public guidService = GuidService;
  public rows?: OrderGridDto[];
  public columns!: Column<OrderGridDto>[];
  public configuration?: Partial<Configuration<OrderGridDto>>;
  public filters: (Filter | FilterName)[];
  public createNewDisabled: boolean;

  private readonly subscriptions = new Subscription();

  constructor(
    private entityService: EntityService,
    private router: Router,
    private route: ActivatedRoute,
    private orderService: OrderService,
    private localizationService: LocalizationService,
    authenticationService: AuthenticationService,
  ) {
    this.createNewDisabled = !authenticationService.currentUser.hasRole.masterDataOrdersManage;

    const defaultStartDate = DateTime.now().startOf('year');
    const defaultEndDate = DateTime.now().endOf('year');

    this.filters = [
      {name: 'orderStartDate', defaultValue: defaultStartDate, isPrimary: true},
      {name: 'orderDueDate', defaultValue: defaultEndDate, isPrimary: true},
      {name: 'orderId', showHidden: true, isPrimary: true},
      {name: 'customerId', showHidden: true, isPrimary: true},
    ];
  }

  public ngOnInit(): void {
    const filterChanged = this.entityService.reloadRequested
      .pipe(
        switchMap(filter => this.loadData(filter)),
        this.entityService.withUpdatesFrom(this.entityService.orderChanged, this.orderService),
      )
      .subscribe(rows => this.rows = rows);
    this.subscriptions.add(filterChanged);

    this.configuration = {
      cssWrapper: 'table-responsive',
      cssTable: 'table',
      glyphSortAsc: '',
      glyphSortDesc: '',
      locale: this.localizationService.language,
    };

    const cssHeadCell = 'border-0 text-nowrap';
    const cssHeadCellMd = 'd-none d-md-table-cell';
    const cssDataCellMd = cssHeadCellMd;
    this.columns = [
      {
        title: $localize`:@@DTO.OrderGridDto.Title:[i18n] Order`,
        cssHeadCell: cssHeadCell,
        prop: 'title',
        dataCellTemplate: this.dataCellTemplate
      }, {
        title: $localize`:@@DTO.OrderGridDto.CustomerTitle:[i18n] Customer`,
        prop: 'customerTitle',
        cssHeadCell: cssHeadCell,
        dataCellTemplate: this.dataCellTemplate
      }, {
        title: $localize`:@@DTO.OrderGridDto.StartDate:[i18n] Start date`,
        prop: 'startDate',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd}`,
        cssDataCell: cssDataCellMd,
        dataCellTemplate: this.dataCellTemplate,
        format: (row) => row.startDate.toFormat(this.localizationService.dateTime.dateFormat)
      }, {
        title: $localize`:@@DTO.OrderGridDto.DueDate:[i18n] Due date`,
        prop: 'dueDate',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd}`,
        cssDataCell: cssDataCellMd,
        dataCellTemplate: this.dataCellTemplate,
        format: (row) => row.dueDate.toFormat(this.localizationService.dateTime.dateFormat)
      }, {
        title: '',
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

  private loadData(filter: FilteredRequestParams): Observable<OrderGridDto[]> {
    return this.orderService.getGridFiltered(filter)
      .pipe(single());
  }

  public deleteItem(id: string): void {
    this.orderService
      .delete({id})
      .pipe(single())
      .subscribe(() => {
        this.entityService.orderChanged.next({entity: {id} as OrderGridDto, action: 'deleted'});
      });
  }
}
