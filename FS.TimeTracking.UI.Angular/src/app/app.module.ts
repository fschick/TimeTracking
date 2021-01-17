import {BrowserModule} from '@angular/platform-browser';
import {APP_INITIALIZER, LOCALE_ID, NgModule} from '@angular/core';
import {PageNavigationComponent} from './page/components/page-navigation/page-navigation.component';
import {PageFooterComponent} from './page/components/page-footer/page-footer.component';
import {MasterDataComponent} from './master-data/components/master-data/master-data.component';
import {TimesheetComponent} from './timesheet/components/timesheet/timesheet.component';
import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {ApiModule, Configuration} from './shared/services/api';
import {HttpClientModule} from '@angular/common/http';
import {environment} from '../environments/environment';
import {loadTranslations} from "@angular/localize";
import translationsEN from '../locale/messages.en.json';
import translationsDE from '../locale/messages.de.json';
import localeEN from '@angular/common/locales/en';
import localeDE from '@angular/common/locales/de';
import {registerLocaleData} from "@angular/common";

@NgModule({
  declarations: [
    AppComponent,
    PageNavigationComponent,
    PageFooterComponent,
    MasterDataComponent,
    TimesheetComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ApiModule,
    HttpClientModule,
    ApiModule.forRoot(() =>
      new Configuration({basePath: environment.apiBasePath})
    )
  ],
  providers: [{
    provide: APP_INITIALIZER,
    useFactory: ConfigurationLoaderFactory,
    multi: true
  }, {
    provide: LOCALE_ID,
    useFactory: LocaleLoaderFactory  //returns locale string
  }],
  bootstrap: [AppComponent]
})
export class AppModule {
}

export function ConfigurationLoaderFactory(): () => Promise<void> {
  return () => {
    loadTranslations(flattenTranslations(translationsEN))
    // loadTranslations(flattenTranslations(translationsDE))
    return Promise.resolve();
  }
}

export function LocaleLoaderFactory() {
  registerLocaleData(localeEN);
  registerLocaleData(localeDE);
  return 'en';
  // return 'de';
  // return navigator.languages[0];
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
