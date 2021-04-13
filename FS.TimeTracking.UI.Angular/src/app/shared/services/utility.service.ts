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

    // const dateFormat = this.localizationService.dateTime.dateFormat;
    // const formatPattern = new RegExp('([A-Za-z]+)[^A-Za-z]+([A-Za-z]+)[^A-Za-z]+([A-Za-z]+)');
    // const formatParts = dateFormat.match(formatPattern);
    // if (formatParts === null)
    //   return undefined;

    const formatParts = ['dd', 'MM', 'yyyy'];
    const valuePattern = new RegExp('\\s*(\\d{1,2})?(?:\\D*(\\d{1,2}))?(?:\\D*(\\d{2,4}))?');
    const valueParts = rawInput.match(valuePattern);
    if (valueParts === null)
      return undefined;

    let day: number | undefined;
    let month: number | undefined;
    let year: number | undefined;

    for (let index = 1; index < valueParts.length; index++) {
      const formatPart = formatParts[index - 1];
      let valuePart: number | undefined = parseFloat(valueParts[index]);
      if (isNaN(valuePart))
        valuePart = undefined;

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

    const now = DateTime.now();
    if (day === undefined)
      day = now.day;
    if (month === undefined)
      month = now.month;
    if (year === undefined)
      year = now.year;

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
