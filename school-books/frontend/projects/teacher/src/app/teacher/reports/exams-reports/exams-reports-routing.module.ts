import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ExamsReportViewSkeletonComponent } from './exams-report-view/exams-report-view.component';
import { ExamsReportsComponent } from './exams-reports/exams-reports.component';

const routes: Routes = [
  { path: '', component: ExamsReportsComponent },
  { path: ':examsReportId', component: ExamsReportViewSkeletonComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ExamsReportsRoutingModule {}
