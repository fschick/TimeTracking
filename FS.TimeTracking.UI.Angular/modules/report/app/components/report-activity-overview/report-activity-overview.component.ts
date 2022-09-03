import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {Filter, FilteredRequestParams, FilterName} from '../../../../core/app/components/filter/filter.component';
import {DateTime} from 'luxon';
import {switchMap} from 'rxjs/operators';
import {EntityService} from '../../../../core/app/services/state-management/entity.service';
import {Observable, Subscription} from 'rxjs';
import {ActivityReportGridDto, ActivityReportService} from '../../../../api/timetracking';
import {HttpParams} from '@angular/common/http';
import {LocalizationService} from '../../../../core/app/services/internationalization/localization.service';
import {Column, Configuration, DataCellTemplate} from '../../../../core/app/components/simple-table/simple-table.component';

@Component({
  selector: 'ts-report-activity-overview',
  templateUrl: './report-activity-overview.component.html',
  styleUrls: ['./report-activity-overview.component.scss']
})
export class ReportActivityOverviewComponent implements OnInit, OnDestroy {
  public filters: (Filter | FilterName)[];
  public apiBasePath?: string;
  public rows?: ActivityReportGridDto[];
  public columns!: Column<ActivityReportGridDto>[];
  public configuration: Partial<Configuration<ActivityReportGridDto>>;

  @ViewChild('dailyActivityReportDownloadTemplate', {static: true}) private dailyActivityReportDownloadTemplate?: DataCellTemplate<ActivityReportGridDto>;
  @ViewChild('detailedActivityReportDownloadTemplate', {static: true}) private detailedActivityReportDownloadTemplate?: DataCellTemplate<ActivityReportGridDto>;

  private readonly subscriptions = new Subscription();

  constructor(
    private entityService: EntityService,
    private localizationService: LocalizationService,
    private activityReportService: ActivityReportService,
  ) {
    const defaultStartDate = DateTime.now().minus({month: 1}).startOf('month');
    const defaultEndDate = DateTime.now().minus({month: 1}).endOf('month');

    this.apiBasePath = this.activityReportService.configuration.basePath;

    this.filters = [
      {name: 'timeSheetStartDate', defaultValue: defaultStartDate, isPrimary: true},
      {name: 'timeSheetEndDate', defaultValue: defaultEndDate, isPrimary: true},
      {name: 'customerId', isPrimary: true},
      {name: 'projectId', isPrimary: true},
      {name: 'orderId'},
      {name: 'activityId'},
      {name: 'timeSheetIssue'},
      {name: 'timeSheetComment'},
      {name: 'timeSheetBillable', defaultValue: true},
    ];

    this.configuration = {
      cssWrapper: 'table-responsive',
      cssTable: 'table',
      glyphSortAsc: '',
      glyphSortDesc: '',
      locale: this.localizationService.language,
    };
  }

  public ngOnInit(): void {
    const cssHeadCell = 'text-nowrap';
    this.columns = [
      {
        title: $localize`:@@Page.Report.Activity.Customer:[i18n] Customer`,
        prop: 'customerTitle',
        width: '40%',
        cssDataCell: 'align-middle',
      }, {
        title: $localize`:@@Page.Report.Activity.TitleDaily:[i18n] Daily activity report`,
        prop: 'dailyActivityReportUrl',
        dataCellTemplate: this.dailyActivityReportDownloadTemplate,
        cssHeadCell: 'text-center',
        cssDataCell: 'text-center align-middle',
        sortable: false,
      }, {
        title: $localize`:@@Page.Report.Activity.TitleDetailed:[i18n] Detailed activity report`,
        prop: 'detailedActivityReportUrl',
        dataCellTemplate: this.detailedActivityReportDownloadTemplate,
        cssHeadCell: 'text-center',
        cssDataCell: 'text-center align-middle',
        sortable: false,
      },
    ];

    const loadTimeSheets = this.entityService.filterChanged
      .pipe(switchMap(requestParameters => this.loadOverview(requestParameters)))
      .subscribe(customers => {
        this.rows = customers;
      });
    this.subscriptions.add(loadTimeSheets);
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private loadOverview(requestParameters: FilteredRequestParams): Observable<ActivityReportGridDto[]> {
    return this.activityReportService.getCustomersHavingTimeSheets({...requestParameters, language: this.localizationService.language});
  }
}
