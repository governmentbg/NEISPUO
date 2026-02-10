import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { ExamResultProtocolViewSkeletonComponent } from './exam-result-protocol-view/exam-result-protocol-view.component';
import { ExamResultProtocolsComponent } from './exam-result-protocols/exam-result-protocols.component';
import { GraduationThesisDefenseProtocolViewSkeletonComponent } from './graduation-thesis-defense-protocol-view/graduation-thesis-defense-protocol-view.component';
import { HighSchoolCertificateProtocolViewSkeletonComponent } from './high-school-certificate-protocol-view/high-school-certificate-protocol-view.component';
import { QualificationAcquisitionProtocolViewSkeletonComponent } from './qualification-acquisition-protocol-view/qualification-acquisition-protocol-view.component';
import { QualificationExamResultProtocolViewSkeletonComponent } from './qualification-exam-result-protocol-view/qualification-exam-result-protocol-view.component';
import { SkillsCheckExamResultProtocolViewSkeletonComponent } from './skills-check-exam-result-protocol-view/skills-check-exam-result-protocol-view.component';

const routes: Routes = [
  { path: '', component: ExamResultProtocolsComponent, canDeactivate: [DeactivateGuard] },
  { path: 'new', component: ExamResultProtocolViewSkeletonComponent, canDeactivate: [DeactivateGuard] },
  {
    path: 'newQualification',
    component: QualificationExamResultProtocolViewSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  },
  {
    path: 'newSkillsCheck',
    component: SkillsCheckExamResultProtocolViewSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  },
  {
    path: 'newQualificationAcquisition',
    component: QualificationAcquisitionProtocolViewSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  },
  {
    path: 'newHighSchoolCertificate',
    component: HighSchoolCertificateProtocolViewSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  },
  {
    path: 'newGraduationThesisDefense',
    component: GraduationThesisDefenseProtocolViewSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  },
  {
    path: ':examResultProtocolId',
    component: ExamResultProtocolViewSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  },
  {
    path: 'qualification/:qualificationExamResultProtocolId',
    component: QualificationExamResultProtocolViewSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  },
  {
    path: 'skillsCheck/:skillsCheckExamResultProtocolId',
    component: SkillsCheckExamResultProtocolViewSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  },
  {
    path: 'qualificationAcquisition/:qualificationAcquisitionProtocolId',
    component: QualificationAcquisitionProtocolViewSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  },
  {
    path: 'highSchoolCertificate/:highSchoolCertificateProtocolId',
    component: HighSchoolCertificateProtocolViewSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  },
  {
    path: 'graduationThesisDefense/:graduationThesisDefenseProtocolId',
    component: GraduationThesisDefenseProtocolViewSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ExamResultProtocolsRoutingModule {}
