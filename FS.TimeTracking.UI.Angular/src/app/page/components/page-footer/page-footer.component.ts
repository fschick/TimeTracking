import {Component, OnInit} from '@angular/core';
import {InformationService} from '../../../shared/services/api';
import {combineLatest, Observable} from 'rxjs';
import {map} from 'rxjs/operators';

@Component({
  selector: 'ts-page-footer',
  templateUrl: './page-footer.component.html',
  styleUrls: ['./page-footer.component.scss']
})
export class PageFooterComponent {

  public info$: Observable<{ productName: string; productVersion: string; copyright: string }>;

  constructor(
    private readonly informationService: InformationService
  ) {
    this.info$ = combineLatest([
      this.informationService.getProductName(),
      this.informationService.getProductVersion(),
      this.informationService.getProductCopyright(),
    ]).pipe(map(([productName, productVersion, copyright]) =>
      ({productName, productVersion, copyright}))
    );
  }
}
