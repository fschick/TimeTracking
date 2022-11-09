import {BrowserModule} from '@angular/platform-browser';
import {APP_INITIALIZER, LOCALE_ID, NgModule} from '@angular/core';
import {PageNavigationComponent} from './layout/components/page-navigation/page-navigation.component';
import {PageFooterComponent} from './layout/components/page-footer/page-footer.component';
import {TimesheetComponent} from './timesheet/components/timesheet/timesheet.component';
import {TimeTrackingRoutingModule} from './timetracking-routing.module';
import {TimeTrackingComponent} from './timetracking.component';
import {ApiModule, Configuration, InformationService} from '../../api/timetracking';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {environment} from '../environments/environment';
import {loadTranslations} from '@angular/localize';
import localeEN from '@angular/common/locales/en';
import localeDeDE from '@angular/common/locales/de';
import localeDeCH from '@angular/common/locales/de-CH';
import localeDeAT from '@angular/common/locales/de-AT';
import {DecimalPipe, registerLocaleData} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {MasterDataCustomersComponent} from './master-data/components/master-data-customers/master-data-customers.component';
import {MasterDataProjectsComponent} from './master-data/components/master-data-projects/master-data-projects.component';
import {MasterDataActivitiesComponent} from './master-data/components/master-data-activities/master-data-activities.component';
import {MasterDataCustomersEditComponent} from './master-data/components/master-data-customers-edit/master-data-customers-edit.component';
import {ToastrModule} from 'ngx-toastr';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {MasterDataProjectsEditComponent} from './master-data/components/master-data-projects-edit/master-data-projects-edit.component';
import {NgSelectConfig, NgSelectModule} from '@ng-select/ng-select';
import {MasterDataActivitiesEditComponent} from './master-data/components/master-data-activities-edit/master-data-activities-edit.component';
import {MasterDataOrdersComponent} from './master-data/components/master-data-orders/master-data-orders.component';
import {MasterDataOrdersEditComponent} from './master-data/components/master-data-orders-edit/master-data-orders-edit.component';
import {NgLetModule} from 'ng-let';
import {Settings} from 'luxon';
import {TimesheetEditComponent} from './timesheet/components/timesheet-edit/timesheet-edit.component';
import {MasterDataHolidaysComponent} from './master-data/components/master-data-holidays/master-data-holidays.component';
import {MasterDataSettingsComponent} from './master-data/components/master-data-settings/master-data-settings.component';
import {MasterDataHolidaysEditComponent} from './master-data/components/master-data-holidays-edit/master-data-holidays-edit.component';
import {MasterDataHolidaysImportComponent} from './master-data/components/master-data-holidays-import/master-data-holidays-import.component';
import {NgApexchartsModule} from 'ng-apexcharts';
import {NgbModalModule, NgbPopoverModule, NgbCollapseModule, NgbConfig} from '@ng-bootstrap/ng-bootstrap';
import {ChartCustomersComponent} from './chart/components/chart-customers/chart-customers.component';
import {ChartActivitiesComponent} from './chart/components/chart-activities/chart-activities.component';
import {ChartIssuesComponent} from './chart/components/chart-issues/chart-issues.component';
import {ChartProjectsComponent} from './chart/components/chart-projects/chart-projects.component';
import {ChartOrdersComponent} from './chart/components/chart-orders/chart-orders.component';
import {TimeSheetHeaderComponent} from './timesheet/components/timesheet-header/timesheet-header.component';
import {ChartWorkdayInfoComponent} from './chart/components/chart-workday-info/chart-workday-info.component';
import {ChartTotalsOverviewComponent} from './chart/components/chart-totals-overview/chart-totals-overview.component';
import {CoreModule} from '../../core/app/core.module';
import {LocalizationService} from '../../core/app/services/internationalization/localization.service';
import {ReportModule} from '../../report/app/report.module';
import {AuthenticationInterceptor} from '../../core/app/services/http-interceptors/authentication.interceptor';
import {forkJoin, from, mergeMap, Observable, tap} from 'rxjs';
import {ApiDateTimeInterceptor} from '../../core/app/services/http-interceptors/api-date-time.interceptor';
import {ApiErrorInterceptor} from '../../core/app/services/http-interceptors/api-error.interceptor';
import {ConfigurationService} from '../../core/app/services/configuration.service';
import {AuthenticationService} from '../../core/app/services/authentication.service';
import {AuthorizationModule} from '../../authorization/app/authorization.module';

