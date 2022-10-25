import {TestBed} from '@angular/core/testing';

import {AuthenticationService} from './authentication.service';
import {UtilityService} from './utility.service';
import {LocalizationService} from './internationalization/localization.service';
import {StorageService} from './storage.service';

describe('AuthenticationService', () => {
  let service: AuthenticationService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [UtilityService, LocalizationService, StorageService]
    });
    service = TestBed.inject(AuthenticationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
