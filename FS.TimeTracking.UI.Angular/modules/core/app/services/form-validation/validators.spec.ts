import {Validators} from './validators';
import {FormControl, FormGroup} from '@angular/forms';

describe('Validators', () => {
  it('compare should return no error when fields are empty', () => {
    const form = new FormGroup({fieldA: new FormControl(), fieldB: new FormControl()});
    const compare = Validators.compare('fieldA', 'fieldB');
    const validationResult = compare(form);
    expect(validationResult).toBeNull();
  });

  it('compare should return no error when fields are equal', () => {
    const form = new FormGroup({fieldA: new FormControl('A'), fieldB: new FormControl('A')});
    const compare = Validators.compare('fieldA', 'fieldB');
    const validationResult = compare(form);
    expect(validationResult).toBeNull();
  });

  it('compare should return error when fields are different', () => {
    const form = new FormGroup({fieldA: new FormControl('A'), fieldB: new FormControl('B')});
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
