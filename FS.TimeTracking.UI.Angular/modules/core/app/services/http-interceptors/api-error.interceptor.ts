import {Injectable} from '@angular/core';
import {HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Observable} from 'rxjs';
import {tap} from 'rxjs/operators';
import {ToastrService} from 'ngx-toastr';
import {EnumTranslationService} from '../enum-translation.service';
import {ApplicationError} from '../../../../api/timetracking';

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

        const error = response?.error as ApplicationError;
        const isApplicationError = error != null;

        let message: string;
        if (isApplicationError)
          message = this.enumTranslationService.translate('ErrorCode', error.code);
        else if (response.status === 401)
          message = $localize`:@@Enum.ErrorCode.Unauthorized:[i18n] Access to data requires authentication`;
        else if (response.status === 403)
          message = $localize`:@@Enum.ErrorCode.Forbidden:[i18n] Access to data is not authorized`;
        else
          message = $localize`:@@Enum.ErrorCode.Unknown:[i18n] An unknown error has occurred`;

        // const requestId = response.headers.get('Request-Id');
        // message += `. RequestID: ${requestId}`;

        this.toastrService.error(message);

        if (error?.messages?.length > 0)
          console.error(`${message}: ${error.messages?.join(', ')}`);
      }));
  }
}
