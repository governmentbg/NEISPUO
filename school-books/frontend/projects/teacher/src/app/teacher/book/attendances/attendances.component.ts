import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { faNotesMedical as fadNotesMedical } from '@fortawesome/pro-duotone-svg-icons/faNotesMedical';
import { faCalendarTimes as farCalendarTimes } from '@fortawesome/pro-regular-svg-icons/faCalendarTimes';
import { faCircle as farCircle } from '@fortawesome/pro-regular-svg-icons/faCircle';
import { faCalendarAlt as fasCalendarAlt } from '@fortawesome/pro-solid-svg-icons/faCalendarAlt';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faChevronLeft as fasChevronLeft } from '@fortawesome/pro-solid-svg-icons/faChevronLeft';
import { faChevronRight as fasChevronRight } from '@fortawesome/pro-solid-svg-icons/faChevronRight';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faSlash as fasSlash } from '@fortawesome/pro-solid-svg-icons/faSlash';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import {
  add,
  addDays,
  addMonths,
  format,
  getDaysInMonth,
  getISODay,
  isEqual,
  isSameDay,
  isWithinInterval,
  startOfDay
} from 'date-fns';
import { bg } from 'date-fns/locale';
import {
  AttendancesService,
  Attendances_Get,
  Attendances_GetAllForMonth
} from 'projects/sb-api-client/src/api/attendances.service';
import {
  ClassBooksService,
  ClassBooks_GetRemovedStudents,
  ClassBooks_GetStudents
} from 'projects/sb-api-client/src/api/classBooks.service';
import { SchedulesService, Schedules_GetOffDatesPg } from 'projects/sb-api-client/src/api/schedules.service';
import { AttendanceType } from 'projects/sb-api-client/src/model/attendanceType';
import { PaginatorItem } from 'projects/shared/components/paginator/paginator-item';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { LocalStorageService } from 'projects/shared/services/local-storage.service';
import { range } from 'projects/shared/utils/array';
import { assert } from 'projects/shared/utils/assert';
import { ClassBookInfoType, resolveWithRemovedStudents, UNDO_INTERVAL_IN_MINUTES } from 'projects/shared/utils/book';
import { formatNullableDate } from 'projects/shared/utils/date';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { expiredAt } from 'projects/shared/utils/rxjs';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Observable, Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';
import { AttendanceExcuseDialogSkeletonComponent } from '../attendance-excuse-dialog/attendance-excuse-dialog.component';
import { CLASS_BOOK_INFO } from '../book/book.component';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class AttendancesSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    classBooksService: ClassBooksService,
    attendancesService: AttendancesService,
    schedulesService: SchedulesService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const year = tryParseInt(route.snapshot.paramMap.get('year')) ?? throwParamError('year');
    const month = tryParseInt(route.snapshot.paramMap.get('month')) ?? throwParamError('month');

    this.resolve(
      AttendancesComponent,
      resolveWithRemovedStudents(
        classBooksService,
        (data) => data.students,
        (data) => data.attendances,
        {
          schoolYear,
          instId,
          classBookId,
          classBookInfo: from(classBookInfo),
          year,
          month,
          students: classBooksService.getStudents({
            schoolYear,
            instId,
            classBookId
          }),
          offDatesPg: schedulesService.getOffDatesPg({
            schoolYear,
            instId,
            classBookId
          }),
          attendances: attendancesService.getAllForMonth({
            schoolYear,
            instId,
            classBookId,
            year,
            month
          })
        }
      )
    );
  }
}

type Attendance = ArrayElementType<Attendances_GetAllForMonth>;

