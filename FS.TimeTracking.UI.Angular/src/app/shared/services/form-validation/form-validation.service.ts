import {Injectable} from '@angular/core';
import {
  AbstractControl,
  AbstractControlOptions, AsyncValidatorFn,
  FormControl,
  FormGroup,
  ValidatorFn,
  Validators as AngularValidators
} from '@angular/forms';
import validationDescriptions from './dto-validations.spec.json';
import {Validators as CustomValidators} from './validators';

type ValidationDescription = { [key: string]: any; type: string };
type TypeValidationFromControls<TType> = { [key in keyof TType]: AbstractControl; };

export type ValidationFromControls = { [key: string]: AbstractControl | ValidationFromControls };

export class ValidationFormGroup extends FormGroup {
  constructor(
    public typeName: keyof typeof validationDescriptions,
    controls: { [key: string]: AbstractControl },
    validatorOrOpts?: ValidatorFn | ValidatorFn[] | AbstractControlOptions | null,
    asyncValidator?: AsyncValidatorFn | AsyncValidatorFn[] | null
  ) {
    super(controls, validatorOrOpts, asyncValidator);
  }
}

@Injectable({
  providedIn: 'root'
})
export class FormValidationService {

  public getFormGroup<TType>(
    typeName: keyof typeof validationDescriptions,
    initialValues: Partial<TType> = {},
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
    initialValues: Partial<TType>
  ): Partial<TypeValidationFromControls<TType>> {
    const typeValidations = validationDescriptions[typeName];
    const validationFormControls: Partial<TypeValidationFromControls<TType>> = {};

    const propertiesValidations = Object.entries(typeValidations) as unknown as [keyof TType, ValidationDescription[]][];
    for (const [fieldName, validations] of propertiesValidations) {
      const nestedTypeValidation = validations.find(x => x.type === 'nested');
      validationFormControls[fieldName] = nestedTypeValidation !== undefined
        ? this.getNestedFormGroup(fieldName, nestedTypeValidation, additionalFormControls, initialValues)
        : this.getFormControl(fieldName, validations, initialValues);
    }

    return validationFormControls;
  }

  private getNestedFormGroup<TType>(
    fieldName: keyof TType,
    validation: ValidationDescription,
    formControls: ValidationFromControls,
    initialValues: Partial<TType>
  ) {
    const typeName = validation.class as keyof typeof validationDescriptions;
    const nestedFormControlField = formControls[typeName];
    const additionalFormControls = this.isCustomValidationFromControls(nestedFormControlField) ? nestedFormControlField : {};
    return this.getFormGroup(typeName, initialValues[fieldName], additionalFormControls);
  }

  private isCustomValidationFromControls(
    formControls: AbstractControl | ValidationFromControls
  ): formControls is ValidationFromControls {
    return formControls !== undefined && !(formControls instanceof AbstractControl);
  }

  private getFormControl<TType>(
    fieldName: keyof TType,
    validations: ValidationDescription[],
    initialValues: Partial<TType>
  ) {
    const initialValue = initialValues[fieldName];
    const validators = validations.flatMap(validation => this.getFieldValidators(validation));
    return new FormControl(initialValue, validators);
  }

  private getFieldValidators(validation: ValidationDescription): ValidatorFn[] {
    switch (validation.type) {
      case 'required':
        return [AngularValidators.required];
      case 'length':
        return this.getLengthValidators(validation);
      case 'range':
        return this.getRangeValidators(validation);
      default:
        return [];
    }
  }

  private getLengthValidators(validation: ValidationDescription): ValidatorFn[] {
    const lengthValidators: ValidatorFn[] = [];
    if (validation.min !== undefined)
      lengthValidators.push(AngularValidators.minLength(validation.min));
    if (validation.max !== undefined)
      lengthValidators.push(AngularValidators.maxLength(validation.max));
    return lengthValidators;
  }

  private getRangeValidators(validation: ValidationDescription): ValidatorFn[] {
    const rangeValidators: ValidatorFn[] = [];
    if (validation.min !== undefined)
      rangeValidators.push(AngularValidators.min(validation.min));
    if (validation.max !== undefined)
      rangeValidators.push(AngularValidators.max(validation.max));
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
        return [CustomValidators.compare(fieldName, validation.otherProperty)];
      default:
        return [];
    }
  }
}
