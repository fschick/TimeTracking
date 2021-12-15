import {Component, OnInit, ViewChild} from '@angular/core';
import {ActivityListDto, HolidayListDto, HolidayService} from '../../../shared/services/api';
import {Observable, Subject} from 'rxjs';
import {LocalizationService} from '../../../shared/services/internationalization/localization.service';
import {
  Column,
  Configuration,
  DataCellTemplate,
  SimpleTableComponent
} from '../../../shared/components/simple-table/simple-table.component';
import {single, switchMap} from 'rxjs/operators';
import {ActivatedRoute, Router} from '@angular/router';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {GuidService} from '../../../shared/services/state-management/guid.service';
import {Filter, FilteredRequestParams, FilterName} from '../../../shared/components/filter/filter.component';

@Component({
  selector: 'ts-master-data-holidays',
  templateUrl: './master-data-holidays.component.html',
  styleUrls: ['./master-data-holidays.component.scss']
})
export class MasterDataHolidaysComponent implements OnInit {
  @ViewChild(SimpleTableComponent) private holidayTable?: SimpleTableComponent<HolidayListDto>;
  @ViewChild('dataCellTemplate', {static: true}) private dataCellTemplate?: DataCellTemplate<HolidayListDto>;
  @ViewChild('actionCellTemplate', {static: true}) private actionCellTemplate?: DataCellTemplate<HolidayListDto>;

  public guidService = GuidService;
  public rows$: Observable<HolidayListDto[]>;
  public columns!: Column<HolidayListDto>[];
  public configuration?: Partial<Configuration<HolidayListDto>>;
  public filters: (Filter | FilterName)[];
  public filterChanged = new Subject<FilteredRequestParams>();

  constructor(
    private entityService: EntityService,
    private router: Router,
    private route: ActivatedRoute,
    private holidayService: HolidayService,
    private localizationService: LocalizationService,
  ) {
    this.filters = [
      {name: 'holidayStartDate'},
      {name: 'holidayEndDate'},
      {name: 'holidayTitle'},
      {name: 'holidayType'},
    ];

    this.rows$ = this.filterChanged
      .pipe(
        switchMap(filter => this.loadData(filter)),
        this.entityService.withUpdatesFrom(this.entityService.holidayChanged, this.holidayService),
      );
  }

  public ngOnInit(): void {
    this.configuration = {
      cssWrapper: 'table-responsive',
      cssTable: 'table table-card table-sm align-middle text-break border',
      glyphSortAsc: '',
      glyphSortDesc: '',
      locale: this.localizationService.language,
    };

    const cssHeadCell = 'border-0 text-nowrap';
    const cssHeadCellMd = 'd-none d-md-table-cell';
    const cssDataCellMd = cssHeadCellMd;
    this.columns = [
      {
        title: $localize`:@@DTO.HolidayListDto.Title:[i18n] Title`,
        prop: 'title',
        cssHeadCell: cssHeadCell,
        dataCellTemplate: this.dataCellTemplate
      }, {
        title: $localize`:@@DTO.HolidayListDto.StartDate:[i18n] Start date`,
        prop: 'startDate',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd}`,
        cssDataCell: cssDataCellMd,
        dataCellTemplate: this.dataCellTemplate,
        format: (row) => row.startDate.toFormat(this.localizationService.dateTime.dateFormat)
      }, {
        title: $localize`:@@DTO.HolidayListDto.EndDate:[i18n] End date`,
        prop: 'endDate',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd}`,
        cssDataCell: cssDataCellMd,
        dataCellTemplate: this.dataCellTemplate,
        format: (row) => row.endDate.toFormat(this.localizationService.dateTime.dateFormat)
      }, {
        title: $localize`:@@Common.Action:[i18n] Action`,
        customId: 'delete',
        dataCellTemplate: this.actionCellTemplate,
        cssHeadCell: cssHeadCell,
        cssDataCell: 'text-nowrap action-cell',
        width: '1px',
        sortable: false
      },
    ];
  }

  private loadData(filter: FilteredRequestParams): Observable<HolidayListDto[]> {
    return this.holidayService.getListFiltered(filter)
      .pipe(single());
  }

  public getDataCellValue(row: HolidayListDto, column: Column<HolidayListDto>): string {
    return this.holidayTable?.getCellValue(row, column) ?? '';
  }

  public deleteItem(id: string): void {
    this.holidayService
      .delete({id})
      .pipe(single())
      .subscribe(() => {
        this.entityService.holidayChanged.next({entity: {id} as HolidayListDto, action: 'deleted'});
      });
  }
}
