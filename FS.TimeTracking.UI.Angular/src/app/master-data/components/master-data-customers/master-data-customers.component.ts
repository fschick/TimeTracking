import {Component} from '@angular/core';
import {CustomerDto, CustomerService} from '../../../shared/services/api';
import {Observable} from 'rxjs';
import {Column, Configuration} from '../../../shared/components/simple-table/simple-table.component';
import {StorageService} from '../../../shared/services/storage/storage.service';

@Component({
  selector: 'ts-master-data-customers',
  templateUrl: './master-data-customers.component.html',
  styleUrls: ['./master-data-customers.component.scss']
})
export class MasterDataCustomersComponent {

  public rows$: Observable<CustomerDto[]>;
  public columns!: Column<CustomerDto>[];
  public configuration?: Partial<Configuration<CustomerDto>>;

  private trueString = $localize`:@@Common.True:[i18n] true`;
  private falseString = $localize`:@@Common.False:[i18n] false`;

  constructor(
    private customerService: CustomerService,
    private storageService: StorageService,
  ) {
    this.rows$ = this.customerService.query();

    this.configuration = {
      cssWrapper: 'table-responsive',
      cssTable: 'table table-borderless small',
      glyphSortAsc: '',
      glyphSortDesc: '',
      locale: this.storageService.language,
      trackBy: (_, row) => row.id,
    };

    const hideBelowViewPointMd = {cssHeadCell: 'd-none d-md-table-cell', cssDataCell: 'd-none d-md-table-cell'};
    this.columns = [
      {title: $localize`:@@DTOs.CustomerDto.ShortName:[i18n] Short name`, prop: 'shortName'},
      {title: $localize`:@@DTOs.CustomerDto.CompanyName:[i18n] Company`, prop: 'companyName'},
      {title: $localize`:@@DTOs.CustomerDto.ContactName:[i18n] Contact`, prop: 'contactName'},
      {title: $localize`:@@DTOs.CustomerDto.Street:[i18n] Street`, prop: 'street', ...hideBelowViewPointMd},
      {title: $localize`:@@DTOs.CustomerDto.ZipCode:[i18n] Zip`, prop: 'zipCode', ...hideBelowViewPointMd},
      {title: $localize`:@@DTOs.CustomerDto.City:[i18n] City`, prop: 'city', ...hideBelowViewPointMd},
      {title: $localize`:@@DTOs.CustomerDto.Country:[i18n] Country`, prop: 'country', ...hideBelowViewPointMd},
      {title: $localize`:@@DTOs.CustomerDto.Hidden:[i18n] Hidden`, prop: 'hidden', format: row => row.hidden ? this.trueString : this.falseString},
    ];
  }
}
