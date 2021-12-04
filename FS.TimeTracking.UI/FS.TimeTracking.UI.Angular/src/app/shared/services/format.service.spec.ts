import { TestBed } from '@angular/core/testing';
import { FormatService } from './format.service';
import {DatePipe} from '../pipes/date.pipe';
import {DecimalPipe} from '@angular/common';

describe('FormatService', () => {
  let service: FormatService;

  beforeEach(() => {
    TestBed.configureTestingModule({providers: [DecimalPipe]});
    service = TestBed.inject(FormatService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
