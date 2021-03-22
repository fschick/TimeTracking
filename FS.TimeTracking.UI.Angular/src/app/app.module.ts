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
import {StorageService} from './shared/services/storage/storage.service';
import {MasterDataCustomersEditComponent} from './master-data/components/master-data-customers-edit/master-data-customers-edit.component';
import {ToastrModule} from 'ngx-toastr';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {ApiErrorInterceptor} from './shared/services/error-handling/api-error.interceptor';
import {ConfirmButtonComponent} from './shared/components/confirm-button/confirm-button.component';
import {ReactiveComponentModule} from '@ngrx/component';
import {MasterDataProjectsEditComponent} from './master-data/components/master-data-projects-edit/master-data-projects-edit.component';
import {NgSelectConfig, NgSelectModule} from '@ng-select/ng-select';
import {SimpleConfirmComponent} from './shared/components/simple-confirm/simple-confirm.component';
import {DialogModule} from '@ngneat/dialog';

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
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    BrowserAnimationsModule,
    ReactiveComponentModule,
    NgSelectModule,
    DialogModule.forRoot({
      sizes: {
        inherit: {
        }
      }
    }),
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
    deps: [StorageService, NgSelectConfig],
    multi: true
  }, {
    provide: LOCALE_ID,
    useFactory: localeLoaderFactory,  //returns locale string,
    deps: [StorageService]
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
export function configurationLoaderFactory(storageService: StorageService, ngSelectConfig: NgSelectConfig): () => Promise<void> {
  return () => {
    const translations = storageService.language === 'de' ? translationsDE : translationsEN;
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
export function localeLoaderFactory(storageService: StorageService) {
  const locale = storageService.language === 'de' ? localeDE : localeEN;
  registerLocaleData(locale);
  return storageService.language;
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
