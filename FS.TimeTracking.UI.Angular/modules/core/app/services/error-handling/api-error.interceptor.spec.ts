import {TestBed} from '@angular/core/testing';

import {ApiErrorInterceptor} from './api-error.interceptor';
import {ToastrModule} from 'ngx-toastr';
import {EnumTranslationService} from '../enum-translation.service';
import {UtilityService} from '../utility.service';
import {LocalizationService} from '../internationalization/localization.service';
import {StorageService} from '../storage.service';

describe('ApiErrorInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [ToastrModule.forRoot({})],
    providers: [ApiErrorInterceptor, EnumTranslationService, UtilityService, LocalizationService, StorageService]
  }));

  it('should be created', () => {
    const interceptor: ApiErrorInterceptor = TestBed.inject(ApiErrorInterceptor);
    expect(interceptor).toBeTruthy();
  });
});
