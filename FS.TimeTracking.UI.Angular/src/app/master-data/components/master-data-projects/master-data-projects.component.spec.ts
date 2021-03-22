import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MasterDataProjectsComponent } from './master-data-projects.component';
import {HttpClientModule} from '@angular/common/http';
import {RouterTestingModule} from '@angular/router/testing';

describe('MasterDataProjectsComponent', () => {
  let component: MasterDataProjectsComponent;
  let fixture: ComponentFixture<MasterDataProjectsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientModule, RouterTestingModule],
      declarations: [ MasterDataProjectsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MasterDataProjectsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
