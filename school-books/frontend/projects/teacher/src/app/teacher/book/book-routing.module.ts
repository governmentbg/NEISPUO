import { Injectable, NgModule } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  createUrlTreeFromSnapshot,
  Router,
  RouterModule,
  RouterStateSnapshot,
  Routes
} from '@angular/router';
import { getISOWeek, getISOWeekYear } from 'date-fns';
import { ClassBooksService } from 'projects/sb-api-client/src/api/classBooks.service';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { map, take } from 'rxjs/operators';
import {
  AbsencesDplrEditMode,
  AbsencesDplrEditSkeletonComponent
} from './absences-dplr-edit/absences-dplr-edit.component';
import { AbsencesDplrSkeletonComponent } from './absences-dplr/absences-dplr.component';
import { AbsencesEditMode, AbsencesEditSkeletonComponent } from './absences-edit/absences-edit.component';
import { AbsencesSkeletonComponent } from './absences/absences.component';
import { AttendancesEditMode, AttendancesEditSkeletonComponent } from './attendances-edit/attendances-edit.component';
import { AttendancesSkeletonComponent } from './attendances/attendances.component';
import { BookSkeletonComponent } from './book/book.component';
import { ExamViewSkeletonComponent } from './exam-view/exam-view.component';
import { ExamsComponent } from './exams/exams.component';
import { FirstGradeResultsEditSkeletonComponent } from './first-grade-results-edit/first-grade-results-edit.component';
import { FirstGradeResultsSkeletonComponent } from './first-grade-results/first-grade-results.component';
import { GradeResultSessionsEditSkeletonComponent } from './grade-result-sessions-edit/grade-result-sessions-edit.component';
import { GradeResultSessionsSkeletonComponent } from './grade-result-sessions/grade-result-sessions.component';
import { GradeResultsEditSkeletonComponent } from './grade-results-edit/grade-results-edit.component';
import { GradeResultsSkeletonComponent } from './grade-results/grade-results.component';
import { GradesNewSkeletonComponent } from './grades-new/grades-new.component';
import { GradesViewSkeletonComponent } from './grades-view/grades-view.component';
import { GradesSkeletonComponent } from './grades/grades.component';
import { IndividualWorkViewSkeletonComponent } from './individual-work-view/individual-work-view.component';
import { IndividualWorksComponent } from './individual-works/individual-works.component';
import { MedicalNoticesComponent } from './medical-notices/medical-notices.component';
import { NoteViewSkeletonComponent } from './note-view/note-view.component';
import { NotesComponent } from './notes/notes.component';
import { ParentMeetingViewSkeletonComponent } from './parent-meeting-view/parent-meeting-view.component';
import { ParentMeetingsComponent } from './parent-meetings/parent-meetings.component';
import { PerformanceViewSkeletonComponent } from './performance-view/performance-view.component';
import { PerformancesComponent } from './performances/performances.component';
import { PgResultViewSkeletonComponent } from './pg-result-view/pg-result-view.component';
import { PgResultsSkeletonComponent } from './pg-results/pg-results.component';
import { RemarkViewSkeletonComponent } from './remark-view/remark-view.component';
import { RemarksSkeletonComponent } from './remarks/remarks.component';
import { ReplrParticipationViewSkeletonComponent } from './replr-participation-view/replr-participation-view.component';
import { ReplrParticipationsComponent } from './replr-participations/replr-participations.component';
import { SanctionViewSkeletonComponent } from './sanction-view/sanction-view.component';
import { SanctionsComponent } from './sanctions/sanctions.component';
import { ScheduleSkeletonComponent } from './schedule/schedule.component';
import { SupportViewSkeletonComponent } from './support-view/support-view.component';
import { SupportsComponent } from './supports/supports.component';
import { TopicPlansItemsComponent } from './topic-plans-items/topic-plans-items.component';
import { TopicPlansSkeletonComponent } from './topic-plans/topic-plans.component';
import { TopicsDplrEditMode, TopicsEditDplrSkeletonComponent } from './topics-edit-dplr/topics-edit-dplr.component';
import { TopicsEditMode, TopicsEditSkeletonComponent } from './topics-edit/topics-edit.component';

