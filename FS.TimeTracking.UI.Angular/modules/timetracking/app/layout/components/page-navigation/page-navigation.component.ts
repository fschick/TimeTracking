import {Component} from '@angular/core';
import {environment} from '../../../../environments/environment';
import {LocalizationService} from '../../../../../core/app/services/internationalization/localization.service';
import {$localizeId} from '../../../../../core/app/services/internationalization/localizeId';

@Component({
  selector: 'ts-page-navigation',
  templateUrl: './page-navigation.component.html',
  styleUrls: ['./page-navigation.component.scss']
})
export class PageNavigationComponent {
  public isDevelopment: boolean;
  public languageCode: string;
  public languageName: string;

  constructor(
    private localizationService: LocalizationService
  ) {
    this.isDevelopment = !environment.production;
    this.languageCode = localizationService.language;

    const languageTransUnitId = `@@Language.${this.languageCode}`;
    this.languageName = $localizeId`${languageTransUnitId}:TRANSUNITID:`;
  }

  public setLanguage(language: string): void {
    this.localizationService.language = language;
    location.reload();
  }
}
