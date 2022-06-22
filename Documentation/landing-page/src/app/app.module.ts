import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import {HttpClientModule} from '@angular/common/http';
import { MainPageComponent } from './components/overview/main-page/main-page.component';
import { HeadlineComponent } from './components/overview/sections/headline/headline.component';
import { FeaturesComponent } from './components/overview/sections/features/features.component';
import { DevelopmentComponent } from './components/overview/sections/development/development.component';
import { ConfigurationComponent } from './components/overview/sections/configuration/configuration.component';
import { InstallationComponent } from './components/overview/sections/installation/installation.component';

@NgModule({
  declarations: [
    AppComponent,
    MainPageComponent,
    HeadlineComponent,
    FeaturesComponent,
    DevelopmentComponent,
    ConfigurationComponent,
    InstallationComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
