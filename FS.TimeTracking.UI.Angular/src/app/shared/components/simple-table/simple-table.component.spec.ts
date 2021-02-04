import {ComponentFixture, TestBed} from '@angular/core/testing';

import {SimpleTableComponent} from './simple-table.component';

describe('SimpleTableComponent', () => {
  let component: SimpleTableComponent<unknown>;
  let fixture: ComponentFixture<SimpleTableComponent<unknown>>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SimpleTableComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SimpleTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
