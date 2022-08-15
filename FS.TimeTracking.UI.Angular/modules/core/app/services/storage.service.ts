import {Injectable} from '@angular/core';

@Injectable()
export class StorageService {
  private readonly storagePrefix = 'TimeTracking.';

  public get(key: string, defaultValue: string) {
    return localStorage.getItem(this.storagePrefix + key) ?? defaultValue;
  }

  public set(key: string, value: string) {
    localStorage.setItem(this.storagePrefix + key, value);
  }

  public remove(key: string): void {
    localStorage.removeItem(this.storagePrefix + key);
  }
}
