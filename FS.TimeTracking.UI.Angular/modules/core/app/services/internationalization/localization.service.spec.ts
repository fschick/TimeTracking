import {TestBed} from '@angular/core/testing';
import {LocalizationService} from './localization.service';
import {CoreModule} from '../../core.module';

describe('LocalizationService', () => {
  let service: LocalizationService;

  beforeEach(() => {
    TestBed.configureTestingModule({imports: [CoreModule]});
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
    service.dateTime.removeDateFormat();
    service.language = 'en';
    expect(service.dateTime.dateFormat).toBe('MM/dd/yyyy');
  });

  it('should return \'dd.MM.yyyy\' as date format for german', () => {
    service.dateTime.removeDateFormat();
    service.language = 'de';
    expect(service.dateTime.dateFormat).toBe('dd.MM.yyyy');
  });

  it('should return \'HH:mm\' as time format for english', () => {
    service.language = 'en';
    expect(service.dateTime.timeFormat).toBe('HH:mm');
  });

  it('should return \'HH:mm\' as time format for german', () => {
    service.language = 'de';
    expect(service.dateTime.timeFormat).toBe('HH:mm');
  });

  it('should return \'dd.MM.yyyy, HH:mm\' as  date/time format for english', () => {
    service.language = 'en';
    expect(service.dateTime.dateTimeFormat).toBe('MM/dd/yyyy, HH:mm');
  });

  it('should return \'dd.MM.yyyy, HH:mm\' as date/time format for german', () => {
    service.language = 'de';
    expect(service.dateTime.dateTimeFormat).toBe('dd.MM.yyyy, HH:mm');
  });
});
