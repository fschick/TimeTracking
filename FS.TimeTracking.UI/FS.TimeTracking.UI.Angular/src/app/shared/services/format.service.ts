import {Injectable} from '@angular/core';
import {DateTime, DateTimeFormatOptions, Duration} from 'luxon';
import {DecimalPipe} from '@angular/common';
import {LocalizationService} from './internationalization/localization.service';

@Injectable({
  providedIn: 'root'
})
export class FormatService {

  public shortDateFormat = this.localizationService.dateTime.dateFormat.replace('yyyy', 'yy');

  constructor(
    private localizationService: LocalizationService,
    private decimalPipe: DecimalPipe
  ) { }

  public formatDate(value: DateTime | null | undefined, format: string | DateTimeFormatOptions = this.localizationService.dateTime.dateFormat): string {
    if (value === null || value === undefined)
      return '';

    return typeof format === 'string'
      ? value.toFormat(format)
      : value.toLocaleString(format);
  }

  public formatShortDate(value: DateTime | null | undefined, format: string | DateTimeFormatOptions = this.shortDateFormat): string {
    return this.formatDate(value, format);
  }

  public formatDuration(value: Duration | null | undefined, format: string = 'hh:mm'): string {
    if (value === null || value === undefined)
      return '';

    if (+value < 0)
      return '-' + value.negate().toFormat(format);
    return value.toFormat(format);
  }

  public formatDays(value: number, digitsInfo: string = '1.1-1'): string {
    return this.decimalPipe.transform(value, digitsInfo) ?? '0';
  }

  public formatBudget(value: number, digitsInfo: string = '1.2-2'): string {
    return this.decimalPipe.transform(value, digitsInfo) ?? '0';
  }

  public formatRatio(value: number, digitsInfo: string = '1.0-0'): string {
    return this.decimalPipe.transform(value * 100, digitsInfo) ?? '0';
  }
}
