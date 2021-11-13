import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportCustomersComponent } from './report-customers.component';

describe('ReportCustomersComponent', () => {
  let component: ReportCustomersComponent;
  let fixture: ComponentFixture<ReportCustomersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReportCustomersComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportCustomersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
