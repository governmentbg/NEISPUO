import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { ExamDutyProtocolViewSkeletonComponent } from './exam-duty-protocol-view/exam-duty-protocol-view.component';
import { ExamDutyProtocolsComponent } from './exam-duty-protocols/exam-duty-protocols.component';
import { NvoExamDutyProtocolViewSkeletonComponent } from './nvo-exam-duty-protocol-view/nvo-exam-duty-protocol-view.component';
import { SkillsCheckExamDutyProtocolViewSkeletonComponent } from './skills-check-exam-duty-protocol-view/skills-check-exam-duty-protocol-view.component';
import { StateExamDutyProtocolViewSkeletonComponent } from './state-exam-duty-protocol-view/state-exam-duty-protocol-view.component';

const routes: Routes = [
  { path: '', component: ExamDutyProtocolsComponent, canDeactivate: [DeactivateGuard] },
  { path: 'new', component: ExamDutyProtocolViewSkeletonComponent, canDeactivate: [DeactivateGuard] },
  { path: 'newState', component: StateExamDutyProtocolViewSkeletonComponent, canDeactivate: [DeactivateGuard] },
  {
    path: 'newSkillsCheck',
    component: SkillsCheckExamDutyProtocolViewSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  },
  { path: 'newNvo', component: NvoExamDutyProtocolViewSkeletonComponent, canDeactivate: [DeactivateGuard] },
  { path: ':examDutyProtocolId', component: ExamDutyProtocolViewSkeletonComponent, canDeactivate: [DeactivateGuard] },
  {
    path: 'state/:stateExamDutyProtocolId',
    component: StateExamDutyProtocolViewSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  },
  {
    path: 'skillsCheck/:skillsCheckExamDutyProtocolId',
    component: SkillsCheckExamDutyProtocolViewSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  },
  {
    path: 'nvo/:nvoExamDutyProtocolId',
    component: NvoExamDutyProtocolViewSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ExamDutyProtocolsRoutingModule {}
