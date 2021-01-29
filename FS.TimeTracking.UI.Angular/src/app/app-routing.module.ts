import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';
import {MasterDataComponent} from './master-data/components/master-data/master-data.component';
import {TimesheetComponent} from './timesheet/components/timesheet/timesheet.component';
import {rematch} from './shared/services/routing/rematch';

const routes: Routes = [
  {matcher: rematch('master-data/:entity(customers|projects|activities)?/:id?'), component: MasterDataComponent},
  {path: '', component: TimesheetComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {enableTracing: false})],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
