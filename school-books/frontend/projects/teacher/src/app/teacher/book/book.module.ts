import { NgModule } from '@angular/core';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule } from '@angular/material/dialog';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { NgSelectModule } from '@ng-select/ng-select';
import { UppyAngularDragDropModule, UppyAngularProgressBarModule } from '@uppy/angular';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { AbsenceChipsModule } from 'projects/shared/components/absence-chips/absence-chips.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { BookSettingsModule } from 'projects/shared/components/book-settings/book-settings.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { DecimalGradePickerModule } from 'projects/shared/components/decimal-grade-picker/decimal-grade-picker.module';
import { GradeLinkModule } from 'projects/shared/components/grade-link/grade-link.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { NumberFieldModule } from 'projects/shared/components/number-field/number-field.module';
import { PaginatorModule } from 'projects/shared/components/paginator/paginator.module';
import { QualitativeGradePickerModule } from 'projects/shared/components/qualitative-grade-picker/qualitative-grade-picker.module';
import { SelectFieldModule } from 'projects/shared/components/select-field/select-field.module';
import { SimpleDialogSkeletonTemplateModule } from 'projects/shared/components/skeleton/simple-dialog-skeleton-template.module';
import { SimpleTabSkeletonTemplateModule } from 'projects/shared/components/skeleton/simple-tab-skeleton-template.module';
import { TabCardModule } from 'projects/shared/components/tab-card/tab-card.module';
import { TabsModule } from 'projects/shared/components/tabs/tabs.module';
import { TextareaFieldModule } from 'projects/shared/components/textarea-field/textarea-field.module';
import { VerticalTabsModule } from 'projects/shared/components/vertical-tabs/vertical-tabs.module';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { GradePipesModule } from 'projects/shared/pipes/grade-pipes/grade-pipes.module';
import { TooltipListPipesModule } from 'projects/shared/pipes/tooltip-pipes/tooltip-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  AbsenceExcuseDialogComponent,
  AbsenceExcuseDialogSkeletonComponent
} from './absence-excuse-dialog/absence-excuse-dialog.component';
import {
  AbsencesDplrEditComponent,
  AbsencesDplrEditSkeletonComponent
} from './absences-dplr-edit/absences-dplr-edit.component';
import { AbsencesDplrComponent, AbsencesDplrSkeletonComponent } from './absences-dplr/absences-dplr.component';
import {
  AbsencesEditDialogComponent,
  AbsencesEditDialogSkeletonComponent
} from './absences-edit/absences-edit-dialog/absences-edit-dialog.component';
import { AbsencesEditComponent, AbsencesEditSkeletonComponent } from './absences-edit/absences-edit.component';
import { AbsencesExcuseDialogComponent } from './absences-excuse-dialog/absences-excuse-dialog.component';
import { AbsencesComponent, AbsencesSkeletonComponent } from './absences/absences.component';
import {
  AttendanceExcuseDialogComponent,
  AttendanceExcuseDialogSkeletonComponent
} from './attendance-excuse-dialog/attendance-excuse-dialog.component';
import {
  AttendancesEditComponent,
  AttendancesEditSkeletonComponent
} from './attendances-edit/attendances-edit.component';
import { AttendancesComponent, AttendancesSkeletonComponent } from './attendances/attendances.component';
import { BookRoutingModule } from './book-routing.module';
import { BookComponent, BookSkeletonComponent } from './book/book.component';
import { ExamViewComponent, ExamViewSkeletonComponent } from './exam-view/exam-view.component';
import { ExamsComponent } from './exams/exams.component';
import {
  FirstGradeResultsEditComponent,
  FirstGradeResultsEditSkeletonComponent
} from './first-grade-results-edit/first-grade-results-edit.component';
import {
  FirstGradeResultsComponent,
  FirstGradeResultsSkeletonComponent
} from './first-grade-results/first-grade-results.component';
import {
  GradeResultSessionsEditComponent,
  GradeResultSessionsEditSkeletonComponent
} from './grade-result-sessions-edit/grade-result-sessions-edit.component';
import {
  GradeResultSessionsComponent,
  GradeResultSessionsSkeletonComponent
} from './grade-result-sessions/grade-result-sessions.component';
import {
  GradeResultsEditComponent,
  GradeResultsEditSkeletonComponent
} from './grade-results-edit/grade-results-edit.component';
import { GradeResultsComponent, GradeResultsSkeletonComponent } from './grade-results/grade-results.component';
import { GradesNewComponent, GradesNewSkeletonComponent } from './grades-new/grades-new.component';
import { GradesViewForecastGradeDialogComponent } from './grades-view-forecast-grade-dialog/grades-view-forecast-grade-dialog';
import { GradesViewComponent, GradesViewSkeletonComponent } from './grades-view/grades-view.component';
import { GradesComponent, GradesSkeletonComponent } from './grades/grades.component';
import {
  IndividualWorkViewComponent,
  IndividualWorkViewSkeletonComponent
} from './individual-work-view/individual-work-view.component';
import { IndividualWorksComponent } from './individual-works/individual-works.component';
import { MedicalNoticesComponent } from './medical-notices/medical-notices.component';
import { NoteViewComponent, NoteViewSkeletonComponent } from './note-view/note-view.component';
import { NotesComponent } from './notes/notes.component';
import {
  ParentMeetingViewComponent,
  ParentMeetingViewSkeletonComponent
} from './parent-meeting-view/parent-meeting-view.component';
import { ParentMeetingsComponent } from './parent-meetings/parent-meetings.component';
import {
  PerformanceViewComponent,
  PerformanceViewSkeletonComponent
} from './performance-view/performance-view.component';
import { PerformancesComponent } from './performances/performances.component';
import { PgResultViewComponent, PgResultViewSkeletonComponent } from './pg-result-view/pg-result-view.component';
import { PgResultsComponent, PgResultsSkeletonComponent } from './pg-results/pg-results.component';
import { RemarkViewComponent, RemarkViewSkeletonComponent } from './remark-view/remark-view.component';
import { RemarksComponent, RemarksSkeletonComponent } from './remarks/remarks.component';
import {
  ReplrParticipationViewComponent,
  ReplrParticipationViewSkeletonComponent
} from './replr-participation-view/replr-participation-view.component';
import { ReplrParticipationsComponent } from './replr-participations/replr-participations.component';
import { SanctionViewComponent, SanctionViewSkeletonComponent } from './sanction-view/sanction-view.component';
import { SanctionsComponent } from './sanctions/sanctions.component';
import { ScheduleComponent, ScheduleSkeletonComponent } from './schedule/schedule.component';
import {
  SupportViewDialogComponent,
  SupportViewDialogSkeletonComponent
} from './support-view-dialog/support-view-dialog.component';
import { SupportViewComponent, SupportViewSkeletonComponent } from './support-view/support-view.component';
import { SupportsComponent } from './supports/supports.component';
import { TopicPlansImportDialogComponent } from './topic-plans-import-dialog/topic-plans-import-dialog.component';
import {
  TopicPlansItemDialogComponent,
  TopicPlansItemDialogSkeletonComponent
} from './topic-plans-item-dialog/topic-plans-item-dialog.component';
import { TopicPlansItemsComponent } from './topic-plans-items/topic-plans-items.component';
import { TopicPlansLoadDialogComponent } from './topic-plans-load-dialog/topic-plans-load-dialog.component';
import { TopicPlansComponent, TopicPlansSkeletonComponent } from './topic-plans/topic-plans.component';
import {
  AddTopicDplrDialogComponent,
  AddTopicDplrDialogSkeletonComponent
} from './topics-edit-dplr/add-topic-dplr-dialog/add-topic-dplr-dialog.component';
import {
  TopicsEditDplrComponent,
  TopicsEditDplrSkeletonComponent
} from './topics-edit-dplr/topics-edit-dplr.component';
import {
  AdditionalActivityDialogComponent,
  AdditionalActivityDialogSkeletonComponent
} from './topics-edit/additional-topic-dialog/additional-activity-dialog.component';
import { TopicsEditComponent, TopicsEditSkeletonComponent } from './topics-edit/topics-edit.component';
import { TransferredStudentsBannerComponent } from './transferred-students-banner/transferred-students-banner.component';

