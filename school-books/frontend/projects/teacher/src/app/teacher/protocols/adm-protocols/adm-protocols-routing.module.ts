import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { AdmProtocolsComponent } from './adm-protocols.component';
import { GradeChangeExamsAdmProtocolViewSkeletonComponent } from './grade-change-exams-adm-protocols/grade-change-exams-adm-protocol-view/grade-change-exams-adm-protocol-view.component';
import { StateExamsAdmProtocolViewSkeletonComponent } from './state-exams-adm-protocols/state-exams-adm-protocol-view/state-exams-adm-protocol-view.component';

const routes: Routes = [
  { path: '', component: AdmProtocolsComponent, canDeactivate: [DeactivateGuard] },
  { path: 'stateExams/new', component: StateExamsAdmProtocolViewSkeletonComponent, canDeactivate: [DeactivateGuard] },
  {
    path: 'stateExams/:stateExamsAdmProtocolId',
    component: StateExamsAdmProtocolViewSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  },
  {
    path: 'gradeChangeExams/new',
    component: GradeChangeExamsAdmProtocolViewSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  },
  {
    path: 'gradeChangeExams/:gradeChangeExamsAdmProtocolId',
    component: GradeChangeExamsAdmProtocolViewSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StateExamsAdmProtocolsRoutingModule {}
