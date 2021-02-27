import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ConfirmButtonComponent} from './confirm-button.component';
import {ReactiveComponentModule} from '@ngrx/component';

describe('ConfirmButtonComponent', () => {
  let component: ConfirmButtonComponent;
  let fixture: ComponentFixture<ConfirmButtonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ConfirmButtonComponent],
      imports: [ReactiveComponentModule],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfirmButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
