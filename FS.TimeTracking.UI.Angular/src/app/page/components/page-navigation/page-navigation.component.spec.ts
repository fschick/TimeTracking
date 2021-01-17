import {ComponentFixture, TestBed} from '@angular/core/testing';
import {PageNavigationComponent} from './page-navigation.component';
import {RouterTestingModule} from '@angular/router/testing';

describe('PageNavigationComponent', () => {
  let component: PageNavigationComponent;
  let fixture: ComponentFixture<PageNavigationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      declarations: [PageNavigationComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PageNavigationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
