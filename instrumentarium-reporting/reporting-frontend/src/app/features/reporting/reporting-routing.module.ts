import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '@authentication/guards/auth.guard';
import { SavedReportsPage } from '@reporting/pages/saved-reports/saved-reports.page';
import { ReportBuilderPage } from './pages/report-builder/report-builder.page';
import { ReportListPage } from './pages/report-list/report-list.page';
import { SharedReportsPage } from './pages/shared-reports/shared-reports.page';
import { ReportingLayoutPage } from './reporting-layout.page';

const routes: Routes = [
  {
    path: '',
    component: ReportingLayoutPage,
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: '/reports'
      },
      {
        path: 'reports',
        children: [
          { path: '', pathMatch: 'full', component: ReportListPage },
          {
            path: 'report-builder',
            children: [
              { path: '', pathMatch: 'full', redirectTo: '/reports' }, // -> route without params is invalid
              {
                path: ':databaseView',
                component: ReportBuilderPage
              }
            ]
          }
        ]
      },
      {
        path: 'saved-reports',
        children: [
          { path: '', pathMatch: 'full', component: SavedReportsPage },
          {
            path: 'report-builder',
            children: [
              { path: '', pathMatch: 'full', redirectTo: '/saved-reports' }, // -> route without params is invalid
              {
                path: ':databaseView/:reportId',
                component: ReportBuilderPage
              }
            ]
          }
        ]
      },
      {
        path: 'shared-reports',
        children: [
          { path: '', pathMatch: 'full', component: SharedReportsPage },
          {
            path: 'report-builder',
            children: [
              { path: '', pathMatch: 'full', redirectTo: '/shared-reports' }, // -> route without params is invalid
              {
                path: ':databaseView/:reportId',
                component: ReportBuilderPage
              }
            ]
          }
        ]
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportingRoutingModule {}
