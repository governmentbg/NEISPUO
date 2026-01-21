import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faCalendarDay as fadCalendarDay } from '@fortawesome/pro-duotone-svg-icons/faCalendarDay';
import { faChalkboardTeacher as fadChalkboardTeacher } from '@fortawesome/pro-duotone-svg-icons/faChalkboardTeacher';
import { faCog as fadCog } from '@fortawesome/pro-duotone-svg-icons/faCog';
import { format, isValid, parse } from 'date-fns';
import { MyHourService, MyHour_GetTeacherLessons } from 'projects/sb-api-client/src/api/myHour.service';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { VerticalTabItem } from 'projects/shared/components/vertical-tabs/vertical-tab-item';
import { getLessonName } from 'projects/shared/utils/schedule';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class MyHourLessonViewSkeletonComponent extends SkeletonComponentBase {
  constructor(myHourService: MyHourService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const date = parse(route.snapshot.paramMap.get('date') ?? throwParamError('date'), 'yyyy-MM-dd', new Date(), {});
    if (!isValid(date)) {
      throwParamError('date');
    }

    this.resolve(MyHourLessonViewComponent, {
      schoolYear,
      instId,
      date,
      teacherLessons: myHourService.getTeacherLessons({
        schoolYear,
        instId,
        date
      })
    });
  }
}

@Component({
  selector: 'sb-my-hour-lesson-view',
  templateUrl: './my-hour-lesson-view.component.html'
})
export class MyHourLessonViewComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    date: Date;
    teacherLessons: MyHour_GetTeacherLessons;
  };

  readonly destroyed$ = new Subject<void>();

  readonly fadCalendarDay = fadCalendarDay;
  readonly fadChalkboardTeacher = fadChalkboardTeacher;
  readonly fadCog = fadCog;

  dateFormControl!: FormControl<Date>;
  tabs!: VerticalTabItem[];

  constructor(private route: ActivatedRoute, private router: Router) {}

  ngOnInit() {
    this.dateFormControl = new FormControl<Date>({ value: this.data.date, disabled: true }, { nonNullable: true });

    this.dateFormControl.valueChanges
      .pipe(
        tap((date) => {
          this.router.navigate(['../', format(date, 'yyyy-MM-dd', {})], { relativeTo: this.route });
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
    this.tabs = this.data.teacherLessons.map((sl) => ({
      text: `#${sl.hourNumber} (${sl.startTime} - ${sl.endTime}) ${sl.classBookFullName} - ${getLessonName(sl)}`,
      routeCommands: ['./', sl.classBookId, sl.scheduleLessonId],
      routeExtras: { relativeTo: this.route }
    }));

    if (this.data.teacherLessons.length && !this.route.firstChild) {
      const lesson = this.data.teacherLessons.find((l) => !l.isTaken) ?? this.data.teacherLessons[0];

      this.router.navigate(['./', lesson.classBookId, lesson.scheduleLessonId], { relativeTo: this.route });
    }
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }
}
