/* eslint-disable @angular-eslint/no-input-rename */
import {ChangeDetectionStrategy, Component, EventEmitter, Input, Output, TemplateRef, TrackByFunction} from '@angular/core';

export type SortOrder = 'asc' | 'desc';
export type Filters<TRow> = { [key in keyof TRow]: string };

export type HeaderCellClickEvent<TRow> = {
  column: Column<TRow>; mouseEvent: MouseEvent;
  table: SimpleTableComponent<TRow>;
};

export type DataCellClickEvent<TRow> = {
  row: TRow;
  column: Column<TRow>;
  mouseEvent: MouseEvent;
  table: SimpleTableComponent<TRow>;
};

export type RowChangedEvent<TRow> = {
  rows: Array<TRow>;
  table: SimpleTableComponent<TRow>;
};

export type RowTemplate<TRow> = TemplateRef<{
  columns: Column<TRow>[];
  table: SimpleTableComponent<TRow>;
}>;

export type HeadCellTemplate<TRow> = TemplateRef<{
  column: Column<TRow>;
  table: SimpleTableComponent<TRow>;
}>;

export type FooterCellTemplate<TRow> = TemplateRef<{
  column: Column<TRow>;
  table: SimpleTableComponent<TRow>;
}>;

export type FilterCellTemplate<TRow> = TemplateRef<{
  column: Column<TRow>;
  filters: Filters<TRow>;
  applyFilter: (() => void);
  table: SimpleTableComponent<TRow>;
}>;

export type DataCellTemplate<TRow> = TemplateRef<{
  row: TRow;
  column: Column<TRow>;
  index: number;
  count: number;
  first: boolean;
  last: boolean;
  even: boolean;
  odd: boolean;
  table: SimpleTableComponent<TRow>;
}>;

export class Configuration<TRow> {
  public cssWrapper = '';
  public cssTable = '';
  public cssHeadRow: string | (() => string) = '';
  public cssHeadCell = '';
  public cssFilterCell = '';
  public cssDataRow: string | ((row: TRow) => string) = '';
  public cssDataCell = '';
  public cssFooterRow: string | (() => string) = '';
  public cssFooterCell = '';
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
  prop?: keyof TRow;
  footer?: string | ((column: Column<TRow>) => string),
  customId?: string;
  cssHeadCell?: string | ((column: Column<TRow>) => string);
  cssFilterCell?: string | ((column: Column<TRow>) => string);
  cssDataCell?: string | ((row: TRow, column: Column<TRow>) => string);
  cssFooterCell?: string | ((column: Column<TRow>) => string);
  width?: string;
  format?: ((row: TRow, column: Column<TRow>) => string | undefined);
  sortable?: boolean;
  sortType?: 'string' | 'number' | 'date';
  sort?: ((rowA: TRow, rowB: TRow, direction: SortOrder) => number);
  filterable?: 'contains' | 'startWith' | 'no';
  filter?: ((row: TRow, filterValue: string) => boolean);
  filterCellTemplate?: FilterCellTemplate<TRow>;
  headCellTemplate?: HeadCellTemplate<TRow>;
  dataCellTemplate?: DataCellTemplate<TRow>;
  footerCellTemplate?: FooterCellTemplate<TRow>;
};

