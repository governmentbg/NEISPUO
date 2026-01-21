import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { faNotesMedical as fadNotesMedical } from '@fortawesome/pro-duotone-svg-icons/faNotesMedical';
import { faCalendarTimes as farCalendarTimes } from '@fortawesome/pro-regular-svg-icons/faCalendarTimes';
import { faCircle as farCircle } from '@fortawesome/pro-regular-svg-icons/faCircle';
import { faBookOpen as fasBookOpen } from '@fortawesome/pro-solid-svg-icons/faBookOpen';
import { faCalendarAlt as fasCalendarAlt } from '@fortawesome/pro-solid-svg-icons/faCalendarAlt';
import { faEye as fasEye } from '@fortawesome/pro-solid-svg-icons/faEye';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { add, eachMonthOfInterval, endOfMonth, min } from 'date-fns';
import {
  AbsencesService,
  Absences_GetAllForClassBook,
  Absences_GetAllForStudentAndType
} from 'projects/sb-api-client/src/api/absences.service';
import { ClassBookCurriculumNomsService } from 'projects/sb-api-client/src/api/classBookCurriculumNoms.service';
import {
  ClassBooksService,
  ClassBooks_GetRemovedStudents,
  ClassBooks_GetStudents
} from 'projects/sb-api-client/src/api/classBooks.service';
import { AbsenceType } from 'projects/sb-api-client/src/model/absenceType';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { LocalStorageService } from 'projects/shared/services/local-storage.service';
import { ClassBookInfoType, resolveWithRemovedStudents, UNDO_INTERVAL_IN_MINUTES } from 'projects/shared/utils/book';
import { formatMonth, parseMonth } from 'projects/shared/utils/date';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { expiredAt } from 'projects/shared/utils/rxjs';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Observable, Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';
import { AbsenceExcuseDialogSkeletonComponent } from '../absence-excuse-dialog/absence-excuse-dialog.component';
import { CLASS_BOOK_INFO } from '../book/book.component';

const SCHOOL_YEAR_START_MONTH = 9; // September
const SCHOOL_YEAR_END_MONTH = 7; // July

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class AbsencesSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    classBooksService: ClassBooksService,
    absencesService: AbsencesService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');

    this.resolve(
      AbsencesComponent,
      resolveWithRemovedStudents(
        classBooksService,
        (data) => data.students,
        (data) => data.absences,
        {
          schoolYear,
          instId,
          classBookId,
          classBookInfo: from(classBookInfo),
          students: classBooksService.getStudents({
            schoolYear,
            instId,
            classBookId
          }),
          absences: absencesService.getAllForClassBook({
            schoolYear,
            instId,
            classBookId
          })
        }
      )
    );
  }
}

type Absence = ArrayElementType<Absences_GetAllForStudentAndType> & {
  undoExpired$: Observable<boolean>;
  removing: boolean;
  converting: boolean;
  excusing: boolean;
  canConvert: boolean;
  canExcuse: boolean;
  canUndo: boolean;
  canRemove: boolean;
};

enum PeriodValues {
  WholeYear = 'WholeYear',
  Month = 'Month',
  FromDateToDate = 'FromDateToDate'
}

