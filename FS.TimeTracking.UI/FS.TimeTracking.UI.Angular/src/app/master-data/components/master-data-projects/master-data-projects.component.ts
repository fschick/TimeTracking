import {Component, OnInit, ViewChild} from '@angular/core';
import {
  Column,
  Configuration,
  DataCellTemplate,
  SimpleTableComponent
} from '../../../shared/components/simple-table/simple-table.component';
import {ActivityListDto, ProjectListDto, ProjectService} from '../../../shared/services/api';
import {Observable, Subject} from 'rxjs';
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
export class MasterDataProjectsComponent implements OnInit {

  @ViewChild(SimpleTableComponent) private projectTable?: SimpleTableComponent<ProjectListDto>;
  @ViewChild('dataCellTemplate', {static: true}) private dataCellTemplate?: DataCellTemplate<ProjectListDto>;
  @ViewChild('actionCellTemplate', {static: true}) private actionCellTemplate?: DataCellTemplate<ProjectListDto>;

  public guidService = GuidService;
  public rows$: Observable<ProjectListDto[]>;
  public columns!: Column<ProjectListDto>[];
  public configuration?: Partial<Configuration<ProjectListDto>>;
  public filters: (Filter | FilterName)[];
  public filterChanged = new Subject<FilteredRequestParams>();

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

    this.rows$ = this.filterChanged
      .pipe(
        switchMap(filter => this.loadData(filter)),
        this.entityService.withUpdatesFrom(this.entityService.projectChanged, this.projectService),
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
    this.columns = [
      {
        title: $localize`:@@DTO.ProjectListDto.Title:[i18n] Project`,
        prop: 'title',
        cssHeadCell: cssHeadCell,
        dataCellTemplate: this.dataCellTemplate
      }, {
        title: $localize`:@@DTO.ProjectListDto.CustomerTitle:[i18n] Customer`,
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

  private loadData(filter: FilteredRequestParams): Observable<ProjectListDto[]> {
    return this.projectService.getListFiltered(filter)
      .pipe(single());
  }

  public getDataCellValue(row: ProjectListDto, column: Column<ProjectListDto>): string {
    return this.projectTable?.getCellValue(row, column) ?? '';
  }

  public deleteItem(id: string): void {
    this.projectService
      .delete({id})
      .pipe(single())
      .subscribe(() => {
        this.entityService.projectChanged.next({entity: {id} as ProjectListDto, action: 'deleted'});
      });
  }
}
