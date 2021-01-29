import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MasterDataProjectsComponent } from './master-data-projects.component';

describe('MasterDataProjectsComponent', () => {
  let component: MasterDataProjectsComponent;
  let fixture: ComponentFixture<MasterDataProjectsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
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
