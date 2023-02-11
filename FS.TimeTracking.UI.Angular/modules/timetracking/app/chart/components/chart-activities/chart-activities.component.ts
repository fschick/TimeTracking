import {ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {Column, Configuration, DataCellTemplate, FooterCellTemplate} from '../../../../../core/app/components/simple-table/simple-table.component';
import {ActivityChartService, ActivityWorkTimeDto} from '../../../../../api/timetracking';
import {Observable, Subscription} from 'rxjs';
import {Filter, FilteredRequestParams, FilterName} from '../../../../../core/app/components/filter/filter.component';
import {ChartOptions, ChartService} from '../../services/chart.service';
import {ApexAxisChartSeries} from 'ng-apexcharts';
import {FormatService} from '../../../../../core/app/services/format.service';
import {LocalizationService} from '../../../../../core/app/services/internationalization/localization.service';
import {DateTime} from 'luxon';
import {single, switchMap} from 'rxjs/operators';
import {UtilityService} from '../../../../../core/app/services/utility.service';
import {EntityService} from '../../../../../core/app/services/state-management/entity.service';

@Component({
  selector: 'ts-chart-activities',
  templateUrl: './chart-activities.component.html',
  styleUrls: ['./chart-activities.component.scss']
})
export class ChartActivitiesComponent implements OnInit, OnDestroy {
  @ViewChild('infoCellTemplate', {static: true}) private infoCellTemplate?: DataCellTemplate<ActivityWorkTimeDto>;
  @ViewChild('orderPeriodHeadTemplate', {static: true}) private orderPeriodHeadTemplate?: DataCellTemplate<ActivityWorkTimeDto>;
  @ViewChild('orderPeriodDataTemplate', {static: true}) private orderPeriodDataTemplate?: DataCellTemplate<ActivityWorkTimeDto>;
  @ViewChild('infoFooterTemplate', {static: true}) private infoFooterTemplate?: FooterCellTemplate<ActivityWorkTimeDto>;

  public filters: (Filter | FilterName)[];
  public chartOptions: ChartOptions;
  public chartSeries?: ApexAxisChartSeries;

  public tableConfiguration: Partial<Configuration<ActivityWorkTimeDto>>;
  public tableColumns?: Column<ActivityWorkTimeDto>[];
  public tableRows: ActivityWorkTimeDto[] = [];
  public tableFooter: Partial<ActivityWorkTimeDto> = {};
  private readonly subscriptions = new Subscription();

  public readonly LOCALIZED_DAYS = $localize`:@@Abbreviations.Days:[i18n] days`;

  constructor(
    public formatService: FormatService,
    private utilityService: UtilityService,
    private activityChartService: ActivityChartService,
    private localizationService: LocalizationService,
    private chartService: ChartService,
    private changeDetector: ChangeDetectorRef,
    private entityService: EntityService,
  ) {
    const defaultStartDate = DateTime.now().startOf('year');
    const defaultEndDate = DateTime.now().endOf('year');

    this.filters = [
      {name: 'timeSheetStartDate', defaultValue: defaultStartDate, isPrimary: true},
      {name: 'timeSheetEndDate', defaultValue: defaultEndDate, isPrimary: true},
      {name: 'customerId', isPrimary: true, showHidden: true},
      {name: 'orderId', isPrimary: true, showHidden: true},
      {name: 'projectId', showHidden: true},
      {name: 'activityId', showHidden: true},
      {name: 'timeSheetIssue'},
      {name: 'timeSheetBillable', isPrimary: true, defaultValue: true},
      {name: 'userId'},
    ];

    this.chartOptions = this.chartService.createChartOptions();
    this.tableConfiguration = this.createTableConfiguration();
  }

  public ngOnInit(): void {
    const filterChanged = this.entityService.reloadRequested
      .pipe(switchMap(filter => this.loadData(filter)))
      .subscribe(rows => this.setTableData(rows));
    this.subscriptions.add(filterChanged);

    this.tableColumns = this.createTableColumns();
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  public tableRowsChanged(rows: Array<ActivityWorkTimeDto>): void {
    const maxYValue = Math.max(...rows.map(row => row.daysWorked));
    this.chartOptions = this.chartService.createChartOptions(rows.length, maxYValue);
    this.chartSeries = this.createSeries(rows);
    this.changeDetector.detectChanges();
  }

  private loadData(filter: FilteredRequestParams): Observable<ActivityWorkTimeDto[]> {
    return this.activityChartService.getWorkTimesPerActivity(filter).pipe(single());
  }

  private setTableData(rows: ActivityWorkTimeDto[]) {
    this.tableRows = rows;
    this.tableFooter = {
      daysWorked: this.utilityService.sum(rows.map(row => row.daysWorked)),
      timeWorked: this.utilityService.sumDuration(rows.map(row => row.timeWorked)),
      budgetWorked: this.utilityService.sum(rows.map(row => row.budgetWorked)),
      totalWorkedPercentage: 1,
      currency: rows[0]?.currency,
    };
  }

  private createSeries(workTimes: ActivityWorkTimeDto[]): ApexAxisChartSeries {
    return [
      {
        name: $localize`:@@Page.Chart.Common.Worked:[i18n] Worked`,
        data: workTimes.map(workTime => ({
          x: workTime.activityTitle,
          y: workTime.daysWorked,
          meta: {days: workTime.daysWorked, time: workTime.timeWorked}
        }))
      }
    ];
  }

  private createTableConfiguration(): Partial<Configuration<ActivityWorkTimeDto>> {
    return {
      cssWrapper: 'table-responsive',
      cssTable: 'table',
      cssDataRow: row => row.activityHidden ? 'text-secondary' : '',
      cssFooterRow: 'fw-bold',
      glyphSortAsc: '',
      glyphSortDesc: '',
      locale: this.localizationService.language,
    };
  }

  private createTableColumns(): Column<ActivityWorkTimeDto>[] {
    const cssHeadCell = 'border-0 text-nowrap';
    const cssHeadCellLg = 'd-none d-lg-table-cell';
    const cssDataCellLg = cssHeadCellLg;

    return [
      {
        title: $localize`:@@DTO.WorkTimeDto.ActivityTitle:[i18n] Activity`,
        prop: 'activityTitle',
        cssHeadCell: cssHeadCell,
        footer: $localize`:@@Common.Total:[i18n] Total`,
      }, {
        title: $localize`:@@Page.Chart.Common.DaysWorked:[i18n] Days worked`,
        prop: 'daysWorked',
        cssHeadCell: `${cssHeadCell} text-nowrap text-end`,
        cssDataCell: 'text-nowrap text-end',
        cssFooterCell: 'text-nowrap text-end',
        format: row => `${this.formatService.formatDays(row.daysWorked)} ${this.LOCALIZED_DAYS}`,
        footer: () => `${this.formatService.formatDays(this.tableFooter.daysWorked)} ${this.LOCALIZED_DAYS}`,
      }, {
        title: $localize`:@@Page.Chart.Common.PercentageOfTotalTime:[i18n] Percentage of total time`,
        prop: 'totalWorkedPercentage',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellLg} text-end`,
        cssDataCell: `${cssDataCellLg} text-nowrap text-end`,
        cssFooterCell: `${cssDataCellLg} text-nowrap text-end`,
        format: row => `${this.formatService.formatRatio(row.totalWorkedPercentage)} %`,
        footer: () => `${this.formatService.formatRatio(this.tableFooter.totalWorkedPercentage)} %`,
      }, {
        title: '',
        customId: 'info',
        cssHeadCell: `${cssHeadCell} ps-3 text-center`,
        cssDataCell: 'ps-3 text-center',
        cssFooterCell: 'ps-3 text-center',
        dataCellTemplate: this.infoCellTemplate,
        footerCellTemplate: this.infoFooterTemplate,
        sortable: false,
        width: '1%',
      }
    ];
  }
}