@Component({
  selector: 'ts-simple-table',
  templateUrl: './simple-table.component.html',
  styleUrls: ['./simple-table.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SimpleTableComponent<TRow> {

  public configuration: Configuration<TRow>;

  @Input('configuration')
  public set setConfiguration(configuration: Partial<Configuration<TRow>> | null | undefined) {
    this.configuration = {...new Configuration<TRow>(), ...configuration};
    this.originRowOrder = this.rows.map((row, index) => this.configuration.trackBy(index, row));
    if (!this.configuration.filterRow)
      this.resetFilter();
    this.sortAndFilterRows();
  };

  public rows: TRow[];

  @Input('rows')
  public set setRows(rows: TRow[] | null | undefined) {
    this.rows = rows ?? [];
    this.originRowOrder = this.rows.map((row, index) => this.configuration.trackBy(index, row));
    this.guessColumnsSortType();
    this.sortAndFilterRows();
  };

  public columns?: Column<TRow>[];

  @Input('columns')
  public set setColumns(columns: Column<TRow>[] | null | undefined) {
    this.columns = columns ?? [];
    this.guessColumnsSortType();
    this.updateFiltersWithChangedColumns(this.columns);
    this.sortAndFilterRows();
  }

  @Output() private headerCellClick = new EventEmitter<HeaderCellClickEvent<TRow>>();

  @Output() private dataCellClick = new EventEmitter<DataCellClickEvent<TRow>>();

  @Output() private rowsChanged = new EventEmitter<RowChangedEvent<TRow>>();

  public filters: Filters<TRow> = {} as Filters<TRow>;
  public filteredRows: TRow[];
  private sortOrder: Map<string | number | symbol, SortOrder> = new Map();
  private originRowOrder: any[];

  constructor() {
    this.configuration = new Configuration();
    this.rows = [];
    this.filteredRows = [];
    this.originRowOrder = [];
  }

  public getHeaderRowContext() {
    return {columns: this.columns, table: this};
  }

  public getHeaderCellContext(column: Column<TRow>) {
    return {column, table: this};
  }

  public getFilterCellContext(column: Column<TRow>) {
    return {column, filters: this.filters, applyFilter: () => this.sortAndFilterRows(), table: this};
  }

  public getDataCellContext(row: TRow, column: Column<TRow>, index: number, count: number, first: boolean, last: boolean, even: boolean, odd: boolean) {
    return {row, column, index, count, first, last, even, odd, table: this};
  }

  public getColumnWidth(column: Column<TRow>): string {
    return column.width ? `width: ${column.width}` : '';
  }

  public getCssHeadRow(): string {
    if (typeof this.configuration.cssHeadRow === 'function')
      return this.configuration.cssHeadRow();
    return this.configuration.cssHeadRow ? this.configuration.cssHeadRow : '';
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
    return column.sortable !== false ? this.configuration.cssSortEnabled : '';
  }

  public getCssDataRow(row: TRow): string {
    if (typeof this.configuration.cssDataRow === 'function')
      return this.configuration.cssDataRow(row);
    return this.configuration.cssDataRow ? this.configuration.cssDataRow : '';
  }

  public getCssDataCell(row: TRow, column: Column<TRow>): string {
    if (typeof column.cssDataCell === 'function')
      return column.cssDataCell(row, column);
    return column.cssDataCell ? column.cssDataCell : '';
  }

  public getCssFooterRow(): string {
    if (typeof this.configuration.cssFooterRow === 'function')
      return this.configuration.cssFooterRow();
    return this.configuration.cssFooterRow ? this.configuration.cssFooterRow : '';
  }

  public getCssFooterCell(column: Column<TRow>): string {
    if (typeof column.cssFooterCell === 'function')
      return column.cssFooterCell(column);
    return column.cssFooterCell ? column.cssFooterCell : '';
  }

  public getCssSortOrder(column: Column<TRow>): string {
    const sortKey = this.getSortKey(column);
    if (sortKey === undefined)
      return '';

    const columnSortOrder = this.sortOrder.get(sortKey);
    return columnSortOrder !== undefined
      ? columnSortOrder === 'asc'
        ? this.configuration.cssSortAsc
        : this.configuration.cssSortDesc
      : '';
  }

  public getGlyphSortOrder(column: Column<TRow>) {
    const sortKey = this.getSortKey(column);
    if (sortKey === undefined)
      return '';

    const columnSortOrder = this.sortOrder.get(sortKey);
    return columnSortOrder !== undefined
      ? columnSortOrder === 'asc'
        ? this.configuration.glyphSortAsc
        : this.configuration.glyphSortDesc
      : '';
  }

  public getCellValue(row: TRow, column: Column<TRow>): string {
    if (typeof column.format === 'function')
      return column.format(row, column) ?? '';

    return column.prop !== undefined
      ? this.toString(row[column.prop])
      : '';
  }

  public getFooterValue(column: Column<TRow>): string {
    if (typeof column.footer === 'function')
      return column.footer(column) ?? '';

    return column.footer ?? '';
  }

  public getFooterRequired(): boolean {
    return this.columns?.some(x => x.footer !== undefined) ?? false;
  }

  public applySortOrder(column: Column<TRow>, $event: MouseEvent) {
    this.headerCellClick.emit({column, mouseEvent: $event, table: this});
    if ($event.defaultPrevented)
      return;

    const sortKey = this.getSortKey(column);
    if (column.sortable === false || sortKey === undefined)
      return;

    let columnSortOrder = this.sortOrder.get(sortKey);
    if (!this.configuration.multiSort)
      this.sortOrder.clear();

    columnSortOrder = columnSortOrder === undefined ? 'asc' : columnSortOrder === 'asc' ? 'desc' : undefined;
    if (columnSortOrder === undefined)
      this.sortOrder.delete(sortKey);
    else
      this.sortOrder.set(sortKey, columnSortOrder);

    this.sortAndFilterRows();
  }

  public onDataCellClick(row: TRow, column: Column<TRow>, $event: MouseEvent) {
    this.dataCellClick.emit({row, column, mouseEvent: $event, table: this});
  }

  public toString<T>(value: T): string {
    if (value === null || value === undefined)
      return '';

    switch (typeof value) {
      case 'string':
        return value;
      case 'number':
      case 'bigint':
        return value.toLocaleString(this.configuration.locale);
      case 'object':
        if (value instanceof Date)
          return value.toLocaleString(this.configuration.locale);
    }

    return String(value);
  }

  private updateFiltersWithChangedColumns(columns: Column<TRow>[]): void {
    const filterProperties = Object.keys(this.filters) as (keyof TRow)[];

    const filterableColumns = columns.filter(x => x.filterable !== 'no');

    const missingFilters = filterableColumns.filter(col => !filterProperties.some(filterProp => filterProp === col.prop));
    for (const column of missingFilters)
      if (column.prop !== undefined)
        this.filters[column.prop] = '';

    const extraFilters = filterProperties.filter(filterProp => !filterableColumns.some(col => col.prop === filterProp));
    for (const filter of extraFilters)
      delete this.filters[filter];
  }

  private guessColumnsSortType(): void {
    if (this.rows.length === 0 || !this.columns || this.columns.length === 0)
      return;

    const columnsWithMissingSortType = this.columns.filter(col => col.prop && !col.sortType);
    for (const column of columnsWithMissingSortType) {
      const propValue = this.rows[0][column.prop!];
      if (propValue instanceof Date)
        column.sortType = 'date';
      else if (typeof propValue === 'number')
        column.sortType = 'number';
    }
  }

  private getSortKey(column: Column<TRow>): string | number | symbol | undefined {
    return column.prop ?? column.customId;
  }

  private sortAndFilterRows() {
    if (!this.columns)
      return;

    this.sortRows();
    this.applyFilter();
    this.rowsChanged.emit({rows: this.filteredRows, table: this});
  }

  private sortRows(): void {
    this.rows.sort((rowA, rowB) => {
      // Compare/sort by column.
      for (const [sortKey, direction] of this.sortOrder) {
        const column = this.columns?.find(x => x.prop === sortKey || x.customId === sortKey);
        if (column === undefined)
          continue;

        const prop = sortKey as keyof TRow;
        let columnResult = 0;

        const columnSortFn = column?.sort;
        if (typeof columnSortFn === 'function')
          columnResult = columnSortFn(rowA, rowB, direction) * (direction === 'asc' ? 1 : -1);
        else if (column.sortType === undefined || column.sortType === 'string')
          columnResult = this.getCellValue(rowA, column).localeCompare(this.getCellValue(rowB, column)) * (direction === 'asc' ? 1 : -1)
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
      const rowKeyA = this.configuration.trackBy(rowIndexA, rowA);
      const rowKeyB = this.configuration.trackBy(rowIndexB, rowB);
      return this.originRowOrder.indexOf(rowKeyA) - this.originRowOrder.indexOf(rowKeyB);
    });
  }

  private applyFilter(): void {
    this.filteredRows = this.filterRows(this.rows);
  }

  private filterRows(rows: TRow[]): TRow[] {
    if (!this.columns)
      return rows;

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
          const cellValue = this.getCellValue(row, column).toLowerCase();
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

  private resetFilter(): void {
    const filterProperties = Object.keys(this.filters) as (keyof TRow)[];
    for (const property of filterProperties)
      this.filters[property] = '';
  }
}
