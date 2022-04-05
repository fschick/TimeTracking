import {BrowserModule} from '@angular/platform-browser';
import {APP_INITIALIZER, LOCALE_ID, NgModule} from '@angular/core';
import {PageNavigationComponent} from './layout/components/page-navigation/page-navigation.component';
import {PageFooterComponent} from './layout/components/page-footer/page-footer.component';
import {TimesheetComponent} from './timesheet/components/timesheet/timesheet.component';
import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {ApiModule, Configuration, SettingService} from './shared/services/api';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {environment} from '../environments/environment';
import {loadTranslations} from '@angular/localize';
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
import {MasterDataHolidaysComponent} from './master-data/components/master-data-holidays/master-data-holidays.component';
import {MasterDataSettingsComponent} from './master-data/components/master-data-settings/master-data-settings.component';
import {MasterDataHolidaysEditComponent} from './master-data/components/master-data-holidays-edit/master-data-holidays-edit.component';
import {MasterDataHolidaysImportComponent} from './master-data/components/master-data-holidays-import/master-data-holidays-import.component';
import {NgApexchartsModule} from 'ng-apexcharts';
import {NgbModalModule, NgbPopoverModule, NgbCollapseModule} from '@ng-bootstrap/ng-bootstrap';
import {TimesheetFilterComponent} from './shared/components/filter/filter.component';
import {DateMonthPickerComponent} from './shared/components/date-month-picker/date-month-picker.component';
import {RouterLinkCtrlClickDirective} from './shared/directives/router-link-ctrl-click.directive';
import {ChartCustomersComponent} from './chart/components/chart-customers/chart-customers.component';
import {ChartActivitiesComponent} from './chart/components/chart-activities/chart-activities.component';
import {ChartIssuesComponent} from './chart/components/chart-issues/chart-issues.component';
import {ChartProjectsComponent} from './chart/components/chart-projects/chart-projects.component';
import {ChartOrdersComponent} from './chart/components/chart-orders/chart-orders.component';
import {PageHeaderComponent} from './layout/components/page-header/page-header.component';
import {NgLetModule} from 'ng-let';
import { ClearableInputComponent } from './shared/components/clearable-input/clearable-input.component';

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
    ChartCustomersComponent,
    ChartActivitiesComponent,
    ChartIssuesComponent,
    ChartProjectsComponent,
    ChartOrdersComponent,
    MasterDataHolidaysComponent,
    MasterDataSettingsComponent,
    MasterDataHolidaysEditComponent,
    MasterDataHolidaysImportComponent,
    DateMonthPickerComponent,
    RouterLinkCtrlClickDirective,
    PageHeaderComponent,
    ClearableInputComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
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
      new Configuration({basePath: environment.apiBasePath})
    ),
  ],
  providers: [
    DatePipe,
    DecimalPipe,
    {
      provide: APP_INITIALIZER,
      useFactory: configurationLoaderFactory,
      deps: [SettingService, LocalizationService, NgSelectConfig],
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

export function configurationLoaderFactory(settingService: SettingService, localizationService: LocalizationService, ngSelectConfig: NgSelectConfig): () => Promise<void> {
  return () => {
    let language;
    switch (localizationService.language) {
      case 'de':
      case 'de-DE':
      case 'de-CH':
      case 'de-AT':
        language = 'de';
        break;
      default:
        language = 'en';
        break;
    }

    return settingService.getTranslations({language}).toPromise().then(translations => {
      loadTranslations(flattenTranslations(translations));

      ngSelectConfig.addTagText = $localize`:@@Component.NgSelect.AddTagText:[i18n] Add item`;
      ngSelectConfig.loadingText = $localize`:@@Component.NgSelect.LoadingText:[i18n] Loading...`;
      ngSelectConfig.clearAllText = $localize`:@@Component.NgSelect.ClearAllText:[i18n] Clear all`;
      ngSelectConfig.notFoundText = $localize`:@@Component.NgSelect.NotFoundText:[i18n] No items found`;
      ngSelectConfig.typeToSearchText = $localize`:@@Component.NgSelect.TypeToSearchText:[i18n] Type to search`;
    });
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
