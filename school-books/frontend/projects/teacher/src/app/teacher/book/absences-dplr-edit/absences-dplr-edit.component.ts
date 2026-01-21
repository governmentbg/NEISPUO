import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { faAlignJustify as fasAlignJustify } from '@fortawesome/pro-solid-svg-icons/faAlignJustify';
import { faChevronLeft as fasChevronLeft } from '@fortawesome/pro-solid-svg-icons/faChevronLeft';
import { faChevronRight as fasChevronRight } from '@fortawesome/pro-solid-svg-icons/faChevronRight';
import { addWeeks, format, getISOWeek, isSameDay, setISODay, setISOWeek } from 'date-fns';
import getISOWeekYear from 'date-fns/getISOWeekYear';
import { bg } from 'date-fns/locale';
import {
  AbsencesService,
  Absences_CreateAbsenceRequestParams,
  Absences_RemoveAbsencesRequestParams
} from 'projects/sb-api-client/src/api/absences.service';
import { AbsencesDplrService, AbsencesDplr_GetAllForWeek } from 'projects/sb-api-client/src/api/absencesDplr.service';
import { ClassBooksService, ClassBooks_GetStudents } from 'projects/sb-api-client/src/api/classBooks.service';
import {
  ClassBookStudentNomsService,
  ClassBookStudentNoms_GetNomsByTerm
} from 'projects/sb-api-client/src/api/classBookStudentNoms.service';
import {
  SchedulesService,
  Schedules_GetClassBookScheduleForWeek
} from 'projects/sb-api-client/src/api/schedules.service';
import { AbsenceType } from 'projects/sb-api-client/src/model/absenceType';
import { AbsenceChip } from 'projects/shared/components/absence-chips/absence-chips.component';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { PaginatorItem } from 'projects/shared/components/paginator/paginator-item';
import {
  IShouldPreventLeave,
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { EventService, EventType } from 'projects/shared/services/event.service';
import { max, min, range, stableSort } from 'projects/shared/utils/array';
import { ClassBookInfoType } from 'projects/shared/utils/book';
import { formatNullableDate } from 'projects/shared/utils/date';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { extendScheduleHour } from 'projects/shared/utils/schedule';
import { ArrayElementType } from 'projects/shared/utils/type';
import { enumFromStringValue, throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Observable, of, Subject } from 'rxjs';
import { catchError, map, switchMap, takeUntil, tap } from 'rxjs/operators';
import { AbsencesDplrType } from '../absences-dplr/absences-dplr.component';
import {
  AbsencesEditDialogResult,
  AbsencesEditDialogSkeletonComponent
} from '../absences-edit/absences-edit-dialog/absences-edit-dialog.component';
import { CLASS_BOOK_INFO } from '../book/book.component';

export enum AbsencesDplrEditMode {
  New = 'New',
  Remove = 'Remove'
}

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class AbsencesDplrEditSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    classBooksService: ClassBooksService,
    absencesDplrService: AbsencesDplrService,
    schedulesService: SchedulesService,
    classBookStudentNomsService: ClassBookStudentNomsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const year = tryParseInt(route.snapshot.paramMap.get('year')) ?? throwParamError('year');
    const weekNumber = tryParseInt(route.snapshot.paramMap.get('weekNumber')) ?? throwParamError('weekNumber');
    const type = enumFromStringValue(AbsencesDplrType, route.snapshot.paramMap.get('type')) ?? throwParamError('type');
    const absenceType = type === AbsencesDplrType.Absence ? AbsenceType.DplrAbsence : AbsenceType.DplrAttendance;
    const mode = route.snapshot.data.mode as AbsencesDplrEditMode;

    this.resolve(AbsencesDplrEditComponent, {
      schoolYear,
      instId,
      classBookId,
      year,
      weekNumber,
      mode,
      absenceType,
      classBookInfo: from(classBookInfo),
      students: classBooksService.getStudents({
        schoolYear,
        instId,
        classBookId
      }),
      individualCurriculumStudents: classBookStudentNomsService.getNomsByTerm({
        instId,
        schoolYear,
        classBookId,
        showOnlyWithIndividualCurriculumSchedule: true
      }),
      absences: absencesDplrService.getAllForWeek({
        schoolYear,
        instId,
        classBookId,
        year,
        weekNumber,
        type: absenceType
      }),
      schedule: schedulesService.getClassBookScheduleForWeek({
        schoolYear,
        instId,
        classBookId,
        year,
        weekNumber
      })
    });
  }
}

