import {BrowserModule} from '@angular/platform-browser';
import {APP_INITIALIZER, LOCALE_ID, NgModule} from '@angular/core';
import {PageNavigationComponent} from './page/components/page-navigation/page-navigation.component';
import {PageFooterComponent} from './page/components/page-footer/page-footer.component';
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
import localeDE from '@angular/common/locales/de';
import {registerLocaleData} from '@angular/common';
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
import {ConfirmButtonComponent} from './shared/components/confirm-button/confirm-button.component';
import {ReactiveComponentModule} from '@ngrx/component';
import {MasterDataProjectsEditComponent} from './master-data/components/master-data-projects-edit/master-data-projects-edit.component';
import {NgSelectConfig, NgSelectModule} from '@ng-select/ng-select';
import {SimpleConfirmComponent} from './shared/components/simple-confirm/simple-confirm.component';
import {MasterDataActivitiesEditComponent} from './master-data/components/master-data-activities-edit/master-data-activities-edit.component';
import {ApiDateTimeInterceptorInterceptor} from './shared/services/error-handling/api-date-time-interceptor.interceptor';
import {MasterDataOrdersComponent} from './master-data/components/master-data-orders/master-data-orders.component';
import {MasterDataOrdersEditComponent} from './master-data/components/master-data-orders-edit/master-data-orders-edit.component';
import {BootstrapDatepickerComponent} from './shared/components/bootstrap-datepicker/bootstrap-datepicker.component';
import {LocalizationService} from './shared/services/internationalization/localization.service';

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
    ConfirmButtonComponent,
    MasterDataProjectsEditComponent,
    SimpleConfirmComponent,
    MasterDataActivitiesEditComponent,
    MasterDataOrdersComponent,
    MasterDataOrdersEditComponent,
    BootstrapDatepickerComponent
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
    ToastrModule.forRoot({
      extendedTimeOut: 2500,
      positionClass: 'toast-bottom-right'
    }),
    ApiModule.forRoot(() =>
      new Configuration({basePath: environment.apiBasePath})
    ),
  ],
  providers: [{
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
    useClass: ApiDateTimeInterceptorInterceptor,
    multi: true
  }, {
    provide: HTTP_INTERCEPTORS,
    useClass: ApiErrorInterceptor,
    multi: true
  }],
  bootstrap: [AppComponent]
})
export class AppModule {
}

// eslint-disable-next-line prefer-arrow/prefer-arrow-functions
export function configurationLoaderFactory(localizationService: LocalizationService, ngSelectConfig: NgSelectConfig): () => Promise<void> {
  return () => {
    const translations = localizationService.language === 'de' ? translationsDE : translationsEN;
    loadTranslations(flattenTranslations(translations));

    ngSelectConfig.addTagText = $localize`:@@Component.NgSelect.AddTagText:[i18n] Add item`;
    ngSelectConfig.loadingText = $localize`:@@Component.NgSelect.LoadingText:[i18n] Loading...`;
    ngSelectConfig.clearAllText = $localize`:@@Component.NgSelect.ClearAllText:[i18n] Clear all`;
    ngSelectConfig.notFoundText = $localize`:@@Component.NgSelect.NotFoundText:[i18n] No items found`;
    ngSelectConfig.typeToSearchText = $localize`:@@Component.NgSelect.TypeToSearchText:[i18n] Type to search`;

    return Promise.resolve();
  };
}

// eslint-disable-next-line prefer-arrow/prefer-arrow-functions
export function localeLoaderFactory(localizationService: LocalizationService) {
  const locale = localizationService.language === 'de' ? localeDE : localeEN;
  registerLocaleData(locale);
  return localizationService.language;
}

/**
 * @param obj Object                The translations to flatten
 * @param parent String (Optional)  The prefix to add before each key, also used for recursion
 * @param result                    The result (parameter used by recursion)
 **/
// eslint-disable-next-line prefer-arrow/prefer-arrow-functions
export function flattenTranslations<T>(obj: T, parent: string = '', result: { [key: string]: string } = {}): { [key: string]: string } {

  for (const [propertyName, property] of Object.entries(obj)) {
    if (typeof property === 'object' && property !== null)
      flattenTranslations(property, parent + propertyName + '.', result);
    else if (typeof property === 'string')
      result[parent + propertyName] = property;
  }

  return result;
}
