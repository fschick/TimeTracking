import {ChangeDetectionStrategy, Component, Input, OnChanges, SimpleChanges, TemplateRef, TrackByFunction} from '@angular/core';

export type SortOrder = 'asc' | 'desc';
export type Filters<TRow> = { [key in keyof TRow]: string };

export type RowTemplate<TRow> = TemplateRef<{ columns: Column<TRow>[] }>;

export type HeadCellTemplate<TRow> = TemplateRef<{ column: Column<TRow> }>;
export type FilterCellTemplate<TRow> = TemplateRef<{ column: Column<TRow>; filters: Filters<TRow>; applyFilter: (() => void) }>;
export type DataCellTemplate<TRow> = TemplateRef<{ row: TRow; column: Column<TRow> }>;

export class Configuration<TRow> {
  public cssWrapper = '';
  public cssTable = '';
  public cssHeadRow = '';
  public cssHeadCell = '';
  public cssFilterCell = '';
  public cssDataRow = '';
  public cssDataCell = '';
  public cssSortEnabled = 'sort-enabled';
  public cssSortAsc = 'sort-asc';
  public cssSortDesc = 'sort-desc';
  public glyphSortAsc = '↓';
  public glyphSortDesc = '↑';
  public multiSort = false;
  public filterRow = false;
  public locale: string = navigator.language;
  public headRowTemplate?: RowTemplate<TRow>;
  public filterRowTemplate?: RowTemplate<TRow>;
  public dataRowTemplate?: RowTemplate<TRow>;
  public footRowTemplate?: RowTemplate<TRow>;
  public trackBy: TrackByFunction<TRow> = (index: number, item: TRow) => item;
}

export type Column<TRow> = {
  title?: string;
  prop: keyof TRow;
  cssHeadCell?: string | ((column: Column<TRow>) => string);
  cssFilterCell?: string | ((column: Column<TRow>) => string);
  cssDataCell?: string | ((row: TRow, column: Column<TRow>) => string);
  width?: string;
  format?: ((row: TRow, column: Column<TRow>) => string | undefined);
  sortable?: boolean;
  sort?: ((rowA: TRow, rowB: TRow) => number);
  filterable?: 'contains' | 'startWith' | 'no';
  filter?: ((row: TRow, filterValue: string) => boolean);
  filterCellTemplate?: FilterCellTemplate<TRow>;
  headCellTemplate?: HeadCellTemplate<TRow>;
  dataCellTemplate?: DataCellTemplate<TRow>;
};