@Injectable({ providedIn: 'root' })
export class RetirectToCurrentWeek implements CanActivate {
  constructor(private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    // TODO redirect to the closest week from the schedule
    const now = new Date();
    return createUrlTreeFromSnapshot(route, [getISOWeekYear(now), getISOWeek(now)]);
  }
}

@Injectable({ providedIn: 'root' })
export class RetirectToCurrentMonth implements CanActivate {
  constructor(private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const now = new Date();
    return createUrlTreeFromSnapshot(route, [now.getFullYear(), now.getMonth() + 1]);
  }
}

@Injectable({ providedIn: 'root' })
export class RetirectToDefaultGradesCurriculum implements CanActivate {
  constructor(private router: Router, private classBooksService: ClassBooksService) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    if (route.firstChild) {
      // we are going to a concrete curriculum, no need to redirect to default
      return true;
    }

    return this.classBooksService
      .getDefaultCurriculum({
        schoolYear: tryParseInt(route.paramMap.get('schoolYear')) ?? throwParamError('schoolYear'),
        instId: tryParseInt(route.paramMap.get('instId')) ?? throwParamError('instId'),
        classBookId: tryParseInt(route.paramMap.get('classBookId')) ?? throwParamError('classBookId'),
        excludeGradeless: true
      })
      .pipe(
        take(1),
        map((curriculum) =>
          curriculum.curriculumId
            ? // navigate to the default curriculum
              createUrlTreeFromSnapshot(route, [curriculum.curriculumId])
            : // no default(e.g. no curriculums) then proceed to the current route
              // which will display a no-curriculum message
              true
        )
      );
  }
}

@Injectable({ providedIn: 'root' })
export class RetirectToDefaultTopicPlansCurriculum implements CanActivate {
  constructor(private router: Router, private classBooksService: ClassBooksService) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    if (route.firstChild) {
      // we are going to a concrete curriculum, no need to redirect to default
      return true;
    }

    return this.classBooksService
      .getDefaultCurriculum({
        schoolYear: tryParseInt(route.paramMap.get('schoolYear')) ?? throwParamError('schoolYear'),
        instId: tryParseInt(route.paramMap.get('instId')) ?? throwParamError('instId'),
        classBookId: tryParseInt(route.paramMap.get('classBookId')) ?? throwParamError('classBookId'),
        excludeGradeless: true
      })
      .pipe(
        take(1),
        map((curriculum) =>
          curriculum.curriculumId
            ? // navigate to the default curriculum
              createUrlTreeFromSnapshot(route, [curriculum.curriculumId])
            : // no default(e.g. no curriculums) then proceed to the current route
              // which will display a no-curriculum message
              true
        )
      );
  }
}

