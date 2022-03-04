import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {CustomerListDto, CustomerService} from '../../../shared/services/api';
import {Observable, Subject, Subscription} from 'rxjs';
import {LocalizationService} from '../../../shared/services/internationalization/localization.service';
import {
  Column,
  Configuration,
  DataCellTemplate,
  SimpleTableComponent
} from '../../../shared/components/simple-table/simple-table.component';
import {single, switchMap} from 'rxjs/operators';
import {ActivatedRoute, Router} from '@angular/router';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {GuidService} from '../../../shared/services/state-management/guid.service';
import {Filter, FilteredRequestParams, FilterName} from '../../../shared/components/filter/filter.component';

@Component({
  selector: 'ts-master-data-customers',
  templateUrl: './master-data-customers.component.html',
  styleUrls: ['./master-data-customers.component.scss']
})
export class MasterDataCustomersComponent implements OnInit, OnDestroy {
  @ViewChild(SimpleTableComponent) private customerTable?: SimpleTableComponent<CustomerListDto>;
  @ViewChild('dataCellTemplate', {static: true}) private dataCellTemplate?: DataCellTemplate<CustomerListDto>;
  @ViewChild('actionCellTemplate', {static: true}) private actionCellTemplate?: DataCellTemplate<CustomerListDto>;

  public guidService = GuidService;
  public rows?: CustomerListDto[];
  public columns!: Column<CustomerListDto>[];
  public configuration?: Partial<Configuration<CustomerListDto>>;
  public filters: (Filter | FilterName)[];
  public filterChanged = new Subject<FilteredRequestParams>();
  private readonly subscriptions = new Subscription();

  constructor(
    private entityService: EntityService,
    private router: Router,
    private route: ActivatedRoute,
    private customerService: CustomerService,
    private localizationService: LocalizationService,
  ) {
    this.filters = [
      {name: 'customerId', showHidden: true},
      {name: 'customerNumber'},
      {name: 'customerCompanyName'},
      {name: 'customerHidden'},
    ];
  }

  public ngOnInit(): void {
    const filterChanged = this.filterChanged
      .pipe(
        switchMap(filter => this.loadData(filter)),
        this.entityService.withUpdatesFrom(this.entityService.customerChanged, this.customerService),
      )
      .subscribe(rows => this.rows = rows);
    this.subscriptions.add(filterChanged);

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
        title: $localize`:@@DTO.CustomerDto.Title:[i18n] Title`,
        prop: 'title',
        cssHeadCell: cssHeadCell,
        dataCellTemplate: this.dataCellTemplate
      }, {
        title: $localize`:@@DTO.CustomerDto.CompanyName:[i18n] Company`,
        prop: 'companyName',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd}`,
        cssDataCell: cssDataCellMd,
        dataCellTemplate: this.dataCellTemplate
      }, {
        title: $localize`:@@DTO.CustomerDto.ContactName:[i18n] Contact`,
        prop: 'contactName',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd}`,
        cssDataCell: cssDataCellMd,
        dataCellTemplate: this.dataCellTemplate
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

  public getDataCellValue(row: CustomerListDto, column: Column<CustomerListDto>): string {
    return this.customerTable?.getCellValue(row, column) ?? '';
  }

  private loadData(filter: FilteredRequestParams): Observable<CustomerListDto[]> {
    return this.customerService.getListFiltered(filter)
      .pipe(single());
  }

  public deleteItem(id: string): void {
    this.customerService
      .delete({id})
      .pipe(single())
      .subscribe(() => {
        this.entityService.customerChanged.next({entity: {id} as CustomerListDto, action: 'deleted'});
      });
  }
}