type ScheduleHour = ArrayElementType<Schedules_GetClassBookScheduleForWeek['hours']>;
// schedule hour with only hourNumber required
type PartialScheduleHour = Partial<ScheduleHour> & { hourNumber: number };
type ScheduleHourWithAbsences = ArrayElementType<ArrayElementType<ReturnType<typeof getScheduleByDay>>['hours']>;

@Component({
  selector: 'sb-absences-dplr-edit',
  templateUrl: './absences-dplr-edit.component.html'
})
export class AbsencesDplrEditComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    year: number;
    weekNumber: number;
    mode: AbsencesDplrEditMode;
    absenceType: AbsenceType;
    classBookInfo: ClassBookInfoType;
    students: ClassBooks_GetStudents;
    individualCurriculumStudents: ClassBookStudentNoms_GetNomsByTerm;
    absences: AbsencesDplr_GetAllForWeek;
    schedule: Schedules_GetClassBookScheduleForWeek;
  };

  readonly ABSENCES_EDIT_MODE_NEW = AbsencesDplrEditMode.New;

  readonly destroyed$ = new Subject<void>();

  readonly form = this.fb.group({});

  readonly fasAlignJustify = fasAlignJustify;

  isAbsence = false;

  weekPaginatorItems!: PaginatorItem[];
  individualCurriculumStudentControl!: UntypedFormControl;
  selectedStudentId!: number | null;
  scheduleByDay!: ReturnType<typeof getScheduleByDay>;
  hasPastMonthLockMessage: string | null = null;
  modeTitle!: string;

  addedAbsences: Absences_CreateAbsenceRequestParams['createAbsenceCommand']['absences'] = [];
  removedAbsences: Absences_RemoveAbsencesRequestParams['removeAbsencesCommand']['absences'] = [];

  constructor(
    private fb: UntypedFormBuilder,
    private absencesService: AbsencesService,
    private absencesDplrService: AbsencesDplrService,
    private schedulesService: SchedulesService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private eventService: EventService,
    private dialog: MatDialog
  ) {}

  ngOnInit() {
    this.isAbsence = this.data.absenceType === AbsenceType.DplrAbsence;

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
              .getClassBookScheduleForWeek({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                classBookId: this.data.classBookId,
                year: this.data.year,
                weekNumber: this.data.weekNumber,
                personId: studentId
              })
              .pipe(
                map((schedule) => {
                  return [this.selectedStudentId, schedule] as const;
                }),
                catchError((err) => {
                  GlobalErrorHandler.instance.handleError(err);
                  return of([null, null] as const);
                })
              );
          } else {
            return of([null, null] as const);
          }
        }),
        tap(([studentId, schedule]) => {
          this.selectedStudentId = studentId;
          this.scheduleByDay = getScheduleByDay(this.data, studentId, schedule);
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();

    this.scheduleByDay = getScheduleByDay(this.data, null, null);

    this.modeTitle =
      this.data.mode === AbsencesDplrEditMode.New
        ? `Въвеждане ${this.isAbsence ? 'на отсъствия' : 'на присъствия'}`
        : this.data.mode === AbsencesDplrEditMode.Remove
        ? `Премахване ${this.isAbsence ? 'на отсъствия' : 'на присъствия'}`
        : '';

    if (
      this.data.classBookInfo.bookAllowsModifications &&
      this.data.classBookInfo.firstEditableMonthStartDate != null &&
      this.data.classBookInfo.firstEditableMonthStartDate > weekStart
    ) {
      this.hasPastMonthLockMessage = `${this.modeTitle} преди ${formatNullableDate(
        this.data.classBookInfo.firstEditableMonthStartDate
      )} е забранено`;
    }
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  absenceAdded(hour: ScheduleHourWithAbsences, input: string): void {
    if (this.data.mode !== AbsencesDplrEditMode.New) {
      return;
    }

    let student = null;
    const students = this.data.students.filter((s) => s.classNumber != null && s.classNumber.toString() === input);

    if (students.length === 0) {
      this.showError('Няма ученик с такъв номер.');
      return;
    } else if (this.selectedStudentId != null && students.some((s) => this.selectedStudentId !== s.personId)) {
      this.showError('Когато е избрано ИУП разписание могат да се въвеждат отсъствия само за конкретния ученик.');
      return;
    } else if (students.length === 1) {
      // just one active, continue
      student = students[0];

      if (student.isTransferred) {
        this.showWarning(
          `Ученик #${student.classNumber} ${student.firstName} ${student.lastName} е отписан от този клас.`
        );
      }
    } else {
      const activeStudents = students.filter((s) => !s.isTransferred);

      if (activeStudents.length === 1) {
        // just one active, continue
        student = activeStudents[0];
      } else {
        this.showError('Има повече от един ученик с този номер.');
        return;
      }
    }

    if (hour.absenceChips.find((a) => a.classNumber != null && a.classNumber.toString() === input)) {
      this.showError(`Ученикът вече има въведено ${this.isAbsence ? 'отсъствие' : 'присъствие'}.`);
      return;
    }

    hour.absenceChips.push({
      scheduleLessonId: hour.scheduleLessonId,
      classNumber: student.classNumber
    });

    this.addedAbsences.push({
      personId: student.personId,
      type: this.data.absenceType,
      date: hour.date ?? throwError('date must not be empty'),
      scheduleLessonId: hour.scheduleLessonId ?? throwError('scheduleLessonId must not be empty'),
      teacherAbsenceId: hour.teacherAbsenceId
    });
  }

  removeLastAbsence(hour: ScheduleHourWithAbsences): void {
    const absenceChips = hour.absenceChips;
    const lastAbsenceChip = absenceChips.length > 0 ? absenceChips[absenceChips.length - 1] : null;

    if (this.data.mode !== AbsencesDplrEditMode.New || lastAbsenceChip == null || lastAbsenceChip.readonly) {
      return;
    }

    // remove last chip
    absenceChips.splice(absenceChips.length - 1, 1);

    // remove from addedAbsences
    this.addedAbsences.splice(
      this.addedAbsences.findIndex(
        (a) =>
          a.scheduleLessonId === hour.scheduleLessonId &&
          a.personId ===
            this.data.students.find((s) => s.classNumber === lastAbsenceChip.classNumber && !s.isTransferred)?.personId
      ),
      1
    );
  }

  absenceSelected(hour: ScheduleHourWithAbsences, absenceChip: AbsenceChip): void {
    switch (this.data.mode) {
      case AbsencesDplrEditMode.New:
        return;

      case AbsencesDplrEditMode.Remove: {
        if (absenceChip.readonly) {
          return;
        }

        if (!absenceChip.removed) {
          this.removedAbsences.push({
            scheduleLessonId: absenceChip.scheduleLessonId ?? throwError('removed absences must have scheduleLessonId'),
            absenceId: absenceChip.absenceId ?? throwError('removed absences must have absenceId')
          });
        } else {
          this.removedAbsences.splice(
            this.removedAbsences.findIndex(({ absenceId }) => absenceId === absenceChip.absenceId),
            1
          );
        }

        absenceChip.removed = !absenceChip.removed;

        break;
      }
    }
  }

  shouldPreventLeave() {
    return this.addedAbsences.length > 0 || this.removedAbsences.length > 0;
  }

  openAbsencesEditDialog(hour: ScheduleHourWithAbsences) {
    openTypedDialog(this.dialog, AbsencesEditDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        date: hour.date!,
        hourNumber: hour.hourNumber,
        scheduleLessonId: hour.scheduleLessonId!,
        classBookInfo: this.data.classBookInfo,
        students: this.data.students,
        individualCurriculumStudents: this.data.individualCurriculumStudents,
        absenceChips: hour.absenceChips,
        lateChips: [],
        absences: this.data.absences
          .filter((a) => a.scheduleLessonId === hour.scheduleLessonId)
          .map((a) => ({
            absenceId: a.absenceId,
            personId: a.personId,
            type: this.data.absenceType,
            createDate: a.createDate,
            hasUndoAccess: a.hasUndoAccess
          })),
        isDplrAbsenceMode: this.isAbsence
      }
    })
      .afterClosed()
      .toPromise()
      .then((r) => {
        if (r) {
          return this.reloadAbsences(hour, r);
        }

        return Promise.resolve();
      });
  }

  reloadAbsences(hour: ScheduleHourWithAbsences, result: AbsencesEditDialogResult): void {
    for (const absenceChip of result.absenceChips) {
      this.absenceAdded(hour, absenceChip.classNumber?.toString() ?? '');
    }

    for (const absnece of result.absencesForRemoval) {
      this.removeAbsenceFromDialogResult(hour, absnece.personId, absnece.isLate);
    }
  }

  removeAbsenceFromDialogResult(hour: ScheduleHourWithAbsences, personId: number, isLate: boolean): void {
    const lastAbsenceChip = hour.absenceChips.length > 0 ? hour.absenceChips[hour.absenceChips.length - 1] : null;

    if (this.data.mode !== AbsencesDplrEditMode.New || lastAbsenceChip == null || lastAbsenceChip.readonly) {
      return;
    }

    // remove last chip
    hour.absenceChips.splice(hour.absenceChips.length - 1, 1);

    // remove from addedAbsences
    this.addedAbsences.splice(
      this.addedAbsences.findIndex((a) => a.scheduleLessonId === hour.scheduleLessonId && a.personId === personId),
      1
    );
  }

  onSave(save: SaveToken) {
    let result: Promise<void>;

    switch (this.data.mode) {
      case AbsencesDplrEditMode.New: {
        result = this.absencesService
          .createAbsence({
            schoolYear: this.data.schoolYear,
            instId: this.data.instId,
            classBookId: this.data.classBookId,
            createAbsenceCommand: {
              absences: this.addedAbsences
            }
          })
          .toPromise();
        break;
      }

      case AbsencesDplrEditMode.Remove: {
        result = this.absencesService
          .removeAbsences({
            schoolYear: this.data.schoolYear,
            instId: this.data.instId,
            classBookId: this.data.classBookId,
            removeAbsencesCommand: {
              absences: this.removedAbsences
            }
          })
          .toPromise();
        break;
      }
    }

    this.actionService
      .execute({
        httpAction: () =>
          result.then(() => {
            this.addedAbsences = [];
            this.removedAbsences = [];
            this.router.navigate(['../../../'], { relativeTo: this.route });
          })
      })
      .then((success) => save.done(success));
  }

  showError(message: string) {
    this.eventService.dispatch({ type: EventType.SnackbarError, args: { message } });
  }

  showWarning(message: string) {
    this.eventService.dispatch({ type: EventType.SnackbarWarning, args: { message } });
  }
}

