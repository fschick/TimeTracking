import { TestBed } from '@angular/core/testing';

import { EntityChangedService } from './entity-changed.service';

describe('EntityChangedService', () => {
  let service: EntityChangedService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EntityChangedService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
