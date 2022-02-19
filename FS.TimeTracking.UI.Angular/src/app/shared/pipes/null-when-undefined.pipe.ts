import {Pipe, PipeTransform} from '@angular/core';

//TODO: Remove as soon as https://github.com/ng-select/ng-select/pull/2045 is released.
@Pipe({
  name: 'tsNullWhenUndefined'
})
export class NullWhenUndefinedPipe implements PipeTransform {

  transform<T>(value: T | null | undefined): T | null {
    return value != null ? value : null;
  }

}
