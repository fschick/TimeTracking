import {Component} from '@angular/core';
import {OrderService, TypeaheadService} from '../../../shared/services/api';
import {Observable, of} from 'rxjs';
import {MasterDataOrdersEditComponent} from './master-data-orders-edit.component';
import {ComponentFixture, fakeAsync, TestBed, tick} from '@angular/core/testing';
import {Router} from '@angular/router';
import {HttpClientModule} from '@angular/common/http';
import {RouterTestingModule} from '@angular/router/testing';
import {ReactiveComponentModule} from '@ngrx/component';
import {NgSelectModule} from '@ng-select/ng-select';
import {ReactiveFormsModule} from '@angular/forms';

const fakeOrderService = {
  get: (): Observable<any> => of({})
} as Partial<OrderService>;

const fakeTypeaheadService = {
  getCustomers: (): Observable<any[]> => of([])
} as Partial<TypeaheadService>;

@Component({
  template: '<router-outlet></router-outlet>',
})
class TestRootComponent {
}

describe('MasterDataOrderEditComponent', () => {
  let component: MasterDataOrdersEditComponent;
  let rootFixture: ComponentFixture<TestRootComponent>;
  let fixture: ComponentFixture<MasterDataOrdersEditComponent>;
  let router: Router;

  const navigateById = (id: string) => {
    rootFixture.ngZone?.run(() => router.navigate([id])); // [1] [2]
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [
        TestRootComponent,
        MasterDataOrdersEditComponent
      ],
      imports: [
        HttpClientModule,
        NgSelectModule,
        ReactiveFormsModule,
        ReactiveComponentModule,
        RouterTestingModule.withRoutes([
          {path: ':id', component: MasterDataOrdersEditComponent}
        ])
      ],
      providers: [
        {provide: OrderService, useValue: fakeOrderService},
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
    fixture = TestBed.createComponent(MasterDataOrdersEditComponent);
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
