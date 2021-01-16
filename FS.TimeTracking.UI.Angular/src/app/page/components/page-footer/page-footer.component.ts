import {Component, OnInit} from '@angular/core';
import {InformationService} from "../../../shared/services/api";
import {combineLatest, Observable} from "rxjs";
import {map} from "rxjs/operators";

@Component({
  selector: 'page-footer',
  templateUrl: './page-footer.component.html',
  styleUrls: ['./page-footer.component.scss']
})
export class PageFooterComponent {

  public info$: Observable<{ productName: string, productVersion: string, copyright: string }>;

  constructor(
    private readonly _informationService: InformationService
  ) {
    this.info$ = combineLatest([
      this._informationService.getProductName(),
      this._informationService.getProductVersion(),
      this._informationService.getProductCopyright(),
    ]).pipe(map(([productName, productVersion, copyright]) =>
      ({productName, productVersion, copyright}))
    );
  }
}
