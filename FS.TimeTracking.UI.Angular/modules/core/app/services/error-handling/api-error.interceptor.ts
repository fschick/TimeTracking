import {Injectable} from '@angular/core';
import {HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Observable} from 'rxjs';
import {tap} from 'rxjs/operators';
import {ToastrService} from 'ngx-toastr';
import {DatabaseErrorCode, ErrorInformation} from '../../../../api/timetracking';
import {CoreModule} from '../../core.module';

@Injectable({
  providedIn: CoreModule
})
export class ApiErrorInterceptor implements HttpInterceptor {

  constructor(private toastrService: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(tap(
      () => {},
      (response: HttpErrorResponse) => {

        let message: string;
        const error = response.error as ErrorInformation;

        switch (response.status) {
          case 404:
            message = $localize`:@@API.NotFound:[i18n] The record could not be found`;
            break;
          case 409:
            if (request.method === 'DELETE' && error.databaseErrorCode === DatabaseErrorCode.foreignKeyViolation)
              message = $localize`:@@API.Delete.ForeignKeyViolation:[i18n] The record could not be deleted because other data depend on it`;
            else
              message = $localize`:@@API.Conflict:[i18n] The operation conflicts with other data`;
            break;
          default:
            message = $localize`:@@API.InternalServerError:[i18n] Internal server error occurred`;
        }

        // const requestId = response.headers.get('Request-Id');
        // message += `. RequestID: ${requestId}`;

        console.error(message);
        this.toastrService.error(message);
      }));
  }
}
