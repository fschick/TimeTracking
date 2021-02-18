import {Injectable} from '@angular/core';
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Observable} from 'rxjs';
import {tap} from 'rxjs/operators';
import {ToastrService} from 'ngx-toastr';

@Injectable()
export class ApiErrorInterceptor implements HttpInterceptor {

  constructor(private toastrService: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(tap(
      () => {},
      error => {
        const requestId = error.headers.get('Request-Id');
        const message = $localize`:@@API.InternalServerError:\r\n` + '.\r\nRequestID:' + requestId;
        console.error(message);
        this.toastrService.error(message);
      }));
  }
}
