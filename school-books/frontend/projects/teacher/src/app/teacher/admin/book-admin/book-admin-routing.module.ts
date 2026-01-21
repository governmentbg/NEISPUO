import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { BookAdminNewSkeletonComponent } from './book-admin-new/book-admin-new.component';
import { BookAdminReorganizeCombineSkeletonComponent } from './book-admin-reorganize/book-admin-reorganize-combine/book-admin-reorganize-combine.component';
import { BookAdminReorganizeSeparateSkeletonComponent } from './book-admin-reorganize/book-admin-reorganize-separate/book-admin-reorganize-separate.component';
import { BookAdminTabsSkeletonComponent } from './book-admin-tabs/book-admin-tabs.component';
import { BookAdminCurriculumComponent } from './book-admin-view/book-admin-curriculum/book-admin-curriculum.component';
import { BookAdminMainDataSkeletonComponent } from './book-admin-view/book-admin-main-data/book-admin-main-data.component';
import { BookAdminClassBookPrintSkeletonComponent } from './book-admin-view/book-admin-print/book-admin-classbook-print/book-admin-classbook-print.component';
import { BookAdminPrintTabsSkeletonComponent } from './book-admin-view/book-admin-print/book-admin-print-tabs/book-admin-print-tabs.component';
import { BookAdminStudentBookPrintSkeletonComponent } from './book-admin-view/book-admin-print/book-admin-studentbook-print/book-admin-studentbook-print.component';
import { BookAdminSchedulesComponent } from './book-admin-view/book-admin-schedules/book-admin-schedules.component';
import { BookAdminSchoolYearProgramSkeletonComponent } from './book-admin-view/book-admin-school-year-program/book-admin-school-year-program.component';
import { BookAdminStudentsSkeletonComponent } from './book-admin-view/book-admin-students/book-admin-students.component';
import { BookAdminViewSkeletonComponent } from './book-admin-view/book-admin-view.component';
import { BookAdminComponent } from './book-admin/book-admin.component';

const routes: Routes = [
  {
    path: '',
    component: BookAdminTabsSkeletonComponent,
    children: [
      {
        path: ':classKind',
        component: BookAdminComponent
      }
    ]
  },

  {
    path: ':classKind/new',
    component: BookAdminNewSkeletonComponent
  },

  {
    path: ':classKind/combine',
    component: BookAdminReorganizeCombineSkeletonComponent
  },

  {
    path: ':classKind/separate',
    component: BookAdminReorganizeSeparateSkeletonComponent
  },

  {
    path: ':classKind/:classBookId',
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'main' },
      {
        path: '',
        component: BookAdminViewSkeletonComponent,
        children: [
          {
            path: 'main',
            component: BookAdminMainDataSkeletonComponent,
            canDeactivate: [DeactivateGuard]
          },
          {
            path: 'subjects',
            component: BookAdminCurriculumComponent
          },
          {
            path: 'students',
            component: BookAdminStudentsSkeletonComponent
          },
          {
            path: 'schedules',
            component: BookAdminSchedulesComponent,
            data: {
              isIndividualSchedule: false
            }
          },
          {
            path: 'individual-schedules',
            component: BookAdminSchedulesComponent,
            data: {
              isIndividualSchedule: true
            }
          },
          {
            path: 'schoolYearProgram',
            component: BookAdminSchoolYearProgramSkeletonComponent
          },
          {
            path: 'print',
            component: BookAdminPrintTabsSkeletonComponent,
            children: [
              { path: '', pathMatch: 'full', redirectTo: 'classBook' },
              {
                path: 'classBook',
                component: BookAdminClassBookPrintSkeletonComponent
              },
              {
                path: 'studentBook',
                component: BookAdminStudentBookPrintSkeletonComponent
              }
            ]
          }
        ]
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BookAdminRoutingModule {}
