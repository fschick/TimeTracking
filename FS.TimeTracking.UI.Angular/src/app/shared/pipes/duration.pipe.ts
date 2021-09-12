import {Pipe, PipeTransform} from '@angular/core';
import {Duration} from 'luxon';

@Pipe({
  name: 'tsDuration'
})
export class DurationPipe implements PipeTransform {

  transform(value: Duration | null | undefined, format: string = 'hh:mm'): string {
    return value
      ? value.toFormat(format)
      : '';
  }

}
