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
import {FormSubmitDirective} from './directives/form-submit.directive';
import {NumericDirective} from './directives/numeric.directive';
import {DatePickerDirective} from './directives/date-picker.directive';
import {TimeDirective} from './directives/time.directive';
import {RouterLinkCtrlClickDirective} from './directives/router-link-ctrl-click.directive';
import {OptionalLabelDirective} from './directives/optional-label.directive';
import {DateParserService} from './services/date-parser.service';

const services = [
  LocalizationService,
  StorageService,
  EntityService,
  GuidService,
  FormatService,
  UtilityService,
  FormValidationService,
  EnumTranslationService,
  DateParserService,
]

const pipes = [
  DatePipe,
  DurationPipe,
  TimePipe,
];

const directives = [
  FormSubmitDirective,
  NumericDirective,
  DatePickerDirective,
  TimeDirective,
  RouterLinkCtrlClickDirective,
  OptionalLabelDirective,
]

@NgModule({
  declarations: [
    ...pipes,
    ...directives
  ],
  imports: [],
  providers: [
    // Provide services explicit, not via Injectable/providedIn. See
    // https://github.com/NativeScript/NativeScript/issues/8982#issuecomment-716460053 and
    // https://github.com/angular/angular-cli/issues/10170#issuecomment-415758304 and
    // https://angular.io/guide/providers#providedin-and-ngmodules
    ...services,
    ...pipes
  ],
  exports: [
    ...pipes,
    ...directives
  ],
})
export class CoreModule {
}
