import {BrowserModule} from '@angular/platform-browser';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {NgSelectModule} from '@ng-select/ng-select';
import {NgModule} from '@angular/core';
import {NgbCollapseModule} from '@ng-bootstrap/ng-bootstrap';
import {ReportActivityOverviewComponent} from './components/report-activity-overview/report-activity-overview.component';
import {CoreModule} from '../../core/app/core.module';
import {RouterModule} from '@angular/router';
import { ReportActivityPreviewComponent } from './components/report-activity-preview/report-activity-preview.component';

const components = [
  ReportActivityOverviewComponent,
  ReportActivityPreviewComponent,
];

@NgModule({
  declarations: [
    ...components,
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    NgSelectModule,
    NgbCollapseModule,
    CoreModule,
    RouterModule,
  ],
  providers: [
  ],
  exports: [
    ...components,
  ],
})
export class ReportModule {
}