@NgModule({
  declarations: [
    TimeTrackingComponent,
    PageNavigationComponent,
    PageFooterComponent,
    TimesheetComponent,
    MasterDataCustomersComponent,
    MasterDataCustomersEditComponent,
    MasterDataProjectsComponent,
    MasterDataActivitiesComponent,
    MasterDataProjectsEditComponent,
    MasterDataActivitiesEditComponent,
    MasterDataOrdersComponent,
    MasterDataOrdersEditComponent,
    TimesheetEditComponent,
    ChartCustomersComponent,
    ChartActivitiesComponent,
    ChartIssuesComponent,
    ChartProjectsComponent,
    ChartOrdersComponent,
    MasterDataHolidaysComponent,
    MasterDataSettingsComponent,
    MasterDataHolidaysEditComponent,
    MasterDataHolidaysImportComponent,
    TimeSheetHeaderComponent,
    ChartWorkdayInfoComponent,
    ChartTotalsOverviewComponent,
  ],
  imports: [
    BrowserModule,
    TimeTrackingRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    CoreModule,
    ReportModule,
    AuthorizationModule,
    NgSelectModule,
    NgApexchartsModule,
    NgbModalModule,
    NgbPopoverModule,
    NgbCollapseModule,
    NgLetModule,
    ToastrModule.forRoot({
      extendedTimeOut: 2500,
      positionClass: 'toast-bottom-right'
    }),
    ApiModule.forRoot(() =>
      new Configuration({basePath: environment.timeTrackingApiBasePath})
    )
  ],
  providers: [
    DecimalPipe,
    {
      provide: APP_INITIALIZER,
      useFactory: configurationLoaderFactory,
      deps: [InformationService, LocalizationService, ConfigurationService, AuthenticationService, NgbConfig, NgSelectConfig],
      multi: true
    }, {
      provide: LOCALE_ID,
      useFactory: localeLoaderFactory,  //returns locale string,
      deps: [LocalizationService]
    }, {
      provide: HTTP_INTERCEPTORS,
      useClass: ApiDateTimeInterceptor,
      multi: true
    }, {
      provide: HTTP_INTERCEPTORS,
      useClass: ApiErrorInterceptor,
      multi: true
    }, {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthenticationInterceptor,
      multi: true
    }
  ],
  bootstrap: [TimeTrackingComponent]
})
export class TimeTrackingModule {
}

export function configurationLoaderFactory(informationService: InformationService, localizationService: LocalizationService, configurationService: ConfigurationService, authenticationService: AuthenticationService, ngbConfig: NgbConfig, ngSelectConfig: NgSelectConfig): () => Promise<any> | Observable<any> {
  return () => {
    const language = getLanguage(localizationService);
    const getTranslations = informationService.getTranslations({language});
    const getClientConfiguration = informationService.getClientConfiguration();

    return forkJoin([getTranslations, getClientConfiguration])
      .pipe(
        tap(([translations, clientConfiguration]) => {
          // Disable animations in e2e tests.
          ngbConfig.animation = !((window as any).Cypress);

          configurationService.clientConfiguration = clientConfiguration;
          activateTranslations(translations, ngbConfig, ngSelectConfig);
        }),
        mergeMap(() => from(authenticationService.init())),
      );
  };
}

function getLanguage(localizationService: LocalizationService): string {
  switch (localizationService.language) {
    case 'de':
    case 'de-DE':
    case 'de-CH':
    case 'de-AT':
      return 'de';
    default:
      return 'en';
  }
}

function activateTranslations(translations: any, ngbConfig: NgbConfig, ngSelectConfig: NgSelectConfig) {
  loadTranslations(flattenTranslations(translations));

  ngSelectConfig.addTagText = $localize`:@@Component.NgSelect.AddTagText:[i18n] Add item`;
  ngSelectConfig.loadingText = $localize`:@@Component.NgSelect.LoadingText:[i18n] Loading...`;
  ngSelectConfig.clearAllText = $localize`:@@Component.NgSelect.ClearAllText:[i18n] Clear all`;
  ngSelectConfig.notFoundText = $localize`:@@Component.NgSelect.NotFoundText:[i18n] No items found`;
  ngSelectConfig.typeToSearchText = $localize`:@@Component.NgSelect.TypeToSearchText:[i18n] Type to search`;
  ngSelectConfig.appendTo = 'body';
}

export function localeLoaderFactory(localizationService: LocalizationService) {
  let locale: any;
  switch (localizationService.language) {
    case 'de':
    case 'de-DE':
      locale = localeDeDE;
      break;
    case 'de-CH':
      locale = localeDeCH;
      break;
    case 'de-AT':
      locale = localeDeAT;
      break;
    default:
      locale = localeEN;
  }
  registerLocaleData(locale);
  Settings.defaultLocale = localizationService.language;
  return localizationService.language;
}

/**
 * @param obj Object                The translations to flatten
 * @param parent String (Optional)  The prefix to add before each key, also used for recursion
 * @param result                    The result (parameter used by recursion)
 **/
export function flattenTranslations<T>(obj: [s: string], parent: string = '', result: { [key: string]: string } = {}): { [key: string]: string } {

  for (const [propertyName, property] of Object.entries(obj)) {
    if (typeof property === 'object' && property !== null)
      flattenTranslations(property, parent + propertyName + '.', result);
    else if (typeof property === 'string')
      result[parent + propertyName] = property;
  }

  return result;
}
