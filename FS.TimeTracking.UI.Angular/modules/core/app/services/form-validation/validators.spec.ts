import {Validators} from './validators';
import {UntypedFormControl, UntypedFormGroup} from '@angular/forms';

describe('Validators', () => {
  it('compare should return no error when fields are empty', () => {
    const form = new UntypedFormGroup({fieldA: new UntypedFormControl(), fieldB: new UntypedFormControl()});
    const compare = Validators.compare('fieldA', 'fieldB');
    const validationResult = compare(form);
    expect(validationResult).toBeNull();
  });

  it('compare should return no error when fields are equal', () => {
    const form = new UntypedFormGroup({fieldA: new UntypedFormControl('A'), fieldB: new UntypedFormControl('A')});
    const compare = Validators.compare('fieldA', 'fieldB');
    const validationResult = compare(form);
    expect(validationResult).toBeNull();
  });

  it('compare should return error when fields are different', () => {
    const form = new UntypedFormGroup({fieldA: new UntypedFormControl('A'), fieldB: new UntypedFormControl('B')});
    const compare = Validators.compare('fieldA', 'fieldB');
    const validationResult = compare(form);
    expect(validationResult).toEqual({
      compare: {
        fieldsDoesNotMatch: true,
        fieldName: 'fieldA',
        otherFieldName: 'fieldB'
      }
    });
  });
});
