import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { FinalizationInternalSkeletonComponent } from './finalization-internal/finalization-internal.component';

const routes: Routes = [
  { path: '', component: FinalizationInternalSkeletonComponent, canDeactivate: [DeactivateGuard] },
  { path: '**', redirectTo: '/not-found' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FinalizationInternalRoutingModule {}
