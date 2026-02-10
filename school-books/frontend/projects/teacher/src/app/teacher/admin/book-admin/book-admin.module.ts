import { NgModule } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule } from '@angular/material/dialog';
import { MatRadioModule } from '@angular/material/radio';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { NumberFieldModule } from 'projects/shared/components/number-field/number-field.module';
import { SelectFieldModule } from 'projects/shared/components/select-field/select-field.module';
import { ShiftFormModule } from 'projects/shared/components/shift-form/shift-form.module';
import { SimpleDialogSkeletonTemplateModule } from 'projects/shared/components/skeleton/simple-dialog-skeleton-template.module';
import { TabCardModule } from 'projects/shared/components/tab-card/tab-card.module';
import { TabsModule } from 'projects/shared/components/tabs/tabs.module';
import { TextareaFieldModule } from 'projects/shared/components/textarea-field/textarea-field.module';
import { VerticalTabsModule } from 'projects/shared/components/vertical-tabs/vertical-tabs.module';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import { BookAdminNewComponent, BookAdminNewSkeletonComponent } from './book-admin-new/book-admin-new.component';
import {
  BookAdminReorganizeCombineComponent,
  BookAdminReorganizeCombineSkeletonComponent
} from './book-admin-reorganize/book-admin-reorganize-combine/book-admin-reorganize-combine.component';
import {
  BookAdminReorganizeSeparateComponent,
  BookAdminReorganizeSeparateSkeletonComponent
} from './book-admin-reorganize/book-admin-reorganize-separate/book-admin-reorganize-separate.component';
import { BookAdminRoutingModule } from './book-admin-routing.module';
import { BookAdminTabsComponent, BookAdminTabsSkeletonComponent } from './book-admin-tabs/book-admin-tabs.component';
import { BookAdminCurriculumItemDialogComponent } from './book-admin-view/book-admin-curriculum/book-admin-curriculum-item-dialog.component';
import { BookAdminCurriculumComponent } from './book-admin-view/book-admin-curriculum/book-admin-curriculum.component';
import {
  BookAdminMainDataComponent,
  BookAdminMainDataSkeletonComponent
} from './book-admin-view/book-admin-main-data/book-admin-main-data.component';
import {
  BookAdminClassBookPrintComponent,
  BookAdminClassBookPrintSkeletonComponent
} from './book-admin-view/book-admin-print/book-admin-classbook-print/book-admin-classbook-print.component';
import {
  BookAdminPrintTabsComponent,
  BookAdminPrintTabsSkeletonComponent
} from './book-admin-view/book-admin-print/book-admin-print-tabs/book-admin-print-tabs.component';
import {
  BookAdminStudentBookPrintStudentDialogComponent,
  BookAdminStudentBookPrintStudentDialogSkeletonComponent
} from './book-admin-view/book-admin-print/book-admin-studentbook-print-student-dialog/book-admin-studentbook-print-student-dialog.component';
import {
  BookAdminStudentBookPrintComponent,
  BookAdminStudentBookPrintSkeletonComponent
} from './book-admin-view/book-admin-print/book-admin-studentbook-print/book-admin-studentbook-print.component';
import { BookAdminSchedulesComponent } from './book-admin-view/book-admin-schedules/book-admin-schedules.component';
import {
  ScheduleDialogComponent,
  ScheduleDialogSkeletonComponent
} from './book-admin-view/book-admin-schedules/schedule-dialog/schedule-dialog.component';
import { WizardHeadingComponent } from './book-admin-view/book-admin-schedules/schedule-dialog/wizard-heading/wizard-heading.component';
import {
  ScheduleIndividualDialogComponent,
  ScheduleIndividualDialogSkeletonComponent
} from './book-admin-view/book-admin-schedules/schedule-individual-dialog/schedule-individual-dialog.component';
import {
  SplitScheduleDialogComponent,
  SplitScheduleDialogSkeletonComponent
} from './book-admin-view/book-admin-schedules/split-schedule-dialog/split-schedule-dialog.component';
import {
  BookAdminSchoolYearProgramComponent,
  BookAdminSchoolYearProgramSkeletonComponent
} from './book-admin-view/book-admin-school-year-program/book-admin-school-year-program.component';
import {
  BookAdminStudentDialogComponent,
  BookAdminStudentDialogSkeletonComponent
} from './book-admin-view/book-admin-students/book-admin-student-dialog.component';
import {
  BookAdminStudentNumbersDialogComponent,
  BookAdminStudentNumbersDialogSkeletonComponent
} from './book-admin-view/book-admin-students/book-admin-student-numbers-dialog.component';
import {
  BookAdminStudentsComponent,
  BookAdminStudentsSkeletonComponent
} from './book-admin-view/book-admin-students/book-admin-students.component';
import { BookAdminViewComponent, BookAdminViewSkeletonComponent } from './book-admin-view/book-admin-view.component';
import { BookAdminComponent } from './book-admin/book-admin.component';

@NgModule({
  declarations: [
    BookAdminComponent,
    BookAdminNewComponent,
    BookAdminNewSkeletonComponent,
    BookAdminReorganizeSeparateComponent,
    BookAdminReorganizeSeparateSkeletonComponent,
    BookAdminReorganizeCombineComponent,
    BookAdminReorganizeCombineSkeletonComponent,
    BookAdminTabsComponent,
    BookAdminTabsSkeletonComponent,
    BookAdminMainDataSkeletonComponent,
    BookAdminMainDataComponent,
    BookAdminViewSkeletonComponent,
    BookAdminViewComponent,
    BookAdminCurriculumComponent,
    BookAdminCurriculumItemDialogComponent,
    BookAdminStudentsSkeletonComponent,
    BookAdminStudentsComponent,
    BookAdminStudentNumbersDialogSkeletonComponent,
    BookAdminStudentNumbersDialogComponent,
    BookAdminStudentDialogSkeletonComponent,
    BookAdminStudentDialogComponent,
    BookAdminSchedulesComponent,
    ScheduleDialogSkeletonComponent,
    ScheduleDialogComponent,
    ScheduleIndividualDialogComponent,
    ScheduleIndividualDialogSkeletonComponent,
    WizardHeadingComponent,
    BookAdminSchoolYearProgramSkeletonComponent,
    BookAdminSchoolYearProgramComponent,
    SplitScheduleDialogSkeletonComponent,
    SplitScheduleDialogComponent,
    BookAdminPrintTabsComponent,
    BookAdminPrintTabsSkeletonComponent,
    BookAdminClassBookPrintSkeletonComponent,
    BookAdminClassBookPrintComponent,
    BookAdminStudentBookPrintSkeletonComponent,
    BookAdminStudentBookPrintComponent,
    BookAdminStudentBookPrintStudentDialogSkeletonComponent,
    BookAdminStudentBookPrintStudentDialogComponent
  ],
  imports: [
    BookAdminRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    BannerModule,
    DateFieldModule,
    MatCheckboxModule,
    MatRadioModule,
    MatDialogModule,
    NomSelectModule,
    SimpleDialogSkeletonTemplateModule,
    SelectFieldModule,
    TabCardModule,
    TabsModule,
    TextareaFieldModule,
    VerticalTabsModule,
    DatePipesModule,
    MatTooltipModule,
    NumberFieldModule,
    ShiftFormModule,
    MatCardModule
  ],
  providers: [DeactivateGuard]
})
export class BookAdminModule {}
