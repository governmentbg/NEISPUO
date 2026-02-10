import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ROUTING_CONSTANTS } from '@shared/constants/routing.constants';

const routes: Routes = [

  {
    path: ROUTING_CONSTANTS.SURVEY,
    data: {
      breadcrumb: 'Кампании'
    },
    loadChildren: () => import('@surveys/surveys.module').then((m) => m.SurveysModule)
  },
  { path: '**', redirectTo: `/${ROUTING_CONSTANTS.LOGIN}` }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    scrollPositionRestoration: 'enabled', relativeLinkResolution: 'legacy', paramsInheritanceStrategy: 'always'
  })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
