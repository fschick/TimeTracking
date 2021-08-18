import {Pipe, PipeTransform} from '@angular/core';
import {LocalizationService} from '../services/internationalization/localization.service';
import {DateTime, DateTimeFormatOptions} from 'luxon';
import {DatePipe} from './date.pipe';

@Pipe({
  name: 'tsTime'
})
export class TimePipe implements PipeTransform {

  constructor(private datePipe: DatePipe) {}

  transform(value: DateTime | null | undefined, format: string | DateTimeFormatOptions = DateTime.TIME_SIMPLE): string {
    return this.datePipe.transform(value, format);
  }

}
