import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { LectureScheduleViewSkeletonComponent } from './lecture-schedule-view/lecture-schedule-view.component';
import { LectureSchedulesComponent } from './lecture-schedules/lecture-schedules.component';

const routes: Routes = [
  { path: '', component: LectureSchedulesComponent, canDeactivate: [DeactivateGuard] },
  { path: ':lectureScheduleId', component: LectureScheduleViewSkeletonComponent, canDeactivate: [DeactivateGuard] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LectureSchedulesRoutingModule {}
