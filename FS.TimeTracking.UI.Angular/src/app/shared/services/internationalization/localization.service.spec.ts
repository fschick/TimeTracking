import {TestBed} from '@angular/core/testing';
import {LocalizationService} from './localization.service';

describe('LocalizationService', () => {
  let service: LocalizationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LocalizationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return \'.\' as decimal symbol for english', () => {
    service.language = 'en';
    expect(service.numbers.decimalSymbol).toBe('.');
  });

  it('should return \',\' as decimal symbol for german', () => {
    service.language = 'de';
    expect(service.numbers.decimalSymbol).toBe(',');
  });

  it('should return \',\' as digit grouping symbol for english', () => {
    service.language = 'en';
    expect(service.numbers.digitGroupingSymbol).toBe(',');
  });

  it('should return \'.\' as digit grouping symbol for german', () => {
    service.language = 'de';
    expect(service.numbers.digitGroupingSymbol).toBe('.');
  });


  it('should return \'MM/dd/yyyy\' as date format for english', () => {
    service.language = 'en';
    expect(service.dateTime.dateFormat).toBe('MM/dd/yyyy');
  });

  it('should return \'dd.MM.yyyy\' as date format for german', () => {
    service.language = 'de';
    expect(service.dateTime.dateFormat).toBe('dd.MM.yyyy');
  });

  it('should return \'HH:mm:ss\' as time format for english', () => {
    service.language = 'en';
    expect(service.dateTime.timeFormat).toBe('HH:mm:ss');
  });

  it('should return \'HH:mm:ss\' as time format for german', () => {
    service.language = 'de';
    expect(service.dateTime.timeFormat).toBe('HH:mm:ss');
  });

  it('should return \'dd.MM.yyyy, HH:mm:ss\' as  date/time format for english', () => {
    service.language = 'en';
    expect(service.dateTime.dateTimeFormat).toBe('MM/dd/yyyy, HH:mm:ss');
  });

  it('should return \'dd.MM.yyyy, HH:mm:ss\' as date/time format for german', () => {
    service.language = 'de';
    expect(service.dateTime.dateTimeFormat).toBe('dd.MM.yyyy, HH:mm:ss');
  });
});
