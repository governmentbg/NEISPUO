import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faCheckSquare as fadCheckSquare } from '@fortawesome/pro-duotone-svg-icons/faCheckSquare';
import { faMapMarkerAlt as fadMapMarkerAlt } from '@fortawesome/pro-duotone-svg-icons/faMapMarkerAlt';
import { faChevronLeft as fasChevronLeft } from '@fortawesome/pro-solid-svg-icons/faChevronLeft';
import { faChevronRight as fasChevronRight } from '@fortawesome/pro-solid-svg-icons/faChevronRight';
import { addWeeks, format, getISOWeek, getISOWeekYear, setISODay, setISOWeek } from 'date-fns';
import { bg } from 'date-fns/locale';
import {
  ClassBookStudentNomsService,
  ClassBookStudentNoms_GetNomsByTerm
} from 'projects/sb-api-client/src/api/classBookStudentNoms.service';
import {
  SchedulesService,
  Schedules_GetClassBookScheduleTableForWeek
} from 'projects/sb-api-client/src/api/schedules.service';
import { TopicsService, Topics_GetAllForWeek } from 'projects/sb-api-client/src/api/topics.service';
import { PaginatorItem } from 'projects/shared/components/paginator/paginator-item';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { extendScheduleHour } from 'projects/shared/utils/schedule';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { Observable, of, Subject } from 'rxjs';
import { catchError, switchMap, takeUntil, tap } from 'rxjs/operators';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class ScheduleSkeletonComponent extends SkeletonComponentBase {
  constructor(
    schedulesService: SchedulesService,
    topicsService: TopicsService,
    classBookStudentNomsService: ClassBookStudentNomsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const year = tryParseInt(route.snapshot.paramMap.get('year')) ?? throwParamError('year');
    const weekNumber = tryParseInt(route.snapshot.paramMap.get('weekNumber')) ?? throwParamError('weekNumber');

    this.resolve(ScheduleComponent, {
      schoolYear,
      instId,
      classBookId,
      year,
      weekNumber,
      individualCurriculumStudents: classBookStudentNomsService.getNomsByTerm({
        instId,
        schoolYear,
        classBookId,
        showOnlyWithIndividualCurriculumSchedule: true
      }),
      schedule: schedulesService.getClassBookScheduleTableForWeek({
        schoolYear,
        instId,
        classBookId,
        year,
        weekNumber
      }),
      topics: topicsService.getAllForWeek({
        schoolYear,
        instId,
        classBookId,
        year,
        weekNumber
      })
    });
  }
}

@Component({
  selector: 'sb-schedule',
  templateUrl: './schedule.component.html',
  styleUrls: ['./schedule.component.scss']
})
export class ScheduleComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    year: number;
    weekNumber: number;
    individualCurriculumStudents: ClassBookStudentNoms_GetNomsByTerm;
    schedule: Schedules_GetClassBookScheduleTableForWeek;
    topics: Topics_GetAllForWeek;
  };

  weekPaginatorItems!: PaginatorItem[];
  individualCurriculumStudentControl!: UntypedFormControl;
  templateData!: ReturnType<typeof getTemplateData>;

  readonly destroyed$ = new Subject<void>();
  readonly fadCheckSquare = fadCheckSquare;
  readonly fadMapMarkerAlt = fadMapMarkerAlt;

  constructor(private schedulesService: SchedulesService) {}

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

    this.individualCurriculumStudentControl = new UntypedFormControl(null);
    (this.individualCurriculumStudentControl.valueChanges as Observable<number | null>)
      .pipe(
        switchMap((studentId) => {
          if (studentId) {
            return this.schedulesService
              .getClassBookScheduleTableForWeek({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                classBookId: this.data.classBookId,
                year: this.data.year,
                weekNumber: this.data.weekNumber,
                personId: studentId
              })
              .pipe(
                catchError((err) => {
                  GlobalErrorHandler.instance.handleError(err);
                  return of(null);
                })
              );
          } else {
            return of(null);
          }
        }),
        tap((schedule) => {
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

function getTemplateData(
  data: ScheduleComponent['data'],
  studentSchedule: Schedules_GetClassBookScheduleTableForWeek | null
) {
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

        return {
          day,
          slotNumber,
          isOffDay: schedule.offDays.includes(day),
          hours:
            slot?.hours?.map((h) => {
              const topic = data.topics.find((t) => t.scheduleLessonId === h.scheduleLessonId);
              const isTaken = topic != null;

              return {
                ...extendScheduleHour(h),
                isTaken: isTaken,
                topicTitles: topic?.titles,
                topicTeacher:
                  topic?.teachers?.length && !h.extTeacherName
                    ? topic.teachers
                        .map((t) => `${t.teacherFirstName} ${t.teacherLastName}` + (t.isReplTeacher ? '(зам.)' : ''))
                        .join(', ')
                    : null,
                topicTeacherShort:
                  topic?.teachers?.length && !h.extTeacherName
                    ? topic.teachers.map((t) => t.teacherLastName + (t.isReplTeacher ? '(зам.)' : '')).join(', ')
                    : null
              };
            }) ?? []
        };
      })
    )
  };
}