@Component({
  selector: 'sb-attendances',
  templateUrl: './attendances.component.html',
  styleUrls: ['./attendances.component.scss']
})
export class AttendancesComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    classBookInfo: ClassBookInfoType;
    year: number;
    month: number;
    offDatesPg: Schedules_GetOffDatesPg;
    students: ClassBooks_GetStudents;
    attendances: Attendances_GetAllForMonth;
    removedStudents: ClassBooks_GetRemovedStudents;
  };

  readonly farCalendarTimes = farCalendarTimes;
  readonly fasCalendarAlt = fasCalendarAlt;
  readonly fasCheck = fasCheck;
  readonly fasPlus = fasPlus;
  readonly fasSlash = fasSlash;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasTrashAlt = fasTrashAlt;
  readonly farCircle = farCircle;
  readonly fadNotesMedical = fadNotesMedical;

  readonly ATTENDANCE_TYPE_PRESENCE = AttendanceType.Presence;
  readonly ATTENDANCE_TYPE_UNEXCUSED_ABSENCE = AttendanceType.UnexcusedAbsence;
  readonly ATTENDANCE_TYPE_EXCUSED_ABSENCE = AttendanceType.ExcusedAbsence;

  readonly destroyed$ = new Subject<void>();

  monthPaginatorItems!: PaginatorItem[];
  templateData!: ReturnType<typeof getTemplateData>;
  showTransferredStudentsBanner = false;
  hasPastMonthLockMessage: string | null = null;

  selectedStudentPersonId?: number;
  selectedAttendanceId?: number;
  selectedAttendance?: Attendances_Get & {
    undoExpired$: Observable<boolean>;
    removing: boolean;
    excusing: boolean;
    canExcuse: boolean;
    canUndo: boolean;
    canRemove: boolean;
  };
  loadingAttendance = false;

  constructor(
    private attendancesService: AttendancesService,
    private actionService: ActionService,
    private dialog: MatDialog,
    private localStorageService: LocalStorageService
  ) {}

  ngOnInit() {
    const monthStart = new Date(this.data.year, this.data.month - 1, 1);

    const prevMonthStart = addMonths(monthStart, -1);
    const prevMonthYear = prevMonthStart.getFullYear();
    const prevMonthNumber = prevMonthStart.getMonth() + 1;

    const nextMonthStart = addMonths(monthStart, 1);
    const nextMonthYear = nextMonthStart.getFullYear();
    const nextMonthNumber = nextMonthStart.getMonth() + 1;

    this.monthPaginatorItems = [
      {
        icon: fasChevronLeft,
        routeCommands: ['../../', prevMonthYear, prevMonthNumber]
      },
      {
        text: format(monthStart, 'LLLL', { locale: bg })
      },
      {
        icon: fasChevronRight,
        routeCommands: ['../../', nextMonthYear, nextMonthNumber]
      }
    ];

    this.showTransferredStudentsBanner = this.data.students.find((s) => s.isTransferred) != null;
    this.localStorageService.bookTransferredHidden$
      .pipe(
        tap((bookTransferredHidden) => {
          this.templateData = getTemplateData(this.data, bookTransferredHidden);
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();

    if (
      this.data.classBookInfo.bookAllowsModifications &&
      this.data.classBookInfo.firstEditableMonthStartDate != null &&
      this.data.classBookInfo.firstEditableMonthStartDate > monthStart
    ) {
      this.hasPastMonthLockMessage = `Въвеждането/изтриването/уважаването на присъствия преди ${formatNullableDate(
        this.data.classBookInfo.firstEditableMonthStartDate
      )} е забранено`;
    }
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  reloadAttendances() {
    return this.attendancesService
      .getAllForMonth({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        year: this.data.year,
        month: this.data.month
      })
      .toPromise()
      .then((attendances) => {
        this.selectedStudentPersonId = undefined;
        this.selectedAttendanceId = undefined;
        this.selectedAttendance = undefined;

        this.data.attendances = attendances;
        this.templateData = getTemplateData(this.data, this.localStorageService.getBookTransferredHidden());
      })
      .catch((err) => GlobalErrorHandler.instance.handleError(err));
  }

  viewAttendance(studentPersonId: number, attendanceId: number) {
    this.selectedAttendance = undefined;

    if (this.selectedStudentPersonId === studentPersonId && this.selectedAttendanceId === attendanceId) {
      this.selectedStudentPersonId = undefined;
      this.selectedAttendanceId = undefined;

      return;
    }

    this.selectedStudentPersonId = studentPersonId;
    this.selectedAttendanceId = attendanceId;
    this.loadingAttendance = true;

    return this.attendancesService
      .get({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        attendanceId
      })
      .toPromise()
      .then((a) => {
        const bookAllowsAttendanceAbsenceTopicModifications =
          this.data.classBookInfo.checkBookAllowsAttendanceAbsenceTopicModifications(a.date);
        this.selectedAttendance = {
          ...a,
          undoExpired$: expiredAt(add(a.createDate, { minutes: UNDO_INTERVAL_IN_MINUTES })),
          excusing: false,
          removing: false,
          canUndo: bookAllowsAttendanceAbsenceTopicModifications && a.hasUndoAccess,
          canExcuse: bookAllowsAttendanceAbsenceTopicModifications && a.hasExcuseAccess,
          canRemove: bookAllowsAttendanceAbsenceTopicModifications && a.hasRemoveAccess
        };
      })
      .catch((err) => {
        this.selectedStudentPersonId = undefined;
        this.selectedAttendanceId = undefined;

        GlobalErrorHandler.instance.handleError(err);
      })
      .finally(() => {
        this.loadingAttendance = false;
      });
  }

  openExcuseAttendanceDialog() {
    assert(this.selectedAttendanceId != null);
    assert(this.selectedAttendance);

    this.selectedAttendance.excusing = true;

    openTypedDialog(this.dialog, AttendanceExcuseDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        attendanceId: this.selectedAttendanceId
      }
    })
      .afterClosed()
      .toPromise()
      .then((ok) => {
        if (ok) {
          return this.reloadAttendances();
        }

        return Promise.resolve();
      })
      .finally(() => {
        if (this.selectedAttendance) {
          this.selectedAttendance.excusing = false;
        }
      });
  }

  removeSelectedAttendance() {
    assert(this.selectedAttendanceId != null);
    assert(this.selectedAttendance);

    this.selectedAttendance.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      attendanceId: this.selectedAttendanceId
    };

    const attendaceTypeName =
      this.selectedAttendance?.type === AttendanceType.Presence
        ? 'присъствието'
        : this.selectedAttendance?.type === AttendanceType.UnexcusedAbsence
        ? 'отсъствието по неуважителни причини'
        : this.selectedAttendance?.type === AttendanceType.ExcusedAbsence
        ? 'отсъствието по уважителни причини'
        : '';

    this.actionService
      .execute({
        confirmMessage: `Сигурни ли сте, че искате да изтриете ${attendaceTypeName}?`,
        errorsMessage: `Не може да изтриете ${attendaceTypeName}, защото:`,
        httpAction: () => this.attendancesService.removeAttendance(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          return this.reloadAttendances();
        }

        return Promise.resolve();
      })
      .finally(() => {
        if (this.selectedAttendance) {
          this.selectedAttendance.removing = false;
        }
      });
  }
}

