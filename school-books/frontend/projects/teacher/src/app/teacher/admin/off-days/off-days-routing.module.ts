import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { OffDayViewSkeletonComponent } from './off-day-view/off-day-view.component';
import { OffDaysComponent } from './off-days/off-days.component';

const routes: Routes = [
  { path: '', component: OffDaysComponent, canDeactivate: [DeactivateGuard] },
  { path: ':offDayId', component: OffDayViewSkeletonComponent, canDeactivate: [DeactivateGuard] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OffDaysRoutingModule {}
