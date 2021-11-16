import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MasterDataSettingsComponent } from './master-data-settings.component';
import {HttpClientModule} from '@angular/common/http';
import {RouterTestingModule} from '@angular/router/testing';
import {ReactiveFormsModule} from '@angular/forms';
import {ReactiveComponentModule} from '@ngrx/component';

describe('MasterDataSettingsComponent', () => {
  let component: MasterDataSettingsComponent;
  let fixture: ComponentFixture<MasterDataSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientModule, RouterTestingModule, ReactiveFormsModule, ReactiveComponentModule],
      declarations: [ MasterDataSettingsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MasterDataSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
