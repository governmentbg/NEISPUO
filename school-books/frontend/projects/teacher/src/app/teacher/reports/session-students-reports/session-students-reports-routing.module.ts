import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SessionStudentsReportViewSkeletonComponent } from './session-students-report-view/session-students-report-view.component';
import { SessionStudentsReportsComponent } from './session-students-reports/session-students-reports.component';

const routes: Routes = [
  { path: '', component: SessionStudentsReportsComponent },
  { path: ':sessionStudentsReportId', component: SessionStudentsReportViewSkeletonComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SessionStudentsReportsRoutingModule {}
