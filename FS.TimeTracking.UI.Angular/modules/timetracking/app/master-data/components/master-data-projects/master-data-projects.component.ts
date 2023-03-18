import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {
  Column,
  Configuration,
  DataCellTemplate,
  SimpleTableComponent
} from '../../../../../core/app/components/simple-table/simple-table.component';
import {ProjectGridDto, ProjectService} from '../../../../../api/timetracking';
import {Observable, Subscription} from 'rxjs';
import {EntityService} from '../../../../../core/app/services/state-management/entity.service';
import {ActivatedRoute, Router} from '@angular/router';
import {LocalizationService} from '../../../../../core/app/services/internationalization/localization.service';
import {single, switchMap} from 'rxjs/operators';
import {GuidService} from '../../../../../core/app/services/state-management/guid.service';
import {Filter, FilteredRequestParams, FilterName} from '../../../../../core/app/components/filter/filter.component';
import {AuthenticationService} from '../../../../../core/app/services/authentication.service';

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
  public createNewDisabled: boolean;

  private readonly subscriptions = new Subscription();

  constructor(
    private entityService: EntityService,
    private router: Router,
    private route: ActivatedRoute,
    private projectService: ProjectService,
    private localizationService: LocalizationService,
    authenticationService: AuthenticationService,
  ) {
    this.createNewDisabled = !authenticationService.currentUser.hasRole.masterDataProjectsManage;

    this.filters = [
      {name: 'projectId', showHidden: true, isPrimary: true},
      {name: 'customerId', showHidden: true, isPrimary: true},
      {name: 'projectHidden', isPrimary: true},
    ];
  }

  public ngOnInit(): void {
    const filterChanged = this.entityService.reloadRequested
      .pipe(
        switchMap(filter => this.loadData(filter)),
        this.entityService.withUpdatesFrom(this.entityService.projectChanged, this.projectService),
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

  private loadData(filter: FilteredRequestParams): Observable<ProjectGridDto[]> {
    return this.projectService.getGridFiltered(filter)
      .pipe(single());
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
