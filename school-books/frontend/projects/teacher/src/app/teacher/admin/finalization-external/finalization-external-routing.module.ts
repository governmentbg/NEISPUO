import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { FinalizationExternalSkeletonComponent } from './finalization-external/finalization-external.component';

const routes: Routes = [
  { path: '', component: FinalizationExternalSkeletonComponent, canDeactivate: [DeactivateGuard] },
  { path: '**', redirectTo: '/not-found' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FinalizationExternalRoutingModule {}
