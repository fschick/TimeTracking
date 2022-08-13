import {Pipe, PipeTransform} from '@angular/core';
import {Duration} from 'luxon';
import {FormatService} from '../services/format.service';

@Pipe({
  name: 'tsDuration'
})
export class DurationPipe implements PipeTransform {
  private readonly hoursAbbr: string;
  private readonly minutesAbbr: string;

  constructor(
    private formatService: FormatService,
  ) {
    this.hoursAbbr = this.formatService.escapeDurationFormat($localize`:@@Abbreviations.Hours:[i18n] h`);
    this.minutesAbbr = this.formatService.escapeDurationFormat($localize`:@@Abbreviations.Minutes:[i18n] m`);
  }

  transform(value: Duration | null | undefined, format?: string): string {
    if (format === undefined)
      format = `h${this.hoursAbbr} m${this.minutesAbbr}`;
    return this.formatService.formatDuration(value, format);
  }

}
