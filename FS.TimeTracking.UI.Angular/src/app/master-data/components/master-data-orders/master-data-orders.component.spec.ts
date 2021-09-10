import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MasterDataOrdersComponent } from './master-data-orders.component';
import {HttpClientModule} from '@angular/common/http';
import {RouterTestingModule} from '@angular/router/testing';
import {ReactiveComponentModule} from '@ngrx/component';

describe('MasterDataOrdersComponent', () => {
  let component: MasterDataOrdersComponent;
  let fixture: ComponentFixture<MasterDataOrdersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientModule, RouterTestingModule, ReactiveComponentModule],
      declarations: [ MasterDataOrdersComponent ]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MasterDataOrdersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
