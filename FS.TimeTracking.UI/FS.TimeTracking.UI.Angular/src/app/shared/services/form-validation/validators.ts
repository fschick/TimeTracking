import {AbstractControl, ValidationErrors, ValidatorFn} from '@angular/forms';

export type ComparisonType = 'equal' | 'notEqual' | 'lessThan' | 'lessThanOrEqual' | 'greaterThan' | 'greaterThanOrEqual';

export class Validators {
  public static compare<TType>(fieldName: keyof TType, otherFieldName: keyof TType): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const field = control.get(fieldName as string);
      const otherField = control.get(otherFieldName as string);

      if (field && otherField && field.value !== otherField.value) {
        return {
          compare: {
            fieldName: fieldName,
            otherFieldName: otherFieldName,
            fieldsDoesNotMatch: true,
          }
        };
      }

      return null;
    };
  }

  public static compareTo<TType>(fieldName: keyof TType, otherFieldName: keyof TType, comparisonType: ComparisonType): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const field = control.get(fieldName as string);
      const otherField = control.get(otherFieldName as string);

      if (!field || !otherField || !field.value || !otherField.value)
        return null;

      let isValid: boolean;
      switch (comparisonType) {
        case 'equal':
          isValid = field.value === otherField.value;
          break;
        case 'notEqual':
          isValid = field.value !== otherField.value;
          break;
        case 'lessThan':
          isValid = field.value < otherField.value;
          break;
        case 'lessThanOrEqual':
          isValid = field.value <= otherField.value;
          break;
        case 'greaterThan':
          isValid = field.value > otherField.value;
          break;
        case 'greaterThanOrEqual':
          isValid = field.value >= otherField.value;
          break;
        default:
          isValid = true;
          break;
      }

      if (isValid)
        return null;

      return {
        compareTo: {
          fieldName,
          otherFieldName,
          comparisonType,
        }
      };
    };
  }
}
