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
import {MasterDataHolidaysComponent} from './master-data/components/master-data-holidays/master-data-holidays.component';
import {MasterDataSettingsComponent} from './master-data/components/master-data-settings/master-data-settings.component';
import {MasterDataHolidaysEditComponent} from './master-data/components/master-data-holidays-edit/master-data-holidays-edit.component';
import {MasterDataHolidaysImportComponent} from './master-data/components/master-data-holidays-import/master-data-holidays-import.component';
import {ChartCustomersComponent} from './chart/components/chart-customers/chart-customers.component';
import {ChartProjectsComponent} from './chart/components/chart-projects/chart-projects.component';
import {ChartActivitiesComponent} from './chart/components/chart-activities/chart-activities.component';
import {ChartIssuesComponent} from './chart/components/chart-issues/chart-issues.component';
import {ChartOrdersComponent} from './chart/components/chart-orders/chart-orders.component';
import {rematch} from '../../core/app/services/routing/rematch';
import {reportModuleRoutes} from '../../report/app/report.module.routes';
import {authorizationModuleRoutes} from '../../authorization/app/authorization.module.routes';

const guidRegex = '[a-zA-Z0-9]{8}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{12}';
const guidId = `:id(${guidRegex})`;

const timeTrackingModuleRoutes: Routes = [
  {
    path: 'master-data',
    children: [
      {
        path: 'customers', component: MasterDataCustomersComponent,
        children: [{matcher: rematch(guidId), component: MasterDataCustomersEditComponent}]
      },
      {
        path: 'projects', component: MasterDataProjectsComponent,
        children: [{matcher: rematch(guidId), component: MasterDataProjectsEditComponent}]
      },
      {
        path: 'activities', component: MasterDataActivitiesComponent,
        children: [{matcher: rematch(guidId), component: MasterDataActivitiesEditComponent}]
      },
      {
        path: 'orders', component: MasterDataOrdersComponent,
        children: [{matcher: rematch(guidId), component: MasterDataOrdersEditComponent}]
      },
      {
        path: 'holidays', component: MasterDataHolidaysComponent,
        children: [
          {path: 'import', component: MasterDataHolidaysImportComponent},
          {matcher: rematch(guidId), component: MasterDataHolidaysEditComponent},
        ]
      },
    ],
  },
  {
    path: 'administration',
    children: [
      {
        path: 'settings', component: MasterDataSettingsComponent,
      },
    ],
  },
  {
    path: 'chart',
    children: [
      {
        path: 'customers', component: ChartCustomersComponent,
      },
      {
        path: 'projects', component: ChartProjectsComponent,
      },
      {
        path: 'activities', component: ChartActivitiesComponent,
      },
      {
        path: 'issues', component: ChartIssuesComponent,
      },
      {
        path: 'orders', component: ChartOrdersComponent,
      },
    ]
  },
  {
    path: '', component: TimesheetComponent,
    children: [{matcher: rematch(guidId), component: TimesheetEditComponent}]
  },
];

const routes = [
  ...timeTrackingModuleRoutes,
  ...reportModuleRoutes,
  ...authorizationModuleRoutes,
]

@NgModule({
  imports: [RouterModule.forRoot(routes, {enableTracing: false})],
  exports: [RouterModule]
})
export class TimeTrackingRoutingModule {
}
