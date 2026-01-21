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
import { AbsencesDplrSkeletonComponent } from './absences-dplr/absences-dplr.component';
import { AbsencesSkeletonComponent } from './absences/absences.component';
import { AttendancesSkeletonComponent } from './attendances/attendances.component';
import { BookSkeletonComponent } from './book/book.component';
import { ExamsComponent } from './exams/exams.component';
import { FirstGradeResultsSkeletonComponent } from './first-grade-results/first-grade-results.component';
import { GradeResultsSkeletonComponent } from './grade-results/grade-results.component';
import { GradesSkeletonComponent } from './grades/grades.component';
import { IndividualWorksComponent } from './individual-works/individual-works.component';
import { NotesComponent } from './notes/notes.component';
import { ParentMeetingsComponent } from './parent-meetings/parent-meetings.component';
import { PgResultsComponent } from './pg-results/pg-results.component';
import { RemarksSkeletonComponent } from './remarks/remarks.component';
import { SanctionsComponent } from './sanctions/sanctions.component';
import { ScheduleSkeletonComponent } from './schedule/schedule.component';
import { SupportViewSkeletonComponent } from './support-view/support-view.component';
import { SupportsComponent } from './supports/supports.component';
import { TopicsViewSkeletonComponent } from './topics-view/topics-view.component';
import { TopicsSkeletonComponent } from './topics/topics.component';

@Injectable({ providedIn: 'root' })
export class RetirectToCurrentWeek implements CanActivate {
  constructor(private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    // TODO redirect to the closest week from the schedule
    const now = new Date();
    return createUrlTreeFromSnapshot(route, [getISOWeekYear(now), getISOWeek(now)]);
  }
}

const routes: Routes = [
  {
    path: ':classBookId/:personId',
    component: BookSkeletonComponent,
    children: [
      {
        path: 'grades',
        component: GradesSkeletonComponent
      },
      {
        path: 'absences',
        component: AbsencesSkeletonComponent
      },
      {
        path: 'absences-dplr',
        component: AbsencesDplrSkeletonComponent
      },
      {
        path: 'topics',
        children: [
          {
            path: '',
            component: TopicsSkeletonComponent,
            children: [
              {
                path: ':curriculumId',
                component: TopicsViewSkeletonComponent
              }
            ]
          }
        ]
      },
      {
        path: 'attendances',
        component: AttendancesSkeletonComponent
      },
      {
        path: 'remarks',
        component: RemarksSkeletonComponent
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
        component: ParentMeetingsComponent
      },
      {
        path: 'notes',
        component: NotesComponent
      },
      {
        path: 'sanctions',
        component: SanctionsComponent
      },
      {
        path: 'exams',
        component: ExamsComponent
      },
      {
        path: 'supports',
        children: [
          { path: '', component: SupportsComponent },
          { path: ':supportId', component: SupportViewSkeletonComponent }
        ]
      },
      {
        path: 'first-grade-results',
        component: FirstGradeResultsSkeletonComponent
      },
      {
        path: 'pg-results',
        component: PgResultsComponent
      },
      {
        path: 'individual-works',
        component: IndividualWorksComponent
      },
      {
        path: 'grade-results',
        component: GradeResultsSkeletonComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BookRoutingModule {}