function getTemplateData(data: AttendancesComponent['data'], bookTransferredHidden: boolean) {
  const schoolYearStartDateLimit = data.classBookInfo.schoolYearStartDateLimit;
  const schoolYearStartDate = data.classBookInfo.schoolYearStartDate;
  const schoolYearEndDate = data.classBookInfo.schoolYearEndDate;
  const schoolYearEndDateLimit = data.classBookInfo.schoolYearEndDateLimit;

  const today = startOfDay(new Date());
  const isAfterToday = (date: Date) => date >= addDays(today, 1);

  const monthStart = new Date(data.year, data.month - 1, 1);

  const monthText = format(monthStart, 'LLLL', { locale: bg });

  let hasOffProgramUnexcusedAbsences = false;

  const days = range(1, getDaysInMonth(monthStart)).map((dayOfMonth) => {
    const date = new Date(data.year, data.month - 1, dayOfMonth);

    const isOutsideSchoolYear = !isWithinInterval(date, { start: schoolYearStartDate, end: schoolYearEndDate });
    const isOutsideSchoolYearLimits = !isWithinInterval(date, {
      start: schoolYearStartDateLimit,
      end: schoolYearEndDateLimit
    });
    const offDay = data.offDatesPg.find((od) => isEqual(date, od.date));

    const isOffDay = getISODay(date) > 5 || (offDay && !offDay.isPgOffProgramDay);
    const isInactiveDay = isOffDay || isOutsideSchoolYearLimits || isAfterToday(date);
    const isOffProgramDay =
      !isOffDay && !isOutsideSchoolYearLimits && ((offDay && offDay.isPgOffProgramDay) || isOutsideSchoolYear);

    const bookAllowsAttendanceAbsenceTopicModifications =
      data.classBookInfo.checkBookAllowsAttendanceAbsenceTopicModifications(date);
    return {
      dateParam: format(date, 'yyyy-MM-dd', {}),
      canCreate:
        !isOffDay &&
        !isOutsideSchoolYearLimits &&
        bookAllowsAttendanceAbsenceTopicModifications &&
        (data.classBookInfo.hasCreateAttendanceAccess ||
          data.classBookInfo.createAttendanceReplTeacherAccessDates?.some((replDate) => isSameDay(replDate, date))),
      canRemove:
        !isOffDay &&
        !isOutsideSchoolYearLimits &&
        bookAllowsAttendanceAbsenceTopicModifications &&
        data.classBookInfo.hasRemoveAttendanceAccess,
      dayOfMonth,
      isOffProgramDay,
      isInactiveDay,
      hasOffProgramUnexcusedAbsences: false
    };
  });

  const studentAttendancesMap = new Map(
    data.students
      .map((s) => ({
        ...s,
        isRemoved: false
      }))
      .concat(
        data.removedStudents.map((s) => ({
          ...s,
          isRemoved: true,
          isTransferred: false,
          hasSpecialNeedFirstGradeResult: false
        }))
      )
      .map((s) => [
        s.personId,
        {
          ...s,

          showInfoLink: !s.isTransferred && !s.isRemoved,
          abnormalStatus: s.isRemoved ? 'ИЗТРИТ' : s.isTransferred ? 'ОТПИСАН' : '',

          attendances: Array.from(days, (): Attendance | null => null),
          attendancesTotal: 0,
          presencesTotal: 0
        }
      ])
  );

  const studentAttendancesTotals = {
    presencesDayTotals: Array.from(days, () => 0),
    attendancesDayTotals: Array.from(days, () => 0),
    presencesCountTotal: 0,
    averagePresencesCount: <number | null>null,
    studentsEnrolledCount: data.students.filter((s) => !s.isTransferred).length,
    studentsEnrolledDayTotals: Array.from(days, () => 0)
  };

  for (const attendance of data.attendances) {
    const dayOfMonth = attendance.date.getDate();

    const student = studentAttendancesMap.get(attendance.personId);
    if (student) {
      student.attendances[dayOfMonth - 1] = attendance;
      student.attendancesTotal++;

      if (attendance.type === AttendanceType.Presence) {
        student.presencesTotal++;
        studentAttendancesTotals.presencesDayTotals[dayOfMonth - 1]++;
        studentAttendancesTotals.presencesCountTotal++;
      }

      if (attendance.type === AttendanceType.UnexcusedAbsence && days[dayOfMonth - 1].isOffProgramDay) {
        days[dayOfMonth - 1].hasOffProgramUnexcusedAbsences = true;
        hasOffProgramUnexcusedAbsences = true;
      }

      studentAttendancesTotals.attendancesDayTotals[dayOfMonth - 1]++;

      if (!student.isTransferred) {
        studentAttendancesTotals.studentsEnrolledDayTotals[dayOfMonth - 1]++;
      }
    }
  }

  studentAttendancesTotals.averagePresencesCount =
    studentAttendancesTotals.presencesCountTotal / days.filter((d) => !d.isInactiveDay).length;

  const bookAllowsAttendanceAbsenceTopicModifications =
    data.classBookInfo.checkBookAllowsAttendanceAbsenceTopicModifications(new Date());
  return {
    todayParam: format(today, 'yyyy-MM-dd', {}),
    canCreateToday:
      bookAllowsAttendanceAbsenceTopicModifications &&
      (data.classBookInfo.hasCreateAttendanceAccess ||
        data.classBookInfo.createAttendanceReplTeacherAccessDates?.some((replDate) => isSameDay(replDate, today))),
    canRemoveToday: bookAllowsAttendanceAbsenceTopicModifications && data.classBookInfo.hasRemoveAttendanceAccess,
    monthText,
    days,
    studentAttendances: [...studentAttendancesMap.values()].filter(
      (s) => !bookTransferredHidden || !s.isTransferred || s.attendancesTotal
    ),
    studentAttendancesTotals,
    hasOffProgramUnexcusedAbsences
  };
}
