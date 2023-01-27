import {Injectable} from '@angular/core';
import {
  AbstractControl,
  AbstractControlOptions,
  AsyncValidatorFn,
  FormArray,
  FormControl,
  FormGroup,
  ValidatorFn,
  Validators as AngularValidators
} from '@angular/forms';
import validationDescriptions from './dto-validations.spec.json';
import {Validators as CustomValidators} from './validators';

type ValidationDescription = { [key: string]: any; type: string };
type TypeValidationFromControls<TType> = { [key in keyof TType]: AbstractControl; };

export type ValidationFromControls = { [key: string]: AbstractControl };

export class ValidationFormGroup extends FormGroup {
  constructor(
    public typeName: keyof typeof validationDescriptions | string,
    controls: ValidationFromControls,
    validatorOrOpts?: ValidatorFn | ValidatorFn[] | AbstractControlOptions | null,
    asyncValidator?: AsyncValidatorFn | AsyncValidatorFn[] | null
  ) {
    super(controls, validatorOrOpts, asyncValidator);
  }
}

@Injectable()
export class FormValidationService {

  public getFormGroup<TType>(
    typeName: keyof typeof validationDescriptions,
    initialValues?: Partial<TType>,
    additionalFormControls: ValidationFromControls = {},
    additionalFormValidators: ValidatorFn[] = []
  ): ValidationFormGroup {
    const generatedFormControls = this.getFormControls(typeName, additionalFormControls, initialValues);
    const formControls: TypeValidationFromControls<TType> = {...generatedFormControls, ...additionalFormControls};

    const generatedFormGroupValidators = this.getFormGroupValidators(typeName);
    const formValidators = [...generatedFormGroupValidators, ...additionalFormValidators];

    return new ValidationFormGroup(typeName, formControls, {validators: formValidators});
  }

  private getFormControls<TType>(
    typeName: keyof typeof validationDescriptions,
    additionalFormControls: ValidationFromControls,
    initialValues: Partial<TType> | undefined
  ): Partial<TypeValidationFromControls<TType>> {
    const typeValidations = validationDescriptions[typeName];
    const validationFormControls: Partial<TypeValidationFromControls<TType>> = {};

    const propertiesValidations = Object.entries(typeValidations) as unknown as [keyof TType, ValidationDescription[]][];
    for (const [fieldName, validations] of propertiesValidations) {
      const nestedTypeValidation = validations.find(x => x.type === 'nested');
      const nestedListValidation = validations.find(x => x.type === 'list');

      const initialValue = initialValues ? initialValues[fieldName] : undefined;
      if (nestedTypeValidation !== undefined) {
        validationFormControls[fieldName] = this.getNestedFormGroup(nestedTypeValidation, additionalFormControls, initialValue);
      } else if (nestedListValidation !== undefined) {
        const initialValueList = (initialValue ?? []) as Partial<TType>[keyof TType][];
        const nestedFormGroups = initialValueList.map(initialValue => this.getNestedFormGroup(nestedListValidation, additionalFormControls, initialValue));
        validationFormControls[fieldName] = new FormArray(nestedFormGroups);
      } else {
        validationFormControls[fieldName] = this.getFormControl(validations, initialValue);
      }
    }

    return validationFormControls;
  }

  private getNestedFormGroup<TType>(
    validation: ValidationDescription,
    formControls: ValidationFromControls,
    initialValue: Partial<TType>[keyof TType] | undefined
  ) {
    const typeName = validation['class'] as keyof typeof validationDescriptions;
    const nestedFormControlField = formControls[typeName];
    const additionalFormControls = this.isCustomValidationFromControls(nestedFormControlField) ? nestedFormControlField : {};
    return this.getFormGroup(typeName, initialValue ?? {}, additionalFormControls);
  }

  private isCustomValidationFromControls(
    formControls: AbstractControl | ValidationFromControls
  ): formControls is ValidationFromControls {
    return formControls !== undefined && !(formControls instanceof AbstractControl);
  }

  private getFormControl<TType>(
    validations: ValidationDescription[],
    initialValue: Partial<TType>[keyof TType] | undefined
  ) {
    const validators = validations.flatMap(validation => this.getFieldValidators(validation));
    return new FormControl(initialValue, validators);
  }

  private getFieldValidators(validation: ValidationDescription): ValidatorFn[] {
    switch (validation.type) {
      case 'required':
        return [AngularValidators.required];
      case 'email':
        return [AngularValidators.email];
      case 'length':
        return this.getFieldLengthValidators(validation);
      case 'range':
        return this.getFieldRangeValidators(validation);
      default:
        return [];
    }
  }

  private getFieldLengthValidators(validation: ValidationDescription): ValidatorFn[] {
    const lengthValidators: ValidatorFn[] = [];
    if (validation['min'] !== undefined)
      lengthValidators.push(AngularValidators.minLength(validation['min']));
    if (validation['max'] !== undefined)
      lengthValidators.push(AngularValidators.maxLength(validation['max']));
    return lengthValidators;
  }

  private getFieldRangeValidators(validation: ValidationDescription): ValidatorFn[] {
    const rangeValidators: ValidatorFn[] = [];
    if (validation['min'] !== undefined)
      rangeValidators.push(AngularValidators.min(validation['min']));
    if (validation['max'] !== undefined)
      rangeValidators.push(AngularValidators.max(validation['max']));
    return rangeValidators;
  }

  private getFormGroupValidators<TType>(typeName: keyof typeof validationDescriptions): ValidatorFn[] {
    const typeValidations = validationDescriptions[typeName];
    const formGroupValidators: ValidatorFn[] = [];

    const propertiesValidations = Object.entries(typeValidations) as unknown as [keyof TType, ValidationDescription[]][];
    for (const [fieldName, validations] of propertiesValidations) {
      const validators = validations.flatMap(validation => this.getFormGroupValidator(fieldName, validation));
      formGroupValidators.push(...validators);
    }

    return formGroupValidators;
  }

  private getFormGroupValidator<TType>(fieldName: keyof TType, validation: ValidationDescription): ValidatorFn[] {
    switch (validation.type) {
      case 'compare':
        return [CustomValidators.compare(fieldName, validation['otherProperty'])];
      case 'compareTo':
        return [CustomValidators.compareTo(fieldName, validation['otherProperty'], validation['comparisonType'])];
      default:
        return [];
    }
  }
}
