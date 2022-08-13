import { TestBed } from '@angular/core/testing';

import { GuidService } from './guid.service';

describe('GuidService', () => {
  let service: GuidService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GuidService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
