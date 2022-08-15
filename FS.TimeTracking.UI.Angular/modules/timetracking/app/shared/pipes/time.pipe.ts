import {Pipe, PipeTransform} from '@angular/core';
import {LocalizationService} from '../../../../core/app/services/internationalization/localization.service';
import {DateTime, DateTimeFormatOptions} from 'luxon';
import {FormatService} from '../../../../core/app/services/format.service';

@Pipe({
  name: 'tsTime'
})
export class TimePipe implements PipeTransform {

  constructor(
    private localizationService: LocalizationService,
    private formatService: FormatService,
  ) {}

  transform(value: DateTime | null | undefined, format: string | DateTimeFormatOptions = this.localizationService.dateTime.timeFormat): string {
    return this.formatService.formatDate(value, format);
  }

}
