import {Component, ElementRef, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {Filter, FilteredRequestParams, FilterName} from '../../../../../core/app/components/filter/filter.component';
import {DateTime} from 'luxon';
import {switchMap} from 'rxjs/operators';
import {EntityService} from '../../../../../core/app/services/state-management/entity.service';
import {EMPTY, Observable, Subscription} from 'rxjs';
import {ActivityReportDto, ActivityReportService} from '../../../../../api/timetracking';
import {DomSanitizer, SafeHtml} from '@angular/platform-browser';

@Component({
  selector: 'ts-report-activity-overview',
  templateUrl: './report-activity-overview.component.html',
  styleUrls: ['./report-activity-overview.component.scss']
})
export class ReportActivityOverviewComponent implements OnInit, OnDestroy {
  public filters: (Filter | FilterName)[];
  public preview: SafeHtml = '';

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
      {name: 'showDetails', defaultValue: false, isPrimary: true},
      {name: 'timeSheetStartDate', defaultValue: defaultStartDate, isPrimary: true},
      {name: 'timeSheetEndDate', defaultValue: defaultEndDate, isPrimary: true},
      {name: 'customerId'},
      {name: 'orderId'},
      {name: 'projectId'},
      {name: 'activityId'},
      {name: 'timeSheetIssue'},
      {name: 'timeSheetComment'},
      {name: 'timeSheetBillable'},
    ];
  }

  public ngOnInit(): void {
    const loadTimeSheets = this.entityService.filterChanged
      .pipe(
        switchMap(filter => this.loadReport(filter)),
        switchMap(reportDto => this.loadPreview(reportDto)),
      )
      .subscribe(x => this.preview = this.sanitizer.bypassSecurityTrustHtml(x));
    this.subscriptions.add(loadTimeSheets);
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private loadReport(filter: FilteredRequestParams): Observable<ActivityReportDto> {
    return this.activityReportService.getDetailedActivityReport(filter);
  }

  private loadPreview(activityReportDto: ActivityReportDto | undefined): Observable<any> {
    const returnType = 'text/plain' as unknown as undefined;
    // return this.activityReportService.getDetailedActivityReportPreview({activityReportDto}, undefined, undefined, {httpHeaderAccept: returnType});
    // return this.activityReportService.generateActivityReportPreview({activityReportDto}, undefined, undefined, {httpHeaderAccept: returnType});
    return EMPTY;
  }
}