const routes: Routes = [
  {
    path: ':classBookId',
    component: BookSkeletonComponent,
    children: [
      {
        path: 'grades',
        children: [
          {
            path: '',
            component: GradesSkeletonComponent,
            canActivate: [RetirectToDefaultGradesCurriculum],
            children: [
              {
                path: ':curriculumId',
                component: GradesViewSkeletonComponent
              }
            ]
          },
          {
            path: ':curriculumId/new',
            component: GradesNewSkeletonComponent,
            canDeactivate: [DeactivateGuard]
          }
        ]
      },
      {
        path: 'absences',
        children: [
          {
            path: '',
            component: AbsencesSkeletonComponent
          },
          { path: 'new', pathMatch: 'full', canActivate: [RetirectToCurrentWeek], children: [] },
          {
            path: 'new/:year/:weekNumber',
            component: AbsencesEditSkeletonComponent,
            canDeactivate: [DeactivateGuard],
            data: {
              mode: AbsencesEditMode.New
            }
          },
          { path: 'excuse', pathMatch: 'full', canActivate: [RetirectToCurrentWeek], children: [] },
          {
            path: 'excuse/:year/:weekNumber',
            component: AbsencesEditSkeletonComponent,
            canDeactivate: [DeactivateGuard],
            data: {
              mode: AbsencesEditMode.Excuse
            }
          },
          { path: 'remove', pathMatch: 'full', canActivate: [RetirectToCurrentWeek], children: [] },
          {
            path: 'remove/:year/:weekNumber',
            component: AbsencesEditSkeletonComponent,
            canDeactivate: [DeactivateGuard],
            data: {
              mode: AbsencesEditMode.Remove
            }
          },
          {
            path: 'medical-notices',
            component: MedicalNoticesComponent
          }
        ]
      },
      {
        path: 'absences-dplr',
        children: [
          {
            path: '',
            component: AbsencesDplrSkeletonComponent
          },
          { path: 'new', pathMatch: 'full', canActivate: [RetirectToCurrentWeek], children: [] },
          {
            path: 'new/:year/:weekNumber',
            component: AbsencesDplrEditSkeletonComponent,
            canDeactivate: [DeactivateGuard],
            data: {
              mode: AbsencesDplrEditMode.New
            }
          },
          { path: 'remove', pathMatch: 'full', canActivate: [RetirectToCurrentWeek], children: [] },
          {
            path: 'remove/:year/:weekNumber',
            component: AbsencesDplrEditSkeletonComponent,
            canDeactivate: [DeactivateGuard],
            data: {
              mode: AbsencesDplrEditMode.Remove
            }
          }
        ]
      },
      {
        path: 'topics',
        children: [
          { path: '', pathMatch: 'full', canActivate: [RetirectToCurrentWeek], children: [] },
          {
            path: ':year/:weekNumber',
            component: TopicsEditSkeletonComponent,
            data: {
              mode: TopicsEditMode.View
            }
          },
          {
            path: 'new/:year/:weekNumber',
            component: TopicsEditSkeletonComponent,
            canDeactivate: [DeactivateGuard],
            data: {
              mode: TopicsEditMode.New
            }
          },
          {
            path: 'remove/:year/:weekNumber',
            component: TopicsEditSkeletonComponent,
            canDeactivate: [DeactivateGuard],
            data: {
              mode: TopicsEditMode.Remove
            }
          }
        ]
      },
      {
        path: 'topics-dplr',
        children: [
          { path: '', pathMatch: 'full', canActivate: [RetirectToCurrentWeek], children: [] },
          {
            path: ':year/:weekNumber',
            component: TopicsEditDplrSkeletonComponent,
            data: {
              mode: TopicsDplrEditMode.View
            }
          },
          {
            path: 'new/:year/:weekNumber',
            component: TopicsEditDplrSkeletonComponent,
            canDeactivate: [DeactivateGuard],
            data: {
              mode: TopicsDplrEditMode.New
            }
          },
          {
            path: 'remove/:year/:weekNumber',
            component: TopicsEditDplrSkeletonComponent,
            canDeactivate: [DeactivateGuard],
            data: {
              mode: TopicsDplrEditMode.Remove
            }
          },
          {
            path: 'removeTopicDplr/:year/:weekNumber',
            component: TopicsEditDplrSkeletonComponent,
            canDeactivate: [DeactivateGuard],
            data: {
              mode: TopicsDplrEditMode.RemoveTopicDplr
            }
          }
        ]
      },
      {
        path: 'attendances',
        children: [
          { path: '', pathMatch: 'full', redirectTo: 'view' },
          { path: 'view', pathMatch: 'full', canActivate: [RetirectToCurrentMonth], children: [] },
          {
            path: 'view/:year/:month',
            children: [
              { path: '', component: AttendancesSkeletonComponent },
              {
                path: 'medical-notices',
                component: MedicalNoticesComponent
              }
            ]
          },
          {
            path: 'new/:date',
            component: AttendancesEditSkeletonComponent,
            canDeactivate: [DeactivateGuard],
            data: {
              mode: AttendancesEditMode.New
            }
          },
          {
            path: 'remove/:date',
            component: AttendancesEditSkeletonComponent,
            canDeactivate: [DeactivateGuard],
            data: {
              mode: AttendancesEditMode.Remove
            }
          }
        ]
      },
      {
        path: 'remarks',
        children: [
          { path: '', component: RemarksSkeletonComponent },
          { path: 'new', component: RemarkViewSkeletonComponent, canDeactivate: [DeactivateGuard] },
          { path: ':remarkId', component: RemarkViewSkeletonComponent, canDeactivate: [DeactivateGuard] }
        ]
      },
      {
        path: 'schedule',
        children: [
          { path: '', pathMatch: 'full', canActivate: [RetirectToCurrentWeek], children: [] },
          {
            path: ':year/:weekNumber',
            component: ScheduleSkeletonComponent
          }
        ]
      },
      {
        path: 'parent-meetings',
        children: [
          { path: '', component: ParentMeetingsComponent, canDeactivate: [DeactivateGuard] },
          { path: ':parentMeetingId', component: ParentMeetingViewSkeletonComponent, canDeactivate: [DeactivateGuard] }
        ]
      },
      {
        path: 'notes',
        children: [
          { path: '', component: NotesComponent },
          { path: 'new', component: NoteViewSkeletonComponent, canDeactivate: [DeactivateGuard] },
          { path: ':noteId', component: NoteViewSkeletonComponent, canDeactivate: [DeactivateGuard] }
        ]
      },
      {
        path: 'sanctions',
        children: [
          { path: '', component: SanctionsComponent },
          { path: 'new', component: SanctionViewSkeletonComponent, canDeactivate: [DeactivateGuard] },
          { path: ':sanctionId', component: SanctionViewSkeletonComponent, canDeactivate: [DeactivateGuard] }
        ]
      },
      {
        path: 'exams',
        children: [
          { path: '', component: ExamsComponent },
          { path: 'new', component: ExamViewSkeletonComponent, canDeactivate: [DeactivateGuard] },
          { path: ':examId', component: ExamViewSkeletonComponent, canDeactivate: [DeactivateGuard] }
        ]
      },
      {
        path: 'topic-plans',
        children: [
          {
            path: '',
            component: TopicPlansSkeletonComponent,
            canActivate: [RetirectToDefaultTopicPlansCurriculum],
            children: [
              {
                path: ':curriculumId',
                component: TopicPlansItemsComponent
              }
            ]
          }
        ]
      },
      {
        path: 'supports',
        children: [
          { path: '', component: SupportsComponent },
          { path: 'new', component: SupportViewSkeletonComponent, canDeactivate: [DeactivateGuard] },
          { path: ':supportId', component: SupportViewSkeletonComponent, canDeactivate: [DeactivateGuard] }
        ]
      },
      {
        path: 'first-grade-results',
        children: [
          { path: '', component: FirstGradeResultsSkeletonComponent, canDeactivate: [DeactivateGuard] },
          { path: 'edit', component: FirstGradeResultsEditSkeletonComponent, canDeactivate: [DeactivateGuard] }
        ]
      },
      {
        path: 'pg-results',
        children: [
          { path: '', component: PgResultsSkeletonComponent },
          { path: 'new', component: PgResultViewSkeletonComponent, canDeactivate: [DeactivateGuard] },
          { path: ':pgResultId', component: PgResultViewSkeletonComponent, canDeactivate: [DeactivateGuard] }
        ]
      },
      {
        path: 'individual-works',
        children: [
          { path: '', component: IndividualWorksComponent },
          { path: 'new', component: IndividualWorkViewSkeletonComponent, canDeactivate: [DeactivateGuard] },
          {
            path: ':individualWorkId',
            component: IndividualWorkViewSkeletonComponent,
            canDeactivate: [DeactivateGuard]
          }
        ]
      },
      {
        path: 'grade-results',
        children: [
          { path: '', component: GradeResultsSkeletonComponent },
          { path: 'edit', component: GradeResultsEditSkeletonComponent, canDeactivate: [DeactivateGuard] }
        ]
      },
      {
        path: 'sessions',
        children: [
          { path: '', component: GradeResultSessionsSkeletonComponent },
          { path: 'edit', component: GradeResultSessionsEditSkeletonComponent, canDeactivate: [DeactivateGuard] }
        ]
      },
      {
        path: 'performances',
        children: [
          { path: '', component: PerformancesComponent },
          { path: 'new', component: PerformanceViewSkeletonComponent, canDeactivate: [DeactivateGuard] },
          { path: ':performanceId', component: PerformanceViewSkeletonComponent, canDeactivate: [DeactivateGuard] }
        ]
      },
      {
        path: 'replrParticipations',
        children: [
          { path: '', component: ReplrParticipationsComponent },
          { path: 'new', component: ReplrParticipationViewSkeletonComponent, canDeactivate: [DeactivateGuard] },
          {
            path: ':replrParticipationId',
            component: ReplrParticipationViewSkeletonComponent,
            canDeactivate: [DeactivateGuard]
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
export class BookRoutingModule {}
