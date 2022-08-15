import {NgModule} from '@angular/core';
import {TimePipe} from './pipes/time.pipe';
import {DurationPipe} from './pipes/duration.pipe';
import {DatePipe} from './pipes/date.pipe';
import {LocalizationService} from './services/internationalization/localization.service';
import {StorageService} from './services/storage.service';
import {EntityService} from './services/state-management/entity.service';
import {FormatService} from './services/format.service';
import {UtilityService} from './services/utility.service';
import {FormValidationService} from './services/form-validation/form-validation.service';
import {EnumTranslationService} from './services/enum-translation.service';
import {GuidService} from './services/state-management/guid.service';

const pipes = [
  DatePipe,
  DurationPipe,
  TimePipe,
];

const services = [
  LocalizationService,
  StorageService,
  EntityService,
  GuidService,
  FormatService,
  UtilityService,
  FormValidationService,
  EnumTranslationService,
]

@NgModule({
  declarations: [
    ...pipes
  ],
  imports: [],
  providers: [
    ...services,
    ...pipes
  ],
  exports: [
    ...pipes
  ],
})
export class CoreModule {
}
