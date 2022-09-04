import {FormSubmitDirective} from './form-submit.directive';
import {Component} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {By} from '@angular/platform-browser';
import {FormGroup, FormGroupDirective, FormsModule, ReactiveFormsModule} from '@angular/forms';

@Component({
  template: `
    <form [formGroup]="testForm"></form>
  `
})
class TestComponent {
  public testForm = new FormGroup({});
}

describe('FormSubmitDirective', () => {
  let fixture: ComponentFixture<TestComponent>;

  beforeEach(() => {
    fixture = TestBed.configureTestingModule({
      imports: [FormsModule, ReactiveFormsModule],
      declarations: [TestComponent, FormSubmitDirective],
      providers: [FormGroupDirective]
    })
      .createComponent(TestComponent);

    fixture.detectChanges(); // initial binding
  });

  it('should add/remove css class ng-submitted to form element when form is submitted/reset ', () => {
    const form = fixture.debugElement.query(By.directive(FormSubmitDirective));
    const ngForm = form.injector.get(FormGroupDirective) as FormGroupDirective;

    expect(form.classes['ng-submitted']).toBeUndefined();

    ngForm.ngSubmit.emit();
    fixture.detectChanges();
    expect(form.classes['ng-submitted']).toBeTrue();

    ngForm.resetForm();
    fixture.detectChanges();
    expect(form.classes['ng-submitted']).toBeUndefined();
  });
});
