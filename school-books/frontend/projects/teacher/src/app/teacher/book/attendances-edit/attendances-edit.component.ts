import { Component, Inject, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { faChevronLeft as fasChevronLeft } from '@fortawesome/pro-solid-svg-icons/faChevronLeft';
import { faChevronRight as fasChevronRight } from '@fortawesome/pro-solid-svg-icons/faChevronRight';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { faUndo as fasUndo } from '@fortawesome/pro-solid-svg-icons/faUndo';
import { addDays, format, getISODay, isEqual, isSameDay, isValid, isWithinInterval, parse } from 'date-fns';
import { bg } from 'date-fns/locale';
import { AttendancesService, Attendances_GetAllForDate } from 'projects/sb-api-client/src/api/attendances.service';
import { ClassBooksService, ClassBooks_GetStudents } from 'projects/sb-api-client/src/api/classBooks.service';
import { SchedulesService, Schedules_GetOffDatesPg } from 'projects/sb-api-client/src/api/schedules.service';
import { AttendanceType } from 'projects/sb-api-client/src/model/attendanceType';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { PaginatorItem } from 'projects/shared/components/paginator/paginator-item';
import {
  IShouldPreventLeave,
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { LocalStorageService } from 'projects/shared/services/local-storage.service';
import { ClassBookInfoType } from 'projects/shared/utils/book';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from } from 'rxjs';
import {
  AbsencesExcuseDialogComponent,
  AbsencesExcuseDialogResult
} from '../absences-excuse-dialog/absences-excuse-dialog.component';
import { CLASS_BOOK_INFO } from '../book/book.component';

export enum AttendancesEditMode {
  New = 'New',
  Remove = 'Remove'
}

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class AttendancesEditSkeletonComponent extends SkeletonComponentBase {
  constructor(
    classBooksService: ClassBooksService,
    attendancesService: AttendancesService,
    schedulesService: SchedulesService,
    route: ActivatedRoute,
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const date = parse(route.snapshot.paramMap.get('date') ?? throwParamError('date'), 'yyyy-MM-dd', new Date(), {});
    if (!isValid(date)) {
      throwParamError('date');
    }

    const mode = route.snapshot.data.mode as AttendancesEditMode;

    this.resolve(AttendancesEditComponent, {
      schoolYear,
      instId,
      classBookId,
      classBookInfo: from(classBookInfo),
      date,
      mode,
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
      attendances: attendancesService.getAllForDate({
        schoolYear,
        instId,
        classBookId,
        date
      })
    });
  }
}

type StudentAttendance = ArrayElementType<ReturnType<typeof getTemplateData>['studentAttendances']>;

@Component({
  selector: 'sb-attendances-edit',
  templateUrl: './attendances-edit.component.html',
  styleUrls: ['./attendances-edit.component.scss']
})
export class AttendancesEditComponent implements OnInit, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    classBookInfo: ClassBookInfoType;
    date: Date;
    mode: AttendancesEditMode;
    students: ClassBooks_GetStudents;
    offDatesPg: Schedules_GetOffDatesPg;
    attendances: Attendances_GetAllForDate;
  };

  readonly fasTrashAlt = fasTrashAlt;
  readonly fasUndo = fasUndo;

  readonly ATTENDANCES_EDIT_MODE_NEW = AttendancesEditMode.New;
  readonly ATTENDANCES_EDIT_MODE_REMOVE = AttendancesEditMode.Remove;

  readonly ATTENDANCE_TYPE_PRESENCE = AttendanceType.Presence;
  readonly ATTENDANCE_TYPE_UNEXCUSED_ABSENCE = AttendanceType.UnexcusedAbsence;
  readonly ATTENDANCE_TYPE_EXCUSED_ABSENCE = AttendanceType.ExcusedAbsence;

  readonly form = this.fb.group({});

  datePaginatorItems!: PaginatorItem[];
  templateData!: ReturnType<typeof getTemplateData>;
  isDirty = false;

  constructor(
    public localStorageService: LocalStorageService,
    private fb: UntypedFormBuilder,
    private attendancesService: AttendancesService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private dialog: MatDialog
  ) {}

  ngOnInit() {
    const prevDate = addDays(this.data.date, -1);
    const nextDate = addDays(this.data.date, 1);

    this.datePaginatorItems = [
      {
        icon: fasChevronLeft,
        routeCommands: ['../', format(prevDate, 'yyyy-MM-dd', {})]
      },
      {
        text: format(this.data.date, 'EEEE dd.MM', { locale: bg })
      },
      {
        icon: fasChevronRight,
        routeCommands: ['../', format(nextDate, 'yyyy-MM-dd', {})]
      }
    ];

    this.templateData = getTemplateData(this.data);
  }

  setPresence(studentAttendance: StudentAttendance) {
    this.isDirty = true;
    studentAttendance.type = AttendanceType.Presence;
  }

  setUnexcusedAbsence(studentAttendance: StudentAttendance) {
    this.isDirty = true;
    studentAttendance.type = AttendanceType.UnexcusedAbsence;
  }

  setExcusedAbsence(studentAttendance: StudentAttendance) {
    this.isDirty = true;
    studentAttendance.type = AttendanceType.ExcusedAbsence;
  }

  remove(studentAttendance: StudentAttendance) {
    this.isDirty = true;
    studentAttendance.isRemoved = true;
  }

  undoRemove(studentAttendance: StudentAttendance) {
    this.isDirty = true;
    studentAttendance.isRemoved = false;
  }

  shouldPreventLeave() {
    return this.isDirty;
  }

  async onSave(save: SaveToken) {
    let result: Promise<void>;

    switch (this.data.mode) {
      case AttendancesEditMode.New: {
        const addedAttendances = this.templateData.studentAttendances
          .filter((s) => !s.attendanceId && s.type != null)
          .map((s) => ({
            personId: s.personId,
            type: s.type ?? throwError()
          }));

        if (addedAttendances.length === 0) {
          result = Promise.resolve();
        } else {
          let excusedDialogResult: AbsencesExcuseDialogResult | undefined = undefined;

          if (addedAttendances.find((a) => a.type === AttendanceType.ExcusedAbsence)) {
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
          result = this.attendancesService
            .createAttendances({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              date: this.data.date,
              createAttendancesCommand: {
                excusedReasonId: excusedDialogResult?.excusedReasonId,
                excusedReasonComment: excusedDialogResult?.excusedReasonComment,
                attendances: addedAttendances
              }
            })
            .toPromise();
        }

        break;
      }

      case AttendancesEditMode.Remove: {
        const removedAttendanceIds = this.templateData.studentAttendances
          .filter((s) => s.isRemoved)
          .map((s) => s.attendanceId ?? throwError());

        if (removedAttendanceIds.length === 0) {
          result = Promise.resolve();
        } else {
          result = this.attendancesService
            .removeAttendances({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              removeAttendancesCommand: {
                attendanceIds: removedAttendanceIds
              }
            })
            .toPromise();
        }

        break;
      }
    }

    this.actionService
      .execute({
        httpAction: () =>
          result.then(() => {
            this.isDirty = false;
            this.router.navigate(['../../'], { relativeTo: this.route });
          })
      })
      .then((success) => save.done(success));
  }
}

