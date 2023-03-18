import {Injectable} from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import {mergeMap, Observable} from 'rxjs';
import {AuthenticationService} from '../authentication.service';

@Injectable()
export class AuthenticationInterceptor implements HttpInterceptor {

  constructor(
    private authenticationService: AuthenticationService,
  ) {
  }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    if (!this.authenticationService.currentUser.isAuthenticated)
      return next.handle(request);

    return this.authenticationService
      .getAccessToken()
      .pipe(mergeMap(token => this.runRequestWithToken(request, token, next)))
  }

  private runRequestWithToken<T>(request: HttpRequest<T>, token: string, next: HttpHandler): Observable<HttpEvent<T>> {
    request = request
      .clone({headers: request.headers.set('Authorization', `Bearer ${token}`)});

    return next.handle(request);
  }
}
