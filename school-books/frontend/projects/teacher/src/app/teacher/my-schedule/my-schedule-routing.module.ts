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
import { MyScheduleContentSkeletonComponent } from './my-schedule-content/my-schedule-content.component';
import { MyScheduleComponent } from './my-schedule/my-schedule.component';

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
    path: '',
    component: MyScheduleComponent,
    children: [
      { path: '', pathMatch: 'full', canActivate: [RetirectToCurrentWeek], children: [] },
      {
        path: ':year/:weekNumber',
        component: MyScheduleContentSkeletonComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MyScheduleRoutingModule {}
