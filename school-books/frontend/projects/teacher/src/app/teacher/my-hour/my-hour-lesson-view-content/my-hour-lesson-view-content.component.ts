import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { faCalendarTimes as fadCalendarTimes } from '@fortawesome/pro-duotone-svg-icons/faCalendarTimes';
import { faCommentDots as fadCommentDots } from '@fortawesome/pro-duotone-svg-icons/faCommentDots';
import { faPencilAlt as fadPencilAlt } from '@fortawesome/pro-duotone-svg-icons/faPencilAlt';
import { faSpinnerThird as fadSpinnerThird } from '@fortawesome/pro-duotone-svg-icons/faSpinnerThird';
import { faTrashAlt as fadTrashAlt } from '@fortawesome/pro-duotone-svg-icons/faTrashAlt';
import { faCalendarTimes as farCalendarTimes } from '@fortawesome/pro-regular-svg-icons/faCalendarTimes';
import { faCircle as farCircle } from '@fortawesome/pro-regular-svg-icons/faCircle';
import { faBookOpen as fasBookOpen } from '@fortawesome/pro-solid-svg-icons/faBookOpen';
import { faCalendarAlt as fasCalendarAlt } from '@fortawesome/pro-solid-svg-icons/faCalendarAlt';
import { faPencilAlt as fasPencilAlt } from '@fortawesome/pro-solid-svg-icons/faPencilAlt';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { add, isValid, parse } from 'date-fns';
import { AbsencesService } from 'projects/sb-api-client/src/api/absences.service';
import { ClassBooksService, ClassBooks_GetRemovedStudents } from 'projects/sb-api-client/src/api/classBooks.service';
import { GradesService, Grades_Get } from 'projects/sb-api-client/src/api/grades.service';
import {
  MyHourService,
  MyHour_GetTeacherLesson,
  MyHour_GetTeacherLessonAbsences,
  MyHour_GetTeacherLessonGrades
} from 'projects/sb-api-client/src/api/myHour.service';
import { TopicsService, Topics_RemoveTopicsRequestParams } from 'projects/sb-api-client/src/api/topics.service';
import { AbsenceType } from 'projects/sb-api-client/src/model/absenceType';
import { ClassBookType } from 'projects/sb-api-client/src/model/classBookType';
import { GradeCategory } from 'projects/sb-api-client/src/model/gradeCategory';
import { SchoolTerm } from 'projects/sb-api-client/src/model/schoolTerm';
import {
  SIMPLE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { LocalStorageService } from 'projects/shared/services/local-storage.service';
import { groupBy } from 'projects/shared/utils/array';
import { assert } from 'projects/shared/utils/assert';
import {
  ClassBookInfoType,
  classBookTypeAllowsGrades,
  extendClassBookInfo,
  getTermFromDate,
  resolveWithRemovedStudents,
  UNDO_INTERVAL_IN_MINUTES
} from 'projects/shared/utils/book';
import { formatNullableDate } from 'projects/shared/utils/date';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { reloadRoute } from 'projects/shared/utils/router';
import { expiredAt } from 'projects/shared/utils/rxjs';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { combineLatest, Observable, of, Subject } from 'rxjs';
import { map, takeUntil, tap } from 'rxjs/operators';
import { AbsenceExcuseDialogSkeletonComponent } from 'src/app/teacher/book/absence-excuse-dialog/absence-excuse-dialog.component';

@Component({
  template: SIMPLE_SKELETON_TEMPLATE
})
export class MyHourLessonViewContentSkeletonComponent extends SkeletonComponentBase {
  constructor(classBooksService: ClassBooksService, myHourService: MyHourService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const scheduleLessonId =
      tryParseInt(route.snapshot.paramMap.get('scheduleLessonId')) ?? throwParamError('scheduleLessonId');
    const date = parse(route.snapshot.paramMap.get('date') ?? throwParamError('date'), 'yyyy-MM-dd', new Date(), {});
    if (!isValid(date)) {
      throwParamError('date');
    }

    this.resolve(
      MyHourLessonViewContentComponent,
      resolveWithRemovedStudents(
        classBooksService,
        (data) => data.teacherLesson.students,
        (data) => [
          ...data.absences.map((a) => ({
            personId: a.personId
          })),
          ...data.grades.map((g) => ({
            personId: g.personId
          }))
        ],
        {
          schoolYear,
          instId,
          classBookId,
          date,
          scheduleLessonId,
          classBookInfo: classBooksService
            .get({
              schoolYear,
              instId,
              classBookId
            })
            .pipe(map(extendClassBookInfo)),
          teacherLesson: myHourService.getTeacherLesson({
            schoolYear,
            instId,
            classBookId,
            scheduleLessonId
          }),
          absences: myHourService.getTeacherLessonAbsences({
            schoolYear,
            instId,
            classBookId,
            scheduleLessonId
          }),
          grades: myHourService.getTeacherLessonGrades({
            schoolYear,
            instId,
            classBookId,
            scheduleLessonId
          })
        }
      )
    );
  }
}

export enum MyHourLessonViewMode {
  MyHour = 'MyHour',
  BookVerification = 'BookVerification'
}

@Component({
  selector: 'sb-my-hour-lesson-view-content',
  templateUrl: './my-hour-lesson-view-content.component.html'
})
export class MyHourLessonViewContentComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    date: Date;
    scheduleLessonId: number;
    classBookInfo: ClassBookInfoType;
    teacherLesson: MyHour_GetTeacherLesson;
    absences: MyHour_GetTeacherLessonAbsences;
    grades: MyHour_GetTeacherLessonGrades;
    removedStudents: ClassBooks_GetRemovedStudents;
  };

  @Input() mode = MyHourLessonViewMode.MyHour;

  readonly fadCalendarTimes = fadCalendarTimes;
  readonly fadCommentDots = fadCommentDots;
  readonly fadSpinnerThird = fadSpinnerThird;
  readonly fadPencilAlt = fadPencilAlt;
  readonly fadTrashAlt = fadTrashAlt;

  readonly farCalendarTimes = farCalendarTimes;
  readonly farCircle = farCircle;

  readonly fasBookOpen = fasBookOpen;
  readonly fasCalendarAlt = fasCalendarAlt;
  readonly fasPencilAlt = fasPencilAlt;
  readonly fasPlus = fasPlus;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasTrashAlt = fasTrashAlt;

  readonly ABSENCE_TYPE_LATE = AbsenceType.Late;
  readonly ABSENCE_TYPE_UNEXCUSED = AbsenceType.Unexcused;
  readonly ABSENCE_TYPE_EXCUSED = AbsenceType.Excused;
  readonly ABSENCE_TYPE_DPLR_ABSENCE = AbsenceType.DplrAbsence;
  readonly ABSENCE_TYPE_DPLR_ATTENDANCE = AbsenceType.DplrAttendance;

  readonly GRADE_CATEGORY_DECIMAL = GradeCategory.Decimal;
  readonly GRADE_CATEGORY_QUALITATIVE = GradeCategory.Qualitative;
  readonly GRADE_CATEGORY_SPECIAL_NEEDS = GradeCategory.SpecialNeeds;

  readonly destroyed$ = new Subject<void>();

  templateData!: ReturnType<typeof getTemplateData>;
  canCreate!: boolean;
  isTaken!: boolean;
  schoolTerm!: SchoolTerm;
  bookLockedMessage = '';
  bookTypeAllowsGrades = false;
  isDplr = false;
  isPg = false;

  removingTopic = false;

  selectedGradeStudentPersonId?: number;
  selectedGradeId?: number;
  selectedGrade?: Grades_Get & { canUndo: boolean; canRemove: boolean };
  selectedGradeUndoExpired$?: Observable<boolean>;
  loadingGrade = false;
  removingGrade = false;

  selectedAbsenceStudentPersonId?: number;
  selectedAbsenceId?: number;
  selectedAbsence?: ArrayElementType<MyHour_GetTeacherLessonAbsences> & {
    canConvert: boolean;
    canExcuse: boolean;
    canUndo: boolean;
    canRemove: boolean;
  };
  selectedAbsenceUndoExpired$?: Observable<boolean>;
  convertingAbsence = false;
  excusingAbsence = false;
  removingAbsence = false;

  constructor(
    private localStorageService: LocalStorageService,
    private dialog: MatDialog,
    private myHourService: MyHourService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private topicsService: TopicsService,
    private gradesService: GradesService,
    private absencesService: AbsencesService
  ) {}

  ngOnInit() {
    this.isTaken = this.data.teacherLesson.topicId != null;
    this.schoolTerm =
      getTermFromDate(this.data.classBookInfo, this.data.date) ?? throwError('Date outside school term');
    this.bookTypeAllowsGrades = classBookTypeAllowsGrades(this.data.classBookInfo.bookType);
    this.isDplr = this.data.classBookInfo.bookType === ClassBookType.Book_DPLR;
    this.isPg = this.data.classBookInfo.bookType === ClassBookType.Book_PG;

    combineLatest([
      this.localStorageService.bookTransferredHidden$,
      this.localStorageService.bookNotEnrolledHidden$,
      this.localStorageService.bookGradelessHidden$
    ])
      .pipe(
        tap(([bookTransferredHidden, bookNotEnrolledHidden, bookGradelessHidden]) => {
          this.templateData = getTemplateData(
            this.data,
            this.schoolTerm,
            bookTransferredHidden,
            bookNotEnrolledHidden,
            bookGradelessHidden
          );
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();

    const bookHasPastMonthLock = this.data.classBookInfo.checkBookHasPastMonthLock(this.data.date);
    const bookHasFutureEntryLock = this.data.classBookInfo.checkBookHasFutureEntryLock(this.data.date);

    this.canCreate =
      this.mode === MyHourLessonViewMode.MyHour &&
      this.data.classBookInfo.bookAllowsModifications &&
      !bookHasPastMonthLock &&
      !bookHasFutureEntryLock &&
      !this.data.teacherLesson.isVerified;

    if (bookHasFutureEntryLock && this.mode === MyHourLessonViewMode.MyHour) {
      this.bookLockedMessage = 'Въвеждане на информация за бъдещи дати е забранено';
    } else if (bookHasPastMonthLock) {
      this.bookLockedMessage = `Дневникът е заключен за ${formatNullableDate(this.data.date)}`;
    } else if (this.data.teacherLesson.isVerified) {
      this.bookLockedMessage = 'Часът е проверен от директора';
    }
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  removeTopic() {
    assert(this.data.teacherLesson.topicId != null);

    this.removingTopic = true;

    const removeParams: Topics_RemoveTopicsRequestParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      removeTopicsCommand: {
        topics: [
          {
            topicId: this.data.teacherLesson.topicId,
            scheduleLessonId: this.data.teacherLesson.scheduleLessonId
          }
        ]
      }
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да премахнете темата?',
        errorsMessage: 'Не може да премахнете темата, защото:',
        httpAction: () => this.topicsService.removeTopics(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          reloadRoute(this.router, this.route);
        }

        return Promise.resolve();
      })
      .finally(() => {
        this.removingTopic = false;
      });
  }

  reloadAbsences() {
    return this.myHourService
      .getTeacherLessonAbsences({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        scheduleLessonId: this.data.scheduleLessonId
      })
      .toPromise()
      .then((absences) => {
        this.selectedAbsenceStudentPersonId = undefined;
        this.selectedAbsenceId = undefined;
        this.selectedAbsence = undefined;
        this.selectedAbsenceUndoExpired$ = undefined;

        this.data.absences = absences;
        this.templateData = getTemplateData(
          this.data,
          this.schoolTerm,
          this.localStorageService.getBookTransferredHidden(),
          this.localStorageService.getBookNotEnrolledHidden(),
          this.localStorageService.getBookGradelessHidden()
        );
      })
      .catch((err) => GlobalErrorHandler.instance.handleError(err));
  }

  selectStudentAbsence(studentPersonId: number, absenceId: number) {
    this.selectedGradeStudentPersonId = undefined;
    this.selectedGradeId = undefined;
    this.selectedGrade = undefined;
    this.selectedGradeUndoExpired$ = undefined;

    this.selectedAbsence = undefined;
    this.selectedAbsenceUndoExpired$ = undefined;

    if (this.selectedAbsenceStudentPersonId === studentPersonId && this.selectedAbsenceId === absenceId) {
      this.selectedAbsenceStudentPersonId = undefined;
      this.selectedAbsenceId = undefined;

      return;
    }

    this.selectedAbsenceStudentPersonId = studentPersonId;
    this.selectedAbsenceId = absenceId;

    const bookAllowsAbsenceModifications = this.data.classBookInfo.checkBookAllowsAttendanceAbsenceTopicModifications(
      this.data.date
    );

    const absence = this.data.absences.find((a) => a.absenceId === absenceId) ?? throwError();

    this.selectedAbsence = {
      ...absence,
      canConvert: bookAllowsAbsenceModifications && absence.type === AbsenceType.Unexcused && absence.hasUndoAccess,
      canExcuse: bookAllowsAbsenceModifications && absence.type === AbsenceType.Unexcused && absence.hasExcuseAccess,
      canUndo: bookAllowsAbsenceModifications && absence.hasUndoAccess,
      canRemove: bookAllowsAbsenceModifications && absence.hasRemoveAccess
    };
    this.selectedAbsenceUndoExpired$ = expiredAt(add(absence.createDate, { minutes: UNDO_INTERVAL_IN_MINUTES }));
  }

  convertToLateAbsence(absenceId: number) {
    this.convertingAbsence = true;

    const params = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      absenceId: absenceId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да конвертирате отсъствието в закъснение?',
        errorsMessage: 'Не може да конвертирате отсъствието в закъснение, защото:',
        httpAction: () => this.absencesService.convertToLateAbsence(params).toPromise()
      })
      .then((done) => {
        if (done) {
          return this.reloadAbsences();
        }

        return Promise.resolve();
      })
      .finally(() => {
        this.convertingAbsence = false;
      });
  }

  openExcuseAbsenceDialog(absenceId: number) {
    this.excusingAbsence = true;

    openTypedDialog(this.dialog, AbsenceExcuseDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        absenceId: absenceId
      }
    })
      .afterClosed()
      .toPromise()
      .then((ok) => {
        if (ok) {
          return this.reloadAbsences();
        }

        return Promise.resolve();
      })
      .finally(() => {
        this.excusingAbsence = false;
      });
  }

  removeAbsence(absenceId: number) {
    this.removingAbsence = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      absenceId: absenceId
    };

    const absence = this.data.absences.find((a) => a.absenceId === absenceId) ?? throwError();

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
          return this.reloadAbsences();
        }

        return Promise.resolve();
      })
      .finally(() => {
        this.removingAbsence = false;
      });
  }

  reloadGrades() {
    return this.myHourService
      .getTeacherLessonGrades({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        scheduleLessonId: this.data.scheduleLessonId
      })
      .toPromise()
      .then((grades) => {
        this.selectedGradeStudentPersonId = undefined;
        this.selectedGradeId = undefined;
        this.selectedGrade = undefined;
        this.selectedGradeUndoExpired$ = undefined;

        this.data.grades = grades;
        this.templateData = getTemplateData(
          this.data,
          this.schoolTerm,
          this.localStorageService.getBookTransferredHidden(),
          this.localStorageService.getBookNotEnrolledHidden(),
          this.localStorageService.getBookGradelessHidden()
        );
      })
      .catch((err) => GlobalErrorHandler.instance.handleError(err));
  }

  selectStudentGrade(studentPersonId: number, gradeId: number) {
    this.selectedAbsenceStudentPersonId = undefined;
    this.selectedAbsenceId = undefined;
    this.selectedAbsence = undefined;
    this.selectedAbsenceUndoExpired$ = undefined;

    this.selectedGrade = undefined;
    this.selectedGradeUndoExpired$ = undefined;

    if (this.selectedGradeStudentPersonId === studentPersonId && this.selectedGradeId === gradeId) {
      this.selectedGradeStudentPersonId = undefined;
      this.selectedGradeId = undefined;

      return;
    }

    this.selectedGradeStudentPersonId = studentPersonId;
    this.selectedGradeId = gradeId;
    this.loadingGrade = true;

    return this.gradesService
      .get({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        gradeId
      })
      .toPromise()
      .then((grade) => {
        const bookAllowsGradeModifications = this.data.classBookInfo.checkBookAllowsGradeModifications(
          grade.type,
          grade.date
        );
        this.selectedGrade = {
          ...grade,
          canUndo: bookAllowsGradeModifications && grade.hasUndoAccess,
          canRemove: bookAllowsGradeModifications && grade.hasRemoveAccess
        };
        this.selectedGradeUndoExpired$ = expiredAt(add(grade.createDate, { minutes: UNDO_INTERVAL_IN_MINUTES }));
      })
      .catch((err) => {
        this.selectedGradeStudentPersonId = undefined;
        this.selectedGradeId = undefined;

        GlobalErrorHandler.instance.handleError(err);
      })
      .finally(() => {
        this.loadingGrade = false;
      });
  }

  removeSelectedGrade() {
    assert(this.selectedGradeId != null);

    this.removingGrade = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      gradeId: this.selectedGradeId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете оценката?',
        errorsMessage: 'Не може да изтриете оценката, защото:',
        httpAction: () => this.gradesService.remove(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          return this.reloadGrades();
        }

        return Promise.resolve();
      })
      .finally(() => {
        this.removingGrade = false;
      });
  }
}

