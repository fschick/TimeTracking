import {Component} from '@angular/core';
import {environment} from '../../../../environments/environment';
import {LocalizationService} from '../../../../../core/app/services/internationalization/localization.service';
import {$localizeId} from '../../../../../core/app/services/internationalization/localizeId';
import {ConfigurationService} from '../../../../../core/app/services/configuration.service';
import {AuthenticationService, User} from '../../../../../core/app/services/authentication.service';

@Component({
  selector: 'ts-page-navigation',
  templateUrl: './page-navigation.component.html',
  styleUrls: ['./page-navigation.component.scss']
})
export class PageNavigationComponent {
  public isDevelopment: boolean;
  public languageCode: string;
  public languageName: string;
  public reportingEnabled: boolean;
  public authorizationEnabled: boolean;
  public currentUser: User;

  constructor(
    configurationService: ConfigurationService,
    private localizationService: LocalizationService,
    private authenticationService: AuthenticationService,
  ) {
    this.isDevelopment = !environment.production;
    this.languageCode = localizationService.language;

    this.reportingEnabled = configurationService.clientConfiguration.features.reporting;
    this.authorizationEnabled = configurationService.clientConfiguration.features.authorization;

    const languageTransUnitId = `@@Language.${this.languageCode}`;
    this.languageName = $localizeId`${languageTransUnitId}:TRANSUNITID:`;

    this.currentUser = authenticationService.currentUser;
  }

  public login(): void {
    this.authenticationService.login();
  }

  public logout(): void {
    this.authenticationService.logout();
  }

  public setLanguage(language: string): void {
    this.localizationService.language = language;
    location.reload();
  }
}
