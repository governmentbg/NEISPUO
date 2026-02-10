import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faChevronLeft as fasChevronLeft } from '@fortawesome/pro-solid-svg-icons/faChevronLeft';
import { faChevronRight as fasChevronRight } from '@fortawesome/pro-solid-svg-icons/faChevronRight';
import { addWeeks, format, getISOWeek, getISOWeekYear, setISODay, setISOWeek } from 'date-fns';
import { bg } from 'date-fns/locale';
import {
  MyScheduleService,
  MySchedule_GetTeacherScheduleTableForWeek
} from 'projects/sb-api-client/src/api/mySchedule.service';
import { PaginatorItem } from 'projects/shared/components/paginator/paginator-item';
import {
  SIMPLE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { BookTabRoutesConfig, mapBookTypeToTabs } from 'projects/shared/utils/book-tabs';
import { extendScheduleHour, getLessonName, getLessonNameShort } from 'projects/shared/utils/schedule';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';

@Component({
  template: SIMPLE_SKELETON_TEMPLATE
})
export class MyScheduleContentSkeletonComponent extends SkeletonComponentBase {
  constructor(myScheduleService: MyScheduleService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const year = tryParseInt(route.snapshot.paramMap.get('year')) ?? throwParamError('year');
    const weekNumber = tryParseInt(route.snapshot.paramMap.get('weekNumber')) ?? throwParamError('weekNumber');

    this.resolve(MyScheduleContentComponent, {
      schoolYear,
      instId,
      year,
      weekNumber,
      schedule: myScheduleService.getTeacherScheduleTableForWeek({
        schoolYear,
        instId,
        year,
        weekNumber
      })
    });
  }
}

@Component({
  selector: 'sb-my-schedule-content',
  templateUrl: './my-schedule-content.component.html'
})
export class MyScheduleContentComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    year: number;
    weekNumber: number;
    schedule: MySchedule_GetTeacherScheduleTableForWeek;
  };

  weekPaginatorItems!: PaginatorItem[];
  templateData!: ReturnType<typeof getTemplateData>;

  readonly destroyed$ = new Subject<void>();

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

    this.templateData = getTemplateData(this.data, null);
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }
}

function getTemplateData(
  data: MyScheduleContentComponent['data'],
  studentSchedule: MySchedule_GetTeacherScheduleTableForWeek | null
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
                bookRouteCommands: ['../../../book', h.classBookId, ...firstTabRoute]
              };
            }) ?? []
        };
      })
    )
  };
}
