import {Injectable} from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  private readonly storagePrefix = 'TimeTracking.';
  private readonly languageKey = this.storagePrefix + 'language';

  constructor() {
  }

  public get language(): string {
    return localStorage.getItem(this.languageKey) ?? navigator.languages[0].substring(0, 2);
  }

  public set language(value: string) {
    localStorage.setItem(this.languageKey, value);
  }
}
