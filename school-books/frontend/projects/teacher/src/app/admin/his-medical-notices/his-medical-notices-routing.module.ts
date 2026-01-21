import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HisMedicalNoticeViewSkeletonComponent } from './his-medical-notice-view/his-medical-notice-view.component';
import { HisMedicalNoticesComponent } from './his-medical-notices/his-medical-notices.component';

const routes: Routes = [
  { path: '', component: HisMedicalNoticesComponent },
  { path: ':hisMedicalNoticeId', component: HisMedicalNoticeViewSkeletonComponent },
  { path: '**', redirectTo: '/not-found' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HisMedicalNoticesRoutingModule {}
