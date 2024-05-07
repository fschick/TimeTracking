import {Injectable} from '@angular/core';
import Keycloak, {KeycloakInitOptions, KeycloakTokenParsed} from 'keycloak-js';
import {from, map, Observable} from 'rxjs';
import {ConfigurationService} from './configuration.service';
import {KeycloakConfigurationDto} from '../../../api/timetracking';
import {switchMap, tap} from 'rxjs/operators';
import {RoleNames} from '../../../authorization/app/services/RoleNames';
import {DateTime} from "luxon";

// https://stackoverflow.com/a/65642944/1271211
// export type SnakeToCamelCase<S extends string> = S extends `${infer T}_${infer U}` ? `${T}${Capitalize<SnakeToCamelCase<U>>}` : S;
// export type SnakeToPascalCase<S extends string> = Capitalize<SnakeToCamelCase<S>>
// export type UserRoles = Record<`can${SnakeToPascalCase<Lowercase<keyof typeof RoleNames>>}`, boolean>;

export type UserRoles = Record<keyof typeof RoleNames, boolean>;

export type PermissionGroupsRoles = {
  chartsView: boolean;
  reportsView: boolean;
  masterDataView: boolean;
  administrationView: boolean;
}

export interface User {
  id?: string;
  name?: string;
  userAccountUrl?: string;
  isAuthenticated: boolean;
  hasRole: UserRoles;
  hasRolesInGroup: PermissionGroupsRoles;
}

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private keycloak?: Keycloak;
  private configuration?: KeycloakConfigurationDto;
  private authorizationEnabled: boolean = false;

  public currentUser: User;

  constructor(
    private configurationService: ConfigurationService,
  ) {
    this.currentUser = this.getCurrentUser();
  }

  public init(): Observable<any> {
    this.authorizationEnabled = this.configurationService.clientConfiguration.features.authorization;
    if (!this.authorizationEnabled) {
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
        tap(() => this.currentUser = this.getCurrentUser())
      );
  }

  public getAccessToken(): Observable<string> {
    if (!this.keycloak)
      throw Error('Keycloak authentication service is not initialized');

    // this.dumpTokenInformation();

    if (this.isRefreshTokenExpired(5))
      this.keycloak.login();

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
    const initOptions: KeycloakInitOptions = {
      onLoad: 'check-sso',
      silentCheckSsoRedirectUri: window.location.origin + '/assets/silent-check-sso.html'
    };
    return from(keycloak.init(initOptions));
  }

  private ensureLoggedIn(isAuthenticated: boolean): void {
    if (!isAuthenticated)
      this.login();
  }

  private loadUserProfile(): Observable<any> {
    return from(this.keycloak!.loadUserProfile())
  }

  private getCurrentUser(): User {
    const roles = Object
      .entries(RoleNames)
      .reduce((permissions: any, [propertyName, value]) => {
        permissions[propertyName] = this.authorizationEnabled ? this.hasTole(value) : true;
        return permissions;
      }, {}) as UserRoles;

    return {
      id: this.keycloak?.profile?.id,
      name: this.keycloak?.profile?.username,
      userAccountUrl: `${this.keycloak?.createAccountUrl({redirectUri: location.href})}`,
      isAuthenticated: this.keycloak?.authenticated ?? false,
      hasRole: roles,
      hasRolesInGroup: {
        chartsView: roles.chartsByCustomerView || roles.chartsByOrderView || roles.chartsByProjectView || roles.chartsByActivityView || roles.chartsByIssueView,
        reportsView: roles.reportActivitySummaryView || roles.reportActivityDetailView,
        masterDataView: roles.masterDataCustomersView || roles.masterDataProjectsView || roles.masterDataActivitiesView || roles.masterDataOrdersView || roles.masterDataHolidaysView,
        administrationView: roles.administrationUsersView || roles.administrationSettingsView,
      }
    };
  }

  private hasTole(role: string): boolean {
    if (!this.keycloak?.authenticated)
      return false;

    return this.keycloak!.hasResourceRole(role, this.configuration?.clientId);
  }

  private isRefreshTokenExpired = (minValidity: number) => {
    return this.isTokenExpired(this.keycloak!.refreshTokenParsed, minValidity);
  };

  private isTokenExpired = (token?: KeycloakTokenParsed, minValidity: number = 5) => {
    if (!token?.exp)
      return true;
    var expiresIn = token.exp - Math.ceil(new Date().getTime() / 1000) + this.keycloak!.timeSkew!;
    expiresIn -= minValidity;
    return expiresIn < 0;
  };

  private dumpTokenInformation() {
    const getTokenTimeStamps = (token?: KeycloakTokenParsed) => {
      const authAt = token?.auth_time ? DateTime.fromSeconds(token.auth_time) : null;
      const issuedAt = token?.iat ? DateTime.fromSeconds(token.iat) : null;
      const expireAt = token?.exp ? DateTime.fromSeconds(token.exp) : null;
      const expireIn = expireAt ? expireAt.diffNow('seconds').seconds : -999;
      return {
        authAt: authAt?.toFormat('yyyy-MM-dd HH:mm:ss'),
        issuedAt: issuedAt?.toFormat('yyyy-MM-dd HH:mm:ss'),
        expireAt: expireAt?.toFormat('yyyy-MM-dd HH:mm:ss'),
        expireIn: expireIn.toFixed(0),
        isExpired: this.isTokenExpired(token),
      }
    };

    const tokenTimestamps = {
      id: getTokenTimeStamps(this.keycloak!.idTokenParsed),
      access: getTokenTimeStamps(this.keycloak!.tokenParsed),
      refresh: getTokenTimeStamps(this.keycloak!.refreshTokenParsed),
    };

    console.table(tokenTimestamps);
  }
}
