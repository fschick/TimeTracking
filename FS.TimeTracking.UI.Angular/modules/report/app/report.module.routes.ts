import {Routes} from '@angular/router';
import {ReportActivityOverviewComponent} from './components/report-activity-overview/report-activity-overview.component';
import {ReportActivityPreviewComponent} from './components/report-activity-preview/report-activity-preview.component';

export const reportModuleRoutes: Routes = [
  {
    path: 'report',
    children: [
      {
        path: 'activity', component: ReportActivityOverviewComponent,
      },
      {
        path: 'activity/preview/:reportType', component: ReportActivityPreviewComponent
      },
    ]
  }
];
