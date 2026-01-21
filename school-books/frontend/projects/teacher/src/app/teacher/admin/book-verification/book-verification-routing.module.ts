import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BookVerificationDayViewSkeletonComponent } from './book-verification-day-view/book-verification-day-view.component';
import { BookVerificationLessonViewSkeletonComponent } from './book-verification-lesson-view/book-verification-lesson-view.component';
import { BookVerificationMonthViewSkeletonComponent } from './book-verification-month-view/book-verification-month-view.component';
import { BookVerificationYearViewSkeletonComponent } from './book-verification-year-view/book-verification-year-view.component';

const routes: Routes = [
  { path: '', component: BookVerificationYearViewSkeletonComponent },
  {
    path: ':year/:month',
    component: BookVerificationMonthViewSkeletonComponent
  },
  {
    path: ':year/:month/:day',
    component: BookVerificationDayViewSkeletonComponent
  },
  {
    path: ':year/:month/:day/:classBookId/:scheduleLessonId',
    component: BookVerificationLessonViewSkeletonComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BookVerificationRoutingModule {}
