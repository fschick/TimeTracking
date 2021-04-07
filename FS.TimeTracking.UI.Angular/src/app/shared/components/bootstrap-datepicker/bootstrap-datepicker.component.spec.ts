import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BootstrapDatepickerComponent } from './bootstrap-datepicker.component';

describe('BootstrapDatepickerComponent', () => {
  let component: BootstrapDatepickerComponent;
  let fixture: ComponentFixture<BootstrapDatepickerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BootstrapDatepickerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BootstrapDatepickerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
