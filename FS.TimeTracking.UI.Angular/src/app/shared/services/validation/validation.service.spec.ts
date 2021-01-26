import {TestBed} from '@angular/core/testing';
import validationDescriptions from './validation.spec.json';

import {ValidationService} from './validation.service';
import {CustomerDto} from '../api';
import {FormControl} from '@angular/forms';

describe('ValidationService', () => {
  let service: ValidationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ValidationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getFormGroup should return expected form controls', () => {
    const formGroup = service.getFormGroup<CustomerDto>('CustomerDto');
    const createdFormControlNames = Object.keys(formGroup.controls);
    const expectedFormControlNames = Object.keys(validationDescriptions.CustomerDto);
    expect(createdFormControlNames).toEqual(expectedFormControlNames);
  });

  it('getFormGroup should initialize controls with given values', () => {
    const formGroup = service.getFormGroup<CustomerDto>('CustomerDto', {companyName: 'TestCompany'});
    const initialValue = formGroup.get('companyName')?.value;
    expect(initialValue).toEqual('TestCompany');
  });

  it('getFormGroup should give additional controls precedence over generated controls', () => {
    const customCompanyControl = {companyName: new FormControl('TestCompanyOverride')};
    const formGroup = service.getFormGroup<CustomerDto>('CustomerDto', {companyName: 'TestCompany'}, customCompanyControl);
    const initialValue = formGroup.get('companyName')?.value;
    expect(initialValue).toEqual('TestCompanyOverride');
  });
});
