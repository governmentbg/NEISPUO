import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegularGradePointAverageByClassesReportViewSkeletonComponent } from './regular-grade-point-average-by-classes-report-view/regular-grade-point-average-by-classes-report-view.component';
import { RegularGradePointAverageByClassesReportsComponent } from './regular-grade-point-average-by-classes-reports/regular-grade-point-average-by-classes-reports.component';

const routes: Routes = [
  { path: '', component: RegularGradePointAverageByClassesReportsComponent },
  {
    path: ':regularGradePointAverageByClassesReportId',
    component: RegularGradePointAverageByClassesReportViewSkeletonComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RegularGradePointAverageByClassesReportsRoutingModule {}
