import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { faAlignJustify as fasAlignJustify } from '@fortawesome/pro-solid-svg-icons/faAlignJustify';
import { faChevronLeft as fasChevronLeft } from '@fortawesome/pro-solid-svg-icons/faChevronLeft';
import { faChevronRight as fasChevronRight } from '@fortawesome/pro-solid-svg-icons/faChevronRight';
import { add, addWeeks, format, getISOWeek, isAfter, isSameDay, setISODay, setISOWeek } from 'date-fns';
import getISOWeekYear from 'date-fns/getISOWeekYear';
import { bg } from 'date-fns/locale';
import {
  AbsencesService,
  Absences_CreateAbsenceRequestParams,
  Absences_ExcuseAbsencesRequestParams,
  Absences_GetAllForWeek,
  Absences_RemoveAbsencesRequestParams
} from 'projects/sb-api-client/src/api/absences.service';
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
import { ClassBooksGetStudentsVO } from 'projects/sb-api-client/src/model/classBooksGetStudentsVO';
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
import { ClassBookInfoType, UNDO_INTERVAL_IN_MINUTES } from 'projects/shared/utils/book';
import { formatNullableDate } from 'projects/shared/utils/date';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { extendScheduleHour } from 'projects/shared/utils/schedule';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Observable, of, Subject } from 'rxjs';
import { catchError, map, switchMap, takeUntil, tap } from 'rxjs/operators';
import {
  AbsencesExcuseDialogComponent,
  AbsencesExcuseDialogResult
} from '../absences-excuse-dialog/absences-excuse-dialog.component';
import { CLASS_BOOK_INFO } from '../book/book.component';
import {
  AbsencesEditDialogResult,
  AbsencesEditDialogSkeletonComponent
} from './absences-edit-dialog/absences-edit-dialog.component';

