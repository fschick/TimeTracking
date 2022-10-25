import {TestBed} from '@angular/core/testing';

import {AuthenticationInterceptor} from './authentication.interceptor';
import {ApiErrorInterceptor} from './api-error.interceptor';
import {EnumTranslationService} from '../enum-translation.service';
import {UtilityService} from '../utility.service';
import {LocalizationService} from '../internationalization/localization.service';
import {StorageService} from '../storage.service';

describe('AuthenticationInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [AuthenticationInterceptor, EnumTranslationService, UtilityService, LocalizationService, StorageService]
  }));

  it('should be created', () => {
    const interceptor: AuthenticationInterceptor = TestBed.inject(AuthenticationInterceptor);
    expect(interceptor).toBeTruthy();
  });
});
