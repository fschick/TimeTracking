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
    const now = DateTime.local();
    let parsedDate: DateTime | undefined;
    let expectedDate: DateTime | undefined;

    // Empty input
    parsedDate = service.parseDate('');
    expectedDate = undefined;
    expect(parsedDate?.toString()).toBe(expectedDate);

    parsedDate = service.parseDate(' ');
    expectedDate = DateTime.local(now.year, now.month, now.day);
    expect(parsedDate?.toString()).toBe(expectedDate.toString());

    // Relative input
    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    parsedDate = service.parseDate(`${now.day - 1}`);
    expectedDate = DateTime.local(now.year, now.month, now.day - 1);
    expect(parsedDate?.toString()).toBe(expectedDate.toString());

    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    parsedDate = service.parseDate(`${now.day - 1} ${now.month - 1} `);
    expectedDate = DateTime.local(now.year, now.month - 1, now.day - 1);
    expect(parsedDate?.toString()).toBe(expectedDate.toString());

    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    parsedDate = service.parseDate(`${now.day - 1} ${now.month - 1} ${now.year - 1} `);
    expectedDate = DateTime.local(now.year - 1, now.month - 1, now.day - 1);
    expect(parsedDate?.toString()).toBe(expectedDate.toString());

    // Fixed input, 4-digit year
    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    parsedDate = service.parseDate(`02 04 1938`);
    expectedDate = DateTime.local(1938, 4, 2);
    expect(parsedDate?.toString()).toBe(expectedDate.toString());

    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    parsedDate = service.parseDate(`2 04 1938`);
    expectedDate = DateTime.local(1938, 4, 2);
    expect(parsedDate?.toString()).toBe(expectedDate.toString());

    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    parsedDate = service.parseDate(`02 4 1938`);
    expectedDate = DateTime.local(1938, 4, 2);
    expect(parsedDate?.toString()).toBe(expectedDate.toString());

    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    parsedDate = service.parseDate(` 2 4 1938`);
    expectedDate = DateTime.local(1938, 4, 2);
    expect(parsedDate?.toString()).toBe(expectedDate.toString());

    // Fixed input, 2-digit year
    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    parsedDate = service.parseDate(`02 04 20`);
    expectedDate = DateTime.local(2020, 4, 2);
    expect(parsedDate?.toString()).toBe(expectedDate.toString());

    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    parsedDate = service.parseDate(`02 04 80`);
    expectedDate = DateTime.local(1980, 4, 2);
    expect(parsedDate?.toString()).toBe(expectedDate.toString());

    // Fixed input, partial values
    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    parsedDate = service.parseDate(`020420`);
    expectedDate = DateTime.local(2020, 4, 2);
    expect(parsedDate?.toString()).toBe(expectedDate.toString());

    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    parsedDate = service.parseDate(`0204`);
    expectedDate = DateTime.local(now.year, 4, 2);
    expect(parsedDate?.toString()).toBe(expectedDate.toString());

    localizationService.dateTime.dateFormat = 'dd.MM.yyyy';
    parsedDate = service.parseDate(`024`);
    expectedDate = DateTime.local(now.year, 4, 2);
    expect(parsedDate?.toString()).toBe(expectedDate.toString());
  });
});
