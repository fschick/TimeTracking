import {TestBed} from '@angular/core/testing';

import {GuidService} from './guid.service';
import {CoreModule} from '../../core.module';

describe('GuidService', () => {
  let service: GuidService;

  beforeEach(() => {
    TestBed.configureTestingModule({imports: [CoreModule]});
    service = TestBed.inject(GuidService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
