import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DateAbsencesReportViewSkeletonComponent } from './date-absences-report-view/date-absences-report-view.component';
import { DateAbsencesReportsComponent } from './date-absences-reports/date-absences-reports.component';

const routes: Routes = [
  { path: '', component: DateAbsencesReportsComponent },
  { path: ':dateAbsencesReportId', component: DateAbsencesReportViewSkeletonComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DateAbsencesReportsRoutingModule {}