@Component({
  selector: 'ts-simple-table',
  templateUrl: './simple-table.component.html',
  styleUrls: ['./simple-table.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SimpleTableComponent<TRow> implements OnChanges {
  @Input() public rows: TRow[];
  @Input() public columns: Column<TRow>[];
  @Input() public configuration?: Partial<Configuration<TRow>>;
  public mergedConfig: Configuration<TRow>;

  public filters: Filters<TRow> = {} as Filters<TRow>;
  public filteredRows: TRow[];
  private sortOrder: Map<keyof TRow, SortOrder> = new Map();
  private originRowOrder: any[];

  constructor() {
    this.rows = [];
    this.filteredRows = [];
    this.columns = [];
    this.originRowOrder = [];
    this.mergedConfig = new Configuration();
  }

  public ngOnChanges(changes: SimpleChanges): void {
    if (changes.configuration !== undefined) {
      this.mergedConfig = {...new Configuration<TRow>(), ...changes.configuration.currentValue};
      this.originRowOrder = this.rows.map((row, index) => this.mergedConfig.trackBy(index, row));
      if (!this.mergedConfig.filterRow)
        this.resetFilter();
    }

    if (changes.columns !== undefined)
      this.applyChangedColumnsToFilter();

    if (changes.rows !== undefined) {
      if (this.rows === undefined)
        this.rows = [];
      this.originRowOrder = this.rows.map((row, index) => this.mergedConfig.trackBy(index, row));
    }

    this.sortRows();
    this.applyFilter();
  }

  public getFilterTemplateContext(column: Column<TRow>) {
    return {column, filters: this.filters, applyFilter: () => this.applyFilter()};
  }

  public getColumnWidth(column: Column<TRow>): string {
    return column.width ? `width: ${column.width}` : '';
  }

  public getCssHeadCell(column: Column<TRow>): string {
    if (typeof column.cssHeadCell === 'function')
      return column.cssHeadCell(column);
    return column.cssHeadCell ? column.cssHeadCell : '';
  }

  public getCssFilterCell(column: Column<TRow>): string {
    if (typeof column.cssFilterCell === 'function')
      return column.cssFilterCell(column);
    return column.cssFilterCell ? column.cssFilterCell : '';
  }

  public getCssSortEnabled(column: Column<TRow>): string {
    return column.sortable !== false ? this.mergedConfig.cssSortEnabled : '';
  }

  public getCssDataCell(row: TRow, column: Column<TRow>): string {
    if (typeof column.cssDataCell === 'function')
      return column.cssDataCell(row, column);
    return column.cssDataCell ? column.cssDataCell : '';
  }

  public getCssSortOrder(column: Column<TRow>): string {
    const columnSortOrder = this.sortOrder.get(column.prop);
    return columnSortOrder !== undefined
      ? columnSortOrder === 'asc'
        ? this.mergedConfig.cssSortAsc
        : this.mergedConfig.cssSortDesc
      : '';
  }

  public getGlyphSortOrder(column: Column<TRow>) {
    const columnSortOrder = this.sortOrder.get(column.prop);
    return columnSortOrder !== undefined
      ? columnSortOrder === 'asc'
        ? this.mergedConfig.glyphSortAsc
        : this.mergedConfig.glyphSortDesc
      : '';
  }

  public getCellValue(row: TRow, column: Column<TRow>): string | undefined {
    if (column.format)
      return column.format(row, column);

    const value = row[column.prop] as any;
    return value?.toLocaleString(this.mergedConfig.locale);
  }

  public applySortOrder(column: Column<TRow>) {
    if (column.sortable === false)
      return;

    let columnSortOrder = this.sortOrder.get(column.prop);
    if (!this.mergedConfig.multiSort)
      this.sortOrder.clear();

    columnSortOrder = columnSortOrder === undefined ? 'asc' : columnSortOrder === 'asc' ? 'desc' : undefined;
    if (columnSortOrder === undefined)
      this.sortOrder.delete(column.prop);
    else
      this.sortOrder.set(column.prop, columnSortOrder);

    this.sortRows();
    this.applyFilter();
  }

  public applyFilter(): void {
    this.filteredRows = this.filterRows(this.rows);
  }

  private applyChangedColumnsToFilter(): void {
    const filterProperties = Object.keys(this.filters) as (keyof TRow)[];

    const filterableColumn = this.columns.filter(x => x.filterable !== 'no');

    const missingFilters = filterableColumn.filter(col => !filterProperties.some(filterProp => filterProp === col.prop));
    for (const column of missingFilters)
      this.filters[column.prop] = '';

    const extraFilters = filterProperties.filter(filterProp => !filterableColumn.some(col => col.prop === filterProp));
    for (const filter of extraFilters)
      delete this.filters[filter];
  }

  private sortRows(): void {
    this.rows.sort((rowA, rowB) => {
      // Compare/sort by column.
      for (const [prop, direction] of this.sortOrder) {
        let columnResult = 0;
        const columnSortFn = this.columns.find(x => x.prop === prop)?.sort;
        if (columnSortFn)
          columnResult = columnSortFn(rowA, rowB) * (direction === 'asc' ? 1 : -1);
        else if (rowA[prop] > rowB[prop])
          columnResult = direction === 'asc' ? 1 : -1;
        else if (rowA[prop] < rowB[prop])
          columnResult = direction === 'asc' ? -1 : 1;
        if (columnResult !== 0)
          return columnResult;
      }

      // When rows remain same/unsorted, compare/sort by origin order.
      const rowIndexA = this.rows.indexOf(rowA);
      const rowIndexB = this.rows.indexOf(rowB);
      const rowKeyA = this.mergedConfig.trackBy(rowIndexA, rowA);
      const rowKeyB = this.mergedConfig.trackBy(rowIndexB, rowB);
      return this.originRowOrder.indexOf(rowKeyA) - this.originRowOrder.indexOf(rowKeyB);
    });
  }

  private resetFilter(): void {
    const filterProperties = Object.keys(this.filters) as (keyof TRow)[];
    for (const property of filterProperties)
      this.filters[property] = '';
  }

  private filterRows(rows: TRow[]): TRow[] {
    const columns = Object.fromEntries(this.columns.map(col => ([col.prop, col])));
    const filters = Object.entries(this.filters) as [keyof TRow, any];
    const activeFilters = filters
      .filter(([_, filterValue]) => filterValue !== '')
      .map(([prop, filterValue]) => ({filterValue: filterValue.toLowerCase(), column: columns[prop]}));

    return rows.filter(row => {
      for (const {filterValue, column} of activeFilters) {
        if (column.filter && !column.filter(row, filterValue)) {
          return false;
        } else {
          const cellValue = this.getCellValue(row, column)?.toLowerCase();
          const filterMatches = column.filterable === 'startWith'
            ? cellValue?.startsWith(filterValue)
            : cellValue?.includes(filterValue);
          if (!filterMatches)
            return false;
        }
      }

      return true;
    });
  }
}