@NgModule({
  declarations: [
    BookComponent,
    BookSkeletonComponent,
    GradesSkeletonComponent,
    GradesComponent,
    GradesViewSkeletonComponent,
    GradesViewComponent,
    GradesNewComponent,
    GradesNewSkeletonComponent,
    GradesViewForecastGradeDialogComponent,
    AbsencesComponent,
    AbsencesSkeletonComponent,
    AbsencesEditComponent,
    AbsencesEditSkeletonComponent,
    AbsencesEditDialogSkeletonComponent,
    AbsencesEditDialogComponent,
    AbsencesDplrComponent,
    AbsencesDplrSkeletonComponent,
    AbsencesDplrEditComponent,
    AbsencesDplrEditSkeletonComponent,
    AbsenceExcuseDialogSkeletonComponent,
    AbsenceExcuseDialogComponent,
    AbsencesExcuseDialogComponent,
    MedicalNoticesComponent,
    AttendanceExcuseDialogSkeletonComponent,
    AttendanceExcuseDialogComponent,
    ParentMeetingsComponent,
    ParentMeetingViewSkeletonComponent,
    ParentMeetingViewComponent,
    NotesComponent,
    NoteViewComponent,
    NoteViewSkeletonComponent,
    SanctionsComponent,
    SanctionViewComponent,
    SanctionViewSkeletonComponent,
    ExamsComponent,
    ExamViewComponent,
    ExamViewSkeletonComponent,
    SupportsComponent,
    SupportViewComponent,
    SupportViewSkeletonComponent,
    SupportViewDialogComponent,
    SupportViewDialogSkeletonComponent,
    RemarksComponent,
    RemarksSkeletonComponent,
    RemarkViewComponent,
    RemarkViewSkeletonComponent,
    TopicsEditComponent,
    TopicsEditSkeletonComponent,
    TopicsEditDplrComponent,
    TopicsEditDplrSkeletonComponent,
    AddTopicDplrDialogComponent,
    AddTopicDplrDialogSkeletonComponent,
    AdditionalActivityDialogComponent,
    AdditionalActivityDialogSkeletonComponent,
    FirstGradeResultsComponent,
    FirstGradeResultsSkeletonComponent,
    FirstGradeResultsEditComponent,
    FirstGradeResultsEditSkeletonComponent,
    PgResultsComponent,
    PgResultsSkeletonComponent,
    PgResultViewComponent,
    PgResultViewSkeletonComponent,
    IndividualWorksComponent,
    IndividualWorkViewComponent,
    IndividualWorkViewSkeletonComponent,
    AttendancesComponent,
    AttendancesSkeletonComponent,
    AttendancesEditComponent,
    AttendancesEditSkeletonComponent,
    GradeResultsComponent,
    GradeResultsSkeletonComponent,
    GradeResultsEditComponent,
    GradeResultsEditSkeletonComponent,
    GradeResultSessionsComponent,
    GradeResultSessionsSkeletonComponent,
    GradeResultSessionsEditComponent,
    GradeResultSessionsEditSkeletonComponent,
    ScheduleComponent,
    ScheduleSkeletonComponent,
    PerformancesComponent,
    PerformanceViewComponent,
    PerformanceViewSkeletonComponent,
    ReplrParticipationsComponent,
    ReplrParticipationViewComponent,
    ReplrParticipationViewSkeletonComponent,
    TransferredStudentsBannerComponent,
    TopicPlansSkeletonComponent,
    TopicPlansComponent,
    TopicPlansItemsComponent,
    TopicPlansImportDialogComponent,
    TopicPlansItemDialogSkeletonComponent,
    TopicPlansItemDialogComponent,
    TopicPlansLoadDialogComponent
  ],
  imports: [
    TabsModule,
    BookRoutingModule,
    CommonFormUiModule,
    AbsenceChipsModule,
    ActionServiceModule,
    BannerModule,
    DateFieldModule,
    DatePipesModule,
    DecimalGradePickerModule,
    GradeLinkModule,
    GradePipesModule,
    MatButtonToggleModule,
    MatCheckboxModule,
    MatDialogModule,
    MatMenuModule,
    MatTooltipModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    NomSelectModule,
    NumberFieldModule,
    PaginatorModule,
    QualitativeGradePickerModule,
    SelectFieldModule,
    SimpleTabSkeletonTemplateModule,
    SimpleDialogSkeletonTemplateModule,
    TabCardModule,
    TextareaFieldModule,
    VerticalTabsModule,
    UppyAngularDragDropModule,
    UppyAngularProgressBarModule,
    NgSelectModule,
    BookSettingsModule,
    TooltipListPipesModule
  ],
  providers: [DeactivateGuard]
})
export class BookModule {}
