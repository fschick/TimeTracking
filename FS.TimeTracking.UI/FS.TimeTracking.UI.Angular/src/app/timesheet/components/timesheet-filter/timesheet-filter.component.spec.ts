import {ComponentFixture, TestBed} from '@angular/core/testing';
import {TimesheetFilterComponent} from './timesheet-filter.component';
import {HttpClientModule} from '@angular/common/http';
import {RouterTestingModule} from '@angular/router/testing';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {ReactiveComponentModule} from '@ngrx/component';
import {NgSelectModule} from '@ng-select/ng-select';

describe('TimesheetFilterComponent', () => {
  let component: TimesheetFilterComponent;
  let fixture: ComponentFixture<TimesheetFilterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientModule, RouterTestingModule, FormsModule, ReactiveFormsModule, ReactiveComponentModule, NgSelectModule],
      declarations: [TimesheetFilterComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TimesheetFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
