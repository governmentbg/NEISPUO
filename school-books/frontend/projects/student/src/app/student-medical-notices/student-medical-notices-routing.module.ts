import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { StudentMedicalNoticesSkeletonComponent } from './student-medical-notices.component';

const routes: Routes = [
  { path: '', component: StudentMedicalNoticesSkeletonComponent },
  { path: ':personId', component: StudentMedicalNoticesSkeletonComponent },
  { path: '**', redirectTo: '/not-found' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StudentMedicalNoticesRoutingModule {}
