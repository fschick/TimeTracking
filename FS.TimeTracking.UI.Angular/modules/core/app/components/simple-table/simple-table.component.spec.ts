import {ComponentFixture, TestBed} from '@angular/core/testing';
import {Column, SimpleTableComponent} from './simple-table.component';
import {By} from '@angular/platform-browser';
import {ChangeDetectorRef, DebugElement} from '@angular/core';
import {FormsModule} from '@angular/forms';

interface TestDto {
  id: number;
  name: string;
  age: number;
}

const testColumns: Column<TestDto>[] = [
  {title: 'Id', prop: 'id'},
  {title: 'Name', prop: 'name'},
  {title: 'Age', prop: 'age'},
];

const testRows: TestDto[  ] = [
  {id: 1, name: 'Bruce', age: 12},
  {id: 3, name: 'Norwood', age: 10},
  {id: 2, name: 'Abby', age: 10},
];

describe('SimpleTableComponent', () => {
  let fixture: ComponentFixture<SimpleTableComponent<TestDto>>;
  let component: SimpleTableComponent<TestDto>;
  let debugElement: DebugElement;
  let changeDetector: ChangeDetectorRef; // Required for components using changeDetectionStrategy.OnPush

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FormsModule],
      declarations: [SimpleTableComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed
      .createComponent<SimpleTableComponent<TestDto>>(SimpleTableComponent);
    component = fixture.componentInstance;
    debugElement = fixture.debugElement;
    component.setColumns = testColumns;
    component.setRows = testRows;
    fixture.detectChanges();
    changeDetector = fixture.debugElement.injector.get(ChangeDetectorRef);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should create rows and columns', () => {
    // Check
    const headerCells = debugElement.queryAll(By.css('th'));
    expect(headerCells.length).toEqual(3);

    const dataCells = debugElement.queryAll(By.css('td'));
    expect(dataCells.length).toEqual(9);

    const filterInputs = debugElement.queryAll(By.css('input'));
    expect(filterInputs.length).toEqual(0);
  });

  it('should create filter row when requested', () => {
    // Act
    component.setConfiguration = {filterRow: true};
    changeDetector.detectChanges();

    // Check
    const filterInputs = debugElement.queryAll(By.css('input'));
    expect(filterInputs.length).toEqual(3);
  });

  it('should sort single column ascending when header clicked once', async () => {
    // Act
    const ageHeaderCell = debugElement.query(By.css('th:nth-child(3)'));
    const idHeaderCell = debugElement.query(By.css('th'));
    ageHeaderCell.triggerEventHandler('click', null); // Sort by age.
    idHeaderCell.triggerEventHandler('click', null);
    changeDetector.detectChanges();

    // Check
    // await fixture.whenStable();
    setTimeout(() => {
      const sortedIds = debugElement
      .queryAll(By.css('tr td:first-child'))
      .map(x => x.nativeElement.innerText);
    expect(sortedIds).toEqual(['1', '2', '3']);

      const sortedAges = debugElement
      .queryAll(By.css('tr td:nth-child(3)'))
      .map(x => x.nativeElement.innerText);
    expect(sortedAges).toEqual(['12', '10', '10']);
    }, 1000);
  });

  it('should sort descending when header clicked twice', async () => {
    // Act
    const idHeaderCell = debugElement.query(By.css('th'));
    idHeaderCell.triggerEventHandler('click', null);
    idHeaderCell.triggerEventHandler('click', null);
    changeDetector.detectChanges();

    // Check
    // await fixture.whenStable();
    setTimeout(() => {
    const sortedIds = debugElement
      .queryAll(By.css('tr td:first-child'))
      .map(x => x.nativeElement.innerText);
    expect(sortedIds).toEqual(['3', '2', '1']);
    }, 1000);
  });

  it('should restore origin order on third header clicked', async () => {
    // Act
    const idHeaderCell = debugElement.query(By.css('th'));
    idHeaderCell.triggerEventHandler('click', null);
    idHeaderCell.triggerEventHandler('click', null);
    idHeaderCell.triggerEventHandler('click', null);
    changeDetector.detectChanges();

    // Check
    // await fixture.whenStable();
    setTimeout(() => {
    const sortedIds = debugElement
      .queryAll(By.css('tr td:first-child'))
      .map(x => x.nativeElement.innerText);
    expect(sortedIds).toEqual(['1', '3', '2']);
    }, 1000);
  });

  it('should sort multi rows when configured', async () => {
    // Act
    component.setConfiguration = {multiSort: true};
    changeDetector.detectChanges();

    const idHeaderCell = debugElement.query(By.css('th'));
    const ageHeaderCell = debugElement.query(By.css('th:nth-child(3)'));
    ageHeaderCell.triggerEventHandler('click', null); // Sort by age.
    idHeaderCell.triggerEventHandler('click', null); // Sort by id.
    changeDetector.detectChanges();

    // Check
    // await fixture.whenStable();
    setTimeout(() => {
    const sortedIds = debugElement
      .queryAll(By.css('tr td:first-child'))
      .map(x => x.nativeElement.innerText);
    expect(sortedIds).toEqual(['2', '3', '1']);

    const sortedAges = debugElement
      .queryAll(By.css('tr td:nth-child(3)'))
      .map(x => x.nativeElement.innerText);
    expect(sortedAges).toEqual(['10', '10', '12']);
    }, 1000);
  });
});
