import {Component} from '@angular/core';
import {environment} from '../../../../environments/environment';

@Component({
  selector: 'ts-page-navigation',
  templateUrl: './page-navigation.component.html',
  styleUrls: ['./page-navigation.component.scss']
})
export class PageNavigationComponent {
  isDevelopment: boolean;

  constructor() {
    this.isDevelopment = !environment.production;
  }
}
