import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FinalGradePointAverageByStudentsReportViewSkeletonComponent } from './final-grade-point-average-by-students-report-view/final-grade-point-average-by-students-report-view.component';
import { FinalGradePointAverageByStudentsReportsComponent } from './final-grade-point-average-by-students-reports/final-grade-point-average-by-students-reports.component';

const routes: Routes = [
  { path: '', component: FinalGradePointAverageByStudentsReportsComponent },
  {
    path: ':finalGradePointAverageByStudentsReportId',
    component: FinalGradePointAverageByStudentsReportViewSkeletonComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FinalGradePointAverageByStudentsReportsRoutingModule {}
