import {TestBed} from '@angular/core/testing';
import {FormatService} from './format.service';
import {DecimalPipe} from '@angular/common';

describe('FormatServiceService', () => {
  let service: FormatService;

  beforeEach(() => {
    TestBed.configureTestingModule({providers: [DecimalPipe]});
    service = TestBed.inject(FormatService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
