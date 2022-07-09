import {Component, ElementRef, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {Filter, FilteredRequestParams, FilterName} from '../../../../../core/app/components/filter/filter.component';
import {DateTime} from 'luxon';
import {switchMap} from 'rxjs/operators';
import {EntityService} from '../../../../../core/app/services/state-management/entity.service';
import {Observable, Subscription} from 'rxjs';
import {ActivityReportService, ReportPreviewDto} from '../../../../../api/timetracking';
import {DomSanitizer, SafeUrl} from '@angular/platform-browser';
import {HttpParams} from '@angular/common/http';

@Component({
  selector: 'ts-report-activity-overview',
  templateUrl: './report-activity-overview.component.html',
  styleUrls: ['./report-activity-overview.component.scss']
})
export class ReportActivityOverviewComponent implements OnInit, OnDestroy {
  public filters: (Filter | FilterName)[];
  public previewImages?: SafeUrl[];
  public totalReportPages?: number;
  public downloadLink?: string;

  @ViewChild('reportPreview') private reportPreview?: ElementRef;
  private readonly subscriptions = new Subscription();

  constructor(
    private entityService: EntityService,
    private activityReportService: ActivityReportService,
    private sanitizer: DomSanitizer,
  ) {
    const defaultStartDate = DateTime.now().minus({month: 1}).startOf('month');
    const defaultEndDate = DateTime.now().minus({month: 1}).endOf('month');

    this.filters = [
      {name: 'timeSheetStartDate', defaultValue: defaultStartDate, isPrimary: true},
      {name: 'timeSheetEndDate', defaultValue: defaultEndDate, isPrimary: true},
      {name: 'customerId', isPrimary: true},
      {name: 'projectId', isPrimary: true},
      {name: 'orderId'},
      {name: 'activityId'},
      {name: 'timeSheetIssue'},
      {name: 'timeSheetComment'},
      {name: 'timeSheetBillable'},
    ];
  }

  public ngOnInit(): void {
    const loadTimeSheets = this.entityService.filterChanged
      .pipe(switchMap(requestParameters => this.loadPreview(requestParameters)))
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
    this.downloadLink = this.createDownloadLink(requestParameters);
    return this.activityReportService.getDetailedActivityReportPreview(requestParameters);
  }

  private createDownloadLink(requestParameters: FilteredRequestParams): string {
    var httpParams = new HttpParams({encoder: this.activityReportService.encoder});

    for (const [key, value] of Object.entries(requestParameters))
      httpParams = httpParams.append(key, value);

    return `${this.activityReportService.configuration.basePath}/api/v1/ActivityReport/GetDetailedActivityReport?${httpParams.toString()}`
  }
}
