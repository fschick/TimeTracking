import {Component, ElementRef, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {Filter, FilteredRequestParams, FilterName} from '../../../../core/app/components/filter/filter.component';
import {DomSanitizer, SafeUrl} from '@angular/platform-browser';
import {Observable, Subscription} from 'rxjs';
import {EntityService} from '../../../../core/app/services/state-management/entity.service';
import {ActivityReportService, ActivityReportType, ReportPreviewDto} from '../../../../api/timetracking';
import {DateTime} from 'luxon';
import {switchMap} from 'rxjs/operators';
import {HttpParams} from '@angular/common/http';
import {LocalizationService} from '../../../../core/app/services/internationalization/localization.service';
import {ActivatedRoute, Router} from '@angular/router';
import {$localizeId} from '../../../../core/app/services/internationalization/localizeId';
import {UtilityService} from '../../../../core/app/services/utility.service';

@Component({
  selector: 'ts-report-activity-preview',
  templateUrl: './report-activity-preview.component.html',
  styleUrls: ['./report-activity-preview.component.scss']
})
export class ReportActivityPreviewComponent implements OnInit, OnDestroy {
  public title: string;
  public filters: (Filter | FilterName)[];
  public previewImages?: SafeUrl[];
  public totalReportPages?: number;
  public downloadLink?: string;

  @ViewChild('reportPreview') private reportPreview?: ElementRef;
  private readonly subscriptions = new Subscription();
  private readonly reportType: ActivityReportType;

  constructor(
    router: Router,
    route: ActivatedRoute,
    utilityService: UtilityService,
    private entityService: EntityService,
    private localizationService: LocalizationService,
    private activityReportService: ActivityReportService,
    private sanitizer: DomSanitizer,
  ) {
    router.routeReuseStrategy.shouldReuseRoute = () => false;

    const defaultStartDate = DateTime.now().minus({month: 1}).startOf('month');
    const defaultEndDate = DateTime.now().minus({month: 1}).endOf('month');

    this.reportType = route.snapshot.params['reportType'];
    const titleTranslationId = `@@Page.Report.Activity.Title${utilityService.capitalize(this.reportType)}`;
    this.title = $localizeId`${titleTranslationId}:TRANSUNITID:`;

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
  }

  public ngOnInit(): void {
    const loadTimeSheets = this.entityService.reloadRequested
      .pipe(switchMap(filter => this.loadPreview(filter)))
      .subscribe(preview => {
        this.totalReportPages = preview.totalPages;
        this.previewImages = preview.pages?.map(pageData => this.sanitizer.bypassSecurityTrustUrl(`data:image/png;base64,${pageData}`)) ?? [];
      });
    this.subscriptions.add(loadTimeSheets);
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private loadPreview(requestParameters: FilteredRequestParams): Observable<ReportPreviewDto> {
    const reportRequestParameters = {
      ...requestParameters,
      language: this.localizationService.language,
    };

    this.downloadLink = this.createDownloadLink(reportRequestParameters);
    switch (this.reportType) {
      case 'daily':
        return this.activityReportService.getDailyActivityReportPreview(reportRequestParameters);
      case 'detailed':
        return this.activityReportService.getDetailedActivityReportPreview(reportRequestParameters);
    }
  }

  private createDownloadLink(requestParameters: FilteredRequestParams): string {
    let httpParams = new HttpParams({encoder: this.activityReportService.encoder});

    for (const [key, value] of Object.entries(requestParameters))
      httpParams = httpParams.append(key, value);

    return `${this.activityReportService.configuration.basePath}/api/v1/ActivityReport/GetActivityReport?${httpParams.toString()}`
  }
}
