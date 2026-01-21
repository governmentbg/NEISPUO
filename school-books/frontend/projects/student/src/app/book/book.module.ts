import { NgModule } from '@angular/core';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { GradeLinkModule } from 'projects/shared/components/grade-link/grade-link.module';
import { PaginatorModule } from 'projects/shared/components/paginator/paginator.module';
import { SimpleTabSkeletonTemplateModule } from 'projects/shared/components/skeleton/simple-tab-skeleton-template.module';
import { StudentSettingsModule } from 'projects/shared/components/student-settings/student-settings.module';
import { TabCardModule } from 'projects/shared/components/tab-card/tab-card.module';
import { TabsModule } from 'projects/shared/components/tabs/tabs.module';
import { TextareaFieldModule } from 'projects/shared/components/textarea-field/textarea-field.module';
import { VerticalTabsModule } from 'projects/shared/components/vertical-tabs/vertical-tabs.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { GradePipesModule } from 'projects/shared/pipes/grade-pipes/grade-pipes.module';
import { AbsencesDplrComponent, AbsencesDplrSkeletonComponent } from './absences-dplr/absences-dplr.component';
import { AbsencesComponent, AbsencesSkeletonComponent } from './absences/absences.component';
import { AttendancesComponent, AttendancesSkeletonComponent } from './attendances/attendances.component';
import { BookRoutingModule } from './book-routing.module';
import { BookComponent, BookSkeletonComponent } from './book/book.component';
import { ExamsComponent } from './exams/exams.component';
import {
  FirstGradeResultsComponent,
  FirstGradeResultsSkeletonComponent
} from './first-grade-results/first-grade-results.component';
import { GradeResultsComponent, GradeResultsSkeletonComponent } from './grade-results/grade-results.component';
import { GradesComponent, GradesSkeletonComponent } from './grades/grades.component';
import { IndividualWorksComponent } from './individual-works/individual-works.component';
import { NotesComponent } from './notes/notes.component';
import { ParentMeetingsComponent } from './parent-meetings/parent-meetings.component';
import { PgResultsComponent } from './pg-results/pg-results.component';
import { RemarksComponent, RemarksSkeletonComponent } from './remarks/remarks.component';
import { SanctionsComponent } from './sanctions/sanctions.component';
import { ScheduleComponent, ScheduleSkeletonComponent } from './schedule/schedule.component';
import { SupportViewComponent, SupportViewSkeletonComponent } from './support-view/support-view.component';
import { SupportsComponent } from './supports/supports.component';
import { TopicsViewComponent, TopicsViewSkeletonComponent } from './topics-view/topics-view.component';
import { TopicsComponent, TopicsSkeletonComponent } from './topics/topics.component';

@NgModule({
  declarations: [
    BookComponent,
    BookSkeletonComponent,
    GradesComponent,
    GradesSkeletonComponent,
    AbsencesComponent,
    AbsencesSkeletonComponent,
    AbsencesDplrComponent,
    AbsencesDplrSkeletonComponent,
    ParentMeetingsComponent,
    NotesComponent,
    SanctionsComponent,
    ExamsComponent,
    SupportsComponent,
    SupportViewComponent,
    SupportViewSkeletonComponent,
    RemarksComponent,
    RemarksSkeletonComponent,
    TopicsComponent,
    TopicsSkeletonComponent,
    TopicsViewComponent,
    TopicsViewSkeletonComponent,
    FirstGradeResultsComponent,
    FirstGradeResultsSkeletonComponent,
    PgResultsComponent,
    PgResultsComponent,
    IndividualWorksComponent,
    AttendancesComponent,
    AttendancesSkeletonComponent,
    GradeResultsComponent,
    GradeResultsSkeletonComponent,
    ScheduleComponent,
    ScheduleSkeletonComponent
  ],
  imports: [
    BookRoutingModule,
    CommonFormUiModule,
    SimpleTabSkeletonTemplateModule,
    BannerModule,
    DateFieldModule,
    DatePipesModule,
    GradeLinkModule,
    GradePipesModule,
    MatProgressSpinnerModule,
    PaginatorModule,
    TabCardModule,
    TextareaFieldModule,
    VerticalTabsModule,
    TabsModule,
    MatSelectModule,
    MatMenuModule,
    StudentSettingsModule
  ]
})
export class BookModule {}
