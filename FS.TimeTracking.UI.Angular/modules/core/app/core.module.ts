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
import {FormValidationErrorsComponent} from './components/form-validation-errors/form-validation-errors.component';
import {SimpleTableComponent} from './components/simple-table/simple-table.component';
import {SimpleConfirmComponent} from './components/simple-confirm/simple-confirm.component';
import {TimesheetFilterComponent} from './components/filter/filter.component';
import {DateMonthPickerComponent} from './components/date-month-picker/date-month-picker.component';
import {ClearableInputComponent} from './components/clearable-input/clearable-input.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {BrowserModule} from '@angular/platform-browser';
import {NgbCollapseModule} from '@ng-bootstrap/ng-bootstrap';
import {NgSelectModule} from '@ng-select/ng-select';

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
];

const components = [
  FormValidationErrorsComponent,
  SimpleTableComponent,
  SimpleConfirmComponent,
  TimesheetFilterComponent,
  DateMonthPickerComponent,
  ClearableInputComponent,

];

@NgModule({
  declarations: [
    ...pipes,
    ...directives,
    ...components,
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    NgSelectModule,
    NgbCollapseModule,
  ],
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
    ...directives,
    ...components,
  ],
})
export class CoreModule {
}
