import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {MasterDataCustomersComponent} from './master-data/components/master-data-customers/master-data-customers.component';
import {MasterDataCustomersEditComponent} from './master-data/components/master-data-customers-edit/master-data-customers-edit.component';
import {MasterDataOrdersComponent} from './master-data/components/master-data-orders/master-data-orders.component';
import {MasterDataOrdersEditComponent} from './master-data/components/master-data-orders-edit/master-data-orders-edit.component';
import {MasterDataProjectsComponent} from './master-data/components/master-data-projects/master-data-projects.component';
import {MasterDataProjectsEditComponent} from './master-data/components/master-data-projects-edit/master-data-projects-edit.component';
import {MasterDataActivitiesComponent} from './master-data/components/master-data-activities/master-data-activities.component';
import {MasterDataActivitiesEditComponent} from './master-data/components/master-data-activities-edit/master-data-activities-edit.component';
import {TimesheetComponent} from './timesheet/components/timesheet/timesheet.component';
import {TimesheetEditComponent} from './timesheet/components/timesheet-edit/timesheet-edit.component';
import {ReportOrdersComponent} from './report/components/report-orders/report-orders.component';
import {ReportIssuesComponent} from './report/components/report-issues/report-issues.component';
import {ReportActivitiesComponent} from './report/components/report-activities/report-activities.component';
import {ReportProjectsComponent} from './report/components/report-projects/report-projects.component';
import {ReportCustomersComponent} from './report/components/report-customers/report-customers.component';

const routes: Routes = [
  {
    path: 'master-data/customers', component: MasterDataCustomersComponent,
    children: [{path: ':id', component: MasterDataCustomersEditComponent}]
  },
  {
    path: 'master-data/projects', component: MasterDataProjectsComponent,
    children: [{path: ':id', component: MasterDataProjectsEditComponent}]
  },
  {
    path: 'master-data/activities', component: MasterDataActivitiesComponent,
    children: [{path: ':id', component: MasterDataActivitiesEditComponent}]
  },
  {
    path: 'master-data/orders', component: MasterDataOrdersComponent,
    children: [{path: ':id', component: MasterDataOrdersEditComponent}]
  },
  {
    path: 'report/customers', component: ReportCustomersComponent,
  },
  {
    path: 'report/projects', component: ReportProjectsComponent,
  },
  {
    path: 'report/activities', component: ReportActivitiesComponent,
  },
  {
    path: 'report/issues', component: ReportIssuesComponent,
  },
  {
    path: 'report/orders', component: ReportOrdersComponent,
  },
  {
    path: '', component: TimesheetComponent,
    children: [{path: ':id', component: TimesheetEditComponent}]
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {enableTracing: false})],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
