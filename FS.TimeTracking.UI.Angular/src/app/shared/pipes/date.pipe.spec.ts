import {DatePipe} from './date.pipe';
import {TestBed} from '@angular/core/testing';
import {DateTime} from 'luxon';

describe('DatePipe', () => {
  let pipe: DatePipe;

  beforeEach(() => {
    TestBed.configureTestingModule({providers: [DatePipe]});
    pipe = TestBed.inject(DatePipe);
  });

  it('create an instance', () => {
    expect(pipe).toBeTruthy();
  });

  it('date should be formatted to DateTime.DATE_SHORT as default', () => {
    const date = DateTime.local(2020, 6, 15).setLocale('en');
    const transformedDate = pipe.transform(date);
    const expectedDate = '6/15/2020';
    expect(transformedDate).toBe(expectedDate);
  });

  it('date should be formattable via predefined format ', () => {
    const date = DateTime.local(2020, 6, 15).setLocale('en');
    const transformedDate = pipe.transform(date, DateTime.DATE_FULL);
    const expectedDate = 'June 15, 2020';
    expect(transformedDate).toBe(expectedDate);
  });

  it('date should be formattable via custom pattern', () => {
    const date = DateTime.local(2020, 6, 15).setLocale('en');
    const transformedDate = pipe.transform(date, 'yyyy-MM-dd');
    const expectedDate = '2020-06-15';
    expect(transformedDate).toBe(expectedDate);
  });
});
