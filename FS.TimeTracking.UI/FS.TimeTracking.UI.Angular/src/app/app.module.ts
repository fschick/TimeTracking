import {BrowserModule} from '@angular/platform-browser';
import {APP_INITIALIZER, LOCALE_ID, NgModule} from '@angular/core';
import {PageNavigationComponent} from './layout/components/page-navigation/page-navigation.component';
import {PageFooterComponent} from './layout/components/page-footer/page-footer.component';
import {TimesheetComponent} from './timesheet/components/timesheet/timesheet.component';
import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {ApiModule, Configuration} from './shared/services/api';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {environment} from '../environments/environment';
import {loadTranslations} from '@angular/localize';
import translationsEN from '../locale/messages.en.json';
import translationsDE from '../locale/messages.de.json';
import localeEN from '@angular/common/locales/en';
import localeDeDE from '@angular/common/locales/de';
import localeDeCH from '@angular/common/locales/de-CH';
import localeDeAT from '@angular/common/locales/de-AT';
import {DecimalPipe, registerLocaleData} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {FormValidationErrorsComponent} from './shared/components/form-validation-errors/form-validation-errors.component';
import {FormSubmitDirective} from './shared/directives/form-submit.directive';
import {MasterDataCustomersComponent} from './master-data/components/master-data-customers/master-data-customers.component';
import {MasterDataProjectsComponent} from './master-data/components/master-data-projects/master-data-projects.component';
import {MasterDataActivitiesComponent} from './master-data/components/master-data-activities/master-data-activities.component';
import {SimpleTableComponent} from './shared/components/simple-table/simple-table.component';
import {MasterDataCustomersEditComponent} from './master-data/components/master-data-customers-edit/master-data-customers-edit.component';
import {ToastrModule} from 'ngx-toastr';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {ApiErrorInterceptor} from './shared/services/error-handling/api-error.interceptor';
import {ReactiveComponentModule} from '@ngrx/component';
import {MasterDataProjectsEditComponent} from './master-data/components/master-data-projects-edit/master-data-projects-edit.component';
import {NgSelectConfig, NgSelectModule} from '@ng-select/ng-select';
import {SimpleConfirmComponent} from './shared/components/simple-confirm/simple-confirm.component';
import {MasterDataActivitiesEditComponent} from './master-data/components/master-data-activities-edit/master-data-activities-edit.component';
import {ApiDateTimeInterceptor} from './shared/services/error-handling/api-date-time.interceptor';
import {MasterDataOrdersComponent} from './master-data/components/master-data-orders/master-data-orders.component';
import {MasterDataOrdersEditComponent} from './master-data/components/master-data-orders-edit/master-data-orders-edit.component';
import {LocalizationService} from './shared/services/internationalization/localization.service';
import {NumericDirective} from './shared/directives/numeric.directive';
import {DatePickerDirective} from './shared/directives/date-picker.directive';
import {Settings} from 'luxon';
import {TimePipe} from './shared/pipes/time.pipe';
import {DatePipe} from './shared/pipes/date.pipe';
import {DurationPipe} from './shared/pipes/duration.pipe';
import {IconComponent} from './shared/components/icon/icon.component';
import {TimesheetEditComponent} from './timesheet/components/timesheet-edit/timesheet-edit.component';
import {TimeDirective} from './shared/directives/time.directive';
import {TimesheetFilterComponent} from './timesheet/components/timesheet-filter/timesheet-filter.component';
import {ReportCustomersComponent} from './report/components/report-customers/report-customers.component';
import {ReportActivitiesComponent} from './report/components/report-activities/report-activities.component';
import {ReportIssuesComponent} from './report/components/report-issues/report-issues.component';
import {ReportProjectsComponent} from './report/components/report-projects/report-projects.component';
import {ReportOrdersComponent} from './report/components/report-orders/report-orders.component';
import {MasterDataHolidaysComponent} from './master-data/components/master-data-holidays/master-data-holidays.component';
import {MasterDataSettingsComponent} from './master-data/components/master-data-settings/master-data-settings.component';
import {MasterDataHolidaysEditComponent} from './master-data/components/master-data-holidays-edit/master-data-holidays-edit.component';
import {MasterDataHolidaysImportComponent} from './master-data/components/master-data-holidays-import/master-data-holidays-import.component';
import {NgApexchartsModule} from 'ng-apexcharts';
import {NgbModalModule} from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [
    FormSubmitDirective,
    FormValidationErrorsComponent,
    SimpleTableComponent,
    AppComponent,
    PageNavigationComponent,
    PageFooterComponent,
    TimesheetComponent,
    MasterDataCustomersComponent,
    MasterDataCustomersEditComponent,
    MasterDataProjectsComponent,
    MasterDataActivitiesComponent,
    MasterDataProjectsEditComponent,
    SimpleConfirmComponent,
    MasterDataActivitiesEditComponent,
    MasterDataOrdersComponent,
    MasterDataOrdersEditComponent,
    NumericDirective,
    DatePickerDirective,
    DatePipe,
    TimePipe,
    DurationPipe,
    IconComponent,
    TimesheetEditComponent,
    TimeDirective,
    TimesheetFilterComponent,
    ReportCustomersComponent,
    ReportActivitiesComponent,
    ReportIssuesComponent,
    ReportProjectsComponent,
    ReportOrdersComponent,
    MasterDataHolidaysComponent,
    MasterDataSettingsComponent,
    MasterDataHolidaysEditComponent,
    MasterDataHolidaysImportComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    ReactiveComponentModule,
    NgSelectModule,
    NgApexchartsModule,
    NgbModalModule,
    ToastrModule.forRoot({
      extendedTimeOut: 2500,
      positionClass: 'toast-bottom-right'
    }),
    ApiModule.forRoot(() =>
      new Configuration({basePath: environment.apiBasePath})
    ),
  ],
  providers: [
    DatePipe,
    DecimalPipe,
    {
      provide: APP_INITIALIZER,
      useFactory: configurationLoaderFactory,
      deps: [LocalizationService, NgSelectConfig],
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
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}

export function configurationLoaderFactory(localizationService: LocalizationService, ngSelectConfig: NgSelectConfig): () => Promise<void> {
  return () => {
    let translations: any;
    switch (localizationService.language) {
      case 'de':
      case 'de-DE':
      case 'de-CH':
      case 'de-AT':
        translations = translationsDE;
        break;
      default:
        translations = translationsEN;
        break;
    }

    loadTranslations(flattenTranslations(translations));

    ngSelectConfig.addTagText = $localize`:@@Component.NgSelect.AddTagText:[i18n] Add item`;
    ngSelectConfig.loadingText = $localize`:@@Component.NgSelect.LoadingText:[i18n] Loading...`;
    ngSelectConfig.clearAllText = $localize`:@@Component.NgSelect.ClearAllText:[i18n] Clear all`;
    ngSelectConfig.notFoundText = $localize`:@@Component.NgSelect.NotFoundText:[i18n] No items found`;
    ngSelectConfig.typeToSearchText = $localize`:@@Component.NgSelect.TypeToSearchText:[i18n] Type to search`;

    return Promise.resolve();
  };
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
export function flattenTranslations<T>(obj: T, parent: string = '', result: { [key: string]: string } = {}): { [key: string]: string } {

  for (const [propertyName, property] of Object.entries(obj)) {
    if (typeof property === 'object' && property !== null)
      flattenTranslations(property, parent + propertyName + '.', result);
    else if (typeof property === 'string')
      result[parent + propertyName] = property;
  }

  return result;
}
