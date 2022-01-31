import { TestBed } from '@angular/core/testing';

import { ApiDateTimeInterceptor } from './api-date-time.interceptor';

describe('ApiDateTimeInterceptorInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [
      ApiDateTimeInterceptor
      ]
  }));

  it('should be created', () => {
    const interceptor: ApiDateTimeInterceptor = TestBed.inject(ApiDateTimeInterceptor);
    expect(interceptor).toBeTruthy();
  });
});
