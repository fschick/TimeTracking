import { TestBed } from '@angular/core/testing';

import { EntityService } from './entity.service';
import {CoreModule} from '../../core.module';

describe('EntityChangedService', () => {
  let service: EntityService;

  beforeEach(() => {
    TestBed.configureTestingModule({imports: [CoreModule]});
    service = TestBed.inject(EntityService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
