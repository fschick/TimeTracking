import {ComponentFixture, fakeAsync, TestBed, tick} from '@angular/core/testing';
import {MasterDataHolidaysImportComponent} from './master-data-holidays-import.component';
import {Component} from '@angular/core';
import {MasterDataHolidaysEditComponent} from '../master-data-holidays-edit/master-data-holidays-edit.component';
import {Router} from '@angular/router';
import {HttpClientModule} from '@angular/common/http';
import {NgSelectModule} from '@ng-select/ng-select';
import {ReactiveFormsModule} from '@angular/forms';
import {RouterTestingModule} from '@angular/router/testing';
import {HolidayService} from '../../../shared/services/api';

@Component({
  template: '<router-outlet></router-outlet>',
})
class TestRootComponent {
}

describe('MasterDataHolidaysImportComponent', () => {
  let component: MasterDataHolidaysImportComponent;
  let rootFixture: ComponentFixture<TestRootComponent>;
  let fixture: ComponentFixture<MasterDataHolidaysImportComponent>;
  let router: Router;

  const navigateById = (id: string) => {
    rootFixture.ngZone?.run(() => router.navigate([id])); // [1] [2]
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MasterDataHolidaysImportComponent],
      imports: [
        HttpClientModule,
        NgSelectModule,
        ReactiveFormsModule,
        RouterTestingModule.withRoutes([
          {path: 'import', component: MasterDataHolidaysImportComponent}
        ])
      ]
    });

    await TestBed.compileComponents();
    rootFixture = TestBed.createComponent(TestRootComponent);
    router = TestBed.inject(Router);
  });

  beforeEach(fakeAsync(() => {
    navigateById('import');
    tick();
    fixture = TestBed.createComponent(MasterDataHolidaysImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
