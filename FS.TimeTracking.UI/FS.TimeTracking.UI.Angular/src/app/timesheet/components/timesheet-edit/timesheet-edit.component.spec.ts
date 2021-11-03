import {ComponentFixture, fakeAsync, TestBed, tick} from '@angular/core/testing';
import {TimesheetEditComponent} from './timesheet-edit.component';
import {HttpClientModule} from '@angular/common/http';
import {RouterTestingModule} from '@angular/router/testing';
import {ReactiveFormsModule} from '@angular/forms';
import {ReactiveComponentModule} from '@ngrx/component';
import {NgSelectModule} from '@ng-select/ng-select';
import {Observable, of} from 'rxjs';
import {TimeSheetService, TypeaheadGetProjectsRequestParams, TypeaheadService} from '../../../shared/services/api';
import {Component} from '@angular/core';
import {Router} from '@angular/router';

const fakeTimeSheetService = {
  get: (): Observable<any> => of({})
} as Partial<TimeSheetService>;

const fakeTypeaheadService = {
  getCustomers: (_: TypeaheadGetProjectsRequestParams): Observable<any[]> => of([]),
  getProjects: (_: TypeaheadGetProjectsRequestParams): Observable<any[]> => of([]),
  getActivities: (_: TypeaheadGetProjectsRequestParams): Observable<any[]> => of([]),
  getOrders: (_: TypeaheadGetProjectsRequestParams): Observable<any[]> => of([]),
} as Partial<TypeaheadService>;

@Component({
  template: '<router-outlet></router-outlet>',
})
class TestRootComponent {
}

describe('TimesheetEditComponent', () => {
  let component: TimesheetEditComponent;
  let rootFixture: ComponentFixture<TestRootComponent>;
  let fixture: ComponentFixture<TimesheetEditComponent>;
  let router: Router;

  const navigateById = (id: string) => {
    rootFixture.ngZone?.run(() => router.navigate([id])); // [1] [2]
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TestRootComponent, TimesheetEditComponent],
      imports: [
        HttpClientModule,
        NgSelectModule,
        ReactiveFormsModule,
        ReactiveComponentModule,
        RouterTestingModule.withRoutes([
          {path: ':id', component: TimesheetEditComponent}
        ])
      ],
      providers: [
        {provide: TimeSheetService, useValue: fakeTimeSheetService},
        {provide: TypeaheadService, useValue: fakeTypeaheadService},
      ],
    });

    await TestBed.compileComponents();
    rootFixture = TestBed.createComponent(TestRootComponent);
    router = TestBed.inject(Router);
  });

  beforeEach(fakeAsync(() => {
    navigateById('efb9854c-38e5-4a8f-aad7-71f1b2a483c3');
    tick();
    fixture = TestBed.createComponent(TimesheetEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  afterEach(() => {
    component.close();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
