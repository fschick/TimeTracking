import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {
  Column,
  Configuration,
  DataCellClickEvent,
  DataCellTemplate,
  SimpleTableComponent
} from '../../../shared/components/simple-table/simple-table.component';
import {ProjectListDto, ProjectService} from '../../../shared/services/api';
import {Subscription} from 'rxjs';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {ActivatedRoute, Router} from '@angular/router';
import {StorageService} from '../../../shared/services/storage/storage.service';
import {single} from 'rxjs/operators';

@Component({
  selector: 'ts-master-data-projects',
  templateUrl: './master-data-projects.component.html',
  styleUrls: ['./master-data-projects.component.scss']
})
export class MasterDataProjectsComponent implements OnInit, OnDestroy {

  @ViewChild(SimpleTableComponent) private projectTable?: SimpleTableComponent<ProjectListDto>;
  @ViewChild('editCellTemplate', {static: true}) private editCellTemplate?: DataCellTemplate<ProjectListDto>;
  @ViewChild('dataCellTemplate', {static: true}) private dataCellTemplate?: DataCellTemplate<ProjectListDto>;

  public rows: ProjectListDto[];
  public columns!: Column<ProjectListDto>[];
  public configuration?: Partial<Configuration<ProjectListDto>>;

  private subscriptions = new Subscription();

  constructor(
    public entityService: EntityService,
    private router: Router,
    private route: ActivatedRoute,
    private projectService: ProjectService,
    private storageService: StorageService,
  ) {
    this.rows = [];
  }

  public ngOnInit(): void {
    this.projectService.list().pipe(single()).subscribe(x => this.rows = x);

    const projectChanged = this.entityService.projectChanged
      .pipe(this.entityService.replaceEntityWithOverviewDto(this.projectService))
      .subscribe(changedEvent => {
          const updatedRows = this.entityService.updateCollection(this.rows, 'id', changedEvent);
          return this.rows = [...updatedRows];
        }
      );
    this.subscriptions.add(projectChanged);

    this.configuration = {
      cssWrapper: 'table-responsive',
      cssTable: 'table table-borderless table-hover small',
      glyphSortAsc: '',
      glyphSortDesc: '',
      locale: this.storageService.language,
    };

    const dataCellCss = (row: ProjectListDto) => row.hidden ? 'text-secondary text-decoration-line-through' : '';
    this.columns = [
      {title: '', prop: 'id', dataCellTemplate: this.editCellTemplate, cssDataCell: 'text-nowrap', width: '1px', sortable: false},
      {title: $localize`:@@DTO.ProjectOverviewDto.Name:[i18n] Project`, prop: 'name', cssDataCell: dataCellCss, dataCellTemplate: this.dataCellTemplate},
      {
        title: $localize`:@@DTO.ProjectOverviewDto.CustomerShortName:[i18n] Customer`,
        prop: 'customerShortName',
        cssDataCell: dataCellCss,
        dataCellTemplate: this.dataCellTemplate
      },
      {
        title: $localize`:@@DTO.ProjectOverviewDto.CustomerCompanyName:[i18n] Company`,
        prop: 'customerCompanyName',
        cssDataCell: dataCellCss,
        dataCellTemplate: this.dataCellTemplate
      },
    ];
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  public getDataCellValue(row: ProjectListDto, column: Column<ProjectListDto>): string {
    return this.projectTable?.getCellValue(row, column) ?? '';
  }

  dataCellClick($event: DataCellClickEvent<ProjectListDto>) {
    this.router.navigate([$event.row.id], {relativeTo: this.route});
  }
}
