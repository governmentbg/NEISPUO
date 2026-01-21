import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { faBookOpen as fasBookOpen } from '@fortawesome/pro-solid-svg-icons/faBookOpen';
import { faCalendarAlt as fasCalendarAlt } from '@fortawesome/pro-solid-svg-icons/faCalendarAlt';
import { faEye as fasEye } from '@fortawesome/pro-solid-svg-icons/faEye';
import { faPencilAlt as fasPencilAlt } from '@fortawesome/pro-solid-svg-icons/faPencilAlt';
import { faPlusSquare as fasPlusSquare } from '@fortawesome/pro-solid-svg-icons/faPlusSquare';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { add, isBefore } from 'date-fns';
import {
  ClassBooksService,
  ClassBooks_GetCurriculums,
  ClassBooks_GetRemovedStudents
} from 'projects/sb-api-client/src/api/classBooks.service';
import {
  GradesService,
  Grades_Get,
  Grades_GetCurriculumGrades,
  Grades_GetCurriculumStudents,
  Grades_GetProfilingSubjectForecastGrades
} from 'projects/sb-api-client/src/api/grades.service';
import { ClassBookType } from 'projects/sb-api-client/src/model/classBookType';
import { GradeCategory } from 'projects/sb-api-client/src/model/gradeCategory';
import { GradeType } from 'projects/sb-api-client/src/model/gradeType';
import { QualitativeGrade } from 'projects/sb-api-client/src/model/qualitativeGrade';
import { SchoolTerm } from 'projects/sb-api-client/src/model/schoolTerm';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { BreakpointService } from 'projects/shared/services/breakpoint.service';
import { LocalStorageService } from 'projects/shared/services/local-storage.service';
import { groupBy } from 'projects/shared/utils/array';
import { assert } from 'projects/shared/utils/assert';
import {
  calculateAverageDecimalGrade,
  calculateAverageQualitativeGrade,
  ClassBookInfoType,
  resolveWithRemovedStudents,
  UNDO_INTERVAL_IN_MINUTES
} from 'projects/shared/utils/book';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { expiredAt } from 'projects/shared/utils/rxjs';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { combineLatest, from, Observable, Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';
import { CLASS_BOOK_INFO } from '../book/book.component';
import { GradesViewForecastGradeDialogComponent } from '../grades-view-forecast-grade-dialog/grades-view-forecast-grade-dialog';
import { CLASS_BOOK_GRADES_CURRICULUMS } from '../grades/grades.component';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class GradesViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    @Inject(CLASS_BOOK_GRADES_CURRICULUMS) curriculums: Promise<ClassBooks_GetCurriculums>,
    gradesService: GradesService,
    classBooksService: ClassBooksService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const curriculumId = tryParseInt(route.snapshot.paramMap.get('curriculumId')) ?? throwParamError('curriculumId');

    const curriculum = curriculums.then(
      (cs) => cs.find((c) => c.curriculumId === curriculumId) ?? throwError('curriculum not found')
    );

    this.resolve(
      GradesViewComponent,
      resolveWithRemovedStudents(
        classBooksService,
        (data) => data.students,
        (data) => data.grades,
        {
          schoolYear,
          instId,
          classBookId,
          curriculumId,
          classBookInfo: from(classBookInfo),
          curriculum: from(curriculum),
          students: gradesService.getCurriculumStudents({
            schoolYear,
            instId,
            classBookId,
            curriculumId
          }),
          grades: gradesService.getCurriculumGrades({
            schoolYear,
            instId,
            classBookId,
            curriculumId
          }),
          profilingSubjectForecastGrades: from(
            curriculum.then((c) =>
              c.subjectTypeIsProfilingSubject
                ? gradesService
                    .getProfilingSubjectForecastGrades({
                      schoolYear,
                      instId,
                      classBookId,
                      curriculumId
                    })
                    .toPromise()
                : null
            )
          )
        }
      )
    );
  }
}

type ForecastGrade = {
  category: GradeCategory;
  type: GradeType;
  term: SchoolTerm;
  decimalGrade?: number | null;
  qualitativeGrade?: QualitativeGrade | null;
};

