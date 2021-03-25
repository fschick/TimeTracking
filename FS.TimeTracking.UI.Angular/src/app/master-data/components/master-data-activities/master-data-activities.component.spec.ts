import {ComponentFixture, TestBed} from '@angular/core/testing';

import {MasterDataActivitiesComponent} from './master-data-activities.component';
import {HttpClientModule} from '@angular/common/http';
import {RouterTestingModule} from '@angular/router/testing';

describe('MasterDataActivitiesComponent', () => {
  let component: MasterDataActivitiesComponent;
  let fixture: ComponentFixture<MasterDataActivitiesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientModule, RouterTestingModule],
      declarations: [MasterDataActivitiesComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MasterDataActivitiesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
