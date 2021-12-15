import {Component, OnInit, ViewChild} from '@angular/core';
import {CustomerDto, CustomerService} from '../../../shared/services/api';
import {Observable, Subject} from 'rxjs';
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
export class MasterDataCustomersComponent implements OnInit {
  @ViewChild(SimpleTableComponent) private customerTable?: SimpleTableComponent<CustomerDto>;
  @ViewChild('dataCellTemplate', {static: true}) private dataCellTemplate?: DataCellTemplate<CustomerDto>;
  @ViewChild('actionCellTemplate', {static: true}) private actionCellTemplate?: DataCellTemplate<CustomerDto>;

  public guidService = GuidService;
  public rows$: Observable<CustomerDto[]>;
  public columns!: Column<CustomerDto>[];
  public configuration?: Partial<Configuration<CustomerDto>>;
  public filters: (Filter | FilterName)[];
  public filterChanged = new Subject<FilteredRequestParams>();

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

    this.rows$ = this.filterChanged
      .pipe(
        switchMap(filter => this.loadData(filter)),
        this.entityService.withUpdatesFrom(this.entityService.customerChanged, this.customerService),
      );
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

  public getDataCellValue(row: CustomerDto, column: Column<CustomerDto>): string {
    return this.customerTable?.getCellValue(row, column) ?? '';
  }

  private loadData(filter: FilteredRequestParams): Observable<CustomerDto[]> {
    return this.customerService.getListFiltered(filter)
      .pipe(single());
  }

  public deleteItem(id: string): void {
    this.customerService
      .delete({id})
      .pipe(single())
      .subscribe(() => {
        this.entityService.customerChanged.next({entity: {id} as CustomerDto, action: 'deleted'});
      });
  }
}
