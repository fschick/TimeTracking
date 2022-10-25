import {Injectable} from '@angular/core';
import Keycloak, {KeycloakInitOptions} from 'keycloak-js';
import {forkJoin, from, map, Observable, of} from 'rxjs';
import {ConfigurationService} from './configuration.service';
import {KeycloakConfigurationDto} from '../../../api/timetracking';
import {switchMap, tap} from 'rxjs/operators';
import {RoleNames} from '../../../authorization/app/services/RoleNames';
import {UtilityService} from './utility.service';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private keycloak?: Keycloak;
  private configuration?: KeycloakConfigurationDto;
  private authorizationEnabled: boolean = false;

  get isAuthenticated(): boolean {
    return this.keycloak?.authenticated ?? false;
  }

  get username(): string | undefined {
    return this.keycloak?.profile?.username;
  }

  constructor(
    private configurationService: ConfigurationService
  ) { }

  public init(): Observable<any> {
    if (!this.configurationService.clientConfiguration.features.authorization)
      return from(Promise.resolve());

    this.configuration = this.configurationService.clientConfiguration.keycloak;
    const openIdConnectOptions = {
      url: this.configuration.authServerUrl,
      realm: this.configuration.realm,
      clientId: this.configuration.clientId,
    };

    this.keycloak = new Keycloak(openIdConnectOptions);

    return this
      .initKeycloak(this.keycloak)
      .pipe(switchMap(isAuthenticated => this.loadUserProfile(isAuthenticated)));
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

  private loadUserProfile(isAuthenticated: boolean): Observable<any> {
    if (!isAuthenticated)
      return of(undefined);

    return from(this.keycloak!.loadUserProfile())
  }
}
