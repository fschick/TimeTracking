import {Component} from '@angular/core';
import {ProjectService, TypeaheadGetProjectsRequestParams, TypeaheadService} from '../../../shared/services/api';
import {Observable, of} from 'rxjs';
import {MasterDataProjectsEditComponent} from './master-data-projects-edit.component';
import {ComponentFixture, fakeAsync, TestBed, tick} from '@angular/core/testing';
import {Router} from '@angular/router';
import {HttpClientModule} from '@angular/common/http';
import {RouterTestingModule} from '@angular/router/testing';
import {ReactiveComponentModule} from '@ngrx/component';
import {ReactiveFormsModule} from '@angular/forms';
import {NgSelectModule} from '@ng-select/ng-select';

const fakeProjectService = {
  get: (): Observable<any> => of({})
} as Partial<ProjectService>;

const fakeTypeaheadService = {
  getCustomers: (_: TypeaheadGetProjectsRequestParams): Observable<any[]> => of([])
} as Partial<TypeaheadService>;

@Component({
  template: '<router-outlet></router-outlet>',
})
class TestRootComponent {
}

describe('MasterDataProjectsEditComponent', () => {
  let component: MasterDataProjectsEditComponent;
  let rootFixture: ComponentFixture<TestRootComponent>;
  let fixture: ComponentFixture<MasterDataProjectsEditComponent>;
  let router: Router;

  const navigateById = (id: string) => {
    rootFixture.ngZone?.run(() => router.navigate([id])); // [1] [2]
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TestRootComponent, MasterDataProjectsEditComponent],
      imports: [
        HttpClientModule,
        NgSelectModule,
        ReactiveFormsModule,
        ReactiveComponentModule,
        RouterTestingModule.withRoutes([
          {path: ':id', component: MasterDataProjectsEditComponent}
        ])
      ],
      providers: [
        {provide: ProjectService, useValue: fakeProjectService},
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
    fixture = TestBed.createComponent(MasterDataProjectsEditComponent);
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
