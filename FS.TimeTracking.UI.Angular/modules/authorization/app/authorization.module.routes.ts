import {Routes} from '@angular/router';
import {rematch} from '../../core/app/services/routing/rematch';
import {AdministrationUsersComponent} from './components/administration-users/administration-users.component';
import {AdministrationUsersEditComponent} from './components/administration-users-edit/administration-users-edit.component';

const guidRegex = '[a-zA-Z0-9]{8}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{12}';
const guidId = `:id(${guidRegex})`;

export const authorizationModuleRoutes: Routes = [
  {
    path: 'administration',
    children: [
      {
        path: 'users', component: AdministrationUsersComponent,
        children: [{matcher: rematch(guidId), component: AdministrationUsersEditComponent}]
      },
    ]
  }
];
