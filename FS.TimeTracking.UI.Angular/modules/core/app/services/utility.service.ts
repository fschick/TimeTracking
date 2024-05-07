import {Injectable} from '@angular/core';
import {LocalizationService} from './internationalization/localization.service';
import {Duration} from 'luxon';
import {Observable} from 'rxjs';

@Injectable()
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

  public escapeRegex(value: string): string {
    if (value == null)
      return '';

    //https://github.com/lodash/lodash/blob/master/escapeRegExp.js
    const charactersToEscape = /[\\^$.*+?()[\]{}|]/g;
    return value.replace(charactersToEscape, '\\$&');
  }

  public replaceAt(value: string, index: number, replacement: string): string {
    if (value == null)
      return '';

    return value.substring(0, index) + replacement + value.substr(index + replacement.length);
  }

  public capitalize(value: string): string {
    if (value == null)
      return '';

    return value[0].toUpperCase() + value.slice(1);
  }

  public snakeToCamelcase(value: string): string {
    if (value == null)
      return '';

    return value.toLowerCase()
      .replace(/\w+?(_|$)/g, k => this.capitalize(k))
      .replace(/_/g, '')
  };

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

  public getBase64EncodedFileData(file: File | undefined): Observable<string | undefined> {
    return new Observable(observer => {
      if (file == null) {
        observer.next(undefined);
        observer.complete();
        return;
      }

      const reader = new FileReader();
      reader.onload = () => {
        if (reader.result == null) {
          observer.next(undefined);
        } else {
          const base64Encoded = window.btoa(reader.result.toString());
          observer.next(base64Encoded);
        }

        observer.complete();
      };

      reader.onerror = () => {
        observer.error(reader.error);
      };

      reader.readAsBinaryString(file);
    });
  }

  // https://stackoverflow.com/a/38327540/1271211
  /**
   * @description
   * Takes an Array<V>, and a grouping function, and returns a Map of the array grouped by the grouping function.
   *
   * @param array An array of type V.
   * @param keyFunc A Function that takes the the Array type V as an input, and returns a value of type K. K is generally intended to be a property key of V.
   *
   * @returns Map of the array grouped by the grouping function.
   */
  public groupBy<V, K>(array: Array<V>, keyFunc: (input: V) => K): Map<K, Array<V>> {
    const result = new Map<K, Array<V>>();
    array.forEach((item) => {
      const key = keyFunc(item);
      const collection = result.get(key);
      if (!collection)
        result.set(key, [item]);
      else
        collection.push(item);
    });
    return result;
  }

  // https://stackoverflow.com/a/33121880
  public distinct<V extends string | number>(array: Array<V>): Array<V> {
    return [...new Set(array)];
  }

  public sum(array: Array<number>): number {
    return array.reduce((prev, current) => prev + current, 0);
  }

  public sumDuration(array: Array<Duration>): Duration {
    return array.reduce((prev, current) => prev.plus(current ?? Duration.fromMillis(0)), Duration.fromMillis(0));
  }

  public avg(array: Array<number>): number {
    if (array.length === 0)
      return 0;
    return this.sum(array) / array.length;
  }
}
