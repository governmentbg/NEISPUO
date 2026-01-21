import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faCalendarAlt as fadCalendarAlt } from '@fortawesome/pro-duotone-svg-icons/faCalendarAlt';
import { faChevronLeft as fasChevronLeft } from '@fortawesome/pro-solid-svg-icons/faChevronLeft';
import { faChevronRight as fasChevronRight } from '@fortawesome/pro-solid-svg-icons/faChevronRight';
import { addWeeks, format, getISOWeek, getISOWeekYear, setISODay, setISOWeek } from 'date-fns';
import { bg } from 'date-fns/locale';
import { InstTeacherNomsService } from 'projects/sb-api-client/src/api/instTeacherNoms.service';
import {
  TeacherSchedulesService,
  TeacherSchedules_GetTeacherScheduleTableForWeek
} from 'projects/sb-api-client/src/api/teacherSchedules.service';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import { PaginatorItem } from 'projects/shared/components/paginator/paginator-item';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { BookTabRoutesConfig, mapBookTypeToTabs } from 'projects/shared/utils/book-tabs';
import { extendScheduleHour, getLessonName, getLessonNameShort } from 'projects/shared/utils/schedule';
import { deepEqual, throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { BehaviorSubject, combineLatest, of, Subject } from 'rxjs';
import { catchError, distinctUntilChanged, map, switchMap, takeUntil, tap } from 'rxjs/operators';

@Component({
  selector: 'sb-teacher-schedules',
  templateUrl: './teacher-schedules.component.html'
})
export class TeacherSchedulesComponent implements OnInit, OnDestroy {
  weekPaginatorItems: PaginatorItem[];
  templateData!: ReturnType<typeof getTemplateData>;
  loadingSchedule = false;

  readonly instTeacherNomsService: INomService<number, { instId: number; schoolYear: number }>;
  readonly fadCalendarAlt = fadCalendarAlt;
  readonly form = this.fb.nonNullable.group({
    teacherPersonId: this.fb.nonNullable.control<number | null | undefined>(null)
  });

  private readonly schoolYear: number;
  private readonly instId: number;
  private readonly destroyed$ = new Subject<void>();
  private readonly selectedWeek$: BehaviorSubject<{ year: number; weekNumber: number }>;

  constructor(
    instTeacherNomsService: InstTeacherNomsService,
    route: ActivatedRoute,
    private fb: FormBuilder,
    private teacherSchedulesService: TeacherSchedulesService
  ) {
    this.schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    this.instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');

    const now = new Date();
    const year = getISOWeekYear(now);
    const weekNumber = getISOWeek(now);
    this.weekPaginatorItems = getPaginatorItems(year, weekNumber);
    this.selectedWeek$ = new BehaviorSubject({ year, weekNumber });

    this.instTeacherNomsService = new NomServiceWithParams(instTeacherNomsService, () => ({
      instId: this.instId,
      schoolYear: this.schoolYear,
      includeNotActiveTeachers: true
    }));
  }

  ngOnInit() {
    const { year, weekNumber } = this.selectedWeek$.value;
    this.weekPaginatorItems = getPaginatorItems(year, weekNumber);

    const teacherPersonIdControl =
      this.form.get('teacherPersonId') ?? throwError("'teacherPersonId' control should exist");
    combineLatest([this.selectedWeek$, teacherPersonIdControl.valueChanges])
      .pipe(
        map(
          ([selectedWeek, teacherPersonId]) => [selectedWeek.year, selectedWeek.weekNumber, teacherPersonId] as const
        ),
        distinctUntilChanged((a, b) => deepEqual(a, b)),
        tap(() => {
          this.loadingSchedule = true;
        }),
        switchMap(([year, weekNumber, teacherPersonId]) => {
          if (teacherPersonId) {
            return this.teacherSchedulesService.getTeacherScheduleTableForWeek({
              schoolYear: this.schoolYear,
              instId: this.instId,
              year,
              weekNumber,
              personId: teacherPersonId
            });
          } else {
            return of(null);
          }
        }),
        catchError((err) => {
          GlobalErrorHandler.instance.handleError(err);
          return of(null);
        }),
        tap((schedule) => {
          this.loadingSchedule = false;
          this.templateData = getTemplateData(schedule);
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  paginatorItemClicked({ year, weekNumber }: { year: number; weekNumber: number }) {
    this.weekPaginatorItems = getPaginatorItems(year, weekNumber);
    this.selectedWeek$.next({ year, weekNumber });
  }
}

function getPaginatorItems(year: number, weekNumber: number): PaginatorItem[] {
  const weekStart = setISODay(setISOWeek(new Date(year, 7, 7), weekNumber), 1);
  const weekEnd = setISODay(setISOWeek(new Date(year, 7, 7), weekNumber), 7);

  const prevWeekStart = addWeeks(weekStart, -1);
  const prevWeekYear = getISOWeekYear(prevWeekStart);
  const prevWeekNumber = getISOWeek(prevWeekStart);

  const nextWeekStart = addWeeks(weekStart, 1);
  const nextWeekYear = getISOWeekYear(nextWeekStart);
  const nextWeekNumber = getISOWeek(nextWeekStart);

  return [
    {
      icon: fasChevronLeft,
      clickable: true,
      state: { year: prevWeekYear, weekNumber: prevWeekNumber }
    },
    {
      text: `${format(weekStart, 'dd.MM', { locale: bg })} - ${format(weekEnd, 'dd.MM', { locale: bg })}`
    },
    {
      icon: fasChevronRight,
      clickable: true,
      state: { year: nextWeekYear, weekNumber: nextWeekNumber }
    }
  ];
}

function getTemplateData(teacherSchedule: TeacherSchedules_GetTeacherScheduleTableForWeek | null) {
  if (teacherSchedule != null) {
    const scheduleIncludesWeekend = teacherSchedule.slots.find((slot) => slot.day > 5) != null;

    return {
      shiftHours: teacherSchedule.shiftHours,
      scheduleIncludesWeekend,
      slotsByNumberByDay: teacherSchedule.shiftHours.map((shiftHour) =>
        Array.from({ length: scheduleIncludesWeekend ? 7 : 5 }, (v, dayIndex) => {
          const day = dayIndex + 1;
          const slotNumber = shiftHour.slotNumber;

          const slot = teacherSchedule.slots.find((slot) => slot.day === day && slot.slotNumber === slotNumber);

          return {
            day,
            slotNumber,
            hours:
              slot?.hours?.map((h) => {
                const firstTabRoute = mapBookTypeToTabs(false, BookTabRoutesConfig, {
                  bookType: h.bookType,
                  basicClassId: h.basicClassId
                })[0];

                return {
                  ...extendScheduleHour(h),
                  subject: getLessonName(h),
                  subjectShort: getLessonNameShort(h),
                  bookRouteCommands: ['../book', h.classBookId, ...firstTabRoute]
                };
              }) ?? []
          };
        })
      )
    };
  } else {
    return null;
  }
}
