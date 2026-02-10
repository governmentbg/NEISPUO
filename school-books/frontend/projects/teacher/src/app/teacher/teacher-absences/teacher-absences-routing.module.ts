import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { TeacherAbsenceViewSkeletonComponent } from './teacher-absence-view/teacher-absence-view.component';
import { TeacherAbsencesComponent } from './teacher-absences/teacher-absences.component';

const routes: Routes = [
  { path: '', component: TeacherAbsencesComponent, canDeactivate: [DeactivateGuard] },
  { path: ':teacherAbsenceId', component: TeacherAbsenceViewSkeletonComponent, canDeactivate: [DeactivateGuard] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TeacherAbsencesRoutingModule {}
