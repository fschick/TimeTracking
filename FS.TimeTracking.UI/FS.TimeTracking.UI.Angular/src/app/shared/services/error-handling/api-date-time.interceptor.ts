import {Injectable} from '@angular/core';
import {HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpResponse} from '@angular/common/http';
import {Observable} from 'rxjs';
import {map} from 'rxjs/operators';
import {DateTime, Duration} from 'luxon';

@Injectable()
export class ApiDateTimeInterceptor implements HttpInterceptor {

  private isoDateFormat = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:\.\d+)?(?:Z|[+\-]\d{2}:\d{2})?$/;
  private dotNetTimeSpanFormat = /^(?:(?<days>\d+)\.)?(?<hours>\d{2}):(?<minutes>\d{2}):(?<seconds>\d{2})(?:\.(?<milliseconds>\d{3})\d*)?$/;

  public intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(map((val: HttpEvent<any>) => {
      if (val instanceof HttpResponse) {
        const body = val.body;
        this.convert(body);
      }
      return val;
    }));
  }

  private convert(body: any) {
    if (body === null || body === undefined || typeof body !== 'object')
      return body;

    let duration: Duration | null;
    for (const key of Object.keys(body)) {
      const value = body[key];
      if (typeof value === 'object') {
        this.convert(value);
      } else if (this.isIsoDateString(value)) {
        body[key] = DateTime.fromISO(value);
      } else if ((duration = this.parseDuration(value)) !== null) {
        body[key] = duration;
      }
    }
  }

  private isIsoDateString(value: any): boolean {
    if (value === null || value === undefined || typeof value !== 'string')
      return false;
    return this.isoDateFormat.test(value);
  }

  private parseDuration(value: any): Duration | null {
    if (value === null || value === undefined || typeof value !== 'string')
      return null;

    const timeSpan = value.match(this.dotNetTimeSpanFormat);
    if (timeSpan === null)
      return null;

    return Duration
      .fromObject({
        days: parseInt(timeSpan.groups?.days ?? '0', 10),
        hours: parseInt(timeSpan.groups?.hours ?? '0', 10),
        minutes: parseInt(timeSpan.groups?.minutes ?? '0', 10),
        seconds: parseInt(timeSpan.groups?.seconds ?? '0', 10),
        milliseconds: parseInt(timeSpan.groups?.milliseconds ?? '0', 10),
      });
  }
}
