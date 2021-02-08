import {AbstractControl, ValidationErrors, ValidatorFn} from '@angular/forms';

export class Validators {
  public static compare<TType>(fieldName: keyof TType, otherFieldName: keyof TType): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const field = control.get(fieldName as string);
      const otherField = control.get(otherFieldName as string);

      if (field && otherField && field.value !== otherField.value) {
        return {
          compare: {
            fieldsDoesNotMatch: true,
            fieldName,
            otherFieldName
          }
        };
      }

      return null;
    };
  }
}
