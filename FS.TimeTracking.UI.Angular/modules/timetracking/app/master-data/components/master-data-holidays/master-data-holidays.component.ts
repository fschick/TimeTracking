import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {HolidayGridDto, HolidayService, HolidayType} from '../../../../../api/timetracking';
import {combineLatest, Observable, Subscription} from 'rxjs';
import {LocalizationService} from '../../../../../core/app/services/internationalization/localization.service';
import {Column, Configuration, DataCellTemplate, SimpleTableComponent} from '../../../../../core/app/components/simple-table/simple-table.component';
import {single, startWith, switchMap} from 'rxjs/operators';
import {ActivatedRoute, Router} from '@angular/router';
import {EntityService} from '../../../../../core/app/services/state-management/entity.service';
import {GuidService} from '../../../../../core/app/services/state-management/guid.service';
import {Filter, FilteredRequestParams, FilterName} from '../../../../../core/app/components/filter/filter.component';
import {DateTime} from 'luxon';
import {EnumTranslationService} from '../../../../../core/app/services/enum-translation.service';
import {ConfigurationService} from '../../../../../core/app/services/configuration.service';
import {AuthenticationService} from '../../../../../core/app/services/authentication.service';

@Component({
  selector: 'ts-master-data-holidays',
  templateUrl: './master-data-holidays.component.html',
  styleUrls: ['./master-data-holidays.component.scss']
})
export class MasterDataHolidaysComponent implements OnInit, OnDestroy {
  @ViewChild(SimpleTableComponent) private holidayTable?: SimpleTableComponent<HolidayGridDto>;
  @ViewChild('dataCellTemplate', {static: true}) private dataCellTemplate?: DataCellTemplate<HolidayGridDto>;
  @ViewChild('actionCellTemplate', {static: true}) private actionCellTemplate?: DataCellTemplate<HolidayGridDto>;

  public guidService = GuidService;
  public rows?: HolidayGridDto[];
  public columns!: Column<HolidayGridDto>[];
  public configuration?: Partial<Configuration<HolidayGridDto>>;
  public filters: (Filter | FilterName)[];
  public createNewDisabled: boolean;

  private readonly subscriptions = new Subscription();

  constructor(
    private entityService: EntityService,
    private router: Router,
    private route: ActivatedRoute,
    private holidayService: HolidayService,
    private localizationService: LocalizationService,
    private enumTranslationService: EnumTranslationService,
    private configurationService: ConfigurationService,
    authenticationService: AuthenticationService,
  ) {
    this.createNewDisabled = !authenticationService.currentUser.hasRole.masterDataHolidaysManage;

    const defaultStartDate = DateTime.now().startOf('year');
    const defaultEndDate = DateTime.now().endOf('year');

    this.filters = [
      {name: 'holidayStartDate', defaultValue: defaultStartDate, isPrimary: true},
      {name: 'holidayEndDate', defaultValue: defaultEndDate, isPrimary: true},
      {name: 'holidayTitle', isPrimary: true},
      {name: 'holidayType', isPrimary: true},
    ];
  }

  public ngOnInit(): void {
    const reloadRequested$ = this.entityService.reloadRequested;
    const holidayImported$ = this.entityService.holidaysImported.pipe(startWith(void 0));
    const loadData = combineLatest([reloadRequested$, holidayImported$])
      .pipe(
        switchMap(([filter]) => this.loadData(filter)),
        this.entityService.withUpdatesFrom(this.entityService.holidayChanged, this.holidayService),
      )
      .subscribe(rows => this.rows = rows);
    this.subscriptions.add(loadData);

    this.configuration = {
      cssWrapper: 'table-responsive',
      cssTable: 'table',
      glyphSortAsc: '',
      glyphSortDesc: '',
      locale: this.localizationService.language,
    };

    const cssHeadCell = 'border-0 text-nowrap';
    const cssHeadCellLg = 'd-none d-lg-table-cell';
    const cssDataCellLg = cssHeadCellLg;
    const cssHeadCellMd = 'd-none d-md-table-cell';
    const cssDataCellMd = cssHeadCellMd;

    const columns: Column<HolidayGridDto>[] = [
      {
        title: $localize`:@@DTO.HolidayGridDto.Title:[i18n] Title`,
        prop: 'title',
        cssHeadCell: cssHeadCell,
        dataCellTemplate: this.dataCellTemplate
      }, {
        title: $localize`:@@DTO.HolidayGridDto.StartDate:[i18n] Start date`,
        prop: 'startDate',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd}`,
        cssDataCell: cssDataCellMd,
        dataCellTemplate: this.dataCellTemplate,
        format: (row) => row.startDate.toFormat(this.localizationService.dateTime.dateFormat)
      }, {
        title: $localize`:@@DTO.HolidayGridDto.EndDate:[i18n] End date`,
        prop: 'endDate',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd}`,
        cssDataCell: cssDataCellMd,
        dataCellTemplate: this.dataCellTemplate,
        format: (row) => row.endDate.toFormat(this.localizationService.dateTime.dateFormat)
      }, {
        title: $localize`:@@DTO.HolidayGridDto.Type:[i18n] Type`,
        prop: 'type',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellLg}`,
        cssDataCell: cssDataCellLg,
        dataCellTemplate: this.dataCellTemplate,
        format: (row) => this.enumTranslationService.translate('HolidayType', row.type)
      }, {
        title: '',
        customId: 'delete',
        dataCellTemplate: this.actionCellTemplate,
        cssHeadCell: cssHeadCell,
        cssDataCell: 'text-nowrap action-cell',
        width: '1px',
        sortable: false
      },
    ];

    const authorizationEnabled = this.configurationService.clientConfiguration.features.authorization;
    if (authorizationEnabled)
      columns.splice(4, 0, {
        title: $localize`:@@DTO.HolidayGridDto.Username:[i18n] User`,
        prop: 'username',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellLg}`,
        cssDataCell: cssDataCellLg,
        dataCellTemplate: this.dataCellTemplate,
      });

    this.columns = columns;
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private loadData(filter: FilteredRequestParams): Observable<HolidayGridDto[]> {
    return this.holidayService.getGridFiltered(filter)
      .pipe(single());
  }

  public deleteItem(id: string): void {
    this.holidayService
      .delete({id})
      .pipe(single())
      .subscribe(() => {
        this.entityService.holidayChanged.next({entity: {id} as HolidayGridDto, action: 'deleted'});
      });
  }
}
