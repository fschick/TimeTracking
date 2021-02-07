import {Component, OnInit, ViewChild} from '@angular/core';
import {CustomerDto, CustomerService} from '../../../shared/services/api';
import {Observable} from 'rxjs';
import {StorageService} from '../../../shared/services/storage/storage.service';
import {Column, Configuration, DataCellTemplate, SimpleTableComponent} from '../../../shared/components/simple-table/simple-table.component';

@Component({
  selector: 'ts-master-data-customers',
  templateUrl: './master-data-customers.component.html',
  styleUrls: ['./master-data-customers.component.scss']
})
export class MasterDataCustomersComponent implements OnInit {
  @ViewChild(SimpleTableComponent) private customerTable?: SimpleTableComponent<CustomerDto>;
  @ViewChild('editCellTemplate', {static: true}) private editCellTemplate?: DataCellTemplate<CustomerDto>;
  @ViewChild('dataCellTemplate', {static: true}) private dataCellTemplate?: DataCellTemplate<CustomerDto>;

  public rows$?: Observable<CustomerDto[]>;
  public rows: CustomerDto[];
  public columns!: Column<CustomerDto>[];
  public configuration?: Partial<Configuration<CustomerDto>>;

  constructor(
    private customerService: CustomerService,
    private storageService: StorageService,
  ) {
    this.rows = [];
  }

  public ngOnInit(): void {
    this.rows$ = this.customerService.query();

    this.configuration = {
      cssWrapper: 'table-responsive',
      cssTable: 'table table-borderless table-hover small',
      glyphSortAsc: '',
      glyphSortDesc: '',
      locale: this.storageService.language,
      trackBy: (_, row: CustomerDto) => row.id,
    };

    // const hideBelowViewPointMd = {cssHeadCell: 'd-none d-md-table-cell', cssDataCell: 'd-none d-md-table-cell'};
    const dataCellCss = (row: CustomerDto) => row.hidden ? 'text-secondary text-decoration-line-through' : '';
    this.columns = [
      {title: '', prop: 'id', dataCellTemplate: this.editCellTemplate, width: '3%'},
      {title: $localize`:@@DTOs.CustomerDto.ShortName:[i18n] Short name`, prop: 'shortName', cssDataCell: dataCellCss, dataCellTemplate: this.dataCellTemplate},
      // {title: $localize`:@@DTOs.CustomerDto.CompanyName:[i18n] Company`, prop: 'companyName'},
      {title: $localize`:@@DTOs.CustomerDto.ContactName:[i18n] Contact`, prop: 'contactName', cssDataCell: dataCellCss, dataCellTemplate: this.dataCellTemplate},
      // {title: $localize`:@@DTOs.CustomerDto.Street:[i18n] Street`, prop: 'street', ...hideBelowViewPointMd},
      // {title: $localize`:@@DTOs.CustomerDto.ZipCode:[i18n] Zip`, prop: 'zipCode', ...hideBelowViewPointMd},
      // {title: $localize`:@@DTOs.CustomerDto.City:[i18n] City`, prop: 'city', ...hideBelowViewPointMd},
      // {title: $localize`:@@DTOs.CustomerDto.Country:[i18n] Country`, prop: 'country', ...hideBelowViewPointMd},
      // {title: $localize`:@@DTOs.CustomerDto.Hidden:[i18n] Hidden`, prop: 'hidden', format: row => row.hidden ? this.trueString : this.falseString},
    ];
  }

  getCellValue(row: CustomerDto, column: Column<CustomerDto>) {
    return this.customerTable?.getCellValue(row, column);
    // return row[column.prop];
  }
}
