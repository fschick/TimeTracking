import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportActivitiesComponent } from './report-activities.component';

describe('ReportActivitiesComponent', () => {
  let component: ReportActivitiesComponent;
  let fixture: ComponentFixture<ReportActivitiesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReportActivitiesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportActivitiesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