function getTemplateData(data: AttendancesEditComponent['data']) {
  const schoolYearStartDateLimit = data.classBookInfo.schoolYearStartDateLimit;
  const schoolYearStartDate = data.classBookInfo.schoolYearStartDate;
  const schoolYearEndDate = data.classBookInfo.schoolYearEndDate;
  const schoolYearEndDateLimit = data.classBookInfo.schoolYearEndDateLimit;

  const isOutsideSchoolYear = !isWithinInterval(data.date, { start: schoolYearStartDate, end: schoolYearEndDate });
  const isOutsideSchoolYearLimits = !isWithinInterval(data.date, {
    start: schoolYearStartDateLimit,
    end: schoolYearEndDateLimit
  });
  const offDay = data.offDatesPg.find((od) => isEqual(data.date, od.date));

  const isOffDay = getISODay(data.date) > 5 || (offDay && !offDay.isPgOffProgramDay);
  const isOffProgramDay =
    !isOffDay && !isOutsideSchoolYearLimits && ((offDay && offDay.isPgOffProgramDay) || isOutsideSchoolYear);

  const studentAttendances = data.students.map((s) => {
    const attendance = data.attendances.find((a) => a.personId === s.personId);

    if (attendance) {
      return {
        ...s,
        attendanceId: attendance.attendanceId,
        type: attendance.type,
        isRemoved: false
      };
    } else if (data.mode === AttendancesEditMode.New) {
      return {
        ...s,
        attendanceId: null,
        type: !s.isTransferred ? AttendanceType.Presence : null,
        isRemoved: false
      };
    } else if (data.mode === AttendancesEditMode.Remove) {
      return {
        ...s,
        attendanceId: null,
        type: null,
        isRemoved: false
      };
    } else {
      throwError('Unknow AttendancesEditMode');
    }
  });

  const bookAllowsAttendanceAbsenceTopicModifications =
    data.classBookInfo.checkBookAllowsAttendanceAbsenceTopicModifications(data.date);
  return {
    isOffDay,
    isOffProgramDay,
    isOutsideSchoolYearLimits,
    canCreate:
      !isOffDay &&
      !isOutsideSchoolYearLimits &&
      bookAllowsAttendanceAbsenceTopicModifications &&
      (data.classBookInfo.hasCreateAttendanceAccess ||
        data.classBookInfo.createAttendanceReplTeacherAccessDates?.some((replDate) => isSameDay(replDate, data.date))),
    canRemove:
      !isOffDay &&
      !isOutsideSchoolYearLimits &&
      bookAllowsAttendanceAbsenceTopicModifications &&
      data.classBookInfo.hasRemoveAttendanceAccess,
    studentAttendances
  };
}
