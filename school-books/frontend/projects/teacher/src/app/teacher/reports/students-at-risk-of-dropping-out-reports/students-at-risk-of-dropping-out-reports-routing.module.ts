import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { StudentsAtRiskOfDroppingOutReportViewSkeletonComponent } from './students-at-risk-of-dropping-out-report-view/students-at-risk-of-dropping-out-report-view.component';
import { StudentsAtRiskOfDroppingOutReportsComponent } from './students-at-risk-of-dropping-out-reports/students-at-risk-of-dropping-out-reports.component';

const routes: Routes = [
  { path: '', component: StudentsAtRiskOfDroppingOutReportsComponent },
  { path: ':studentsAtRiskOfDroppingOutReportId', component: StudentsAtRiskOfDroppingOutReportViewSkeletonComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StudentsAtRiskOfDroppingOutReportsRoutingModule {}
