import {Pipe, PipeTransform} from '@angular/core';
import {LocalizationService} from '../services/internationalization/localization.service';
import {DateTime, DateTimeFormatOptions} from 'luxon';

@Pipe({
  name: 'tsDate'
})
export class DatePipe implements PipeTransform {

  constructor(
    private localizationService: LocalizationService
  ) {}

  public transform(value: DateTime | null | undefined, format: string | DateTimeFormatOptions = this.localizationService.dateTime.dateFormat): string {
    if (!value)
      return '';

    return typeof format === 'string'
      ? value.toFormat(format)
      : value.toLocaleString(format);
  }

}
