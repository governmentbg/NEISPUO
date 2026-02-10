import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ScheduleAndAbsencesByTermReportViewSkeletonComponent } from './schedule-and-absences-by-term-report-view/schedule-and-absences-by-term-report-view.component';
import { ScheduleAndAbsencesByTermReportsComponent } from './schedule-and-absences-by-term-reports/schedule-and-absences-by-term-reports.component';

const routes: Routes = [
  { path: '', component: ScheduleAndAbsencesByTermReportsComponent },
  { path: ':scheduleAndAbsencesByTermReportId', component: ScheduleAndAbsencesByTermReportViewSkeletonComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ScheduleAndAbsencesByTermReportsRoutingModule {}
