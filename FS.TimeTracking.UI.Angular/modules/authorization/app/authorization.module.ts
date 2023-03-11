import {NgModule} from '@angular/core';
import {AdministrationUsersComponent} from './components/administration-users/administration-users.component';
import {AdministrationUsersEditComponent} from './components/administration-users-edit/administration-users-edit.component';
import {BrowserModule} from '@angular/platform-browser';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {NgSelectModule} from '@ng-select/ng-select';
import {CoreModule} from '../../core/app/core.module';
import {RouterModule} from '@angular/router';
import {NgLetModule} from 'ng-let';

const components = [
  AdministrationUsersComponent,
  AdministrationUsersEditComponent
];

@NgModule({
  declarations: [
    ...components
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    NgSelectModule,
    CoreModule,
    RouterModule,
    NgLetModule,
  ],
  exports: [
    ...components,
  ],
})
export class AuthorizationModule {
}
