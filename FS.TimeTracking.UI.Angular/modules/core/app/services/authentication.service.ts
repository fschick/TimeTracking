import {Injectable} from '@angular/core';
import Keycloak, {KeycloakInitOptions} from 'keycloak-js';
import {from, map, Observable} from 'rxjs';
import {ConfigurationService} from './configuration.service';
import {KeycloakConfigurationDto} from '../../../api/timetracking';
import {switchMap, tap} from 'rxjs/operators';
import {RoleNames} from '../../../authorization/app/services/RoleNames';
import {UtilityService} from './utility.service';

// https://stackoverflow.com/a/65642944/1271211
type SnakeToCamelCase<S extends string> = S extends `${infer T}_${infer U}` ? `${T}${Capitalize<SnakeToCamelCase<U>>}` : S;
type SnakeToPascalCase<S extends string> = Capitalize<SnakeToCamelCase<S>>
export type UserPermissions = Record<`can${SnakeToPascalCase<Lowercase<keyof typeof RoleNames>>}`, boolean>;

export  type UserPermissionGroups = {
  canChartsView: boolean;
  canReportView: boolean;
  canMasterDataView: boolean;
  canAdministrationView: boolean;
}

export interface UserProfile {
  id?: string;
  name?: string;
  isAuthenticated: boolean;
}

export  type User = UserPermissions & UserPermissionGroups & UserProfile;

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private keycloak?: Keycloak;
  private configuration?: KeycloakConfigurationDto;
  private authorizationEnabled: boolean = false;

  public currentUser?: User;

  constructor(
    private configurationService: ConfigurationService,
    private utilities: UtilityService,
  ) {
  }

  public init(): Observable<any> {
    this.authorizationEnabled = this.configurationService.clientConfiguration.features.authorization;
    if (!this.authorizationEnabled) {
      this.setUser();
      return from(Promise.resolve())
    }

    this.configuration = this.configurationService.clientConfiguration.keycloak;
    const openIdConnectOptions = {
      url: this.configuration.authServerUrl,
      realm: this.configuration.realm,
      clientId: this.configuration.clientId,
    };

    this.keycloak = new Keycloak(openIdConnectOptions);

    return this
      .initKeycloak(this.keycloak)
      .pipe(
        tap(isAuthenticated => this.ensureLoggedIn(isAuthenticated)),
        switchMap(() => this.loadUserProfile()),
        tap(() => this.setUser())
      );
  }

  public getAccessToken(): Observable<string> {
    if (!this.keycloak)
      throw Error('Keycloak authentication service is not initialized');

    return from(this.keycloak.updateToken(5))
      .pipe(map(() => this.keycloak!.token ?? ''));
  }

  public login(): void {
    if (!this.keycloak)
      throw Error('Keycloak authentication service is not initialized');
    this.keycloak.login();
  }

  public logout(): void {
    if (!this.keycloak)
      throw Error('Keycloak authentication service is not initialized');
    this.keycloak.logout();
  }

  private initKeycloak(keycloak: Keycloak): Observable<any> {
    const initOptions: KeycloakInitOptions = {onLoad: 'check-sso', silentCheckSsoRedirectUri: window.location.origin + '/assets/silent-check-sso.html'};
    return from(keycloak.init(initOptions));
  }

  private ensureLoggedIn(isAuthenticated: boolean): void {
    if (!isAuthenticated)
      this.login();
  }

  private loadUserProfile(): Observable<any> {
    return from(this.keycloak!.loadUserProfile())
  }

  private setUser(): void {
    const permissions = Object
      .entries(RoleNames)
      .reduce((permissions: any, [propertyName, value]) => {
        permissions[`can${this.utilities.snakeToCamelcase(propertyName)}`] = this.authorizationEnabled ? this.hasTole(value) : true;
        return permissions;
      }, {}) as UserPermissions;

    this.currentUser = {
      id: this.keycloak?.profile?.id,
      name: this.keycloak?.profile?.username,
      isAuthenticated: this.keycloak?.authenticated ?? false,
      ...permissions,
      canChartsView: permissions.canChartsByCustomerView || permissions.canChartsByOrderView || permissions.canChartsByProjectView || permissions.canChartsByActivityView || permissions.canChartsByIssueView,
      canReportView: permissions.canReportActivitySummaryView || permissions.canReportActivityDetailView,
      canMasterDataView: permissions.canMasterDataCustomersView || permissions.canMasterDataProjectsView || permissions.canMasterDataActivitiesView || permissions.canMasterDataOrdersView || permissions.canMasterDataHolidaysView,
      canAdministrationView: permissions.canAdministrationUsersView || permissions.canAdministrationSettingsView,
    };
  }

  private hasTole(role: string): boolean {
    if (!this.keycloak?.authenticated)
      return false;

    return this.keycloak!.hasResourceRole(role, this.configuration?.clientId);
  }
}
