import {Component, OnInit, ViewChild} from '@angular/core';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {ActivatedRoute, Router} from '@angular/router';
import {ActivityListDto, ActivityService, CustomerDto} from '../../../shared/services/api';
import {LocalizationService} from '../../../shared/services/internationalization/localization.service';
import {
  Column,
  Configuration,
  DataCellTemplate,
  SimpleTableComponent
} from '../../../shared/components/simple-table/simple-table.component';
import {Observable} from 'rxjs';
import {single} from 'rxjs/operators';
import {GuidService} from '../../../shared/services/state-management/guid.service';

@Component({
  selector: 'ts-master-data-activities',
  templateUrl: './master-data-activities.component.html',
  styleUrls: ['./master-data-activities.component.scss']
})
export class MasterDataActivitiesComponent implements OnInit {

  @ViewChild(SimpleTableComponent) private activityTable?: SimpleTableComponent<ActivityListDto>;
  @ViewChild('dataCellTemplate', {static: true}) private dataCellTemplate?: DataCellTemplate<ActivityListDto>;
  @ViewChild('actionCellTemplate', {static: true}) private actionCellTemplate?: DataCellTemplate<ActivityListDto>;

  public guidService = GuidService;
  public rows$: Observable<ActivityListDto[]>;
  public columns!: Column<ActivityListDto>[];
  public configuration?: Partial<Configuration<ActivityListDto>>;

  constructor(
    private entityService: EntityService,
    private router: Router,
    private route: ActivatedRoute,
    private activityService: ActivityService,
    private localizationService: LocalizationService,
  ) {
    this.rows$ = this.activityService.list()
      .pipe(
        single(),
        this.entityService.withUpdatesFrom(this.entityService.activityChanged, this.activityService)
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
        title: $localize`:@@DTO.ActivityListDto.Title:[i18n] Activity`,
        prop: 'title',
        cssHeadCell: cssHeadCell,
        dataCellTemplate: this.dataCellTemplate
      }, {
        title: $localize`:@@DTO.ActivityListDto.ProjectTitle:[i18n] Project`,
        prop: 'projectTitle',
        cssHeadCell: cssHeadCell,
        dataCellTemplate: this.dataCellTemplate
      }, {
        title: $localize`:@@DTO.ActivityListDto.CustomerTitle:[i18n] Customer`,
        prop: 'customerTitle',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd}`,
        cssDataCell: cssDataCellMd,
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

  public deleteItem(id: string): void {
    this.activityService
      .delete(id)
      .pipe(single())
      .subscribe(() => {
        this.entityService.activityChanged.next({entity: {id} as ActivityListDto, action: 'deleted'});
      });
  }
}
