import {Injectable} from '@angular/core';
import {LocalizationService} from './internationalization/localization.service';
import {DateTime} from 'luxon';

@Injectable({
  providedIn: 'root'
})
export class UtilityService {
  public readonly digitCharPattern: string;
  public readonly digitChars: RegExp;
  public readonly digitGroupingChars: RegExp;

  private readonly decimalSymbol: string;

  constructor(
    private localizationService: LocalizationService,
  ) {
    this.decimalSymbol = localizationService.numbers.decimalSymbol;
    this.digitCharPattern = `\\d${this.escapeRegex(this.decimalSymbol)}`;
    this.digitChars = new RegExp(`[${this.digitCharPattern}]`, 'g');
    this.digitGroupingChars = new RegExp(`[^${this.digitCharPattern}]`, 'g');
  }

  public formatNumber(value?: number, options?: Intl.NumberFormatOptions): string {
    if (value === undefined)
      return '';

    options = options ?? {maximumFractionDigits: 20};
    const numberFormatter = new Intl.NumberFormat(this.localizationService.language, options);
    return numberFormatter.format(value);
  }

  public parseNumber(value?: string, fractionDigits?: number): number | undefined {
    if (value === undefined)
      return undefined;

    const numericStr = value.replace(this.digitGroupingChars, '').replace(this.decimalSymbol, '.');

    let parsed = parseFloat(numericStr);
    if (Number.isNaN(parsed))
      return undefined;

    if (fractionDigits !== undefined)
      parsed = this.round(parsed, fractionDigits);

    return parsed;
  }

  public round(value: number, fractionDigits?: number): number {
    const fractionModifier = Math.pow(10, fractionDigits ?? 0);
    return Math.round((value + Number.EPSILON) * fractionModifier) / fractionModifier;
  }

  public isNumericallyEqual(val1: string, val2: string): boolean {
    const rawValue1 = val1?.replace(this.digitGroupingChars, '');
    const rawValue2 = val2?.replace(this.digitGroupingChars, '');
    return rawValue1 === rawValue2;
  }

  public escapeRegex(value: string) {
    //https://github.com/lodash/lodash/blob/master/escapeRegExp.js
    const charactersToEscape = /[\\^$.*+?()[\]{}|]/g;
    return value.replace(charactersToEscape, '\\$&');
  }

  public replaceAt(value: string, index: number, replacement: string): string {
    return value.substr(0, index) + replacement + value.substr(index + replacement.length);
  }

  public getFirstDifference(val1?: string, val2?: string): number {
    if (val1 === val2)
      return -1;

    if (val1 === undefined || val2 === undefined)
      return 0;

    let index = 0;
    while (val1[index] === val2[index])
      index++;

    return index;
  }

  public parseDate(rawInput: string | undefined): DateTime | undefined {
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

    const todayParts = rawInput.match(todayPattern);
    const dayValueParts = rawInput.match(dayPattern);
    const monthAndDayValueParts = rawInput.match(monthAndDayPattern);
    const monthDayAndYearValueParts = rawInput.match(monthDayAndYearPattern);

    if (todayParts)
      return this.parseToday();
    if (dayValueParts)
      return this.parseDay(dayValueParts);
    if (monthAndDayValueParts)
      return this.parseMonthAndDay(formatParts, monthAndDayValueParts);
    if (monthDayAndYearValueParts)
      return this.parseMonthDayAndYear(formatParts, monthDayAndYearValueParts);
    return undefined;
  }

  private parseToday(): DateTime {
    const now = DateTime.now();
    return DateTime.local(now.year, now.month, now.day);
  }

  private parseDay(valueParts: RegExpMatchArray): DateTime {
    const now = DateTime.now();
    const day = parseFloat(valueParts[1]);
    return DateTime.local(now.year, now.month, day);
  }

  private parseMonthAndDay(formatParts: RegExpMatchArray, valueParts: RegExpMatchArray): DateTime | undefined {
    const dayPartIndex = formatParts.findIndex(x => x.startsWith('d'));
    const monthPartIndex = formatParts.findIndex(x => x.startsWith('M'));
    const dayIsFirst = dayPartIndex < monthPartIndex;

    const now = DateTime.now();
    const day = parseFloat(dayIsFirst ? valueParts[1] : valueParts[2]);
    const month = parseFloat(dayIsFirst ? valueParts[2] : valueParts[1]);
    return DateTime.local(now.year, month, day);
  }

  private parseMonthDayAndYear(formatParts: RegExpMatchArray, valueParts: RegExpMatchArray): DateTime | undefined {
    const now = DateTime.now();

    let day = -1;
    let month = -1;
    let year = -1;

    for (let index = 1; index < formatParts.length; index++) {
      const formatPart = formatParts[index];
      const valuePart = parseFloat(valueParts[index]);
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

    return DateTime.local(year, month, day);
  }
}
