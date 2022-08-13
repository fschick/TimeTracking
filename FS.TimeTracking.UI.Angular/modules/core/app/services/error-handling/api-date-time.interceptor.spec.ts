import {TestBed} from '@angular/core/testing';

import {ApiDateTimeInterceptor} from './api-date-time.interceptor';
import {CoreModule} from '../../core.module';

describe('ApiDateTimeInterceptorInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [CoreModule],
    providers: [ApiDateTimeInterceptor]
  }));

  it('should be created', () => {
    const interceptor: ApiDateTimeInterceptor = TestBed.inject(ApiDateTimeInterceptor);
    expect(interceptor).toBeTruthy();
  });
});
