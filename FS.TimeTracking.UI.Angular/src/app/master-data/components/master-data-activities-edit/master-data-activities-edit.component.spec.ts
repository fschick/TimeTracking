import {Component} from '@angular/core';
import {ActivityService, TypeaheadService} from '../../../shared/services/api';
import {Observable, of} from 'rxjs';
import {MasterDataActivitiesEditComponent} from './master-data-activities-edit.component';
import {ComponentFixture, fakeAsync, TestBed, tick} from '@angular/core/testing';
import {Router} from '@angular/router';
import {HttpClientModule} from '@angular/common/http';
import {RouterTestingModule} from '@angular/router/testing';
import {DialogModule} from '@ngneat/dialog';
import {ReactiveComponentModule} from '@ngrx/component';

const fakeActivityService = {
  get: (): Observable<any> => of({})
} as Partial<ActivityService>;

const fakeTypeaheadService = {
  getCustomers: (): Observable<any> => of({}),
  getProjects: (): Observable<any> => of({})
} as Partial<TypeaheadService>;

@Component({
  template: '<router-outlet></router-outlet>',
})
class TestRootComponent {
}

describe('MasterDataActivitiesEditComponent', () => {
  let component: MasterDataActivitiesEditComponent;
  let rootFixture: ComponentFixture<TestRootComponent>;
  let fixture: ComponentFixture<MasterDataActivitiesEditComponent>;
  let router: Router;

  const navigateById = (id: string) => {
    rootFixture.ngZone?.run(() => router.navigate([id])); // [1] [2]
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TestRootComponent, MasterDataActivitiesEditComponent],
      imports: [
        HttpClientModule,
        ReactiveComponentModule,
        RouterTestingModule.withRoutes([
          {path: ':id', component: MasterDataActivitiesEditComponent}
        ]),
        DialogModule.forRoot({
          sizes: {inherit: {}}
        })
      ],
      providers: [
        {provide: ActivityService, useValue: fakeActivityService},
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
    fixture = TestBed.createComponent(MasterDataActivitiesEditComponent);
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
