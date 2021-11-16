import {ComponentFixture, fakeAsync, TestBed, tick} from '@angular/core/testing';
import { MasterDataHolidaysEditComponent } from './master-data-holidays-edit.component';
import {Observable, of} from 'rxjs';
import {CustomerService, HolidayService} from '../../../shared/services/api';
import {MasterDataCustomersEditComponent} from '../master-data-customers-edit/master-data-customers-edit.component';
import {Router} from '@angular/router';
import {Component} from '@angular/core';
import {HttpClientModule} from '@angular/common/http';
import {NgSelectModule} from '@ng-select/ng-select';
import {ReactiveFormsModule} from '@angular/forms';
import {RouterTestingModule} from '@angular/router/testing';

const fakeHolidayService = {
  get: (): Observable<any> => of({})
} as Partial<HolidayService>;

@Component({
  template: '<router-outlet></router-outlet>',
})
class TestRootComponent {
}

describe('MasterDataHolidaysEditComponent', () => {
  let component: MasterDataHolidaysEditComponent;
  let rootFixture: ComponentFixture<TestRootComponent>;
  let fixture: ComponentFixture<MasterDataHolidaysEditComponent>;
  let router: Router;

  const navigateById = (id: string) => {
    rootFixture.ngZone?.run(() => router.navigate([id])); // [1] [2]
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TestRootComponent, MasterDataHolidaysEditComponent],
      imports: [
        HttpClientModule,
        NgSelectModule,
        ReactiveFormsModule,
        RouterTestingModule.withRoutes([
          {path: ':id', component: MasterDataHolidaysEditComponent}
        ])
      ],
      providers: [{provide: HolidayService, useValue: fakeHolidayService}],
    });

    await TestBed.compileComponents();
    rootFixture = TestBed.createComponent(TestRootComponent);
    router = TestBed.inject(Router);
  });

  beforeEach(fakeAsync(() => {
    navigateById('efb9854c-38e5-4a8f-aad7-71f1b2a483c3');
    tick();
    fixture = TestBed.createComponent(MasterDataHolidaysEditComponent);
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