function getScheduleByDay(
  data: AbsencesDplrEditComponent['data'],
  studentId: number | null,
  studentSchedule: Schedules_GetClassBookScheduleForWeek | null
) {
  const studentClassNumbersMap = new Map(data.students.map((s) => [s.personId, s.classNumber]));
  const schedule = studentSchedule ?? data.schedule;
  const scheduleIncludesWeekend = schedule.hours.find((hour) => hour.day > 5) != null;

  return range(1, scheduleIncludesWeekend ? 7 : 5).map((day) => {
    const date = setISODay(setISOWeek(new Date(data.year, 7, 7), data.weekNumber), day);
    const isOffDay = schedule.offDays.includes(day);
    const dateAbsences = data.absences.filter(
      (a) => isSameDay(a.date, date) && (studentId == null || a.personId === studentId)
    );
    let hours: PartialScheduleHour[] = !isOffDay ? schedule.hours.filter((h) => h.day === day) : [];

    // add missing hour numbers
    if (hours.length) {
      const minHourNumber = min(hours, (h) => h.hourNumber) as number;
      const maxHourNumber = max(hours, (h) => h.hourNumber) as number;

      for (const hourNumber of range(minHourNumber, maxHourNumber)) {
        if (!hours.find((rh) => rh.hourNumber === hourNumber)) {
          hours.push({
            hourNumber
          });
        }
      }

      hours = stableSort(hours, (h1, h2) => h1.hourNumber - h2.hourNumber);
    }

    const result = {
      date,
      day,
      dateString: format(date, 'dd.MM.yyyy', { locale: bg }),
      dayString: format(date, 'EEEE', { locale: bg }),
      isOffDay,
      hours: hours
        .map((h) => ({
          ...extendScheduleHour(h),
          canEdit:
            h.date != null &&
            data.classBookInfo.checkBookAllowsAttendanceAbsenceTopicModifications(h.date) &&
            (data.mode === AbsencesDplrEditMode.New
              ? h.hasAbsenceCreateAccess
              : data.mode === AbsencesDplrEditMode.Remove
              ? h.hasAbsenceRemoveAccess
              : false)
        }))
        .map((h) => ({
          ...h,
          absenceChips: dateAbsences
            .filter((a) => a.scheduleLessonId === h.scheduleLessonId)
            .map(
              (a): AbsenceChip => ({
                scheduleLessonId: a.scheduleLessonId,
                absenceId: a.absenceId,
                classNumber: studentClassNumbersMap.get(a.personId),
                readonly: data.mode === AbsencesDplrEditMode.New || !h.canEdit
              })
            )
            .sort((a1, a2) => (a1.classNumber ?? 99999) - (a2.classNumber ?? 99999))
        }))
    };

    return result;
  });
}
