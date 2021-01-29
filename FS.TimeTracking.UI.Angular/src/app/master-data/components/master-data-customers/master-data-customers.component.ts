import {Component} from '@angular/core';
import {CustomerDto, CustomerService} from '../../../shared/services/api';
import {Observable} from 'rxjs';
import {ColumnMode} from '@swimlane/ngx-datatable';

@Component({
  selector: 'ts-master-data-customers',
  templateUrl: './master-data-customers.component.html',
  styleUrls: ['./master-data-customers.component.scss']
})
export class MasterDataCustomersComponent {
  public rows$: Observable<Array<CustomerDto>>;

  public columnMode = ColumnMode.force;
  public columns = [
    {name: $localize`:@@DTOs.CustomerDto.ShortName:[i18n] Short name`, prop: 'shortName'},
    {name: $localize`:@@DTOs.CustomerDto.CompanyName:[i18n] Company`, prop: 'companyName'},
    // {name: $localize`:@@DTOs.CustomerDto.ContactName:[i18n] Contact`, prop: 'contactName'},
    // {name: $localize`:@@DTOs.CustomerDto.Street:[i18n] Street`, prop: 'street'},
    // {name: $localize`:@@DTOs.CustomerDto.ZipCode:[i18n] Zip`, prop: 'zipCode'},
    {name: $localize`:@@DTOs.CustomerDto.City:[i18n] City`, prop: 'city'},
    // {name: $localize`:@@DTOs.CustomerDto.Country:[i18n] Country`, prop: 'country'},
    {name: $localize`:@@DTOs.CustomerDto.Hidden:[i18n] Hidden`, prop: 'hidden'},
  ];

  constructor(private customerService: CustomerService) {
    this.rows$ = this.customerService.query();
  }
}
