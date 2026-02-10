import { NgModule } from '@angular/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule } from '@angular/material/dialog';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { DecimalGradePickerModule } from 'projects/shared/components/decimal-grade-picker/decimal-grade-picker.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { NumberFieldModule } from 'projects/shared/components/number-field/number-field.module';
import { SimpleDialogSkeletonTemplateModule } from 'projects/shared/components/skeleton/simple-dialog-skeleton-template.module';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  ExamResultProtocolViewDialogComponent,
  ExamResultProtocolViewDialogSkeletonComponent
} from './exam-result-protocol-student-view-dialog/exam-result-protocol-student-view-dialog.component';
import {
  ExamResultProtocolViewClassDialogComponent,
  ExamResultProtocolViewClassDialogSkeletonComponent
} from './exam-result-protocol-view-class-dialog/exam-result-protocol-view-class-dialog.component';
import {
  ExamResultProtocolViewComponent,
  ExamResultProtocolViewSkeletonComponent
} from './exam-result-protocol-view/exam-result-protocol-view.component';
import { ExamResultProtocolsRoutingModule } from './exam-result-protocols-routing.module';
import {
  ExamResultProtocolsTypeDialogComponent,
  ExamResultProtocolsTypeDialogSkeletonComponent
} from './exam-result-protocols-type-dialog/exam-result-protocols-type-dialog.component';
import { ExamResultProtocolsComponent } from './exam-result-protocols/exam-result-protocols.component';
import {
  GraduationThesisDefenseProtocolViewComponent,
  GraduationThesisDefenseProtocolViewSkeletonComponent
} from './graduation-thesis-defense-protocol-view/graduation-thesis-defense-protocol-view.component';
import {
  HighSchoolCertificateProtocolViewDialogComponent,
  HighSchoolCertificateProtocolViewDialogSkeletonComponent
} from './high-school-certificate-protocol-student-view-dialog/high-school-certificate-protocol-student-view-dialog.component';
import {
  HighSchoolCertificateProtocolViewClassDialogComponent,
  HighSchoolCertificateProtocolViewClassDialogSkeletonComponent
} from './high-school-certificate-protocol-view-class-dialog/high-school-certificate-protocol-view-class-dialog.component';
import {
  HighSchoolCertificateProtocolViewComponent,
  HighSchoolCertificateProtocolViewSkeletonComponent
} from './high-school-certificate-protocol-view/high-school-certificate-protocol-view.component';
import {
  QualificationAcquisitionProtocolViewDialogComponent,
  QualificationAcquisitionProtocolViewDialogSkeletonComponent
} from './qualification-acquisition-protocol-student-view-dialog/qualification-acquisition-protocol-student-view-dialog.component';
import {
  QualificationAcquisitionProtocolViewComponent,
  QualificationAcquisitionProtocolViewSkeletonComponent
} from './qualification-acquisition-protocol-view/qualification-acquisition-protocol-view.component';
import {
  QualificationExamResultProtocolViewDialogComponent,
  QualificationExamResultProtocolViewDialogSkeletonComponent
} from './qualification-exam-result-protocol-student-view-dialog/qualification-exam-result-protocol-student-view-dialog.component';
import {
  QualificationExamResultProtocolViewClassDialogComponent,
  QualificationExamResultProtocolViewClassDialogSkeletonComponent
} from './qualification-exam-result-protocol-view-class-dialog/qualification-exam-result-protocol-view-class-dialog.component';
import {
  QualificationExamResultProtocolViewComponent,
  QualificationExamResultProtocolViewSkeletonComponent
} from './qualification-exam-result-protocol-view/qualification-exam-result-protocol-view.component';
import {
  SkillsCheckExamResultProtocolViewDialogComponent,
  SkillsCheckExamResultProtocolViewDialogSkeletonComponent
} from './skills-check-exam-result-protocol-evaluator-view-dialog/skills-check-exam-result-protocol-evaluator-view.component';
import {
  SkillsCheckExamResultProtocolViewComponent,
  SkillsCheckExamResultProtocolViewSkeletonComponent
} from './skills-check-exam-result-protocol-view/skills-check-exam-result-protocol-view.component';

@NgModule({
  declarations: [
    ExamResultProtocolsComponent,
    ExamResultProtocolViewSkeletonComponent,
    ExamResultProtocolViewComponent,
    ExamResultProtocolViewDialogComponent,
    ExamResultProtocolViewDialogSkeletonComponent,
    ExamResultProtocolViewClassDialogComponent,
    ExamResultProtocolViewClassDialogSkeletonComponent,
    QualificationExamResultProtocolViewSkeletonComponent,
    QualificationExamResultProtocolViewComponent,
    QualificationExamResultProtocolViewDialogComponent,
    QualificationExamResultProtocolViewDialogSkeletonComponent,
    QualificationExamResultProtocolViewClassDialogComponent,
    QualificationExamResultProtocolViewClassDialogSkeletonComponent,
    SkillsCheckExamResultProtocolViewSkeletonComponent,
    SkillsCheckExamResultProtocolViewComponent,
    SkillsCheckExamResultProtocolViewDialogSkeletonComponent,
    SkillsCheckExamResultProtocolViewDialogComponent,
    QualificationAcquisitionProtocolViewSkeletonComponent,
    QualificationAcquisitionProtocolViewComponent,
    QualificationAcquisitionProtocolViewDialogComponent,
    QualificationAcquisitionProtocolViewDialogSkeletonComponent,
    HighSchoolCertificateProtocolViewSkeletonComponent,
    HighSchoolCertificateProtocolViewComponent,
    HighSchoolCertificateProtocolViewDialogComponent,
    HighSchoolCertificateProtocolViewDialogSkeletonComponent,
    HighSchoolCertificateProtocolViewClassDialogComponent,
    HighSchoolCertificateProtocolViewClassDialogSkeletonComponent,
    ExamResultProtocolsTypeDialogComponent,
    ExamResultProtocolsTypeDialogSkeletonComponent,
    GraduationThesisDefenseProtocolViewSkeletonComponent,
    GraduationThesisDefenseProtocolViewComponent
  ],
  imports: [
    ExamResultProtocolsRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    BannerModule,
    DateFieldModule,
    DatePipesModule,
    DecimalGradePickerModule,
    MatCheckboxModule,
    MatDialogModule,
    NomSelectModule,
    NumberFieldModule,
    SimpleDialogSkeletonTemplateModule
  ],
  providers: [DeactivateGuard]
})
export class ExamResultProtocolsModule {}
