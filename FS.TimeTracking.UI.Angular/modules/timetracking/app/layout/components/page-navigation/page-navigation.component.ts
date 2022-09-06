import {Component} from '@angular/core';
import {environment} from '../../../../environments/environment';
import {LocalizationService} from '../../../../../core/app/services/internationalization/localization.service';
import {$localizeId} from '../../../../../core/app/services/internationalization/localizeId';
import {Observable} from 'rxjs';
import {SettingService} from '../../../../../api/timetracking';
import {map} from 'rxjs/operators';

@Component({
  selector: 'ts-page-navigation',
  templateUrl: './page-navigation.component.html',
  styleUrls: ['./page-navigation.component.scss']
})
export class PageNavigationComponent {
  public isDevelopment: boolean;
  public languageCode: string;
  public languageName: string;
  public reportingEnabled: Observable<boolean>;

  constructor(
    settingService: SettingService,
    private localizationService: LocalizationService,
  ) {
    this.isDevelopment = !environment.production;
    this.languageCode = localizationService.language;

    this.reportingEnabled = settingService.getFeatures().pipe(map(x => x.reporting))

    const languageTransUnitId = `@@Language.${this.languageCode}`;
    this.languageName = $localizeId`${languageTransUnitId}:TRANSUNITID:`;
  }

  public setLanguage(language: string): void {
    this.localizationService.language = language;
    location.reload();
  }
}
