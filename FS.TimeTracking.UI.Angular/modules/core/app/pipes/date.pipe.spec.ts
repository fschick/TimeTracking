import {DatePipe} from './date.pipe';
import {TestBed} from '@angular/core/testing';
import {DateTime} from 'luxon';
import {LocalizationService} from '../services/internationalization/localization.service';
import {DecimalPipe} from '@angular/common';
import {CoreModule} from '../core.module';

describe('DatePipe', () => {
  let localizationService: LocalizationService;
  let pipe: DatePipe;

  beforeEach(() => {
    TestBed.configureTestingModule({providers: [DatePipe, DecimalPipe], imports: [CoreModule]});
    localizationService = TestBed.inject(LocalizationService);
    pipe = TestBed.inject(DatePipe);
  });

  afterEach(() => {
    localizationService.clearUserSettings();
  });

  it('create an instance', () => {
    expect(pipe).toBeTruthy();
  });

  it('date should be formatted to guessed format as default', () => {
    localizationService.language = 'en';
    const date = DateTime.local(2020, 6, 15).setLocale('en');
    const transformedDate = pipe.transform(date);
    const expectedDate = '06/15/2020';
    expect(transformedDate).toBe(expectedDate);
  });

  it('date should be formattable via predefined format ', () => {
    localizationService.language = 'en';
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
