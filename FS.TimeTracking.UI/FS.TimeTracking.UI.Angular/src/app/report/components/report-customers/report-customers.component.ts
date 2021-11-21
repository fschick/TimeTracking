import { Component } from '@angular/core';
import {Subject} from 'rxjs';
import {TimeSheetFilterDto} from '../../../timesheet/components/timesheet-filter/timesheet-filter.component';

@Component({
  selector: 'ts-report-customers',
  templateUrl: './report-customers.component.html',
  styleUrls: ['./report-customers.component.scss']
})
export class ReportCustomersComponent {
  public filterChanged = new Subject<TimeSheetFilterDto>();

  constructor() { }

}