@Component({
  selector: 'sb-grades-view',
  templateUrl: './grades-view.component.html',
  styleUrls: ['./grades-view.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class GradesViewComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    curriculumId: number;
    classBookInfo: ClassBookInfoType;
    curriculum: ArrayElementType<ClassBooks_GetCurriculums>;
    students: Grades_GetCurriculumStudents;
    grades: Grades_GetCurriculumGrades;
    profilingSubjectForecastGrades: Grades_GetProfilingSubjectForecastGrades | null;
    removedStudents: ClassBooks_GetRemovedStudents;
  };

  readonly destroyed$ = new Subject<void>();
  readonly fasBookOpen = fasBookOpen;
  readonly fasCalendarAlt = fasCalendarAlt;
  readonly fasPencilAlt = fasPencilAlt;
  readonly fasPlusSquare = fasPlusSquare;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasEye = fasEye;
  readonly GRADE_CATEGORY_DECIMAL = GradeCategory.Decimal;
  readonly GRADE_CATEGORY_QUALITATIVE = GradeCategory.Qualitative;
  readonly GRADE_CATEGORY_SPECIAL_NEEDS = GradeCategory.SpecialNeeds;

  canCreate!: boolean;
  canCreateForecastGrade!: boolean;
  firstTermExpanded!: boolean;
  secondTermExpanded!: boolean;
  showFinalGrades!: boolean;

  studentsWithGrades!: ReturnType<typeof getStudentsWithGrades>;
  showTransferredStudentsBanner = false;

  selectedStudentPersonId?: number;
  selectedGradeId?: number;
  selectedGrade?: Grades_Get & { canUndo: boolean; canRemove: boolean };
  undoExpired$?: Observable<boolean>;
  loadingGrade = false;
  removingGrade = false;

  constructor(
    public breakpointService: BreakpointService,
    private cd: ChangeDetectorRef,
    private gradesService: GradesService,
    private actionService: ActionService,
    private dialog: MatDialog,
    private localStorageService: LocalStorageService
  ) {
    breakpointService.change$
      .pipe(
        tap(() => {
          this.cd.markForCheck();
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
  }

  ngOnInit() {
    this.showTransferredStudentsBanner = this.data.students.find((s) => s.isTransferred) != null;
    combineLatest([
      this.localStorageService.bookTransferredHidden$,
      this.localStorageService.bookNotEnrolledHidden$,
      this.localStorageService.bookGradelessHidden$
    ])
      .pipe(
        tap(([bookTransferredHidden, bookNotEnrolledHidden, bookGradelessHidden]) => {
          this.studentsWithGrades = getStudentsWithGrades(
            this.data,
            bookTransferredHidden,
            bookNotEnrolledHidden,
            bookGradelessHidden
          );
          this.cd.markForCheck();
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();

    this.canCreate = this.data.classBookInfo.bookAllowsModifications && this.data.curriculum.hasCreateGradeAccess;
    this.canCreateForecastGrade =
      this.data.classBookInfo.bookAllowsModifications && this.data.curriculum.hasCreateForecastGradeAccess;
    this.showFinalGrades = this.data.classBookInfo.basicClassId !== 1;

    const now = new Date();
    const isFirstTerm = isBefore(now, this.data.classBookInfo.secondTermStartDate);

    this.firstTermExpanded = isFirstTerm;
    this.secondTermExpanded = !isFirstTerm;
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  reloadGrades() {
    return this.gradesService
      .getCurriculumGrades({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        curriculumId: this.data.curriculumId
      })
      .toPromise()
      .then((grades) => {
        this.selectedStudentPersonId = undefined;
        this.selectedGradeId = undefined;
        this.selectedGrade = undefined;
        this.undoExpired$ = undefined;

        this.data.grades = grades;
        this.studentsWithGrades = getStudentsWithGrades(
          this.data,
          this.localStorageService.getBookTransferredHidden(),
          this.localStorageService.getBookNotEnrolledHidden(),
          this.localStorageService.getBookGradelessHidden()
        );
      })
      .catch((err) => GlobalErrorHandler.instance.handleError(err));
  }

  expandFirstTerm() {
    this.firstTermExpanded = true;
    this.secondTermExpanded = false;
    this.selectedStudentPersonId = undefined;
    this.selectedGradeId = undefined;
    this.selectedGrade = undefined;
    this.undoExpired$ = undefined;
  }

  expandSecondTerm() {
    this.firstTermExpanded = false;
    this.secondTermExpanded = true;
    this.selectedStudentPersonId = undefined;
    this.selectedGradeId = undefined;
    this.selectedGrade = undefined;
    this.undoExpired$ = undefined;
  }

  selectStudentGrade(studentPersonId: number, gradeId: number) {
    this.selectedGrade = undefined;
    this.undoExpired$ = undefined;

    if (this.selectedStudentPersonId === studentPersonId && this.selectedGradeId === gradeId) {
      this.selectedStudentPersonId = undefined;
      this.selectedGradeId = undefined;

      return;
    }

    this.selectedStudentPersonId = studentPersonId;
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
        this.undoExpired$ = expiredAt(add(grade.createDate, { minutes: UNDO_INTERVAL_IN_MINUTES }));
      })
      .catch((err) => {
        this.selectedStudentPersonId = undefined;
        this.selectedGradeId = undefined;

        GlobalErrorHandler.instance.handleError(err);
      })
      .finally(() => {
        this.loadingGrade = false;
        this.cd.markForCheck();
      });
  }

  addForecastGrade(studentPersonId: number, grade: ForecastGrade) {
    openTypedDialog(this.dialog, GradesViewForecastGradeDialogComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        curriculumId: this.data.curriculumId,
        personId: studentPersonId,
        subjectTypeIsProfilingSubject: this.data.curriculum.subjectTypeIsProfilingSubject,
        ...grade
      }
    })
      .afterClosed()
      .toPromise()
      .then((ok) => {
        if (ok) {
          return this.reloadGrades().then(() => this.cd.markForCheck());
        }

        return Promise.resolve();
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
        this.cd.markForCheck();
      });
  }
}

function getStudentsWithGrades(
  data: GradesViewComponent['data'],
  bookTransferredHidden: boolean,
  bookNotEnrolledHidden: boolean,
  bookGradelessHidden: boolean
) {
  const subjectTypeIsProfilingSubject = data.curriculum.subjectTypeIsProfilingSubject;

  const oct1st = new Date(data.schoolYear, 9, 1);
  const nov1st = new Date(data.schoolYear, 10, 1);
  const dec1st = new Date(data.schoolYear, 11, 1);
  const jan1st = new Date(data.schoolYear + 1, 0, 1);
  const mar1st = new Date(data.schoolYear + 1, 2, 1);
  const apr1st = new Date(data.schoolYear + 1, 3, 1);
  const may1st = new Date(data.schoolYear + 1, 4, 1);
  const jun1st = new Date(data.schoolYear + 1, 5, 1);

  const gradesByStudent = new Map(groupBy(data.grades, (g) => g.personId));
  const profilingSubjectForecastGradesByStudent = new Map(
    data.profilingSubjectForecastGrades?.map((g) => [g.personId, g.profilingSubjectFinalGrade]) ?? []
  );

  return data.students
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
        withoutFinalGrade: false,
        hasSpecialNeeds: false
      }))
    )
    .map((s) => {
      const grades = gradesByStudent.get(s.personId) ?? [];
      const firstTermRegularGrades = grades.filter(
        (g) =>
          g.term === SchoolTerm.TermOne &&
          g.type !== GradeType.Term &&
          g.type !== GradeType.Final &&
          g.type !== GradeType.OtherClassTerm &&
          g.type !== GradeType.OtherSchoolTerm
      );
      const secondTermRegularGrades = grades.filter(
        (g) =>
          g.term === SchoolTerm.TermTwo &&
          g.type !== GradeType.Term &&
          g.type !== GradeType.Final &&
          g.type !== GradeType.OtherClassTerm &&
          g.type !== GradeType.OtherSchoolTerm
      );

      const studentWithGrades = {
        ...s,

        isFirstTermGradeless: s.isTransferred || s.notEnrolledInCurriculum || s.withoutFirstTermGrade,
        isSecondTermGradeless: s.isTransferred || s.notEnrolledInCurriculum || s.withoutSecondTermGrade,
        isFinalGradeless: s.isTransferred || s.notEnrolledInCurriculum || s.withoutFinalGrade,
        showInfoLink: !s.isTransferred && !s.isRemoved,
        abnormalStatus: s.isRemoved
          ? 'ИЗТРИТ'
          : s.isTransferred
          ? 'ОТПИСАН'
          : s.notEnrolledInCurriculum
          ? 'НЕ ИЗУЧАВА ПРЕДМЕТА'
          : s.withoutFirstTermGrade || s.withoutSecondTermGrade || s.withoutFinalGrade
          ? 'ОСВОБОДЕН'
          : '',

        gradesSep: firstTermRegularGrades.filter((g) => g.date < oct1st),
        gradesOct: firstTermRegularGrades.filter((g) => oct1st <= g.date && g.date < nov1st),
        gradesNov: firstTermRegularGrades.filter((g) => nov1st <= g.date && g.date < dec1st),
        gradesDec: firstTermRegularGrades.filter((g) => dec1st <= g.date && g.date < jan1st),
        gradesJan: firstTermRegularGrades.filter((g) => g.date >= jan1st),

        firstTermTermGrades: grades.filter(
          (g) =>
            g.term === SchoolTerm.TermOne &&
            (g.type === GradeType.Term || g.type === GradeType.OtherClassTerm || g.type === GradeType.OtherSchoolTerm)
        ),

        gradesFeb: secondTermRegularGrades.filter((g) => g.date < mar1st),
        gradesMar: secondTermRegularGrades.filter((g) => mar1st <= g.date && g.date < apr1st),
        gradesApr: secondTermRegularGrades.filter((g) => apr1st <= g.date && g.date < may1st),
        gradesMay: secondTermRegularGrades.filter((g) => may1st <= g.date && g.date < jun1st),
        gradesJun: secondTermRegularGrades.filter((g) => g.date >= jun1st),

        secondTermTermGrades: grades.filter(
          (g) =>
            g.term === SchoolTerm.TermTwo &&
            (g.type === GradeType.Term || g.type === GradeType.OtherClassTerm || g.type === GradeType.OtherSchoolTerm)
        ),

        finalGrades: grades.filter((g) => g.type === GradeType.Final),

        gradeCount: grades.length
      };

      const hasQualitativeGrades = data.classBookInfo.bookType === ClassBookType.Book_I_III;

      let firstTermForecastDecimalGrade: number | null = null;
      let secondTermForecastDecimalGrade: number | null = null;
      let finalForecastDecimalGrade: number | null = null;
      let finalForecastQualitativeGrade: QualitativeGrade | null = null;

      if (!hasQualitativeGrades) {
        firstTermForecastDecimalGrade = calculateAverageDecimalGrade(firstTermRegularGrades.map((g) => g.decimalGrade));

        secondTermForecastDecimalGrade = calculateAverageDecimalGrade(
          secondTermRegularGrades.map((g) => g.decimalGrade)
        );

        finalForecastDecimalGrade = calculateAverageDecimalGrade([
          studentWithGrades.firstTermTermGrades[0]?.decimalGrade ?? firstTermForecastDecimalGrade,
          studentWithGrades.secondTermTermGrades[0]?.decimalGrade ?? secondTermForecastDecimalGrade
        ]);

        const profilingSubjectForecastGrade = profilingSubjectForecastGradesByStudent.get(s.personId);
        if (!finalForecastDecimalGrade && profilingSubjectForecastGrade) {
          finalForecastDecimalGrade = profilingSubjectForecastGrade;
        }
      } else {
        finalForecastQualitativeGrade = calculateAverageQualitativeGrade(
          [...firstTermRegularGrades, ...secondTermRegularGrades].map((g) => g.qualitativeGrade)
        );
      }

      const gradeCategory = !hasQualitativeGrades ? GradeCategory.Decimal : GradeCategory.Qualitative;
      const atLeastOneTermGrade =
        studentWithGrades.firstTermTermGrades.length || studentWithGrades.secondTermTermGrades.length;

      return {
        ...studentWithGrades,
        firstTermForecastGrade:
          !studentWithGrades.firstTermTermGrades.length && firstTermForecastDecimalGrade != null
            ? {
                category: gradeCategory,
                type: GradeType.Term,
                term: SchoolTerm.TermOne,
                decimalGrade: firstTermForecastDecimalGrade
              }
            : null,
        secondTermForecastGrade:
          !studentWithGrades.secondTermTermGrades.length && secondTermForecastDecimalGrade != null
            ? {
                category: gradeCategory,
                type: GradeType.Term,
                term: SchoolTerm.TermTwo,
                decimalGrade: secondTermForecastDecimalGrade
              }
            : null,
        finalForecastGrade:
          !studentWithGrades.finalGrades.length &&
          (((atLeastOneTermGrade || subjectTypeIsProfilingSubject) && finalForecastDecimalGrade != null) ||
            finalForecastQualitativeGrade != null)
            ? {
                category: gradeCategory,
                type: GradeType.Final,
                term: SchoolTerm.TermTwo,
                decimalGrade: finalForecastDecimalGrade,
                qualitativeGrade: finalForecastQualitativeGrade
              }
            : null
      };
    })
    .filter((s) => {
      // Always show students with grades
      if (s.gradeCount > 0) {
        return true;
      }

      // Priority 1: Check transferred students first
      if (s.isTransferred) {
        return !bookTransferredHidden;
      }

      // Priority 2: Check not enrolled students
      if (s.notEnrolledInCurriculum) {
        return !bookNotEnrolledHidden;
      }

      // Priority 3: Check gradeless students
      if (s.withoutFirstTermGrade || s.withoutSecondTermGrade) {
        return !bookGradelessHidden;
      }

      // Default: show the student
      return true;
    });
}