@Component({
  selector: 'sb-absences',
  templateUrl: './absences.component.html'
})
export class AbsencesComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    classBookInfo: ClassBookInfoType;
    students: ClassBooks_GetStudents;
    absences: Absences_GetAllForClassBook;
    removedStudents: ClassBooks_GetRemovedStudents;
  };

  readonly fasBookOpen = fasBookOpen;
  readonly fasCalendarAlt = fasCalendarAlt;
  readonly fasPlus = fasPlus;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasTrashAlt = fasTrashAlt;
  readonly farCalendarTimes = farCalendarTimes;
  readonly farCircle = farCircle;
  readonly fadNotesMedical = fadNotesMedical;
  readonly fasEye = fasEye;

  readonly PeriodValues_WholeYear = PeriodValues.WholeYear;
  readonly PeriodValues_Month = PeriodValues.Month;
  readonly PeriodValues_FromDateToDate = PeriodValues.FromDateToDate;

  readonly ABSENCE_TYPE_LATE = AbsenceType.Late;
  readonly ABSENCE_TYPE_UNEXCUSED = AbsenceType.Unexcused;
  readonly ABSENCE_TYPE_EXCUSED = AbsenceType.Excused;

  readonly destroyed$ = new Subject<void>();

  monthsSelect: { id: string; name: string }[] = [];

  readonly searchForm = this.fb.nonNullable.group({
    curriculumId: this.fb.nonNullable.control<number | null | undefined>(null),
    period: this.fb.nonNullable.control<PeriodValues | null | undefined>(PeriodValues.WholeYear, Validators.required),
    month: this.fb.nonNullable.control<string | null | undefined>(null, Validators.required),
    fromDate: this.fb.nonNullable.control<Date | null | undefined>(null),
    toDate: this.fb.nonNullable.control<Date | null | undefined>(null)
  });

  studentAbsences!: ReturnType<typeof getStudentAbsences>;
  showTransferredStudentsBanner = false;

  selectedStudentPersonId?: number;
  selectedType?: AbsenceType;
  selectedAbsences?: Absence[];
  selectedCarriedAbsences?: string;
  loadingAbsences = false;

  canCreate = false;
  canConvert = false;
  canExcuse = false;
  canRemove = false;

  classBookCurriculumNomsService: INomService<number, { schoolYear: number; instId: number; classBookId: number }>;

  constructor(
    private fb: UntypedFormBuilder,
    private actionService: ActionService,
    private absencesService: AbsencesService,
    private dialog: MatDialog,
    private localStorageService: LocalStorageService,
    classBookCurriculumNomsService: ClassBookCurriculumNomsService
  ) {
    this.classBookCurriculumNomsService = new NomServiceWithParams(classBookCurriculumNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId
    }));
  }

  ngOnInit() {
    this.showTransferredStudentsBanner = this.data.students.find((s) => s.isTransferred) != null;

    this.monthsSelect = eachMonthOfInterval({
      start: new Date(this.data.schoolYear, SCHOOL_YEAR_START_MONTH - 1, 1),
      end: min([endOfMonth(new Date(this.data.schoolYear + 1, SCHOOL_YEAR_END_MONTH - 1, 1)), new Date()])
    })
      .map((d) => formatMonth(d))
      .map((m) => ({
        id: m,
        name: m
      }));

    this.searchForm.valueChanges
      .pipe(
        tap(() => {
          const curriculumIdControlValue = this.searchForm.get('curriculumId')?.value;

          const periodDates = this.getPeriodDates();

          this.reloadAbsences(curriculumIdControlValue ?? null, periodDates.startDate, periodDates.endDate);
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();

    this.localStorageService.bookTransferredHidden$
      .pipe(
        tap((bookTransferredHidden) => {
          this.studentAbsences = getStudentAbsences(this.data, bookTransferredHidden);
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();

    // this relates only to the bulk actions and does not check access rights
    this.canCreate = this.data.classBookInfo.bookAllowsModifications;
    this.canExcuse = this.data.classBookInfo.bookAllowsModifications;
    this.canRemove = this.data.classBookInfo.bookAllowsModifications && this.data.classBookInfo.hasRemoveAbsenceAccess;
  }

  getPeriodDates() {
    const periodControlValue = this.searchForm.get('period')?.value;
    const monthControlValue = this.searchForm.get('month')?.value;
    const fromDateControlValue = this.searchForm.get('fromDate')?.value;
    const toDateControlValue = this.searchForm.get('toDate')?.value;

    let startDate: Date | null;
    let endDate: Date | null;

    if (periodControlValue === PeriodValues.Month && monthControlValue) {
      const date = parseMonth(monthControlValue),
        y = date.getFullYear(),
        m = date.getMonth();
      startDate = new Date(y, m, 1);
      endDate = new Date(y, m + 1, 0);
    } else if (periodControlValue === PeriodValues.FromDateToDate) {
      startDate = fromDateControlValue ?? null;
      endDate = toDateControlValue ?? null;
    } else {
      startDate = null;
      endDate = null;
    }

    return { startDate, endDate };
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  reloadAbsences(curriculumId: number | null, fromDate: Date | null, toDate: Date | null) {
    return this.absencesService
      .getAllForClassBook({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        curriculumId,
        fromDate,
        toDate
      })
      .toPromise()
      .then((absences) => {
        this.selectedStudentPersonId = undefined;
        this.selectedAbsences = undefined;

        this.data.absences = absences;
        this.studentAbsences = getStudentAbsences(this.data, this.localStorageService.getBookTransferredHidden());
      })
      .catch((err) => GlobalErrorHandler.instance.handleError(err));
  }

  viewAbsences(studentPersonId: number, type: AbsenceType) {
    const curriculumIdControlValue = this.searchForm.get('curriculumId')?.value;

    const periodDates = this.getPeriodDates();

    this.selectedAbsences = undefined;
    this.selectedCarriedAbsences = undefined;

    if (this.selectedStudentPersonId === studentPersonId && this.selectedType === type) {
      this.selectedStudentPersonId = undefined;
      this.selectedType = undefined;

      return;
    }

    this.selectedStudentPersonId = studentPersonId;
    this.selectedType = type;
    this.loadingAbsences = true;

    return this.absencesService
      .getAllForStudentAndType({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        personId: this.selectedStudentPersonId,
        type: this.selectedType,
        curriculumId: curriculumIdControlValue,
        fromDate: periodDates.startDate,
        toDate: periodDates.endDate
      })
      .toPromise()
      .then((absences) => {
        this.selectedAbsences = absences.map((a) => {
          const bookAllowsAttendanceAbsenceTopicModifications =
            this.data.classBookInfo.checkBookAllowsAttendanceAbsenceTopicModifications(a.date);
          return {
            ...a,
            undoExpired$: expiredAt(add(a.createDate, { minutes: UNDO_INTERVAL_IN_MINUTES })),
            removing: false,
            converting: false,
            excusing: false,
            canConvert:
              bookAllowsAttendanceAbsenceTopicModifications && a.type === AbsenceType.Unexcused && a.hasUndoAccess,
            canExcuse:
              bookAllowsAttendanceAbsenceTopicModifications && a.type === AbsenceType.Unexcused && a.hasExcuseAccess,
            canUndo: bookAllowsAttendanceAbsenceTopicModifications && a.hasUndoAccess,
            canRemove: bookAllowsAttendanceAbsenceTopicModifications && a.hasRemoveAccess
          };
        });

        const selectedCarriedAbsences = this.data.absences.find((a) => a.personId === studentPersonId);
        const excusedCount = selectedCarriedAbsences!.carriedExcusedAbsencesCount;
        const unexcusedCount = selectedCarriedAbsences!.carriedUnexcusedAbsencesCount;
        const lateCount = selectedCarriedAbsences!.carriedLateAbsencesCount;

        if (type === AbsenceType.Excused && excusedCount > 0) {
          this.selectedCarriedAbsences = `извинени отсъствия - ${excusedCount}`;
        } else if (type === AbsenceType.Unexcused && unexcusedCount > 0) {
          this.selectedCarriedAbsences = `неизвинени отсъствия - ${unexcusedCount}`;
        } else if (type === AbsenceType.Late && lateCount > 0) {
          this.selectedCarriedAbsences = `закъснения - ${lateCount}`;
        }
      })
      .catch((err) => {
        this.selectedStudentPersonId = undefined;
        this.selectedType = undefined;

        GlobalErrorHandler.instance.handleError(err);
      })
      .finally(() => {
        this.loadingAbsences = false;
      });
  }

  convertToLateAbsence(absence: Absence) {
    absence.converting = true;

    const params = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      absenceId: absence.absenceId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да конвертирате отсъствието в закъснение?',
        errorsMessage: 'Не може да конвертирате отсъствието в закъснение, защото:',
        httpAction: () => this.absencesService.convertToLateAbsence(params).toPromise()
      })
      .then((done) => {
        if (done) {
          return this.reloadAbsences(null, null, null);
        }

        return Promise.resolve();
      })
      .finally(() => {
        absence.converting = false;
      });
  }

  openExcuseAbsenceDialog(absence: Absence) {
    absence.excusing = true;

    openTypedDialog(this.dialog, AbsenceExcuseDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        absenceId: absence.absenceId
      }
    })
      .afterClosed()
      .toPromise()
      .then((ok) => {
        if (ok) {
          return this.reloadAbsences(null, null, null);
        }

        return Promise.resolve();
      })
      .finally(() => {
        absence.excusing = false;
      });
  }

  removeAbsence(absence: Absence) {
    absence.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      absenceId: absence.absenceId
    };

    this.actionService
      .execute({
        confirmMessage: `Сигурни ли сте, че искате да изтриете ${
          absence.type === AbsenceType.Late ? 'закъснението' : 'отсъствието'
        }?`,
        errorsMessage: `Не може да изтриете ${
          absence.type === AbsenceType.Late ? 'закъснението' : 'отсъствието'
        }, защото:`,
        httpAction: () => this.absencesService.removeAbsence(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          return this.reloadAbsences(null, null, null);
        }

        return Promise.resolve();
      })
      .finally(() => {
        absence.removing = false;
      });
  }
}

function getStudentAbsences(data: AbsencesComponent['data'], bookTransferredHidden: boolean) {
  const absencesMap = new Map(
    data.absences.map((a) => [
      a.personId,
      {
        lateAbsencesCount: a.lateAbsencesCount,
        unexcusedAbsencesCount: a.unexcusedAbsencesCount,
        excusedAbsencesCount: a.excusedAbsencesCount,
        carriedLateAbsencesCount: a.carriedLateAbsencesCount,
        carriedUnexcusedAbsencesCount: a.carriedUnexcusedAbsencesCount,
        carriedExcusedAbsencesCount: a.carriedExcusedAbsencesCount
      }
    ])
  );

  let students = data.students
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
    .map((s) => {
      const absences = absencesMap.get(s.personId);
      return {
        ...s,
        showInfoLink: !s.isTransferred && !s.isRemoved,
        abnormalStatus: s.isRemoved ? 'ИЗТРИТ' : s.isTransferred ? 'ОТПИСАН' : '',

        lateAbsencesCount: (absences?.lateAbsencesCount ?? 0) + (absences?.carriedLateAbsencesCount ?? 0),
        unexcusedAbsencesCount:
          (absences?.unexcusedAbsencesCount ?? 0) + (absences?.carriedUnexcusedAbsencesCount ?? 0),
        excusedAbsencesCount: (absences?.excusedAbsencesCount ?? 0) + (absences?.carriedExcusedAbsencesCount ?? 0)
      };
    });

  if (bookTransferredHidden) {
    students = students.filter(
      (s) => !s.isTransferred || s.lateAbsencesCount || s.unexcusedAbsencesCount || s.excusedAbsencesCount
    );
  }

  const totals = students.reduce(
    (totals, s) => {
      totals.lateAbsencesCountTotal += s.lateAbsencesCount;
      totals.unexcusedAbsencesCountTotal += s.unexcusedAbsencesCount;
      totals.excusedAbsencesCountTotal += s.excusedAbsencesCount;

      return totals;
    },
    {
      lateAbsencesCountTotal: 0,
      unexcusedAbsencesCountTotal: 0,
      excusedAbsencesCountTotal: 0
    }
  );

  return {
    students,
    totals
  };
}
