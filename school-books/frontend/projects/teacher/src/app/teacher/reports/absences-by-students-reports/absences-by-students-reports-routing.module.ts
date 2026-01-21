import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AbsencesByStudentsReportViewSkeletonComponent } from './absences-by-students-report-view/absences-by-students-report-view.component';
import { AbsencesByStudentsReportsComponent } from './absences-by-students-reports/absences-by-students-reports.component';

const routes: Routes = [
  { path: '', component: AbsencesByStudentsReportsComponent },
  { path: ':absencesByStudentsReportId', component: AbsencesByStudentsReportViewSkeletonComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AbsencesByStudentsReportsRoutingModule {}
