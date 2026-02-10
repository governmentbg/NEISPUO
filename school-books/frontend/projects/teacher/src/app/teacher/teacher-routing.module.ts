import { Injectable, NgModule } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterModule,
  RouterStateSnapshot,
  Routes,
  UrlTree
} from '@angular/router';
import { SnackbarRootComponent } from 'projects/shared/components/snackbar-root/snackbar-root.component';
import { AuthService, SysRole } from 'projects/shared/services/auth.service';
import { Project } from 'projects/shared/services/config.service';
import { TeacherSkeletonComponent } from './teacher.component';

@Injectable({
  providedIn: 'root'
})
export class TeacherConversationsGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
    const hasAccess =
      this.authService.tokenPayload.selected_role.SysRoleID === SysRole.Institution ||
      this.authService.tokenPayload.selected_role.SysRoleID === SysRole.Teacher ||
      this.authService.tokenPayload.selected_role.SysRoleID === SysRole.InstitutionExpert;
    if (!hasAccess) {
      return this.router.createUrlTree(['/not-found']);
    }
    return true;
  }
}

const routes: Routes = [
  {
    path: '',
    component: SnackbarRootComponent,
    children: [
      {
        path: '',
        component: TeacherSkeletonComponent,
        children: [
          // empty route
          { path: '', pathMatch: 'full', redirectTo: 'info-board;status=published' },

          // info-board
          {
            path: 'info-board',
            loadChildren: () => import('./info-board/info-board.module').then((m) => m.InfoBoardModule)
          },

          // my-schedule
          {
            path: 'my-schedule',
            loadChildren: () => import('./my-schedule/my-schedule.module').then((m) => m.MyScheduleModule)
          },

          // my-hour
          {
            path: 'my-hour',
            loadChildren: () => import('./my-hour/my-hour.module').then((m) => m.MyHourModule)
          },

          // book
          {
            path: 'book',
            loadChildren: () => import('./book/book.module').then((m) => m.BookModule)
          },

          // reg books
          {
            path: 'spbs-book',
            loadChildren: () => import('./reg-books/spbs-book/spbs-book.module').then((m) => m.SpbsBookModule)
          },
          {
            path: 'reg-book-qualification',
            loadChildren: () =>
              import('./reg-books/reg-book-qualification/reg-book-qualification.module').then(
                (m) => m.RegBookQualificationModule
              )
          },
          {
            path: 'reg-book-qualification-duplicates',
            loadChildren: () =>
              import('./reg-books/reg-book-qualification-duplicates/reg-book-qualification-duplicates.module').then(
                (m) => m.RegBookQualificationDuplicatesModule
              )
          },
          {
            path: 'reg-book-certificates',
            loadChildren: () =>
              import('./reg-books/reg-book-certificates/reg-book-certificates.module').then(
                (m) => m.RegBookCertificatesModule
              )
          },
          {
            path: 'reg-book-certificate-duplicates',
            loadChildren: () =>
              import('./reg-books/reg-book-certificate-duplicates/reg-book-certificate-duplicates.module').then(
                (m) => m.RegBookCertificateDuplicatesModule
              )
          },

          // protocols
          {
            path: 'adm-protocols',
            loadChildren: () =>
              import('./protocols/adm-protocols/adm-protocols.module').then((m) => m.AdmProtocolsModule)
          },
          {
            path: 'exam-duty-protocols',
            loadChildren: () =>
              import('./protocols/exam-duty-protocols/exam-duty-protocols.module').then(
                (m) => m.ExamDutyProtocolsModule
              )
          },
          {
            path: 'exam-result-protocols',
            loadChildren: () =>
              import('./protocols/exam-result-protocols/exam-result-protocols.module').then(
                (m) => m.ExamResultProtocolsModule
              )
          },

          // lecture schedules
          {
            path: 'lecture-schedules',
            loadChildren: () =>
              import('./lecture-schedules/lecture-schedules.module').then((m) => m.LectureSchedulesModule)
          },

          // teacher absences
          {
            path: 'teacher-absences',
            loadChildren: () =>
              import('./teacher-absences/teacher-absences.module').then((m) => m.TeacherAbsencesModule)
          },

          // topic-plans
          {
            path: 'topic-plans',
            loadChildren: () => import('./topic-plans/topic-plans.module').then((m) => m.TopicPlansModule)
          },

          // admin
          {
            path: 'shifts',
            loadChildren: () => import('./admin/shifts/shifts.module').then((m) => m.ShiftsModule)
          },
          {
            path: 'off-days',
            loadChildren: () => import('./admin/off-days/off-days.module').then((m) => m.OffDaysModule)
          },
          {
            path: 'school-year-settings',
            loadChildren: () =>
              import('./admin/school-year-settings/school-year-settings.module').then((m) => m.SchoolYearSettingsModule)
          },
          {
            path: 'book-admin',
            loadChildren: () => import('./admin/book-admin/book-admin.module').then((m) => m.BookAdminModule)
          },
          {
            path: 'missing-topics-reports',
            loadChildren: () =>
              import('./reports/missing-topics-reports/missing-topics-reports.module').then(
                (m) => m.MissingTopicsReportsModule
              )
          },
          {
            path: 'lecture-schedules-reports',
            loadChildren: () =>
              import('./reports/lecture-schedules-reports/lecture-schedules-reports.module').then(
                (m) => m.LectureSchedulesReportsModule
              )
          },
          {
            path: 'students-at-risk-of-dropping-out-reports',
            loadChildren: () =>
              import(
                './reports/students-at-risk-of-dropping-out-reports/students-at-risk-of-dropping-out-reports.module'
              ).then((m) => m.StudentsAtRiskOfDroppingOutReportsModule)
          },
          {
            path: 'gradeless-students-reports',
            loadChildren: () =>
              import('./reports/gradeless-students-reports/gradeless-students-reports.module').then(
                (m) => m.GradelessStudentsReportsModule
              )
          },
          {
            path: 'session-students-reports',
            loadChildren: () =>
              import('./reports/session-students-reports/session-students-reports.module').then(
                (m) => m.SessionStudentsReportsModule
              )
          },
          {
            path: 'absences-by-students-reports',
            loadChildren: () =>
              import('./reports/absences-by-students-reports/absences-by-students-reports.module').then(
                (m) => m.AbsencesByStudentsReportsModule
              )
          },
          {
            path: 'absences-by-classes-reports',
            loadChildren: () =>
              import('./reports/absences-by-classes-reports/absences-by-classes-reports.module').then(
                (m) => m.AbsencesByClassesReportsModule
              )
          },
          {
            path: 'date-absences-reports',
            loadChildren: () =>
              import('./reports/date-absences-reports/date-absences-reports.module').then(
                (m) => m.DateAbsencesReportsModule
              )
          },
          {
            path: 'regular-grade-point-average-by-classes-reports',
            loadChildren: () =>
              import(
                './reports/regular-grade-point-average-by-classes-reports/regular-grade-point-average-by-classes-reports.module'
              ).then((m) => m.RegularGradePointAverageByClassesReportsModule)
          },
          {
            path: 'regular-grade-point-average-by-students-reports',
            loadChildren: () =>
              import(
                './reports/regular-grade-point-average-by-students-reports/regular-grade-point-average-by-students-reports.module'
              ).then((m) => m.RegularGradePointAverageByStudentsReportsModule)
          },
          {
            path: 'final-grade-point-average-by-students-reports',
            loadChildren: () =>
              import(
                './reports/final-grade-point-average-by-students-reports/final-grade-point-average-by-students-reports.module'
              ).then((m) => m.FinalGradePointAverageByStudentsReportsModule)
          },
          {
            path: 'final-grade-point-average-by-classes-reports',
            loadChildren: () =>
              import(
                './reports/final-grade-point-average-by-classes-reports/final-grade-point-average-by-classes-reports.module'
              ).then((m) => m.FinalGradePointAverageByClassesReportsModule)
          },
          {
            path: 'exams-reports',
            loadChildren: () => import('./reports/exams-reports/exams-reports.module').then((m) => m.ExamsReportsModule)
          },
          {
            path: 'schedule-and-absences-by-term-reports',
            loadChildren: () =>
              import(
                './reports/schedule-and-absences-by-term-reports/schedule-and-absences-by-term-reports.module'
              ).then((m) => m.ScheduleAndAbsencesByTermReportsModule)
          },
          {
            path: 'schedule-and-absences-by-month-reports',
            loadChildren: () =>
              import(
                './reports/schedule-and-absences-by-month-reports/schedule-and-absences-by-month-reports.module'
              ).then((m) => m.ScheduleAndAbsencesByMonthReportsModule)
          },
          {
            path: 'schedule-and-absences-by-term-all-classes-reports',
            loadChildren: () =>
              import(
                './reports/schedule-and-absences-by-term-all-classes-reports/schedule-and-absences-by-term-all-classes-reports.module'
              ).then((m) => m.ScheduleAndAbsencesByTermAllClassesReportsModule)
          },
          {
            path: 'teacher-schedules',
            loadChildren: () =>
              import('./admin/teacher-schedules/teacher-schedules.module').then((m) => m.TeacherSchedulesModule)
          },
          {
            path: 'book-verification',
            loadChildren: () =>
              import('./admin/book-verification/book-verification.module').then((m) => m.BookVerificationModule)
          },
          {
            path: 'publications',
            loadChildren: () => import('./admin/publications/publications.module').then((m) => m.PublicationsModule)
          },
          {
            path: 'finalization',
            loadChildren: () =>
              import('./admin/finalization-internal/finalization-internal.module').then(
                (m) => m.FinalizationInternalModule
              )
          },
          {
            path: 'external-finalization',
            loadChildren: () =>
              import('./admin/finalization-external/finalization-external.module').then(
                (m) => m.FinalizationExternalModule
              )
          },

          // student-info
          {
            path: 'student-info',
            loadChildren: () => import('./student-info/student-info.module').then((m) => m.StudentInfoModule)
          },

          // conversations
          {
            path: 'conversations',
            canActivate: [TeacherConversationsGuard],
            loadChildren: () =>
              import('projects/shared/components/conversations/conversations.module').then(
                (m) => m.ConversationsModule
              ),
            data: { project: Project.TeachersApp }
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
export class TeacherRoutingModule {}
