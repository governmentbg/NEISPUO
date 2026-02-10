import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { ShiftViewSkeletonComponent } from './shift-view/shift-view.component';
import { ShiftsComponent } from './shifts/shifts.component';

const routes: Routes = [
  { path: '', component: ShiftsComponent, canDeactivate: [DeactivateGuard] },
  { path: ':shiftId', component: ShiftViewSkeletonComponent, canDeactivate: [DeactivateGuard] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ShiftsRoutingModule {}
