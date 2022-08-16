import {Injectable} from '@angular/core';
import {CoreModule} from '../../core.module';

@Injectable({
  providedIn: CoreModule
})
export class GuidService {
  public static guidEmpty = '00000000-0000-0000-0000-000000000000';

  // // See https://stackoverflow.com/a/2117523/1271211
  public static newGuid(): string {
    // @ts-ignore
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
      // eslint-disable-next-line no-bitwise
      (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
  }
}
