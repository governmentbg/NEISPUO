import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FinalGradePointAverageByClassesReportViewSkeletonComponent } from './final-grade-point-average-by-classes-report-view/final-grade-point-average-by-classes-report-view.component';
import { FinalGradePointAverageByClassesReportsComponent } from './final-grade-point-average-by-classes-reports/final-grade-point-average-by-classes-reports.component';

const routes: Routes = [
  { path: '', component: FinalGradePointAverageByClassesReportsComponent },
  {
    path: ':finalGradePointAverageByClassesReportId',
    component: FinalGradePointAverageByClassesReportViewSkeletonComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FinalGradePointAverageByClassesReportsRoutingModule {}
