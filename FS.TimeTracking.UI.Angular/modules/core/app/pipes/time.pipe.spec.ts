import {TimePipe} from './time.pipe';
import {TestBed} from '@angular/core/testing';
import {DateTime} from 'luxon';
import {DatePipe} from './date.pipe';
import {DecimalPipe} from '@angular/common';
import {CoreModule} from '../core.module';

describe('TimePipe', () => {
  let pipe: TimePipe;

  beforeEach(() => {
    TestBed.configureTestingModule({providers: [DatePipe, TimePipe, DecimalPipe], imports: [CoreModule]});
    pipe = TestBed.inject(TimePipe);
  });

  it('create an instance', () => {
    expect(pipe).toBeTruthy();
  });

  it('time should be formatted to guessed time format as default', () => {
    const date = DateTime.local(2020, 6, 15, 16, 30, 50).setLocale('en');
    const transformedDate = pipe.transform(date);
    const expectedDate = '16:30';
    expect(transformedDate).toBe(expectedDate);
  });

  it('time should be formattable via predefined format ', () => {
    const date = DateTime.local(2020, 6, 15, 8, 30, 50).setLocale('en');
    const transformedDate = pipe.transform(date, DateTime.TIME_24_SIMPLE);
    const expectedDate = '08:30';
    expect(transformedDate).toBe(expectedDate);
  });

  it('time should be formattable via custom pattern', () => {
    const date = DateTime.local(2020, 6, 15, 8, 30, 50).setLocale('en');
    const transformedDate = pipe.transform(date, 'HH:mm:ss');
    const expectedDate = '08:30:50';
    expect(transformedDate).toBe(expectedDate);
  });
});