export enum AbsencesEditMode {
  New = 'New',
  Excuse = 'Excuse',
  Remove = 'Remove'
}

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class AbsencesEditSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    classBooksService: ClassBooksService,
    absencesService: AbsencesService,
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
    const mode = route.snapshot.data.mode as AbsencesEditMode;

    this.resolve(AbsencesEditComponent, {
      schoolYear,
      instId,
      classBookId,
      year,
      weekNumber,
      mode,
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
      absences: absencesService.getAllForWeek({
        schoolYear,
        instId,
        classBookId,
        year,
        weekNumber
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
  selector: 'sb-absences-edit',
  templateUrl: './absences-edit.component.html'
})
export class AbsencesEditComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    year: number;
    weekNumber: number;
    mode: AbsencesEditMode;
    classBookInfo: ClassBookInfoType;
    students: ClassBooks_GetStudents;
    individualCurriculumStudents: ClassBookStudentNoms_GetNomsByTerm;
    absences: Absences_GetAllForWeek;
    schedule: Schedules_GetClassBookScheduleForWeek;
  };

  readonly ABSENCES_EDIT_MODE_NEW = AbsencesEditMode.New;

  readonly destroyed$ = new Subject<void>();

  readonly form = this.fb.group({});
  readonly fasAlignJustify = fasAlignJustify;

  weekPaginatorItems!: PaginatorItem[];
  individualCurriculumStudentControl!: UntypedFormControl;
  selectedStudentId!: number | null;
  scheduleByDay!: ReturnType<typeof getScheduleByDay>;
  hasPastMonthLockMessage: string | null = null;
  modeTitle!: string;

  addedAbsences: Absences_CreateAbsenceRequestParams['createAbsenceCommand']['absences'] = [];
  excusedAbsences: Absences_ExcuseAbsencesRequestParams['excuseAbsencesCommand']['absences'] = [];
  removedAbsences: Absences_RemoveAbsencesRequestParams['removeAbsencesCommand']['absences'] = [];

  constructor(
    private fb: UntypedFormBuilder,
    private absencesService: AbsencesService,
    private schedulesService: SchedulesService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private eventService: EventService,
    private dialog: MatDialog
  ) {}

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
      this.data.mode === AbsencesEditMode.New
        ? 'Въвеждане на отсъствия'
        : this.data.mode === AbsencesEditMode.Excuse
        ? 'Уважаване на отсъствия'
        : this.data.mode === AbsencesEditMode.Remove
        ? 'Премахване на отсъствия'
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

  absenceAdded(hour: ScheduleHourWithAbsences, isLate: boolean, input: string, excused = false): void {
    if (this.data.mode !== AbsencesEditMode.New) {
      return;
    }

    let student: ClassBooksGetStudentsVO | null = null;
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

    if (hour.lateChips.findIndex((a) => a.classNumber != null && a.classNumber.toString() === input) > -1) {
      this.showError('Ученикът вече има въведено закъснение.');
      return;
    }

    const existingAbsence = hour.absenceChips.find((a) => a.classNumber != null && a.classNumber.toString() === input);
    if (existingAbsence) {
      if (isLate && existingAbsence.hasUndoAccess) {
        if (existingAbsence.absenceId) {
          if (existingAbsence.excused) {
            this.showError(
              'Ученикът вече има въведено извинено отсъствие, което не може да бъде конвертирано в закъснение. Ако все пак искате да въведете закъснение изтрийте отсъствието.'
            );
            return;
          } else if (
            existingAbsence.createDate != null &&
            isAfter(new Date(), add(existingAbsence.createDate, { minutes: UNDO_INTERVAL_IN_MINUTES }))
          ) {
            this.showError(
              'Ученикът вече има въведено отсъствие, което не може да бъде конвертирано в закъснение, защото това е възможно само в рамките на един час от въвеждането му.'
            );
            return;
          } else {
            existingAbsence.removed = true;
          }
        } else {
          this.addedAbsences.splice(
            this.addedAbsences.findIndex(
              (a) => a.scheduleLessonId === hour.scheduleLessonId && a.personId === student!.personId
            ),
            1
          );
          hour.absenceChips.splice(hour.absenceChips.indexOf(existingAbsence), 1);
        }
      } else {
        this.showError('Ученикът вече има въведено отсъствие');
        return;
      }
    }

    (isLate ? hour.lateChips : hour.absenceChips).push({
      scheduleLessonId: hour.scheduleLessonId,
      classNumber: student.classNumber,
      excused: excused
    });

    this.addedAbsences.push({
      personId: student.personId,
      type: isLate ? AbsenceType.Late : excused ? AbsenceType.Excused : AbsenceType.Unexcused,
      date: hour.date ?? throwError('date must not be empty'),
      scheduleLessonId: hour.scheduleLessonId ?? throwError('scheduleLessonId must not be empty'),
      teacherAbsenceId: hour.teacherAbsenceId,
      convertToLateId: existingAbsence ? existingAbsence.absenceId : null
    });
  }

  removeLastAbsence(hour: ScheduleHourWithAbsences, isLate: boolean): void {
    const absenceChips = isLate ? hour.lateChips : hour.absenceChips;
    const lastAbsenceChip = absenceChips.length > 0 ? absenceChips[absenceChips.length - 1] : null;

    if (this.data.mode !== AbsencesEditMode.New || lastAbsenceChip == null || lastAbsenceChip.readonly) {
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

  absenceSelected(hour: ScheduleHourWithAbsences, isLate: boolean, absenceChip: AbsenceChip): void {
    switch (this.data.mode) {
      case AbsencesEditMode.New: {
        if (absenceChip.readonly || isLate) {
          return;
        }

        absenceChip.excused = !absenceChip.excused;

        const addedAbsence =
          this.addedAbsences.find(
            (a) =>
              a.scheduleLessonId === hour.scheduleLessonId &&
              a.personId ===
                this.data.students.find((s) => s.classNumber === absenceChip.classNumber && !s.isTransferred)?.personId
          ) ?? throwError('added absence must exist');

        addedAbsence.type = absenceChip.excused ? AbsenceType.Excused : AbsenceType.Unexcused;

        break;
      }

      case AbsencesEditMode.Excuse: {
        if (absenceChip.readonly || isLate) {
          return;
        }

        if (!absenceChip.excused) {
          this.excusedAbsences.push({
            scheduleLessonId: absenceChip.scheduleLessonId ?? throwError('excused absences must have scheduleLessonId'),
            absenceId: absenceChip.absenceId ?? throwError('excused absences must have absenceId')
          });
        } else {
          this.excusedAbsences.splice(
            this.excusedAbsences.findIndex(({ absenceId }) => absenceId === absenceChip.absenceId),
            1
          );
        }

        absenceChip.excused = !absenceChip.excused;

        break;
      }

      case AbsencesEditMode.Remove: {
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
    return (
      this.form.dirty ||
      this.addedAbsences.length > 0 ||
      this.excusedAbsences.length > 0 ||
      this.removedAbsences.length > 0
    );
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
        lateChips: hour.lateChips,
        absences: this.data.absences.filter((a) => a.scheduleLessonId === hour.scheduleLessonId),
        isDplrAbsenceMode: null
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
      this.absenceAdded(hour, false, absenceChip.classNumber?.toString() ?? '', absenceChip.excused);
    }
    for (const absenceChip of result.lateChips) {
      this.absenceAdded(hour, true, absenceChip.classNumber?.toString() ?? '');
    }
    for (const absenceChip of result.chipsForSelection) {
      this.absenceSelected(hour, false, absenceChip);
    }

    for (const absnece of result.absencesForRemoval) {
      this.removeAbsenceFromDialogResult(hour, absnece.personId, absnece.classNumber, absnece.isLate);
    }
  }

  removeAbsenceFromDialogResult(
    hour: ScheduleHourWithAbsences,
    personId: number,
    classNumber: number,
    isLate: boolean
  ): void {
    const absenceChips = isLate ? hour.lateChips : hour.absenceChips;

    if (this.data.mode !== AbsencesEditMode.New) {
      return;
    }

    // remove chip
    absenceChips.splice(
      absenceChips.findIndex((a) => a.scheduleLessonId === hour.scheduleLessonId && a.classNumber === classNumber),
      1
    );

    // remove from addedAbsences
    this.addedAbsences.splice(
      this.addedAbsences.findIndex((a) => a.scheduleLessonId === hour.scheduleLessonId && a.personId === personId),
      1
    );
  }

  async onSave(save: SaveToken) {
    let result: Promise<void>;

    switch (this.data.mode) {
      case AbsencesEditMode.New: {
        let excusedDialogResult: AbsencesExcuseDialogResult | undefined = undefined;

        if (this.addedAbsences.find((a) => a.type === AbsenceType.Excused)) {
          excusedDialogResult = await openTypedDialog(this.dialog, AbsencesExcuseDialogComponent, {
            data: {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId
            }
          })
            .afterClosed()
            .toPromise();

          if (!excusedDialogResult) {
            save.done(false);
            return;
          }
        }

        result = this.absencesService
          .createAbsence({
            schoolYear: this.data.schoolYear,
            instId: this.data.instId,
            classBookId: this.data.classBookId,
            createAbsenceCommand: {
              excusedReasonId: excusedDialogResult?.excusedReasonId,
              excusedReasonComment: excusedDialogResult?.excusedReasonComment,
              absences: this.addedAbsences
            }
          })
          .toPromise();
        break;
      }

      case AbsencesEditMode.Excuse: {
        const r = await openTypedDialog(this.dialog, AbsencesExcuseDialogComponent, {
          data: {
            schoolYear: this.data.schoolYear,
            instId: this.data.instId
          }
        })
          .afterClosed()
          .toPromise();

        if (!r) {
          save.done(false);
          return;
        }

        result = this.absencesService
          .excuseAbsences({
            schoolYear: this.data.schoolYear,
            instId: this.data.instId,
            classBookId: this.data.classBookId,
            excuseAbsencesCommand: {
              excusedReasonId: r.excusedReasonId,
              excusedReasonComment: r.excusedReasonComment,
              absences: this.excusedAbsences
            }
          })
          .toPromise();
        break;
      }

      case AbsencesEditMode.Remove: {
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
            this.form.markAsPristine();
            this.addedAbsences = [];
            this.excusedAbsences = [];
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
  data: AbsencesEditComponent['data'],
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
            (data.mode === AbsencesEditMode.New
              ? h.hasAbsenceCreateAccess
              : data.mode === AbsencesEditMode.Excuse
              ? h.hasAbsenceExcuseAccess
              : data.mode === AbsencesEditMode.Remove
              ? h.hasAbsenceRemoveAccess
              : false)
        }))
        .map((h) => ({
          ...h,
          absenceChips: dateAbsences
            .filter(
              (a) =>
                a.scheduleLessonId === h.scheduleLessonId &&
                (a.type === AbsenceType.Unexcused || a.type === AbsenceType.Excused)
            )
            .map(
              (a): AbsenceChip => ({
                scheduleLessonId: a.scheduleLessonId,
                absenceId: a.absenceId,
                classNumber: studentClassNumbersMap.get(a.personId),
                readonly:
                  data.mode === AbsencesEditMode.New ||
                  (data.mode === AbsencesEditMode.Excuse && a.type === AbsenceType.Excused) ||
                  !h.canEdit,
                excused: a.type === AbsenceType.Excused,
                createDate: a.createDate,
                hasUndoAccess: a.hasUndoAccess
              })
            )
            .sort((a1, a2) => (a1.classNumber ?? 99999) - (a2.classNumber ?? 99999)),
          lateChips: dateAbsences
            .filter((a) => a.scheduleLessonId === h.scheduleLessonId && a.type === AbsenceType.Late)
            .map(
              (a): AbsenceChip => ({
                scheduleLessonId: a.scheduleLessonId,
                absenceId: a.absenceId,
                classNumber: studentClassNumbersMap.get(a.personId),
                readonly: data.mode === AbsencesEditMode.New || data.mode === AbsencesEditMode.Excuse || !h.canEdit,
                excused: false,
                createDate: a.createDate,
                hasUndoAccess: a.hasUndoAccess
              })
            )
            .sort((a1, a2) => (a1.classNumber ?? 99999) - (a2.classNumber ?? 99999))
        }))
    };

    return result;
  });
}