function getTemplateData(
  data: MyHourLessonViewContentComponent['data'],
  schoolTerm: SchoolTerm,
  bookTransferredHidden: boolean,
  bookNotEnrolledHidden: boolean,
  bookGradelessHidden: boolean
) {
  const bookAllowsAttendanceAbsenceTopicModifications =
    data.classBookInfo.checkBookAllowsAttendanceAbsenceTopicModifications(data.date);

  const absencesMap = new Map(data.absences.map((a) => [a.personId, a]));
  const gradesMap = new Map(groupBy(data.grades, (g) => g.personId));

  return {
    topicCanRemove:
      data.teacherLesson.topicId != null &&
      bookAllowsAttendanceAbsenceTopicModifications &&
      data.teacherLesson.topicHasRemoveAccess &&
      !data.teacherLesson.isVerified,
    topicCanUndo:
      data.teacherLesson.topicId != null &&
      bookAllowsAttendanceAbsenceTopicModifications &&
      data.teacherLesson.topicHasUndoAccess &&
      !data.teacherLesson.isVerified,
    topicUndoExpired$:
      data.teacherLesson.topicId != null && bookAllowsAttendanceAbsenceTopicModifications
        ? expiredAt(add(data.teacherLesson.topicCreateDate!, { minutes: UNDO_INTERVAL_IN_MINUTES }))
        : of(true),
    students: data.teacherLesson.students
      .map((s) => ({
        ...s,
        isRemoved: false
      }))
      .concat(
        data.removedStudents.map((s) => ({
          ...s,
          isRemoved: true,
          isTransferred: false,
          notEnrolledInCurriculum: false,
          withoutFirstTermGrade: false,
          withoutSecondTermGrade: false,
          hasSpecialNeeds: false
        }))
      )
      .map((s) => {
        const absence = absencesMap.get(s.personId);
        const grades = gradesMap.get(s.personId) ?? [];

        const isGradeless =
          (schoolTerm === SchoolTerm.TermOne && s.withoutFirstTermGrade) ||
          (schoolTerm === SchoolTerm.TermTwo && s.withoutSecondTermGrade) ||
          (s.withoutFirstTermGrade && s.withoutSecondTermGrade);

        return {
          ...s,

          isGradeless,
          abnormalStatus: s.isRemoved
            ? 'ИЗТРИТ'
            : s.isTransferred
            ? 'ОТПИСАН'
            : s.notEnrolledInCurriculum
            ? 'НЕ ИЗУЧАВА ПРЕДМЕТА'
            : isGradeless
            ? 'ОСВОБОДЕН'
            : '',

          absence,
          grades
        };
      })
      .filter(
        (s) =>
          s.absence ||
          s.grades.length > 0 ||
          ((!bookTransferredHidden || !s.isTransferred) &&
            (!bookNotEnrolledHidden || !s.notEnrolledInCurriculum) &&
            (!bookGradelessHidden || !s.isGradeless))
      )
  };
}
