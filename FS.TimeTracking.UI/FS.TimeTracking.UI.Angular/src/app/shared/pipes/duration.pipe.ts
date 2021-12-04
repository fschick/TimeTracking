import {Pipe, PipeTransform} from '@angular/core';
import {Duration} from 'luxon';
import {FormatService} from '../services/format.service';

@Pipe({
  name: 'tsDuration'
})
export class DurationPipe implements PipeTransform {

  constructor(
    private formatService: FormatService,
  ) {
  }

  transform(value: Duration | null | undefined, format: string = 'hh:mm'): string {
    return this.formatService.formatDuration(value, format);
  }

}
