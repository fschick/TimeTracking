import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MasterDataHolidaysComponent } from './master-data-holidays.component';
import {HttpClientModule} from '@angular/common/http';
import {RouterTestingModule} from '@angular/router/testing';
import {ReactiveComponentModule} from '@ngrx/component';

describe('MasterDataHolidaysComponent', () => {
  let component: MasterDataHolidaysComponent;
  let fixture: ComponentFixture<MasterDataHolidaysComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientModule, RouterTestingModule, ReactiveComponentModule],
      declarations: [ MasterDataHolidaysComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MasterDataHolidaysComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
