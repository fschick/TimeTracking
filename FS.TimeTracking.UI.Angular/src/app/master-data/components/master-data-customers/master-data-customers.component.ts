import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {CustomerDto, CustomerService} from '../../../shared/services/api';
import {Subscription} from 'rxjs';
import {LocalizationService} from '../../../shared/services/internationalization/localization.service';
import {
  Column,
  Configuration,
  DataCellClickEvent,
  DataCellTemplate,
  SimpleTableComponent
} from '../../../shared/components/simple-table/simple-table.component';
import {single} from 'rxjs/operators';
import {ActivatedRoute, Router} from '@angular/router';
import {EntityChanged, EntityService} from '../../../shared/services/state-management/entity.service';
import {GuidService} from '../../../shared/services/state-management/guid.service';

@Component({
  selector: 'ts-master-data-customers',
  templateUrl: './master-data-customers.component.html',
  styleUrls: ['./master-data-customers.component.scss']
})
export class MasterDataCustomersComponent implements OnInit, OnDestroy {
  @ViewChild(SimpleTableComponent) private customerTable?: SimpleTableComponent<CustomerDto>;
  @ViewChild('dataCellTemplate', {static: true}) private dataCellTemplate?: DataCellTemplate<CustomerDto>;
  @ViewChild('actionCellTemplate', {static: true}) private actionCellTemplate?: DataCellTemplate<CustomerDto>;

  public rows: CustomerDto[];
  public columns!: Column<CustomerDto>[];
  public configuration?: Partial<Configuration<CustomerDto>>;

  private subscriptions = new Subscription();

  constructor(
    public guidService: GuidService,
    private entityService: EntityService,
    private router: Router,
    private route: ActivatedRoute,
    private customerService: CustomerService,
    private localizationService: LocalizationService,
  ) {
    this.rows = [];
  }

  public ngOnInit(): void {
    this.customerService.list().pipe(single()).subscribe(x => this.rows = x);
    const customerChanged = this.entityService.customerChanged
      .subscribe((changedEvent: EntityChanged<CustomerDto>) =>
        this.rows = [...this.entityService.updateCollection(this.rows, 'id', changedEvent)]
      );
    this.subscriptions.add(customerChanged);

    this.configuration = {
      cssWrapper: 'table-responsive',
      cssTable: 'table table-borderless table-hover small',
      glyphSortAsc: '',
      glyphSortDesc: '',
      locale: this.localizationService.language,
    };

    const dataCellCss = (row: CustomerDto) => row.hidden ? 'text-secondary text-decoration-line-through' : '';
    this.columns = [
      {title: $localize`:@@DTO.CustomerDto.Title:[i18n] Title`, prop: 'title', cssDataCell: dataCellCss, dataCellTemplate: this.dataCellTemplate},
      {title: $localize`:@@DTO.CustomerDto.CompanyName:[i18n] Company`, prop: 'companyName', cssDataCell: dataCellCss, dataCellTemplate: this.dataCellTemplate},
      {title: $localize`:@@DTO.CustomerDto.ContactName:[i18n] Contact`, prop: 'contactName', cssDataCell: dataCellCss, dataCellTemplate: this.dataCellTemplate},
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

  public getDataCellValue(row: CustomerDto, column: Column<CustomerDto>): string {
    return this.customerTable?.getCellValue(row, column) ?? '';
  }

  dataCellClick($event: DataCellClickEvent<CustomerDto>) {
    if ($event.column.customId !== 'delete') {
      // noinspection JSIgnoredPromiseFromCall
      this.router.navigate([$event.row.id], {relativeTo: this.route});
    }
  }

  public deleteItem(id: string): void {
    this.customerService
      .delete(id)
      .pipe(single())
      .subscribe(() => {
        this.entityService.customerChanged.next({entity: {id} as CustomerDto, action: 'deleted'});
      });
  }
}
