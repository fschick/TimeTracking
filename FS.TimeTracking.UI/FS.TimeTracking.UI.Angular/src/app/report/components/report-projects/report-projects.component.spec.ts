import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportProjectsComponent } from './report-projects.component';

describe('ReportProjectsComponent', () => {
  let component: ReportProjectsComponent;
  let fixture: ComponentFixture<ReportProjectsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReportProjectsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportProjectsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
