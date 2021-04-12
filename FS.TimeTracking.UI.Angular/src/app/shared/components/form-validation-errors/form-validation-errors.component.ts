import {Component, ElementRef, Input, OnInit, Optional, ViewChild} from '@angular/core';
import {FormGroupDirective} from '@angular/forms';
import {filter, map, tap} from 'rxjs/operators';
import {merge, Observable, of} from 'rxjs';
import {ValidationFormGroup} from '../../services/form-validation/form-validation.service';
import {$localizeId} from '../../services/internationalization/localizeId';

@Component({
  selector: 'ts-form-validation-errors',
  templateUrl: './form-validation-errors.component.html',
  styleUrls: ['./form-validation-errors.component.scss']
})
export class FormValidationErrorsComponent implements OnInit {
  @Input() @Optional() form!: FormGroupDirective;

  @ViewChild('container') container?: ElementRef;

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
      map(_ => this.getErrors()),
      tap(errors => {
        if (errors.length > 0)
          setTimeout(() => this.container?.nativeElement.scrollIntoView(), 0);
      })
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
    const validationErrorConverter = this.validationErrorConverters[validation];
    if (validationErrorConverter) {
      return validationErrorConverter(fieldName, error);
    } else {
      console.error(`No error converter for validation '${validation}' found.`);
      return $localize`:@@Validation.Common.InvalidWithField:'${fieldName}:FIELDNAME:' is invalid.`;
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

  // eslint-disable-next-line @typescript-eslint/member-ordering
  private validationErrorConverters: IValidationErrorConverters = {
    // eslint-disable-next-line max-len
    required: (fieldName, _) => $localize`:@@Validation.RequiredWithField:[I18N] The field '${fieldName}:FIELDNAME:' is required`,
    minlength: (fieldName, error) => $localize`:@@Validation.MinLengthWithField:[I18N] '${fieldName}:FIELDNAME:' must be at least ${error.requiredLength}:VALUE: characters`,
    maxlength: (fieldName, error) => $localize`:@@Validation.MaxLengthWithField:[I18N] '${fieldName}:FIELDNAME:' must not have more than ${error.requiredLength}:VALUE: characters`,
    min: (fieldName, error) => $localize`:@@Validation.MinWithField:[I18N] '${fieldName}:FIELDNAME:' must be greater or equal to ${error.min}:VALUE:`,
    max: (fieldName, error) => $localize`:@@Validation.MaxWithField:[I18N] '${fieldName}:FIELDNAME:' must be lower or equal to ${error.max}:VALUE:`,
    compare: (_, error) => $localize`:@@Validation.CompareWithField:[I18N] '${this.translateFieldName(error.fieldName)}:FIELDNAME:' and '${this.translateFieldName(error.otherFieldName)}:OTHERFIELDNAME:' does not match`,
    compareTo: (_, error) => this.getCompareToErrorMessage(error),
  };

  private getCompareToErrorMessage = (error: any): string => {
    const fieldName = this.translateFieldName(error.fieldName);
    const otherFieldName = this.translateFieldName(error.otherFieldName);
    switch (error.comparisonType) {
      case 'equal':
        return $localize`:@@Validation.CompareToEqualWithField:[I18N] '${fieldName}:FIELDNAME:' and '${otherFieldName}:OTHERFIELDNAME:' must be equal`;
      case 'notEqual':
        return $localize`:@@Validation.CompareToNotEqualWithField:[I18N] '${fieldName}:FIELDNAME:' and '${otherFieldName}:OTHERFIELDNAME:' must not be equal`;
      case 'lessThan':
        return $localize`:@@Validation.CompareToLessThanWithField:[I18N] '${fieldName}:FIELDNAME:' must be less than '${otherFieldName}:OTHERFIELDNAME:'`;
      case 'lessThanOrEqual':
        return $localize`:@@Validation.CompareToLessThanOrEqualWithField:[I18N] '${fieldName}:FIELDNAME:' must be less or equal than '${otherFieldName}:OTHERFIELDNAME:'`;
      case 'greaterThan':
        return $localize`:@@Validation.CompareToGreaterThanWithField:[I18N] '${fieldName}:FIELDNAME:' must be grater than '${otherFieldName}:OTHERFIELDNAME:'`;
      case 'greaterThanOrEqual':
        return $localize`:@@Validation.CompareToGreaterThanOrEqualWithField:[I18N] '${fieldName}:FIELDNAME:' must be greater or equal than '${otherFieldName}:OTHERFIELDNAME:'`;
      default:
        console.error(`No error converter for validation 'compareTo/${error.comparisonType}' found.`);
        return $localize`:@@Validation.Common.InvalidWithField:'${fieldName + '/' + otherFieldName}:FIELDNAME:' is invalid.`;
    }
  };
}

type IValidationErrorConverters = {
  [validator in string]: (fieldName: string, error: any) => string;
};

