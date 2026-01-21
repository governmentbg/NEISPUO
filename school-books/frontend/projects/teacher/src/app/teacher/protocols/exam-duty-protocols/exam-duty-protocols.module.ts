import { NgModule } from '@angular/core';
import { MatDialogModule } from '@angular/material/dialog';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { NumberFieldModule } from 'projects/shared/components/number-field/number-field.module';
import { SimpleDialogSkeletonTemplateModule } from 'projects/shared/components/skeleton/simple-dialog-skeleton-template.module';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  ExamDutyProtocolViewDialogComponent,
  ExamDutyProtocolViewDialogSkeletonComponent
} from './exam-duty-protocol-student-view-dialog/exam-duty-protocol-student-view-dialog.component';
import {
  ExamDutyProtocolViewClassDialogComponent,
  ExamDutyProtocolViewClassDialogSkeletonComponent
} from './exam-duty-protocol-view-class-dialog/exam-duty-protocol-view-class-dialog.component';
import {
  ExamDutyProtocolViewComponent,
  ExamDutyProtocolViewSkeletonComponent
} from './exam-duty-protocol-view/exam-duty-protocol-view.component';
import { ExamDutyProtocolsRoutingModule } from './exam-duty-protocols-routing.module';
import {
  ExamDutyProtocolsTypeDialogComponent,
  ExamDutyProtocolsTypeDialogSkeletonComponent
} from './exam-duty-protocols-type-dialog/exam-duty-protocols-type-dialog.component';
import { ExamDutyProtocolsComponent } from './exam-duty-protocols/exam-duty-protocols.component';
import {
  NvoExamDutyProtocolStudentViewDialogComponent,
  NvoExamDutyProtocolStudentViewDialogSkeletonComponent
} from './nvo-exam-duty-protocol-student-view-dialog/nvo-exam-duty-protocol-student-view-dialog.component';
import {
  NvoExamDutyProtocolViewClassDialogComponent,
  NvoExamDutyProtocolViewClassDialogSkeletonComponent
} from './nvo-exam-duty-protocol-view-class-dialog/nvo-exam-duty-protocol-view-class-dialog.component';
import {
  NvoExamDutyProtocolViewComponent,
  NvoExamDutyProtocolViewSkeletonComponent
} from './nvo-exam-duty-protocol-view/nvo-exam-duty-protocol-view.component';
import {
  SkillsCheckExamDutyProtocolViewComponent,
  SkillsCheckExamDutyProtocolViewSkeletonComponent
} from './skills-check-exam-duty-protocol-view/skills-check-exam-duty-protocol-view.component';
import {
  StateExamDutyProtocolViewComponent,
  StateExamDutyProtocolViewSkeletonComponent
} from './state-exam-duty-protocol-view/state-exam-duty-protocol-view.component';

@NgModule({
  declarations: [
    ExamDutyProtocolsComponent,
    ExamDutyProtocolViewSkeletonComponent,
    ExamDutyProtocolViewComponent,
    StateExamDutyProtocolViewSkeletonComponent,
    StateExamDutyProtocolViewComponent,
    SkillsCheckExamDutyProtocolViewSkeletonComponent,
    SkillsCheckExamDutyProtocolViewComponent,
    NvoExamDutyProtocolViewSkeletonComponent,
    NvoExamDutyProtocolViewComponent,
    NvoExamDutyProtocolStudentViewDialogComponent,
    NvoExamDutyProtocolStudentViewDialogSkeletonComponent,
    NvoExamDutyProtocolViewClassDialogComponent,
    NvoExamDutyProtocolViewClassDialogSkeletonComponent,
    ExamDutyProtocolViewDialogComponent,
    ExamDutyProtocolViewDialogSkeletonComponent,
    ExamDutyProtocolViewClassDialogComponent,
    ExamDutyProtocolViewClassDialogSkeletonComponent,
    ExamDutyProtocolsTypeDialogComponent,
    ExamDutyProtocolsTypeDialogSkeletonComponent
  ],
  imports: [
    ExamDutyProtocolsRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    BannerModule,
    DateFieldModule,
    DatePipesModule,
    MatDialogModule,
    NomSelectModule,
    NumberFieldModule,
    SimpleDialogSkeletonTemplateModule
  ],
  providers: [DeactivateGuard]
})
export class ExamDutyProtocolsModule {}
