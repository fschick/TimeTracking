import {TestBed} from '@angular/core/testing';
import {FormatService} from './format.service';
import {DecimalPipe} from '@angular/common';
import {CoreModule} from '../core.module';

describe('FormatServiceService', () => {
  let service: FormatService;

  beforeEach(() => {
    TestBed.configureTestingModule({imports: [CoreModule], providers: [DecimalPipe]});
    service = TestBed.inject(FormatService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
