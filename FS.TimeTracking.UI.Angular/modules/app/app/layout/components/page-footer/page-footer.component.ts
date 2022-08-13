import {Component} from '@angular/core';
import {InformationService, ProductInformationDto} from '../../../../../api/timetracking';
import {combineLatest, Observable} from 'rxjs';
import {map, single} from 'rxjs/operators';

@Component({
  selector: 'ts-page-footer',
  templateUrl: './page-footer.component.html',
  styleUrls: ['./page-footer.component.scss']
})
export class PageFooterComponent {

  public info$: Observable<ProductInformationDto>;

  constructor(
    private readonly informationService: InformationService
  ) {
    this.info$ = this.informationService.getProductInformation().pipe(single());
  }
}
