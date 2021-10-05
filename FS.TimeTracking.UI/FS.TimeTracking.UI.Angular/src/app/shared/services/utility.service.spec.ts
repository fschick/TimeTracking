import {TestBed} from '@angular/core/testing';
import {UtilityService} from './utility.service';
import {DateTime} from 'luxon';
import {LocalizationService} from './internationalization/localization.service';

describe('UtilityService', () => {
  let service: UtilityService;
  let localizationService: LocalizationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    localizationService = TestBed.inject(LocalizationService);
    service = TestBed.inject(UtilityService);
  });

  afterEach(() => {
    localizationService.clearUserSettings();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('round should work as expected ', () => {
    expect(service.round(0)).toBe(0);
    expect(service.round(1)).toBe(1);
    expect(service.round(-1)).toBe(-1);
    expect(service.round(1.1)).toBe(1);
    expect(service.round(-1.1)).toBe(-1);
    expect(service.round(1.4)).toBe(1);
    expect(service.round(1.5)).toBe(2);
    expect(service.round(-1.5)).toBe(-1);
    expect(service.round(-1.6)).toBe(-2);

    expect(service.round(0, 0)).toBe(0);
    expect(service.round(1, 0)).toBe(1);
    expect(service.round(-1, 0)).toBe(-1);
    expect(service.round(1.4, 0)).toBe(1);
    expect(service.round(1.5, 0)).toBe(2);
    expect(service.round(-1.5, 0)).toBe(-1);
    expect(service.round(-1.6, 0)).toBe(-2);

    expect(service.round(0, 2)).toBe(0);
    expect(service.round(1, 2)).toBe(1);
    expect(service.round(-1, 2)).toBe(-1);
    expect(service.round(1.4, 2)).toBe(1.4);
    expect(service.round(1.5, 2)).toBe(1.5);
    expect(service.round(-1.5, 2)).toBe(-1.5);
    expect(service.round(-1.6, 2)).toBe(-1.6);

    expect(service.round(1.34, 2)).toBe(1.34);
    expect(service.round(1.35, 2)).toBe(1.35);
    expect(service.round(-1.35, 2)).toBe(-1.35);
    expect(service.round(-1.36, 2)).toBe(-1.36);

    expect(service.round(1.334, 2)).toBe(1.33);
    expect(service.round(1.335, 2)).toBe(1.34);
    expect(service.round(-1.335, 2)).toBe(-1.33);
    expect(service.round(-1.336, 2)).toBe(-1.34);
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
    expectedDate = DateTime.local(today.year, 1, 1).plus({weeks: 5}).startOf('week');
    expect(parsedDate?.toString()).toBe(expectedDate.toString(), input);

    input = '/5';
    parsedDate = service.parseDate(input, 'end');
    expectedDate = DateTime.local(today.year, 1, 1).plus({weeks: 5}).endOf('week');
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
