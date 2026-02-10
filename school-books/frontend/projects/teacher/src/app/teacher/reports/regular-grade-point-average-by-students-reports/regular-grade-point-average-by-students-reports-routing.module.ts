import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegularGradePointAverageByStudentsReportViewSkeletonComponent } from './regular-grade-point-average-by-students-report-view/regular-grade-point-average-by-students-report-view.component';
import { RegularGradePointAverageByStudentsReportsComponent } from './regular-grade-point-average-by-students-reports/regular-grade-point-average-by-students-reports.component';

const routes: Routes = [
  { path: '', component: RegularGradePointAverageByStudentsReportsComponent },
  {
    path: ':regularGradePointAverageByStudentsReportId',
    component: RegularGradePointAverageByStudentsReportViewSkeletonComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RegularGradePointAverageByStudentsReportsRoutingModule {}
