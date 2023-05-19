import {Component, Input, OnDestroy} from '@angular/core';
import {EntityService} from '../../../../../core/app/services/state-management/entity.service';
import {switchMap} from 'rxjs/operators';
import {FilterName} from '../../../../../core/app/components/filter/filter.component';
import {OrderChartService} from '../../../../../api/timetracking';
import {DateTime} from 'luxon';
import {combineLatest, Observable, of, Subscription} from 'rxjs';

@Component({
  selector: 'ts-chart-workday-info',
  templateUrl: './chart-workday-info.component.html',
  styleUrls: ['./chart-workday-info.component.scss']
})
export class ChartWorkdayInfoComponent implements OnDestroy {
  private readonly subscriptions = new Subscription();

  @Input() public daysWorked: number | undefined;
  @Input() public daysPlanned: number | undefined;
  @Input() public daysDifference: number | undefined;
  @Input() public overbookEntries: number = 0;
  @Input() public type?: 'customer' | 'order';

  public workdaysLeft?: number;
  public workdaysTotal?: number;

  constructor(
    private entityService: EntityService,
    private orderChartService: OrderChartService,
  ) {
    const filterChanged = this.entityService.filterValuesChanged
      .pipe(switchMap(filter => this.loadWorkDays(filter)))
      .subscribe(([workdaysLeft, workdaysTotal]) => {
        this.workdaysLeft = workdaysLeft;
        this.workdaysTotal = workdaysTotal;
      });
    this.subscriptions.add(filterChanged);
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private loadWorkDays(filter: Record<FilterName, any>): Observable<[number?, number?]> {
    if (filter.timeSheetStartDate === undefined || filter.timeSheetEndDate === undefined)
      return of([undefined, undefined]);

    const startDate = filter.timeSheetStartDate;
    const endDate = filter.timeSheetEndDate;
    const workdaysLeft = this.orderChartService.getPersonalWorkdaysCount({startDate: DateTime.now().startOf('day'), endDate});
    const workdaysTotal = this.orderChartService.getPersonalWorkdaysCount({startDate, endDate});
    return combineLatest([workdaysLeft, workdaysTotal]);
  }
}
