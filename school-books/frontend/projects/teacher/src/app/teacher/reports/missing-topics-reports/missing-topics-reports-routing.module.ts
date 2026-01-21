import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MissingTopicsReportViewSkeletonComponent } from './missing-topics-report-view/missing-topics-report-view.component';
import { MissingTopicsReportsComponent } from './missing-topics-reports/missing-topics-reports.component';

const routes: Routes = [
  { path: '', component: MissingTopicsReportsComponent },
  { path: ':missingTopicsReportId', component: MissingTopicsReportViewSkeletonComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MissingTopicsReportsRoutingModule {}
