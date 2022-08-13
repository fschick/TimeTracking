import {TestBed} from '@angular/core/testing';
import validationDescriptions from './dto-validations.spec.json';

import {FormValidationService} from './form-validation.service';
import {CustomerDto} from '../../../../api/timetracking';
import {FormControl} from '@angular/forms';
import {CoreModule} from '../../core.module';

describe('FormValidationService', () => {
  let service: FormValidationService;

  beforeEach(() => {
    TestBed.configureTestingModule({imports: [CoreModule]});
    service = TestBed.inject(FormValidationService);
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
