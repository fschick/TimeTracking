import {TestBed} from '@angular/core/testing';

import {DateParserService} from './date-parser.service';
import {DateTime} from 'luxon';
import {LocalizationService} from './internationalization/localization.service';
import {CoreModule} from '../core.module';

describe('DateParserService', () => {
  let service: DateParserService;
  let localizationService: LocalizationService;

  beforeEach(() => {
    TestBed.configureTestingModule({imports: [CoreModule]});
    localizationService = TestBed.inject(LocalizationService);
    service = TestBed.inject(DateParserService);
  });

  afterEach(() => {
    localizationService.clearUserSettings();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('parseDate should work as expected ', () => {
    const today = DateTime.local().startOf('day');
    let input: string;
    let parsedDate: DateTime | undefined;
    let expectedDate: DateTime | undefined;

    // Empty input
    parsedDate = service.parseDate('');
    expectedDate = undefined;
    expect(parsedDate?.toString()).toBe(expectedDate);

    parsedDate = service.parseDate(' ');
    expectedDate = DateTime.local(today.year, today.month, today.day);
    expect(parsedDate?.toString()).toBe(expectedDate.toString());

    // Dynamic input
    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    input = `${today.day - 1}`;
    parsedDate = service.parseDate(input);
    expectedDate = DateTime.local(today.year, today.month, today.day - 1);
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    input = `${today.day - 1} ${today.month - 1} `;
    parsedDate = service.parseDate(input);
    expectedDate = DateTime.local(today.year, today.month - 1, today.day - 1);
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    input = `${today.day - 1} ${today.month - 1} ${today.year - 1} `;
    parsedDate = service.parseDate(input);
    expectedDate = DateTime.local(today.year - 1, today.month - 1, today.day - 1);
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    // Fixed input, 4-digit year
    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    input = '02 04 1938';
    parsedDate = service.parseDate(input);
    expectedDate = DateTime.local(1938, 4, 2);
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    input = '2 04 1938';
    parsedDate = service.parseDate(input);
    expectedDate = DateTime.local(1938, 4, 2);
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    input = '02 4 1938';
    parsedDate = service.parseDate(input);
    expectedDate = DateTime.local(1938, 4, 2);
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    input = ' 2 4 1938';
    parsedDate = service.parseDate(input);
    expectedDate = DateTime.local(1938, 4, 2);
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    // Fixed input, 2-digit year
    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    input = '02 04 20';
    parsedDate = service.parseDate(input);
    expectedDate = DateTime.local(2020, 4, 2);
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    input = '02 04 80';
    parsedDate = service.parseDate(input);
    expectedDate = DateTime.local(1980, 4, 2);
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    // Fixed input, partial values
    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    input = '020420';
    parsedDate = service.parseDate(input);
    expectedDate = DateTime.local(2020, 4, 2);
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    input = '0204';
    parsedDate = service.parseDate(input);
    expectedDate = DateTime.local(today.year, 4, 2);
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    input = '024';
    parsedDate = service.parseDate(input);
    expectedDate = DateTime.local(today.year, 4, 2);
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    // Relative input, basic operators and units
    input = '+1';
    parsedDate = service.parseDate(input);
    expectedDate = today.plus({days: 1});
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '-1';
    parsedDate = service.parseDate(input);
    expectedDate = today.plus({days: -1});
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '+90';
    parsedDate = service.parseDate(input);
    expectedDate = today.plus({days: 90});
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '-90';
    parsedDate = service.parseDate(input);
    expectedDate = today.plus({days: -90});
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '*5';
    parsedDate = service.parseDate(input);
    expectedDate = DateTime.local(today.year, 5, 1).startOf('month');
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '*5';
    parsedDate = service.parseDate(input, 'end');
    expectedDate = DateTime.local(today.year, 5, 1).endOf('month');
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '/5';
    parsedDate = service.parseDate(input);
    expectedDate = DateTime.local(today.year, 1, 1).set({weekNumber: 5}).startOf('week');
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '/5';
    parsedDate = service.parseDate(input, 'end');
    expectedDate = DateTime.local(today.year, 1, 1).set({weekNumber: 5}).endOf('week');
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    // Relative input, empty unit
    input = '+';
    parsedDate = service.parseDate(input);
    expectedDate = today.plus({days: 1});
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '-';
    parsedDate = service.parseDate(input);
    expectedDate = today.minus({days: 1});
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '*';
    parsedDate = service.parseDate(input);
    expectedDate = today.startOf('month');
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '/';
    parsedDate = service.parseDate(input);
    expectedDate = today.startOf('week');
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '+*';
    parsedDate = service.parseDate(input);
    expectedDate = today.plus({months: 1}).startOf('month');
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '-*';
    parsedDate = service.parseDate(input);
    expectedDate = today.minus({months: 1}).startOf('month');
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '+/';
    parsedDate = service.parseDate(input);
    expectedDate = today.plus({weeks: 1}).startOf('week');
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '-/';
    parsedDate = service.parseDate(input);
    expectedDate = today.minus({weeks: 1}).startOf('week');
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    // Relative input, variant operator and unit position
    input = '+7';
    parsedDate = service.parseDate(input);
    expectedDate = today.plus({days: 7});
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '7+';
    parsedDate = service.parseDate(input);
    expectedDate = today.plus({days: 7});
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '*7';
    parsedDate = service.parseDate(input);
    expectedDate = today.set({month: 7}).startOf('month');
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '7*';
    parsedDate = service.parseDate(input);
    expectedDate = today.set({month: 7}).startOf('month');
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '*+7';
    parsedDate = service.parseDate(input);
    expectedDate = today.plus({months: 7}).startOf('month');
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '+*7';
    parsedDate = service.parseDate(input);
    expectedDate = today.plus({months: 7}).startOf('month');
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '+7*';
    parsedDate = service.parseDate(input);
    expectedDate = today.plus({months: 7}).startOf('month');
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '*7+';
    parsedDate = service.parseDate(input);
    expectedDate = today.plus({months: 7}).startOf('month');
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '7+*';
    parsedDate = service.parseDate(input);
    expectedDate = today.plus({months: 7}).startOf('month');
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '7+*';
    parsedDate = service.parseDate(input);
    expectedDate = today.plus({months: 7}).startOf('month');
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);
  });
});
