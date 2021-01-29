import {ComponentFixture, TestBed} from '@angular/core/testing';

import {MasterDataCustomersComponent} from './master-data-customers.component';
import {HttpClientModule} from '@angular/common/http';

describe('MasterDataCustomersComponent', () => {
  let component: MasterDataCustomersComponent;
  let fixture: ComponentFixture<MasterDataCustomersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientModule],
      declarations: [MasterDataCustomersComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MasterDataCustomersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
