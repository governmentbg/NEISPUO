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
    loadChildren: () => import('./student.module').then((m) => m.StudentModule)
  },
  { path: '**', component: NotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { paramsInheritanceStrategy: 'always' })],
  exports: [RouterModule]
})
export class AppRoutingModule {}
