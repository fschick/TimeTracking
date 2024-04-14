import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {Filter, FilteredRequestParams, FilterName} from '../../../../core/app/components/filter/filter.component';
import {DateTime} from 'luxon';
import {single, switchMap} from 'rxjs/operators';
import {EntityService} from '../../../../core/app/services/state-management/entity.service';
import {Observable, Subscription} from 'rxjs';
import {ActivityReportGridDto, ActivityReportService} from '../../../../api/timetracking';
import {LocalizationService} from '../../../../core/app/services/internationalization/localization.service';
import {
  Column,
  Configuration,
  DataCellTemplate
} from '../../../../core/app/components/simple-table/simple-table.component';
import {FormatService} from '../../../../core/app/services/format.service';
import {UtilityService} from '../../../../core/app/services/utility.service';
import {AuthenticationService, User} from "../../../../core/app/services/authentication.service";

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

  private readonly LOCALIZED_DAYS = $localize`:@@Abbreviations.Days:[i18n] days`;

  constructor(
    private entityService: EntityService,
    private localizationService: LocalizationService,
    private activityReportService: ActivityReportService,
    private formatService: FormatService,
    private utilityService: UtilityService,
    private authenticationService: AuthenticationService,
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
      {name: 'timeSheetBillable', isPrimary: true, defaultValue: true},
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
    this.columns = [
      {
        title: $localize`:@@Page.Report.ActivityOverview.Customer:[i18n] Customer`,
        prop: 'customerTitle',
        cssFooterCell: 'fw-bold',
        footer: $localize`:@@Common.Total:[i18n] Total`,
      }, {
        title: $localize`:@@Page.Report.ActivityOverview.ActivityReport:[i18n] Activity report`,
        prop: 'dailyActivityReportUrl',
        dataCellTemplate: this.dailyActivityReportDownloadTemplate,
        width: '1%',
        cssHeadCell: 'text-center text-nowrap',
        cssDataCell: 'text-center text-nowrap',
        sortable: false,
      }, {
        title: $localize`:@@Page.Report.ActivityOverview.ActivityReportDetailed:[i18n] Detailed`,
        prop: 'detailedActivityReportUrl',
        dataCellTemplate: this.detailedActivityReportDownloadTemplate,
        width: '1%',
        cssHeadCell: 'text-center text-nowrap pe-3',
        cssDataCell: 'text-center text-nowrap pe-3',
        sortable: false,
      }, {
        title: $localize`:@@Page.Report.ActivityOverview.WorkedTime:[i18n] Worked time`,
        prop: 'timeWorked',
        width: '1%',
        cssHeadCell: 'text-end text-nowrap pe-3',
        cssDataCell: 'text-end text-nowrap pe-3',
        cssFooterCell: 'text-end text-nowrap pe-3 fw-bold',
        format: row => `${this.formatService.formatDays(row.daysWorked)} ${this.LOCALIZED_DAYS}`,
        footer: () => this.rows ? `${this.formatService.formatDays(this.utilityService.sum(this.rows.map(row => row.daysWorked)))} ${this.LOCALIZED_DAYS}` : '',
      },
      {
        title: $localize`:@@Page.Report.ActivityOverview.Revenue:[i18n] Revenue`,
        prop: 'budgetWorked',
        width: '1%',
        cssHeadCell: 'text-end text-nowrap',
        cssDataCell: 'text-end text-nowrap',
        cssFooterCell: 'text-end text-nowrap fw-bold',
        format: row => `${this.formatService.formatBudget(row.budgetWorked)} ${row.currency}`,
        footer: () => this.rows ? `${this.formatService.formatBudget(this.utilityService.sum(this.rows.map(row => row.budgetWorked)))} ${this.rows[0]?.currency ?? ''}` : '',
      },
    ];

    if (!this.authenticationService.currentUser.hasRole.reportActivitySummaryView)
      this.columns = this.columns.filter(column => column.prop != 'dailyActivityReportUrl');

    if (!this.authenticationService.currentUser.hasRole.reportActivityDetailView)
      this.columns = this.columns.filter(column => column.prop != 'detailedActivityReportUrl');

    const loadTimeSheets = this.entityService.reloadRequested
      .pipe(switchMap(filter => this.loadOverview(filter)))
      .subscribe(customers => {
        this.rows = customers;
      });
    this.subscriptions.add(loadTimeSheets);
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private loadOverview(requestParameters: FilteredRequestParams): Observable<ActivityReportGridDto[]> {
    return this.activityReportService.getCustomersHavingTimeSheets({
      ...requestParameters,
      language: this.localizationService.language
    });
  }

  public refreshDailyReportDownloadToken(row: ActivityReportGridDto) {
    this.activityReportService.getDailyActivityReportDownloadToken()
      .pipe(single())
      .subscribe(newToken => {
        if (row.dailyActivityReportUrl)
          row.dailyActivityReportUrl = row.dailyActivityReportUrl.replace(/&accessToken=.*?(&|$)/, `&accessToken=${newToken}`);
      });
  }

  public refreshDetailedReportDownloadToken(row: ActivityReportGridDto) {
    this.activityReportService.getDetailedActivityReportDownloadToken()
      .pipe(single())
      .subscribe(newToken => {
        if (row.detailedActivityReportUrl)
          row.detailedActivityReportUrl = row.detailedActivityReportUrl.replace(/&accessToken=.*?(&|$)/, `&accessToken=${newToken}`);
      });
  }
}
