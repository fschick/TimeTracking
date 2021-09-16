import { TestBed } from '@angular/core/testing';

import { ApiDateTimeInterceptorInterceptor } from './api-date-time-interceptor.interceptor';

describe('ApiDateTimeInterceptorInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [
      ApiDateTimeInterceptorInterceptor
      ]
  }));

  it('should be created', () => {
    const interceptor: ApiDateTimeInterceptorInterceptor = TestBed.inject(ApiDateTimeInterceptorInterceptor);
    expect(interceptor).toBeTruthy();
  });
});
