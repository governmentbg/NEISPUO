import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  // {
  //   path: 'auth',
  //   loadChildren: () => import('@authentication/authentication.module').then((m) => m.AuthenticationModule)
  // },
  {
    path: 'portal',
    loadChildren: () => import('@portal/portal.module').then((m) => m.PortalModule)
  },
  { path: '**', redirectTo: '/login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    scrollPositionRestoration: 'enabled', relativeLinkResolution: 'legacy', paramsInheritanceStrategy: 'always'
  })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
