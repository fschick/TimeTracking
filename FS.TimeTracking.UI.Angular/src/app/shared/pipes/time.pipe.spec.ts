import {TimePipe} from './time.pipe';
import {TestBed} from '@angular/core/testing';
import {DateTime} from 'luxon';
import {DatePipe} from './date.pipe';

describe('TimePipe', () => {
  let pipe: TimePipe;

  beforeEach(() => {
    TestBed.configureTestingModule({providers: [DatePipe, TimePipe]});
    pipe = TestBed.inject(TimePipe);
  });

  it('create an instance', () => {
    expect(pipe).toBeTruthy();
  });

  it('date should be formatted to DateTime.TIME_SHORT as default', () => {
    const date = DateTime.local(2020, 6, 15, 8, 30, 50).setLocale('en');
    const transformedDate = pipe.transform(date);
    const expectedDate = '8:30 AM';
    expect(transformedDate).toBe(expectedDate);
  });

  it('date should be formattable via predefined format ', () => {
    const date = DateTime.local(2020, 6, 15, 8, 30, 50).setLocale('en');
    const transformedDate = pipe.transform(date, DateTime.TIME_24_SIMPLE);
    const expectedDate = '08:30';
    expect(transformedDate).toBe(expectedDate);
  });

  it('date should be formattable via custom pattern', () => {
    const date = DateTime.local(2020, 6, 15, 8, 30, 50).setLocale('en');
    const transformedDate = pipe.transform(date, 'HH:mm:ss');
    const expectedDate = '08:30:50';
    expect(transformedDate).toBe(expectedDate);
  });
});
