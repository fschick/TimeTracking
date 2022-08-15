import {Injectable} from '@angular/core';
import {DateTime, DateTimeFormatOptions, Duration} from 'luxon';
import {DecimalPipe} from '@angular/common';
import {LocalizationService} from './internationalization/localization.service';

@Injectable()
export class FormatService {
  public shortDateFormat = this.localizationService.dateTime.dateFormat.replace('yyyy', 'yy');

  constructor(
    private localizationService: LocalizationService,
    private decimalPipe: DecimalPipe,
  ) { }

  public formatNumber(value?: number, options?: Intl.NumberFormatOptions): string {
    if (value === undefined)
      return '';

    options = options ?? {maximumFractionDigits: 20};
    const numberFormatter = new Intl.NumberFormat(this.localizationService.language, options);
    return numberFormatter.format(value);
  }

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
    if (value == null)
      value = Duration.fromMillis(0);

    if (+value < 0)
      return '-' + value.negate().toFormat(format);
    return value.toFormat(format);
  }

  public escapeDateTimeFormat(format: string): string {
    return format.replace(/(S|SSS|u|uu|uuu|s|ss|m|mm|h|hh|H|HH|Z|ZZ|ZZZ|ZZZZ|ZZZZZ|z|a|d|dd|c|E|ccc|EEE|cccc|EEEE|ccccc|EEEEE|L|M|LL|MM|LLL|MMM|LLLL|MMMM|LLLLL|MMMMM|y|yy|yyyy|G|GG|GGGGG|kk|kkkk|W|WW|o|ooo|q|qq|D|DD|DDD|DDDD|t|tt|ttt|tttt|T|TT|TTT|TTTT|f|ff|fff|ffff|F|FF|FFF|FFFF|X|x)/g, '\'$1\'');
  }

  public escapeDurationFormat(format: string): string {
    return format.replace(/([SsmhdMy])/g, '\'$1\'');
  }

  public formatDays(value: number | null | undefined, digitsInfo: string = '1.1-1'): string {
    return this.decimalPipe.transform(value ?? 0, digitsInfo) ?? 'N/A';
  }

  public formatBudget(value: number | null | undefined, digitsInfo: string = '1.2-2'): string {
    return this.decimalPipe.transform(value ?? 0, digitsInfo) ?? 'N/A';
  }

  public formatRatio(value: number | null | undefined, digitsInfo: string = '1.0-0'): string {
    return this.decimalPipe.transform((value ?? 0) * 100, digitsInfo) ?? 'N/A';
  }
}
