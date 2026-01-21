import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faChevronLeft as fasChevronLeft } from '@fortawesome/pro-solid-svg-icons/faChevronLeft';
import { faChevronRight as fasChevronRight } from '@fortawesome/pro-solid-svg-icons/faChevronRight';
import { addWeeks, format, getISOWeek, getISOWeekYear, setISODay, setISOWeek } from 'date-fns';
import { bg } from 'date-fns/locale';
import {
  StudentClassBookService,
  StudentClassBook_GetClassBookInfo,
  StudentClassBook_GetSchedule
} from 'projects/sb-api-client/src/api/studentClassBook.service';
import { PaginatorItem } from 'projects/shared/components/paginator/paginator-item';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { extendScheduleHour } from 'projects/shared/utils/schedule';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, of, Subject } from 'rxjs';
import { catchError, switchMap, takeUntil, tap } from 'rxjs/operators';
import { CLASS_BOOK_INFO } from '../book/book.component';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class ScheduleSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<StudentClassBook_GetClassBookInfo>,
    studentClassBookService: StudentClassBookService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');
    const year = tryParseInt(route.snapshot.paramMap.get('year')) ?? throwParamError('year');
    const weekNumber = tryParseInt(route.snapshot.paramMap.get('weekNumber')) ?? throwParamError('weekNumber');

    this.resolve(ScheduleComponent, {
      schoolYear,
      classBookId,
      personId,
      year,
      weekNumber,
      schedule: studentClassBookService.getSchedule({
        schoolYear,
        classBookId,
        personId,
        year,
        weekNumber,
        showIndividualCurriculum: false
      }),
      classBookInfo: from(classBookInfo)
    });
  }
}

@Component({
  selector: 'sb-schedule',
  templateUrl: './schedule.component.html'
})
export class ScheduleComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    classBookId: number;
    personId: number;
    year: number;
    weekNumber: number;
    schedule: StudentClassBook_GetSchedule;
    classBookInfo: StudentClassBook_GetClassBookInfo;
  };

  showIndividualCurriculumControl!: FormControl<boolean>;
  weekPaginatorItems!: PaginatorItem[];
  templateData!: ReturnType<typeof getTemplateData>;

  loadingSchedule = false;

  readonly destroyed$ = new Subject<void>();

  constructor(private studentClassBookService: StudentClassBookService) {}

  ngOnInit() {
    const weekStart = setISODay(setISOWeek(new Date(this.data.year, 7, 7), this.data.weekNumber), 1);
    const weekEnd = setISODay(setISOWeek(new Date(this.data.year, 7, 7), this.data.weekNumber), 7);

    const prevWeekStart = addWeeks(weekStart, -1);
    const prevWeekYear = getISOWeekYear(prevWeekStart);
    const prevWeekNumber = getISOWeek(prevWeekStart);

    const nextWeekStart = addWeeks(weekStart, 1);
    const nextWeekYear = getISOWeekYear(nextWeekStart);
    const nextWeekNumber = getISOWeek(nextWeekStart);

    this.weekPaginatorItems = [
      {
        icon: fasChevronLeft,
        routeCommands: ['../../', prevWeekYear, prevWeekNumber]
      },
      {
        text: `${format(weekStart, 'dd.MM', { locale: bg })} - ${format(weekEnd, 'dd.MM', { locale: bg })}`
      },
      {
        icon: fasChevronRight,
        routeCommands: ['../../', nextWeekYear, nextWeekNumber]
      }
    ];

    this.showIndividualCurriculumControl = new FormControl<boolean>(false, { nonNullable: true });
    this.showIndividualCurriculumControl.valueChanges
      .pipe(
        tap(() => {
          this.loadingSchedule = true;
        }),
        switchMap((showIndividualCurriculum) =>
          this.studentClassBookService
            .getSchedule({
              schoolYear: this.data.schoolYear,
              classBookId: this.data.classBookId,
              personId: this.data.personId,
              year: this.data.year,
              weekNumber: this.data.weekNumber,
              showIndividualCurriculum
            })
            .pipe(
              catchError((err) => {
                GlobalErrorHandler.instance.handleError(err);
                return of(null);
              })
            )
        ),
        tap((schedule) => {
          this.loadingSchedule = false;
          this.templateData = getTemplateData(this.data, schedule);
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();

    this.templateData = getTemplateData(this.data, null);
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }
}

function getTemplateData(data: ScheduleComponent['data'], studentSchedule: StudentClassBook_GetSchedule | null) {
  const schedule = studentSchedule ?? data.schedule;
  const scheduleIncludesWeekend = schedule.slots.find((slot) => slot.day > 5) != null;

  return {
    shiftHours: schedule.shiftHours,
    scheduleIncludesWeekend,
    slotsByNumberByDay: schedule.shiftHours.map((shiftHour) =>
      Array.from({ length: scheduleIncludesWeekend ? 7 : 5 }, (v, dayIndex) => {
        const day = dayIndex + 1;
        const slotNumber = shiftHour.slotNumber;

        const slot = schedule.slots.find((slot) => slot.day === day && slot.slotNumber === slotNumber);

        const filteredHours =
          slot?.hours?.map((hour) => ({
            ...hour,
            curriculumTeachers: hour.curriculumTeachers ?? []
          })) ?? [];

        return {
          day,
          slotNumber,
          hours: filteredHours.map(extendScheduleHour) ?? [],
          isOffDay: schedule.offDays.includes(day)
        };
      })
    )
  };
}
