import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AbsencesByClassesReportViewSkeletonComponent } from './absences-by-classes-report-view/absences-by-classes-report-view.component';
import { AbsencesByClassesReportsComponent } from './absences-by-classes-reports/absences-by-classes-reports.component';

const routes: Routes = [
  { path: '', component: AbsencesByClassesReportsComponent },
  { path: ':absencesByClassesReportId', component: AbsencesByClassesReportViewSkeletonComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AbsencesByClassesReportsRoutingModule {}
