import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {CustomerDto, CustomerService} from '../../../shared/services/api';
import {Subscription} from 'rxjs';
import {StorageService} from '../../../shared/services/storage/storage.service';
import {Column, Configuration, DataCellTemplate, SimpleTableComponent} from '../../../shared/components/simple-table/simple-table.component';
import {single} from 'rxjs/operators';
import {EntityChanged, EntityService} from '../../../shared/services/state-management/entity.service';

@Component({
  selector: 'ts-master-data-customers',
  templateUrl: './master-data-customers.component.html',
  styleUrls: ['./master-data-customers.component.scss']
})
export class MasterDataCustomersComponent implements OnInit, OnDestroy {
  @ViewChild(SimpleTableComponent) private customerTable?: SimpleTableComponent<CustomerDto>;
  @ViewChild('editCellTemplate', {static: true}) private editCellTemplate?: DataCellTemplate<CustomerDto>;
  @ViewChild('dataCellTemplate', {static: true}) private dataCellTemplate?: DataCellTemplate<CustomerDto>;

  public rows: CustomerDto[];
  public columns!: Column<CustomerDto>[];
  public configuration?: Partial<Configuration<CustomerDto>>;

  private subscriptions = new Subscription();

  constructor(
    private customerService: CustomerService,
    private entityService: EntityService,
    private storageService: StorageService,
  ) {
    this.rows = [];
  }

  public ngOnInit(): void {
    this.customerService.query().pipe(single()).subscribe(x => this.rows = x);
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
      locale: this.storageService.language,
    };

    // const hideBelowViewPointMd = {cssHeadCell: 'd-none d-md-table-cell', cssDataCell: 'd-none d-md-table-cell'};
    const dataCellCss = (row: CustomerDto) => row.hidden ? 'text-secondary text-decoration-line-through' : '';
    this.columns = [
      {title: '', prop: 'id', dataCellTemplate: this.editCellTemplate, width: '3%'},
      {title: $localize`:@@DTO.CustomerDto.ShortName:[i18n] Short name`, prop: 'shortName', cssDataCell: dataCellCss, dataCellTemplate: this.dataCellTemplate},
      {title: $localize`:@@DTO.CustomerDto.CompanyName:[i18n] Company`, prop: 'companyName', cssDataCell: dataCellCss, dataCellTemplate: this.dataCellTemplate},
      {title: $localize`:@@DTO.CustomerDto.ContactName:[i18n] Contact`, prop: 'contactName', cssDataCell: dataCellCss, dataCellTemplate: this.dataCellTemplate},
      // {title: $localize`:@@DTO.CustomerDto.Street:[i18n] Street`, prop: 'street', ...hideBelowViewPointMd},
      // {title: $localize`:@@DTO.CustomerDto.ZipCode:[i18n] Zip`, prop: 'zipCode', ...hideBelowViewPointMd},
      // {title: $localize`:@@DTO.CustomerDto.City:[i18n] City`, prop: 'city', ...hideBelowViewPointMd},
      // {title: $localize`:@@DTO.CustomerDto.Country:[i18n] Country`, prop: 'country', ...hideBelowViewPointMd},
      // {title: $localize`:@@DTO.CustomerDto.Hidden:[i18n] Hidden`, prop: 'hidden', format: row => row.hidden ? this.trueString : this.falseString},
    ];
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }
}
