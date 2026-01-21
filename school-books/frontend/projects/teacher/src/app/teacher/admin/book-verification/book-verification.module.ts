import { NgModule } from '@angular/core';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { ProgressBarModule } from 'projects/shared/components/progress-bar/progress-bar.module';
import { SimpleSkeletonTemplateModule } from 'projects/shared/components/skeleton/simple-skeleton-template.module';
import { TooltipListPipesModule } from 'projects/shared/pipes/tooltip-pipes/tooltip-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import { MyHourModule } from 'src/app/teacher/my-hour/my-hour.module';
import { BookVerificationClassbookTeacherSelectorComponent } from './book-verification-classbook-teacher-selector/book-verification-classbook-teacher-selector.component';
import {
  BookVerificationDayViewComponent,
  BookVerificationDayViewSkeletonComponent
} from './book-verification-day-view/book-verification-day-view.component';
import {
  BookVerificationLessonViewComponent,
  BookVerificationLessonViewSkeletonComponent
} from './book-verification-lesson-view/book-verification-lesson-view.component';
import {
  BookVerificationMonthViewComponent,
  BookVerificationMonthViewSkeletonComponent
} from './book-verification-month-view/book-verification-month-view.component';
import { BookVerificationRoutingModule } from './book-verification-routing.module';
import {
  BookVerificationYearViewComponent,
  BookVerificationYearViewSkeletonComponent
} from './book-verification-year-view/book-verification-year-view.component';

@NgModule({
  declarations: [
    BookVerificationClassbookTeacherSelectorComponent,
    BookVerificationDayViewComponent,
    BookVerificationDayViewSkeletonComponent,
    BookVerificationMonthViewComponent,
    BookVerificationMonthViewSkeletonComponent,
    BookVerificationLessonViewComponent,
    BookVerificationLessonViewSkeletonComponent,
    BookVerificationYearViewComponent,
    BookVerificationYearViewSkeletonComponent
  ],
  imports: [
    MatTooltipModule,
    BookVerificationRoutingModule,
    BannerModule,
    CommonFormUiModule,
    NomSelectModule,
    ProgressBarModule,
    SimpleSkeletonTemplateModule,
    ActionServiceModule,
    TooltipListPipesModule,
    MyHourModule
  ]
})
export class BookVerificationModule {}
