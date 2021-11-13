import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportOrdersComponent } from './report-orders.component';

describe('ReportOrdersComponent', () => {
  let component: ReportOrdersComponent;
  let fixture: ComponentFixture<ReportOrdersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReportOrdersComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportOrdersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
