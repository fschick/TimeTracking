import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';
import {TimesheetComponent} from './timesheet/components/timesheet/timesheet.component';
import {MasterDataCustomersComponent} from './master-data/components/master-data-customers/master-data-customers.component';
import {MasterDataProjectsComponent} from './master-data/components/master-data-projects/master-data-projects.component';
import {MasterDataActivitiesComponent} from './master-data/components/master-data-activities/master-data-activities.component';
import {MasterDataCustomersEditComponent} from './master-data/components/master-data-customers-edit/master-data-customers-edit.component';
import {MasterDataProjectsEditComponent} from './master-data/components/master-data-projects-edit/master-data-projects-edit.component';
import {MasterDataActivitiesEditComponent} from './master-data/components/master-data-activities-edit/master-data-activities-edit.component';

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
  {path: '', component: TimesheetComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {enableTracing: false})],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
