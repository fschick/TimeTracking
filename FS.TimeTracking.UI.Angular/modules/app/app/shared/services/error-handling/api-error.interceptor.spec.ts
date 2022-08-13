import {TestBed} from '@angular/core/testing';

import {ApiErrorInterceptor} from './api-error.interceptor';
import {ToastrModule} from 'ngx-toastr';

describe('ApiErrorInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [ToastrModule.forRoot({})],
    providers: [
      ApiErrorInterceptor
    ]
  }));

  it('should be created', () => {
    const interceptor: ApiErrorInterceptor = TestBed.inject(ApiErrorInterceptor);
    expect(interceptor).toBeTruthy();
  });
});
