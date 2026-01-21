import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faCalendarCheck as farCalendarCheck } from '@fortawesome/pro-regular-svg-icons/faCalendarCheck';
import { faCalendarTimes as farCalendarTimes } from '@fortawesome/pro-regular-svg-icons/faCalendarTimes';
import { faBookOpen as fasBookOpen } from '@fortawesome/pro-solid-svg-icons/faBookOpen';
import { faCalendarAlt as fasCalendarAlt } from '@fortawesome/pro-solid-svg-icons/faCalendarAlt';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTimes as fasTimes } from '@fortawesome/pro-solid-svg-icons/faTimes';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { add, eachMonthOfInterval, endOfMonth, min } from 'date-fns';
import { AbsencesService } from 'projects/sb-api-client/src/api/absences.service';
import {
  AbsencesDplrService,
  AbsencesDplr_GetAllForClassBook,
  AbsencesDplr_GetAllForStudentAndType
} from 'projects/sb-api-client/src/api/absencesDplr.service';
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
import { expiredAt } from 'projects/shared/utils/rxjs';
import { ArrayElementType } from 'projects/shared/utils/type';
import { enumFromStringValue, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, merge, Observable, Subject } from 'rxjs';
import { filter, take, takeUntil, tap } from 'rxjs/operators';
import { CLASS_BOOK_INFO } from '../book/book.component';

export enum AbsencesDplrType {
  Absence = 'Absence',
  Attendance = 'Attendance'
}

const SCHOOL_YEAR_START_MONTH = 9; // September
const SCHOOL_YEAR_END_MONTH = 7; // July

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class AbsencesDplrSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    classBooksService: ClassBooksService,
    absencesDplrService: AbsencesDplrService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const type = enumFromStringValue(AbsencesDplrType, route.snapshot.paramMap.get('type')) ?? throwParamError('type');
    const absenceType = type === AbsencesDplrType.Absence ? AbsenceType.DplrAbsence : AbsenceType.DplrAttendance;

    this.resolve(
      AbsencesDplrComponent,
      resolveWithRemovedStudents(
        classBooksService,
        (data) => data.students,
        (data) => data.absences,
        {
          schoolYear,
          instId,
          classBookId,
          classBookInfo: from(classBookInfo),
          absenceType,
          students: classBooksService.getStudents({
            schoolYear,
            instId,
            classBookId
          }),
          absences: absencesDplrService.getAllForClassBook({
            schoolYear,
            instId,
            classBookId,
            type: absenceType
          })
        }
      )
    );
  }
}

type Absence = ArrayElementType<AbsencesDplr_GetAllForStudentAndType> & {
  undoExpired$: Observable<boolean>;
  removing: boolean;
  canUndo: boolean;
  canRemove: boolean;
};

enum PeriodValues {
  WholeYear = 'WholeYear',
  Month = 'Month',
  FromDateToDate = 'FromDateToDate'
}

