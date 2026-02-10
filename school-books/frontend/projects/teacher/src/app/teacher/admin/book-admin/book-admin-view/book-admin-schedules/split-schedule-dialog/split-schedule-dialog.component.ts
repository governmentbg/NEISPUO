import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCalendarCheck as farCalendarCheck } from '@fortawesome/pro-regular-svg-icons/faCalendarCheck';
import { faCalendarMinus as farCalendarMinus } from '@fortawesome/pro-regular-svg-icons/faCalendarMinus';
import { faCalendarPlus as farCalendarPlus } from '@fortawesome/pro-regular-svg-icons/faCalendarPlus';
import { faClone as farClone } from '@fortawesome/pro-regular-svg-icons/faClone';
import { faTrashAlt as farTrashAlt } from '@fortawesome/pro-regular-svg-icons/faTrashAlt';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { eachWeekOfInterval, endOfWeek, format, getISOWeek, getISOWeekYear } from 'date-fns';
import { bg } from 'date-fns/locale';
import {
  SchedulesService,
  Schedules_Get,
  Schedules_GetOffDates,
  Schedules_GetSchoolYearSettings,
  Schedules_GetUsedDatesWeeks
} from 'projects/sb-api-client/src/api/schedules.service';
import {
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { TypedDialog } from 'projects/shared/utils/dialog';
import { Subject } from 'rxjs';
import { shareReplay, switchMap } from 'rxjs/operators';

export type SplitScheduleDialogData = {
  schoolYear: number;
  instId: number;
  classBookId: number;
  scheduleId: number;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class SplitScheduleDialogSkeletonComponent
  extends SkeletonComponentBase
  implements TypedDialog<SplitScheduleDialogData, boolean>
{
  d!: SplitScheduleDialogData;
  r!: boolean;

  constructor(
    schedulesService: SchedulesService,
    @Inject(MAT_DIALOG_DATA)
    data: SplitScheduleDialogData
  ) {
    super();

    const { schoolYear, instId, classBookId, scheduleId } = data;

    const schedule$ = schedulesService
      .get({
        schoolYear,
        instId,
        classBookId,
        scheduleId
      })
      .pipe(shareReplay(1));

    this.resolve(SplitScheduleDialogComponent, {
      schoolYear,
      instId,
      classBookId,
      scheduleId,
      schoolYearSettings: schedulesService.getSchoolYearSettings({
        schoolYear,
        instId,
        classBookId
      }),
      usedDatesWeeks: schedule$.pipe(
        switchMap((s) =>
          schedulesService.getUsedDatesWeeks({
            schoolYear,
            instId,
            classBookId,
            isIndividualSchedule: s.isIndividualSchedule,
            personId: s.personId,
            exceptScheduleId: scheduleId
          })
        )
      ),
      offDates: schedulesService.getOffDates({
        schoolYear,
        instId,
        classBookId
      }),
      schedule: schedule$
    });
  }
}

type WeekModel = {
  disabled: boolean;
  selected: boolean;
  year: number;
  weekNumber: number;
  startDate: Date;
  startDateDisplay: string;
  endDate: Date;
  endDateDisplay: string;
  monthAbbr: string;
};

@Component({
  selector: 'sb-split-schedule-dialog',
  templateUrl: './split-schedule-dialog.component.html'
})
export class SplitScheduleDialogComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    scheduleId: number;
    schoolYearSettings: Schedules_GetSchoolYearSettings;
    usedDatesWeeks: Schedules_GetUsedDatesWeeks;
    offDates: Schedules_GetOffDates;
    schedule: Schedules_Get;
  };

  private readonly destroyed$ = new Subject<void>();

  readonly fasSpinnerThird = fasSpinnerThird;
  readonly farTrashAlt = farTrashAlt;
  readonly farClone = farClone;
  readonly farCalendarPlus = farCalendarPlus;
  readonly farCalendarMinus = farCalendarMinus;
  readonly farCalendarCheck = farCalendarCheck;

  readonly Object = Object;

  loading = false;
  errors: string[] = [];

  weeks!: WeekModel[];
  weeksByYear!: { [key: string]: WeekModel[] };

  constructor(
    private schedulesService: SchedulesService,
    private dialogRef: MatDialogRef<SplitScheduleDialogSkeletonComponent>
  ) {}

  ngOnInit() {
    this.initializeHelperData();
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  initializeHelperData() {
    this.weeks = eachWeekOfInterval(
      {
        start: this.data.schoolYearSettings.schoolYearStartDate,
        end: this.data.schoolYearSettings.schoolYearEndDate
      },
      { weekStartsOn: 1 }
    ).map((startDate) => {
      const year = getISOWeekYear(startDate);
      const week = getISOWeek(startDate);
      const endDate = endOfWeek(startDate, { weekStartsOn: 1 });

      return {
        disabled: this.data.schedule.weeks.findIndex((dw) => dw.year === year && dw.weekNumber === week) === -1,
        selected: false,
        year: year,
        weekNumber: week,
        startDate,
        startDateDisplay: format(startDate, 'dd.MM', { locale: bg }),
        endDate,
        endDateDisplay: format(endDate, 'dd.MM', { locale: bg }),
        monthAbbr: format(endDate, 'MMM', { locale: bg })
      };
    });

    this.weeksByYear = this.weeks.reduce((acc, w) => {
      const year = w.startDate.getFullYear();
      acc[year] = acc[year] || [];
      acc[year].push(w);
      return acc;
    }, {} as { [key: string]: WeekModel[] });
  }

  weekSelectionChanged(week: WeekModel) {
    week.selected = !week.selected;
  }

  selectAllInternal(filterPredicate: (week: { weekNumber: number }) => boolean) {
    this.weeks
      .filter((w) => !w.disabled)
      .forEach((w) => {
        w.selected = filterPredicate(w);
      });
  }

  selectAll() {
    this.selectAllInternal((w) => true);
  }

  deselectAll() {
    this.selectAllInternal((w) => false);
  }

  selectEven() {
    this.selectAllInternal((w) => w.weekNumber % 2 === 0);
  }

  selectOdd() {
    this.selectAllInternal((w) => w.weekNumber % 2 !== 0);
  }

  validateOneSelected() {
    if (this.weeks.findIndex((w) => !w.disabled && w.selected) === -1) {
      this.errors = ['Трябва да е избрана поне една седмица'];
      return false;
    }

    return true;
  }

  validateOneDeselected() {
    if (this.weeks.findIndex((w) => !w.disabled && !w.selected) === -1) {
      this.errors = ['В старото разписание трябва да остане поне една седмица'];
      return false;
    }

    return true;
  }

  save() {
    this.errors = [];

    if (!this.validateOneSelected() || !this.validateOneDeselected()) {
      return;
    }

    this.loading = true;

    const weeks = this.weeks.filter((w) => w.selected).map((w) => ({ year: w.year, weekNumber: w.weekNumber }));

    this.schedulesService
      .split({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        scheduleId: this.data.scheduleId,
        splitScheduleCommand: {
          weeks
        }
      })
      .toPromise()
      .then(() => this.dialogRef.close(true))
      .catch((err) => {
        if (err.status === 400 && err.error?.errorMessages?.length > 0) {
          this.errors = err.error.errorMessages;
        } else {
          const requestId = err.headers.get('X-Sb-Request-Id');

          this.errors = requestId
            ? [`Възникна грешка! Моля, опитайте отново. Номер на грешка: ${requestId}`]
            : ['Възникна грешка! Моля, опитайте отново.'];
          GlobalErrorHandler.instance.handleError(err, true);
        }
      })
      .finally(() => {
        this.loading = false;
      });
  }
}
