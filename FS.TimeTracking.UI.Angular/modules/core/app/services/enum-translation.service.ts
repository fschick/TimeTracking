import {Injectable} from '@angular/core';
import {UtilityService} from './utility.service';
import {$localizeId} from './internationalization/localizeId';
import {CoreModule} from '../core.module';

@Injectable()
export class EnumTranslationService {

  constructor(
    private utilityService: UtilityService,
  ) { }

  public translate<TEnum>(enumName: string, member: Extract<keyof TEnum, string>): string {
    const enumMemberName = this.utilityService.capitalize(member);
    const enumTransUnitId = `@@Enum.${enumName}.${enumMemberName}`;
    return $localizeId`${enumTransUnitId}:TRANSUNITID:`;
  }
}
