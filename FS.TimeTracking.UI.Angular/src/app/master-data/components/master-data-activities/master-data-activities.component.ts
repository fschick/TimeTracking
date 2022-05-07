import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {ActivatedRoute, Router} from '@angular/router';
import {ActivityGridDto, ActivityService} from '../../../shared/services/api';
import {LocalizationService} from '../../../shared/services/internationalization/localization.service';
import {Column, Configuration, DataCellTemplate, SimpleTableComponent} from '../../../shared/components/simple-table/simple-table.component';
import {Observable, Subscription} from 'rxjs';
import {single, switchMap} from 'rxjs/operators';
import {GuidService} from '../../../shared/services/state-management/guid.service';
import {Filter, FilteredRequestParams, FilterName} from '../../../shared/components/filter/filter.component';

@Component({
  selector: 'ts-master-data-activities',
  templateUrl: './master-data-activities.component.html',
  styleUrls: ['./master-data-activities.component.scss']
})
export class MasterDataActivitiesComponent implements OnInit, OnDestroy {

  @ViewChild(SimpleTableComponent) private activityTable?: SimpleTableComponent<ActivityGridDto>;
  @ViewChild('dataCellTemplate', {static: true}) private dataCellTemplate?: DataCellTemplate<ActivityGridDto>;
  @ViewChild('actionCellTemplate', {static: true}) private actionCellTemplate?: DataCellTemplate<ActivityGridDto>;

  public guidService = GuidService;
  public rows?: ActivityGridDto[];
  public columns!: Column<ActivityGridDto>[];
  public configuration?: Partial<Configuration<ActivityGridDto>>;
  public filters: (Filter | FilterName)[];
  private readonly subscriptions = new Subscription();

  constructor(
    private entityService: EntityService,
    private router: Router,
    private route: ActivatedRoute,
    private activityService: ActivityService,
    private localizationService: LocalizationService,
  ) {
    this.filters = [
      {name: 'activityId', showHidden: true, isPrimary: true},
      {name: 'projectId', showHidden: true, isPrimary: true},
      {name: 'customerId', showHidden: true, isPrimary: true},
      {name: 'activityHidden', isPrimary: true},
    ];
  }

  public ngOnInit(): void {
    const filterChanged = this.entityService.filterChanged
      .pipe(
        switchMap(filter => this.loadData(filter)),
        this.entityService.withUpdatesFrom(this.entityService.activityChanged, this.activityService),
      )
      .subscribe(rows => this.rows = rows);
    this.subscriptions.add(filterChanged);

    this.configuration = {
      cssWrapper: 'table-responsive',
      cssTable: 'table',
      glyphSortAsc: '',
      glyphSortDesc: '',
      locale: this.localizationService.language,
    };

    const cssHeadCell = 'border-0 text-nowrap';
    const cssHeadCellMd = 'd-none d-md-table-cell';
    const cssDataCellMd = cssHeadCellMd;
    this.columns = [
      {
        title: $localize`:@@DTO.ActivityGridDto.Title:[i18n] Activity`,
        prop: 'title',
        cssHeadCell: cssHeadCell,
        dataCellTemplate: this.dataCellTemplate
      }, {
        title: $localize`:@@DTO.ActivityGridDto.ProjectTitle:[i18n] Project`,
        prop: 'projectTitle',
        cssHeadCell: cssHeadCell,
        dataCellTemplate: this.dataCellTemplate
      }, {
        title: $localize`:@@DTO.ActivityGridDto.CustomerTitle:[i18n] Customer`,
        prop: 'customerTitle',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd}`,
        cssDataCell: cssDataCellMd,
        dataCellTemplate: this.dataCellTemplate
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
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private loadData(filter: FilteredRequestParams): Observable<ActivityGridDto[]> {
    return this.activityService.getGridFiltered(filter)
      .pipe(single());
  }

  public deleteItem(id: string): void {
    this.activityService
      .delete({id})
      .pipe(single())
      .subscribe(() => {
        this.entityService.activityChanged.next({entity: {id} as ActivityGridDto, action: 'deleted'});
      });
  }
}
