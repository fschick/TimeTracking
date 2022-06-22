import { TestBed } from '@angular/core/testing';

import { EnumTranslationService } from './enum-translation.service';

describe('EnumTranslationService', () => {
  let service: EnumTranslationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EnumTranslationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
