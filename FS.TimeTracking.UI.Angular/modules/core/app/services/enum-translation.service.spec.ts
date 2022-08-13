import { TestBed } from '@angular/core/testing';

import { EnumTranslationService } from './enum-translation.service';
import {CoreModule} from '../core.module';

describe('EnumTranslationService', () => {
  let service: EnumTranslationService;

  beforeEach(() => {
    TestBed.configureTestingModule({imports: [CoreModule]});
    service = TestBed.inject(EnumTranslationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
