import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GradelessStudentsReportViewSkeletonComponent } from './gradeless-students-report-view/gradeless-students-report-view.component';
import { GradelessStudentsReportsComponent } from './gradeless-students-reports/gradeless-students-reports.component';

const routes: Routes = [
  { path: '', component: GradelessStudentsReportsComponent },
  { path: ':gradelessStudentsReportId', component: GradelessStudentsReportViewSkeletonComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GradelessStudentsReportsRoutingModule {}
