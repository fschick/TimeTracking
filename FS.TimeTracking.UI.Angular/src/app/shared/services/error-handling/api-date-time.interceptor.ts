import {Injectable} from '@angular/core';
import {HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpResponse} from '@angular/common/http';
import {Observable} from 'rxjs';
import {map} from 'rxjs/operators';
import {Duration} from 'luxon';
import {DateParserService} from '../date-parser.service';

@Injectable()
export class ApiDateTimeInterceptor implements HttpInterceptor {

  constructor(
    private dateParserService: DateParserService
  ) {}

  public intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {

    const convertedBody = this.convertRequestBody(request.body);
    const nextRequest = request.clone({body: convertedBody});

    return next.handle(nextRequest).pipe(map((event: HttpEvent<any>) => {
      if (event instanceof HttpResponse && typeof event.body === 'object') {
        const convertedBody = this.dateParserService.convertJsStringsToLuxon(event.body);
        event.clone({body: convertedBody});
      }
      return event;
    }));
  }

  private convertRequestBody(body: any): object {
    if (body === null || body === undefined || typeof body !== 'object')
      return body;

    for (const [key, value] of Object.entries(body)) {
      if (value instanceof Duration) {
        body[key] = value.toISOTime();
      } else if (typeof value === 'object') {
        this.convertRequestBody(value);
      }
    }

    return body;
  }
}
