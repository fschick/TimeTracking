import {ComponentFixture, TestBed} from '@angular/core/testing';

import {FormValidationErrorsComponent} from './form-validation-errors.component';
import {FormGroupDirective} from '@angular/forms';

describe('FormValidationErrorsComponent', () => {
  let component: FormValidationErrorsComponent;
  let fixture: ComponentFixture<FormValidationErrorsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [FormValidationErrorsComponent],
      providers: [FormGroupDirective]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FormValidationErrorsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
