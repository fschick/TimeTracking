import {Injectable} from '@angular/core';
import {HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Observable} from 'rxjs';
import {tap} from 'rxjs/operators';
import {ToastrService} from 'ngx-toastr';
import {DatabaseErrorCode, ErrorInformation} from '../api';

@Injectable()
export class ApiErrorInterceptor implements HttpInterceptor {

  constructor(private toastrService: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(tap(
      () => {},
      (response: HttpErrorResponse) => {

        let message: string;
        const error = response.error as ErrorInformation;
        const requestId = response.headers.get('Request-Id');

        switch (response.status) {
          case 409:
            if (request.method === 'DELETE' && error.databaseErrorCode === DatabaseErrorCode.foreignKeyViolation)
              message = $localize`:@@API.Delete.ForeignKeyViolation:`;
            else
              message = $localize`:@@API.Conflict:`;
            break;
          default:
            message = $localize`:@@API.InternalServerError:`;
        }

        // message += `. RequestID: ${requestId}`;

        console.error(message);
        this.toastrService.error(message);
      }));
  }
}
