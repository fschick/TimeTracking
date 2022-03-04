import {Component} from '@angular/core';
import {environment} from '../../../../environments/environment';
import {LocalizationService} from '../../../shared/services/internationalization/localization.service';

@Component({
  selector: 'ts-page-navigation',
  templateUrl: './page-navigation.component.html',
  styleUrls: ['./page-navigation.component.scss']
})
export class PageNavigationComponent {
  public isDevelopment: boolean;
  public currentLanguage: string;


  constructor(
    private localizationService: LocalizationService
  ) {
    this.isDevelopment = !environment.production;
    this.currentLanguage = localizationService.language;
  }

  public setLanguage(language: string): void {
    this.localizationService.language = language;
    location.reload();
  }
}
