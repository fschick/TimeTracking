import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {CustomerGridDto, CustomerService} from '../../../../../api/timetracking';
import {Observable, Subscription} from 'rxjs';
import {LocalizationService} from '../../../../../core/app/services/internationalization/localization.service';
import {Column, Configuration, DataCellTemplate, SimpleTableComponent} from '../../../../../core/app/components/simple-table/simple-table.component';
import {single, switchMap} from 'rxjs/operators';
import {ActivatedRoute, Router} from '@angular/router';
import {EntityService} from '../../../../../core/app/services/state-management/entity.service';
import {GuidService} from '../../../../../core/app/services/state-management/guid.service';
import {Filter, FilteredRequestParams, FilterName} from '../../../../../core/app/components/filter/filter.component';
import {AuthenticationService} from '../../../../../core/app/services/authentication.service';

@Component({
  selector: 'ts-master-data-customers',
  templateUrl: './master-data-customers.component.html',
  styleUrls: ['./master-data-customers.component.scss']
})
export class MasterDataCustomersComponent implements OnInit, OnDestroy {
  @ViewChild(SimpleTableComponent) private customerTable?: SimpleTableComponent<CustomerGridDto>;
  @ViewChild('dataCellTemplate', {static: true}) private dataCellTemplate?: DataCellTemplate<CustomerGridDto>;
  @ViewChild('actionCellTemplate', {static: true}) private actionCellTemplate?: DataCellTemplate<CustomerGridDto>;

  public guidService = GuidService;
  public rows?: CustomerGridDto[];
  public columns!: Column<CustomerGridDto>[];
  public configuration?: Partial<Configuration<CustomerGridDto>>;
  public filters: (Filter | FilterName)[];
  public createNewDisabled: boolean;

  private readonly subscriptions = new Subscription();

  constructor(
    private entityService: EntityService,
    private router: Router,
    private route: ActivatedRoute,
    private customerService: CustomerService,
    private localizationService: LocalizationService,
    authenticationService: AuthenticationService,
  ) {
    this.createNewDisabled = !authenticationService.currentUser.hasRole.masterDataCustomersManage;

    this.filters = [
      {name: 'customerId', showHidden: true, isPrimary: true},
      {name: 'customerNumber', isPrimary: true},
      {name: 'customerCompanyName', isPrimary: true},
      {name: 'customerHidden', isPrimary: true},
    ];
  }

  public ngOnInit(): void {
    const filterChanged = this.entityService.reloadRequested
      .pipe(
        switchMap(filter => this.loadData(filter)),
        this.entityService.withUpdatesFrom(this.entityService.customerChanged, this.customerService),
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

  private loadData(filter: FilteredRequestParams): Observable<CustomerGridDto[]> {
    return this.customerService.getGridFiltered(filter)
      .pipe(single());
  }

  public deleteItem(id: string): void {
    this.customerService
      .delete({id})
      .pipe(single())
      .subscribe(() => {
        this.entityService.customerChanged.next({entity: {id} as CustomerGridDto, action: 'deleted'});
      });
  }
}
