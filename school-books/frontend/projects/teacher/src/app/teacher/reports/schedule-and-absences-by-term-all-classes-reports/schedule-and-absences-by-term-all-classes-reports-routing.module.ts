import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ScheduleAndAbsencesByTermAllClassesReportViewSkeletonComponent } from './schedule-and-absences-by-term-all-classes-report-view/schedule-and-absences-by-term-all-classes-report-view.component';
import { ScheduleAndAbsencesByTermAllClassesReportsComponent } from './schedule-and-absences-by-term-all-classes-reports/schedule-and-absences-by-term-all-classes-reports.component';

const routes: Routes = [
  { path: '', component: ScheduleAndAbsencesByTermAllClassesReportsComponent },
  {
    path: ':scheduleAndAbsencesByTermAllClassesReportId',
    component: ScheduleAndAbsencesByTermAllClassesReportViewSkeletonComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ScheduleAndAbsencesByTermAllClassesReportsRoutingModule {}
