import {Component, Input, OnInit, Optional} from '@angular/core';
import {FormGroupDirective} from '@angular/forms';
import {filter, map} from 'rxjs/operators';
import {merge, Observable, of} from 'rxjs';
import {ValidationFormGroup} from '../../services/validation/validation.service';
import {$localizeId} from '../../services/validation/localizeId';

@Component({
  selector: 'ts-form-validation-errors',
  templateUrl: './form-validation-errors.component.html',
  styleUrls: ['./form-validation-errors.component.scss']
})
export class FormValidationErrorsComponent implements OnInit {
  @Input() @Optional() form!: FormGroupDirective;

  public errors$!: Observable<string[]>;
  private validationFormGroup!: ValidationFormGroup;

  constructor(
    @Optional() private ngForm: FormGroupDirective
  ) {
  }

  ngOnInit(): void {
    this.ngForm = this.ngForm ?? this.form;

    if (!this.ngForm)
      throw new Error('<ts-form-validation-errors> must be used inside a <form> element or input [form] must be provided');

    this.validationFormGroup = this.ngForm.form as ValidationFormGroup;

    const ngSubmit: Observable<string[]> = this.ngForm.ngSubmit.pipe(
      map(_ => this.getErrors())
    );

    const statusChanges = !this.ngForm.statusChanges
      ? of([])
      : this.ngForm.statusChanges.pipe(
        filter(_ => this.ngForm?.submitted === true),
        map(_ => this.getErrors())
      );

    const ngReset = !this.ngForm.statusChanges
      ? of([])
      : this.ngForm.statusChanges.pipe(
        filter(_ => this.ngForm?.submitted !== true),
        map(_ => [])
      );

    this.errors$ = merge(ngSubmit, statusChanges, ngReset);
  }

  private getErrors(): string[] {
    const controlErrors = this.ngForm
      .directives
      .flatMap(directive => {
        const fieldName = this.translateFieldName(directive.name as string);

        return Object
          .entries(directive.errors ?? [])
          .map(([validation, error]) => this.translateError((fieldName), validation, error));
      });

    const formErrors = Object
      .entries(this.ngForm.errors ?? [])
      .map(([validation, error]) => this.translateError('UNUSED', validation, error));

    return [...controlErrors, ...formErrors];
  }

  private translateError(fieldName: string, validation: string, error: any): string {
    const validationErrorConverter = validationErrorConverters[validation];
    if (validationErrorConverter) {
      return validationErrorConverter(fieldName, error);
    } else {
      console.error(`No error converter for validation '${validation}' found.`);
      return $localize`:@@Validations.Common.InvalidWithField:'${fieldName}:FIELDNAME:' is invalid.`;
    }
  }

  private translateFieldName(fieldName: string): string {
    const fieldNameTransUnitId = `@@DTO.${(this.validationFormGroup.typeName)}.${this.capitalize(fieldName)}`;
    const translatedFieldName = $localizeId`${fieldNameTransUnitId}:TRANSUNITID:`;
    return translatedFieldName !== '' ? translatedFieldName : fieldName;
  }

  private capitalize(value: string): string {
    return value[0].toUpperCase() + value.slice(1);
  }
}

type IValidationErrorConverters = {
  [validator in string]: (fieldName: string, error: any) => string;
};

const validationErrorConverters: IValidationErrorConverters = {
  required: (fieldName, error) => $localize`:@@Validations.RequiredWithField:[I18N] The field '${fieldName}:FIELDNAME:' is required`,
  // eslint-disable-next-line max-len
  minlength: (fieldName, error) => $localize`:@@Validations.MinLengthWithField:[I18N] '${fieldName}:FIELDNAME:' must be at least ${error.requiredLength}:VALUE: characters`,
  maxlength: (fieldName, error) => $localize`:@@Validations.MaxLengthWithField:[I18N] '${fieldName}:FIELDNAME:' must not have more than ${error.requiredLength}:VALUE: characters`,
  min: (fieldName, error) => $localize`:@@Validations.MinWithField:[I18N] '${fieldName}:FIELDNAME:' must be greater or equal to ${error.min}:VALUE:`,
  max: (fieldName, error) => $localize`:@@Validations.MaxWithField:[I18N] '${fieldName}:FIELDNAME:' must be lower or equal to ${error.max}:VALUE:`,
  compare: (fieldName, error) => $localize`:@@Validations.CompareWithField:[I18N] '${error.fieldName}:FIELDNAME:' and '${error.otherFieldName}:OTHERFIELDNAME:' does not match`,
};


