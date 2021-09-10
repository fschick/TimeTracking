import {Component, OnInit, ViewChild} from '@angular/core';
import {
  Column,
  Configuration,
  DataCellClickEvent,
  DataCellTemplate,
  SimpleTableComponent
} from '../../../shared/components/simple-table/simple-table.component';
import {ProjectListDto, ProjectService} from '../../../shared/services/api';
import {Observable} from 'rxjs';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {ActivatedRoute, Router} from '@angular/router';
import {LocalizationService} from '../../../shared/services/internationalization/localization.service';
import {single} from 'rxjs/operators';
import {GuidService} from '../../../shared/services/state-management/guid.service';

@Component({
  selector: 'ts-master-data-projects',
  templateUrl: './master-data-projects.component.html',
  styleUrls: ['./master-data-projects.component.scss']
})
export class MasterDataProjectsComponent implements OnInit {

  @ViewChild(SimpleTableComponent) private projectTable?: SimpleTableComponent<ProjectListDto>;
  @ViewChild('dataCellTemplate', {static: true}) private dataCellTemplate?: DataCellTemplate<ProjectListDto>;
  @ViewChild('actionCellTemplate', {static: true}) private actionCellTemplate?: DataCellTemplate<ProjectListDto>;

  public rows$: Observable<ProjectListDto[]>;
  public columns!: Column<ProjectListDto>[];
  public configuration?: Partial<Configuration<ProjectListDto>>;

  constructor(
    public guidService: GuidService,
    private entityService: EntityService,
    private router: Router,
    private route: ActivatedRoute,
    private projectService: ProjectService,
    private localizationService: LocalizationService,
  ) {
    this.rows$ = this.projectService.list()
      .pipe(
        single(),
        this.entityService.withUpdatesFrom(this.entityService.projectChanged, this.projectService)
      );
  }

  public ngOnInit(): void {
    this.configuration = {
      cssWrapper: 'table-responsive',
      cssTable: 'table table-borderless table-hover small',
      glyphSortAsc: '',
      glyphSortDesc: '',
      locale: this.localizationService.language,
    };

    const dataCellCss = (row: ProjectListDto) => row.hidden ? 'text-secondary text-decoration-line-through' : '';
    this.columns = [
      {title: $localize`:@@DTO.ProjectListDto.Title:[i18n] Project`, prop: 'title', cssDataCell: dataCellCss, dataCellTemplate: this.dataCellTemplate},
      {
        title: $localize`:@@DTO.ProjectListDto.CustomerTitle:[i18n] Customer`,
        prop: 'customerTitle',
        cssDataCell: dataCellCss,
        dataCellTemplate: this.dataCellTemplate
      },
      {
        title: $localize`:@@Common.Action:[i18n] Action`,
        customId: 'delete',
        dataCellTemplate: this.actionCellTemplate,
        cssDataCell: 'text-nowrap align-middle action-cell',
        width: '1px',
        sortable: false
      },
    ];
  }

  public getDataCellValue(row: ProjectListDto, column: Column<ProjectListDto>): string {
    return this.projectTable?.getCellValue(row, column) ?? '';
  }

  public dataCellClick($event: DataCellClickEvent<ProjectListDto>): void {
    if ($event.column.customId !== 'delete') {
      // noinspection JSIgnoredPromiseFromCall
      this.router.navigate([$event.row.id], {relativeTo: this.route});
    }
  }

  public deleteItem(id: string): void {
    this.projectService
      .delete(id)
      .pipe(single())
      .subscribe(() => {
        this.entityService.projectChanged.next({entity: {id} as ProjectListDto, action: 'deleted'});
      });
  }
}
