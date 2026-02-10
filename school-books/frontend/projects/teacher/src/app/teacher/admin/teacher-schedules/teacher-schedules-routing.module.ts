import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TeacherSchedulesComponent } from './teacher-schedules/teacher-schedules.component';

const routes: Routes = [{ path: '', component: TeacherSchedulesComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TeacherSchedulesRoutingModule {}