@Component({
  selector: 'sb-absences-dplr',
  templateUrl: './absences-dplr.component.html'
})
export class AbsencesDplrComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    classBookInfo: ClassBookInfoType;
    absenceType: AbsenceType;
    students: ClassBooks_GetStudents;
    absences: AbsencesDplr_GetAllForClassBook;
    removedStudents: ClassBooks_GetRemovedStudents;
  };

  readonly fasBookOpen = fasBookOpen;
  readonly fasCalendarAlt = fasCalendarAlt;
  readonly fasPlus = fasPlus;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasTimes = fasTimes;
  readonly farCalendarTimes = farCalendarTimes;
  readonly farCalendarCheck = farCalendarCheck;

  dplrAbsencesBannerHidden = false;

  readonly PeriodValues_WholeYear = PeriodValues.WholeYear;
  readonly PeriodValues_Month = PeriodValues.Month;
  readonly PeriodValues_FromDateToDate = PeriodValues.FromDateToDate;

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
  selectedAbsences?: Absence[];
  loadingAbsences = false;

  isAbsence = false;
  canCreate = false;
  canRemove = false;

  classBookCurriculumNomsService: INomService<number, { schoolYear: number; instId: number; classBookId: number }>;

  constructor(
    private fb: UntypedFormBuilder,
    private actionService: ActionService,
    private absencesService: AbsencesService,
    private absencesDplrService: AbsencesDplrService,
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
    this.isAbsence = this.data.absenceType === AbsenceType.DplrAbsence;

    // this relates only to the bulk actions and does not check access rights
    this.canCreate = this.data.classBookInfo.bookAllowsModifications;
    this.canRemove =
      this.data.classBookInfo.bookAllowsModifications && this.data.classBookInfo.hasRemoveDplrAbsenceAccess;

    this.dplrAbsencesBannerHidden = this.localStorageService.getDplrAbsencesBannerHidden();
    this.localStorageService.dplrAbsencesBannerHidden$
      .pipe(
        filter((dplrAbsencesBannerHidden) => dplrAbsencesBannerHidden),
        tap(() => this.hideDplrAbsencesBanner()),
        take(1),
        takeUntil(
          merge(
            // unsubscribe if the user hides the banner himself
            this.localStorageService.dplrAbsencesBannerHidden$.pipe(
              filter((dplrAbsencesBannerHidden) => dplrAbsencesBannerHidden)
            ),
            this.destroyed$
          )
        )
      )
      .subscribe();
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
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

  reloadAbsences(curriculumId: number | null, fromDate: Date | null, toDate: Date | null) {
    return this.absencesDplrService
      .getAllForClassBook({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        type: this.data.absenceType,
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

  viewAbsences(studentPersonId: number) {
    const curriculumIdControlValue = this.searchForm.get('curriculumId')?.value;

    const periodDates = this.getPeriodDates();

    this.selectedAbsences = undefined;

    if (this.selectedStudentPersonId === studentPersonId) {
      this.selectedStudentPersonId = undefined;
      return;
    }

    this.selectedStudentPersonId = studentPersonId;
    this.loadingAbsences = true;

    return this.absencesDplrService
      .getAllForStudentAndType({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        personId: this.selectedStudentPersonId,
        type: this.data.absenceType,
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
            excusing: false,
            canUndo: bookAllowsAttendanceAbsenceTopicModifications && a.hasUndoAccess,
            canRemove: bookAllowsAttendanceAbsenceTopicModifications && a.hasRemoveAccess
          };
        });
      })
      .catch((err) => {
        this.selectedStudentPersonId = undefined;

        GlobalErrorHandler.instance.handleError(err);
      })
      .finally(() => {
        this.loadingAbsences = false;
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

    const absenceTypeName = this.isAbsence ? 'отсъствието' : 'присъствието';
    this.actionService
      .execute({
        confirmMessage: `Сигурни ли сте, че искате да изтриете ${absenceTypeName}?`,
        errorsMessage: `Не може да изтриете ${absenceTypeName}, защото:`,
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

  hideDplrAbsencesBanner() {
    this.dplrAbsencesBannerHidden = true;
    this.localStorageService.setDplrAbsencesBannerHidden(true);
  }
}

function getStudentAbsences(data: AbsencesDplrComponent['data'], bookTransferredHidden: boolean) {
  const absencesMap = new Map(data.absences.map((a) => [a.personId, a.count]));

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
      const count = absencesMap.get(s.personId) ?? 0;

      return {
        ...s,

        showInfoLink: !s.isTransferred && !s.isRemoved,
        abnormalStatus: s.isRemoved ? 'ИЗТРИТ' : s.isTransferred ? 'ОТПИСАН' : '',

        count
      };
    });

  if (bookTransferredHidden) {
    students = students.filter((s) => !s.isTransferred || s.count);
  }

  const total = students.reduce((total, s) => total + s.count, 0);

  return {
    students,
    total
  };
}
