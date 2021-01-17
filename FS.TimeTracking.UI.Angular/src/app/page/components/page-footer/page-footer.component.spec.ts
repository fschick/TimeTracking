import {ComponentFixture, TestBed} from '@angular/core/testing';

import {PageFooterComponent} from './page-footer.component';
import {HttpClientModule} from '@angular/common/http';

describe('PageFooterComponent', () => {
  let component: PageFooterComponent;
  let fixture: ComponentFixture<PageFooterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientModule],
      declarations: [PageFooterComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PageFooterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
