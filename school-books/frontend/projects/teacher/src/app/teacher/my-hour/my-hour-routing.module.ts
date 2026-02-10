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
import { format } from 'date-fns';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { AddAbsencesSkeletonComponent } from './add-absences/add-absences.component';
import { AddGradesSkeletonComponent } from './add-grades/add-grades.component';
import { AddTopicSkeletonComponent } from './add-topic/add-topic.component';
import { MyHourLessonViewContentSkeletonComponent } from './my-hour-lesson-view-content/my-hour-lesson-view-content.component';
import { MyHourLessonViewSkeletonComponent } from './my-hour-lesson-view/my-hour-lesson-view.component';

@Injectable({ providedIn: 'root' })
export class RetirectToCurrentDate implements CanActivate {
  constructor(private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    // TODO redirect to the closest week from the schedule
    const now = new Date();
    return createUrlTreeFromSnapshot(route, [format(now, 'yyyy-MM-dd', {})]);
  }
}

const routes: Routes = [
  { path: '', pathMatch: 'full', canActivate: [RetirectToCurrentDate], children: [] },
  {
    path: ':date',
    component: MyHourLessonViewSkeletonComponent,
    children: [
      {
        path: ':classBookId/:scheduleLessonId',
        component: MyHourLessonViewContentSkeletonComponent
      }
    ]
  },
  {
    path: ':date/:classBookId/:scheduleLessonId/add-topic',
    component: AddTopicSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  },
  {
    path: ':date/:classBookId/:scheduleLessonId/add-grades',
    component: AddGradesSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  },
  {
    path: ':date/:classBookId/:scheduleLessonId/add-absences',
    component: AddAbsencesSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MyHourRoutingModule {}
