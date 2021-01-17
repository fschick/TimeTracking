import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {MasterDataComponent} from './master-data/components/master-data/master-data.component';
import {TimesheetComponent} from './timesheet/components/timesheet/timesheet.component';

const routes: Routes = [
   { path: 'mater-data', component: MasterDataComponent },
   { path: '', component: TimesheetComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { enableTracing: false })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
