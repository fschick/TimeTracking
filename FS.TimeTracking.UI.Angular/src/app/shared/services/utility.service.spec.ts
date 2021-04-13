import {TestBed} from '@angular/core/testing';
import {UtilityService} from './utility.service';
import {DateTime} from 'luxon';

describe('UtilityService', () => {
  let service: UtilityService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UtilityService);
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
    expect(service.parseDate('')).toBe(undefined);
    expect(service.parseDate(' ')?.toString()).toBe(DateTime.local(now.year, now.month, now.day).toString());

    expect(service.parseDate(`${now.day - 1}`)?.toString()).toBe(DateTime.local(now.year, now.month, now.day - 1).toString());
    expect(service.parseDate(`${now.day - 1} ${now.month - 1} `)?.toString()).toBe(DateTime.local(now.year, now.month - 1, now.day - 1).toString());
    expect(service.parseDate(`${now.day - 1} ${now.month - 1} ${now.year - 1} `)?.toString()).toBe(DateTime.local(now.year - 1, now.month - 1, now.day - 1).toString());

    expect(service.parseDate(`02 04 1938`)?.toString()).toBe(DateTime.local(1938, 4, 2).toString());
    expect(service.parseDate(`2 04 1938`)?.toString()).toBe(DateTime.local(1938, 4, 2).toString());
    expect(service.parseDate(`02 4 1938`)?.toString()).toBe(DateTime.local(1938, 4, 2).toString());
    expect(service.parseDate(` 2 4 1938`)?.toString()).toBe(DateTime.local(1938, 4, 2).toString());

    expect(service.parseDate(`02 04 20`)?.toString()).toBe(DateTime.local(2020, 4, 2).toString());
    expect(service.parseDate(`02 04 80`)?.toString()).toBe(DateTime.local(1980, 4, 2).toString());

    expect(service.parseDate(`020420`)?.toString()).toBe(DateTime.local(2020, 4, 2).toString());
    expect(service.parseDate(`0204`)?.toString()).toBe(DateTime.local(now.year, 4, 2).toString());
    expect(service.parseDate(`024`)?.toString()).toBe(DateTime.local(now.year, 4, 2).toString());
  });
});
