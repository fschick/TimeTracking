import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {
  Column,
  Configuration,
  DataCellTemplate,
  SimpleTableComponent
} from '../../../shared/components/simple-table/simple-table.component';
import {ProjectGridDto, ProjectService} from '../../../shared/services/api';
import {Observable, Subscription} from 'rxjs';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {ActivatedRoute, Router} from '@angular/router';
import {LocalizationService} from '../../../shared/services/internationalization/localization.service';
import {single, switchMap} from 'rxjs/operators';
import {GuidService} from '../../../shared/services/state-management/guid.service';
import {Filter, FilteredRequestParams, FilterName} from '../../../shared/components/filter/filter.component';

@Component({
  selector: 'ts-master-data-projects',
  templateUrl: './master-data-projects.component.html',
  styleUrls: ['./master-data-projects.component.scss']
})
export class MasterDataProjectsComponent implements OnInit, OnDestroy {

  @ViewChild(SimpleTableComponent) private projectTable?: SimpleTableComponent<ProjectGridDto>;
  @ViewChild('dataCellTemplate', {static: true}) private dataCellTemplate?: DataCellTemplate<ProjectGridDto>;
  @ViewChild('actionCellTemplate', {static: true}) private actionCellTemplate?: DataCellTemplate<ProjectGridDto>;

  public guidService = GuidService;
  public rows?: ProjectGridDto[];
  public columns!: Column<ProjectGridDto>[];
  public configuration?: Partial<Configuration<ProjectGridDto>>;
  public filters: (Filter | FilterName)[];
  private readonly subscriptions = new Subscription();

  constructor(
    private entityService: EntityService,
    private router: Router,
    private route: ActivatedRoute,
    private projectService: ProjectService,
    private localizationService: LocalizationService,
  ) {
    this.filters = [
      {name: 'projectId', showHidden: true},
      {name: 'customerId', showHidden: true},
      {name: 'projectHidden'},
    ];
  }

  public ngOnInit(): void {
    const filterChanged = this.entityService.filterChanged
      .pipe(
        switchMap(filter => this.loadData(filter)),
        this.entityService.withUpdatesFrom(this.entityService.projectChanged, this.projectService),
      )
      .subscribe(rows => this.rows = rows);
    this.subscriptions.add(filterChanged);

    this.configuration = {
      cssWrapper: 'table-responsive',
      cssTable: 'table table-card table-sm align-middle text-break border',
      glyphSortAsc: '',
      glyphSortDesc: '',
      locale: this.localizationService.language,
    };

    const cssHeadCell = 'border-0 text-nowrap';
    this.columns = [
      {
        title: $localize`:@@DTO.ProjectGridDto.Title:[i18n] Project`,
        prop: 'title',
        cssHeadCell: cssHeadCell,
        dataCellTemplate: this.dataCellTemplate
      }, {
        title: $localize`:@@DTO.ProjectGridDto.CustomerTitle:[i18n] Customer`,
        prop: 'customerTitle',
        cssHeadCell: cssHeadCell,
        dataCellTemplate: this.dataCellTemplate
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

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private loadData(filter: FilteredRequestParams): Observable<ProjectGridDto[]> {
    return this.projectService.getGridFiltered(filter)
      .pipe(single());
  }

  public getDataCellValue(row: ProjectGridDto, column: Column<ProjectGridDto>): string {
    return this.projectTable?.getCellValue(row, column) ?? '';
  }

  public deleteItem(id: string): void {
    this.projectService
      .delete({id})
      .pipe(single())
      .subscribe(() => {
        this.entityService.projectChanged.next({entity: {id} as ProjectGridDto, action: 'deleted'});
      });
  }
}
