import {TestBed} from '@angular/core/testing';
import {UtilityService} from './utility.service';
import {CoreModule} from '../core.module';

describe('UtilityService', () => {
  let service: UtilityService;

  beforeEach(() => {
    TestBed.configureTestingModule({imports: [CoreModule]});
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
});
