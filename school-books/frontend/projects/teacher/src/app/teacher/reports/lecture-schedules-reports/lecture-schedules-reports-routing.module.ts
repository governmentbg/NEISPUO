import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LectureSchedulesReportViewSkeletonComponent } from './lecture-schedules-report-view/lecture-schedules-report-view.component';
import { LectureSchedulesReportsComponent } from './lecture-schedules-reports/lecture-schedules-reports.component';

const routes: Routes = [
  { path: '', component: LectureSchedulesReportsComponent },
  { path: ':lectureSchedulesReportId', component: LectureSchedulesReportViewSkeletonComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LectureSchedulesReportsRoutingModule {}
