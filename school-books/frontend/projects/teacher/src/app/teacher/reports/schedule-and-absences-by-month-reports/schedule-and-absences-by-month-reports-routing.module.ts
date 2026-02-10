import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ScheduleAndAbsencesByMonthReportViewSkeletonComponent } from './schedule-and-absences-by-month-report-view/schedule-and-absences-by-month-report-view.component';
import { ScheduleAndAbsencesByMonthReportsComponent } from './schedule-and-absences-by-month-reports/schedule-and-absences-by-month-reports.component';

const routes: Routes = [
  { path: '', component: ScheduleAndAbsencesByMonthReportsComponent },
  { path: ':scheduleAndAbsencesByMonthReportId', component: ScheduleAndAbsencesByMonthReportViewSkeletonComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ScheduleAndAbsencesByMonthReportsRoutingModule {}
