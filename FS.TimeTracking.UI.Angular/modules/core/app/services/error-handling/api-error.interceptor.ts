import {Injectable} from '@angular/core';
import {HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Observable} from 'rxjs';
import {tap} from 'rxjs/operators';
import {ToastrService} from 'ngx-toastr';
import {RestError, RestErrorCode} from '../../../../api/timetracking';
import {EnumTranslationService} from '../enum-translation.service';

@Injectable()
export class ApiErrorInterceptor implements HttpInterceptor {

  constructor(
    private toastrService: ToastrService,
    private enumTranslationService: EnumTranslationService
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(tap(
      () => {},
      (response: HttpErrorResponse) => {

        const error = response.error as RestError;
        const message = error != null
          ? this.enumTranslationService.translate('RestErrorCode', error.errorCode)
          : $localize`:@@Enum.RestErrorCode.Unknown:[i18n] An unkown error has occurred`;

        // const requestId = response.headers.get('Request-Id');
        // message += `. RequestID: ${requestId}`;

        console.error(message);
        this.toastrService.error(message);
      }));
  }
}
