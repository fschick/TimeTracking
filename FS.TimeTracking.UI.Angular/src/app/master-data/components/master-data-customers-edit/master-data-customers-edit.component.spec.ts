import {Component} from '@angular/core';
import {CustomerService} from '../../../shared/services/api';
import {Observable, of} from 'rxjs';
import {MasterDataCustomersEditComponent} from './master-data-customers-edit.component';
import {ComponentFixture, fakeAsync, TestBed, tick} from '@angular/core/testing';
import {Router} from '@angular/router';
import {HttpClientModule} from '@angular/common/http';
import {RouterTestingModule} from '@angular/router/testing';

const fakeCustomerService = {
  get: (): Observable<any> => of({})
} as Partial<CustomerService>;

@Component({
  template: '<router-outlet></router-outlet>',
})
class TestRootComponent {
}

describe('MasterDataCustomersEditComponent', () => {
  let component: MasterDataCustomersEditComponent;
  let rootFixture: ComponentFixture<TestRootComponent>;
  let fixture: ComponentFixture<MasterDataCustomersEditComponent>;
  let router: Router;

  const navigateById = (id: string) => {
    rootFixture.ngZone?.run(() => router.navigate([id])); // [1] [2]
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TestRootComponent, MasterDataCustomersEditComponent],
      imports: [
        HttpClientModule,
        RouterTestingModule.withRoutes([
          {path: ':id', component: MasterDataCustomersEditComponent}
        ])
      ],
      providers: [{provide: CustomerService, useValue: fakeCustomerService}],
    });

    await TestBed.compileComponents();
    rootFixture = TestBed.createComponent(TestRootComponent);
    router = TestBed.inject(Router);
  });

  beforeEach(fakeAsync(() => {
    navigateById('efb9854c-38e5-4a8f-aad7-71f1b2a483c3');
    tick();
    fixture = TestBed.createComponent(MasterDataCustomersEditComponent);
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
