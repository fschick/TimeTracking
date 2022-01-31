import { TestBed } from '@angular/core/testing';

import { ReportChartService } from './report-chart.service';
import {DatePipe} from '../../shared/pipes/date.pipe';
import {TimePipe} from '../../shared/pipes/time.pipe';
import {DecimalPipe} from '@angular/common';

describe('ReportChartService', () => {
  let service: ReportChartService;

  beforeEach(() => {
    TestBed.configureTestingModule({providers: [DatePipe, TimePipe, DecimalPipe]});
    service = TestBed.inject(ReportChartService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
