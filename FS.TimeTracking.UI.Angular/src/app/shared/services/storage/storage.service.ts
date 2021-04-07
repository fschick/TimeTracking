import {Injectable} from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  private readonly storagePrefix = 'TimeTracking.';
  private readonly languageKey = this.storagePrefix + 'language';
  private readonly dateFormatKey = this.storagePrefix + 'dateFormat';
  private readonly timeFormatKey = this.storagePrefix + 'timeFormat';
  private readonly dateTimeFormatKey = this.storagePrefix + 'dateTimeFormat';

  public get language(): string {
    return localStorage.getItem(this.languageKey) ?? navigator.languages[0].substring(0, 2);
  }

  public set language(value: string) {
    localStorage.setItem(this.languageKey, value);
  }

  public get dateFormat(): string {
    return localStorage.getItem(this.dateFormatKey) ?? this.getDateFormat();
  }

  public set dateFormat(value: string) {
    localStorage.setItem(this.dateFormatKey, value);
  }

  public get timeFormat(): string {
    return localStorage.getItem(this.timeFormatKey) ?? this.getTimeFormat();
  }

  public set timeFormat(value: string) {
    localStorage.setItem(this.timeFormatKey, value);
  }

  public get dateTimeFormat(): string {
    return localStorage.getItem(this.dateTimeFormatKey) ?? this.getDateTimeFormat();
  }

  public set dateTimeFormat(value: string) {
    localStorage.setItem(this.dateTimeFormatKey, value);
  }

  private getDateFormat(): string {
    const formattedDate = this.formatDate.toLocaleString(this.language, {day: '2-digit', month: '2-digit', year: 'numeric'});
    return this.getFormat(formattedDate);
  }

  private getTimeFormat(): string {
    const formattedDate = this.formatDate.toLocaleString(this.language, {hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: false});
    return this.getFormat(formattedDate);
  }

  private getDateTimeFormat(): string {
    const formattedDate = this.formatDate.toLocaleString(this.language, {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
      hour12: false
    });
    return this.getFormat(formattedDate);
  }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  private readonly formatDate = new Date(1900, 1, 31, 10, 20, 30);

  private getFormat(formattedDateTime: string): string {
    return formattedDateTime
      .replace('1900', 'yyyy')
      .replace('01', 'MM')
      .replace('31', 'dd')
      .replace('10', 'HH')
      .replace('20', 'mm')
      .replace('30', 'ss');
  }
}
