import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotFoundComponent } from 'projects/shared/components/not-found/not-found.component';
import {
  RedirectToCorrectAppGuard,
  RedirectToDefaultRouteGuard,
  RedirectToReturnUrlGuard
} from 'projects/shared/other/auth.guard';

const routes: Routes = [
  {
    path: '',
    canActivate: [RedirectToCorrectAppGuard, RedirectToReturnUrlGuard, RedirectToDefaultRouteGuard],
    children: [
      {
        path: 'admin',
        loadChildren: () => import('./admin/admin.module').then((m) => m.AdminModule)
      },
      {
        path: ':schoolYear/:instId',
        loadChildren: () => import('./teacher/teacher.module').then((m) => m.TeacherModule)
      }
    ]
  },
  { path: '**', component: NotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { paramsInheritanceStrategy: 'always' })],
  exports: [RouterModule]
})
export class AppRoutingModule {}
