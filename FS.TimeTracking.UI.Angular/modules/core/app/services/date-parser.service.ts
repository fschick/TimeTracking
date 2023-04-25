import {Injectable} from '@angular/core';
import {DateTime, Duration} from 'luxon';
import {DateObjectUnits} from 'luxon/src/datetime';
import {LocalizationService} from './internationalization/localization.service';
import {DurationLike} from 'luxon/src/duration';
import {CoreModule} from '../core.module';

@Injectable()
export class DateParserService {

  private isoDateFormat = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:\.\d+)?(?:Z|[+\-]\d{2}:\d{2})?$/;
  private dotNetTimeSpanFormat = /^(?<sign>[+-]?)(?:(?<days>\d+)\.)?(?<hours>\d{2}):(?<minutes>\d{2}):(?<seconds>\d{2})(?:\.(?<milliseconds>\d{3})\d*)?$/;

  constructor(
    private localizationService: LocalizationService,
  ) { }

  public parseDate(rawInput: string | undefined, relativeAnchor: 'start' | 'end' = 'start'): DateTime | undefined {
    if (!rawInput)
      return undefined;

    const dateFormat = this.localizationService.dateTime.dateFormat;
    const formatPattern = new RegExp('([A-Za-z]+)[^A-Za-z]+([A-Za-z]+)[^A-Za-z]+([A-Za-z]+)');
    const formatParts = dateFormat.match(formatPattern);
    if (formatParts === null)
      return undefined;

    const firstPartLength = formatParts[1].startsWith('yy') ? '{1,4}' : '{1,2}';
    const secondPartLength = formatParts[2].startsWith('yy') ? '{1,4}' : '{1,2}';
    const thirdPartLength = formatParts[3].startsWith('yy') ? '{1,4}' : '{1,2}';

    const todayPattern = new RegExp('^\\s+$');
    const dayPattern = new RegExp('^\\s*(\\d{1,2})\\s*$');
    const monthAndDayPattern = new RegExp('^\\s*(\\d{1,2})\\D*(\\d{1,2})\\s*$');
    const monthDayAndYearPattern = new RegExp(`^\\s*(\\d${firstPartLength})\\D*(\\d${secondPartLength})\\D*(\\d${thirdPartLength})\\s*$`);
    const relativePattern = new RegExp('^\\s*(?<mod1>[*/+-])?\\s*(?<mod2>[*/+-])?\\s*(?<value>\\d{1,2})?\\s*(?<mod3>[*/+-])?\\s*(?<mod4>[*/+-])?\\s*$');

    const todayParts = rawInput.match(todayPattern);
    const dayValueParts = rawInput.match(dayPattern);
    const monthAndDayValueParts = rawInput.match(monthAndDayPattern);
    const monthDayAndYearValueParts = rawInput.match(monthDayAndYearPattern);
    const relativeParts = rawInput.match(relativePattern);

    if (todayParts)
      return this.parseToday();
    if (dayValueParts)
      return this.parseDay(dayValueParts);
    if (monthAndDayValueParts)
      return this.parseMonthAndDay(formatParts, monthAndDayValueParts);
    if (monthDayAndYearValueParts)
      return this.parseMonthDayAndYear(formatParts, monthDayAndYearValueParts);
    if (relativeParts)
      return this.parseRelative(relativeParts, relativeAnchor);
    return undefined;
  }

  public convertJsStringsToLuxon(body: any): any {
    if (body === null || body === undefined || typeof body !== 'object')
      return body;

    let duration: Duration | null;
    for (const [key, value] of Object.entries(body)) {
      if (typeof value === 'object') {
        this.convertJsStringsToLuxon(value);
      } else if (this.isIsoDateString(value)) {
        body[key] = DateTime.fromISO(value as string);
      } else if ((duration = this.parseDuration(value)) !== null) {
        body[key] = duration;
      }
    }

    return body;
  }

  private parseToday(): DateTime {
    const now = DateTime.now();
    return DateTime.local(now.year, now.month, now.day);
  }

  private parseDay(valueParts: RegExpMatchArray): DateTime {
    const now = DateTime.now();
    const day = Math.min(parseInt(valueParts[1], 10), now.daysInMonth!);
    return DateTime.local(now.year, now.month, day);
  }

  private parseMonthAndDay(formatParts: RegExpMatchArray, valueParts: RegExpMatchArray): DateTime | undefined {
    const dayPartIndex = formatParts.findIndex(x => x.startsWith('d'));
    const monthPartIndex = formatParts.findIndex(x => x.startsWith('M'));
    const dayIsFirst = dayPartIndex < monthPartIndex;

    const now = DateTime.now();

    const maxMonth = 12;
    const month = Math.min(parseInt(dayIsFirst ? valueParts[2] : valueParts[1], 10), maxMonth);

    const maxDay = now.set({month: month}).daysInMonth;
    const day = Math.min(parseInt(dayIsFirst ? valueParts[1] : valueParts[2], 10), maxDay!);

    return DateTime.local(now.year, month, day);
  }

  private parseMonthDayAndYear(formatParts: RegExpMatchArray, valueParts: RegExpMatchArray): DateTime | undefined {
    const now = DateTime.now();

    let day = -1;
    let month = -1;
    let year = -1;

    for (let index = 1; index < formatParts.length; index++) {
      const formatPart = formatParts[index];
      const valuePart = parseInt(valueParts[index], 10);
      switch (formatPart) {
        case 'd':
        case 'dd':
          day = valuePart;
          break;
        case 'M':
        case 'MM':
          month = valuePart;
          break;
        case 'yy':
        case 'yyyy':
          year = valuePart;
          break;
      }
    }

    if (year < 100) {
      const century = Math.floor(now.year / 100) * 100;
      const shortYear = now.year % 100;
      if (year - shortYear < 20)
        year = century + year;
      else
        year = century - 100 + year;
    }

    const maxMonth = 12;
    month = Math.min(month, maxMonth);
    const maxDay = now.set({month: month}).daysInMonth;
    day = Math.min(day, maxDay!);

    return DateTime.local(year, month, day);
  }

  private parseRelative(valueParts: RegExpMatchArray, relativeAnchor: 'start' | 'end'): DateTime {
    const today = DateTime.now().startOf('day');
    const [mod1, mod2] = valueParts.groups ? Object.entries(valueParts.groups).filter(([key, val]) => key.startsWith('mod') && val).map(([, val]) => val) : [];
    const operator = (mod1 === '+' || mod1 === '-') ? mod1 : mod2;
    const unit = (mod1 === '*' || mod1 === '/') ? mod1 : mod2;
    const value = valueParts.groups?.['value'];

    let duration: DurationLike;
    let dateUnit: DateObjectUnits;
    if (unit === '*') {
      const maxMonth = operator ? Number.MAX_SAFE_INTEGER : 12;
      const val = Math.min(value ? parseInt(value, 10) : operator ? 1 : today.month, maxMonth);
      duration = {months: val};
      dateUnit = {month: val};
    } else if (unit === '/') {
      const maxWeek = operator ? Number.MAX_SAFE_INTEGER : 53;
      const val = Math.min(value ? parseInt(value, 10) : operator ? 1 : today.weekNumber, maxWeek);
      duration = {weeks: val};
      dateUnit = {weekNumber: val};
    } else {
      const maxDay = operator ? Number.MAX_SAFE_INTEGER : today.daysInMonth;
      const val = Math.min(value ? parseInt(value, 10) : operator ? 1 : today.day, maxDay!);
      duration = {days: val};
      dateUnit = {day: val};
    }

    let parsedDate: DateTime;
    if (operator === '+')
      parsedDate = today.plus(duration);
    else if (operator === '-')
      parsedDate = today.minus(duration);
    else
      parsedDate = today.set(dateUnit);

    if (unit === '*')
      parsedDate = relativeAnchor === 'start' ? parsedDate.startOf('month') : parsedDate.endOf('month');
    else if (unit === '/')
      parsedDate = relativeAnchor === 'start' ? parsedDate.startOf('week') : parsedDate.endOf('week');

    return parsedDate;
  }

  private isIsoDateString(value: any): boolean {
    if (value === null || value === undefined || typeof value !== 'string')
      return false;
    return this.isoDateFormat.test(value);
  }

  private parseDuration(value: any): Duration | null {
    if (value === null || value === undefined || typeof value !== 'string')
      return null;

    const timeSpan = value.match(this.dotNetTimeSpanFormat);
    if (timeSpan === null)
      return null;

    let duration = Duration
      .fromObject({
        days: parseInt(timeSpan.groups?.['days'] ?? '0', 10),
        hours: parseInt(timeSpan.groups?.['hours'] ?? '0', 10),
        minutes: parseInt(timeSpan.groups?.['minutes'] ?? '0', 10),
        seconds: parseInt(timeSpan.groups?.['seconds'] ?? '0', 10),
        milliseconds: parseInt(timeSpan.groups?.['milliseconds'] ?? '0', 10),
      });

    const isNegative = timeSpan.groups?.['sign'] === '-';
    return isNegative
      ? duration.negate()
      : duration;
  }
}
