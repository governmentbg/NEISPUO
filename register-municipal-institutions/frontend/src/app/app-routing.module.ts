import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '@core/authentication/guards/auth.guard';
import { MunicipalPublicRegisterPageComponent } from '@municipal-institutions/pages/municipal-public-register-page/municipal-public-register-page.component';
import { PortalLayoutPage } from '@shared/modules/portal/pages/portal-layout/portal-layout.page';

const routes: Routes = [
  {
    path: 'public-register-page',
    component: MunicipalPublicRegisterPageComponent
  },

  {
    path: '',
    component: PortalLayoutPage,
    canActivate: [AuthGuard],
    children: [
      {
        path: 'municipal-institutions',
        data: {
          breadcrumb: 'Общински Институции',
        },
        loadChildren: () => import('@municipal-institutions/municipal-institutions-routing.module').then((mi) => mi.MunicipalInstitutionRoutingModule),
      },
      {
        path: 'flex-fields',
        data: {
          breadcrumb: 'Флекс полета',
        },
        loadChildren: () => import('@flex-fields/flex-fields-routing.module').then((ff) => ff.FlexFieldsRoutingModule),
      },
      {
        path: 'procedures',
        data: {
          breadcrumb: 'Процедури',
        },
        loadChildren: () => import('@procedures/procedures-routing.module').then((p) => p.ProceduresRoutingModule),
      },
      {
        path: 'merge-procedure',
        loadChildren: () => import('@merge-procedure/merge-procedure-routing.module').then((p) => p.MergeProceduresRoutingModule),
      },
      {
        path: 'join-procedure',
        loadChildren: () => import('@join-procedure/join-procedure-routing.module').then((p) => p.JoinProceduresRoutingModule),

      },
      {
        path: 'divide-procedure',
        loadChildren: () => import('@divide-procedure/divide-procedure-routing.module').then((p) => p.DivideProceduresRoutingModule),

      },
      {
        path: 'detach-procedure',
        loadChildren: () => import('@detach-procedure/detach-procedure-routing.module').then((p) => p.DetachProceduresRoutingModule),
      },
      { path: '**', redirectTo: '/login' },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    scrollPositionRestoration: 'enabled', relativeLinkResolution: 'legacy', paramsInheritanceStrategy: 'always',
  })],
  exports: [RouterModule],
})
export class AppRoutingModule { }
