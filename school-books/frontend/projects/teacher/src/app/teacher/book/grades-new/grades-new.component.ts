import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  UntypedFormArray,
  UntypedFormBuilder,
  UntypedFormGroup,
  ValidationErrors,
  ValidatorFn,
  Validators
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import {
  GradesService,
  Grades_CreateGradeRequestParams,
  Grades_GetCurriculum,
  Grades_GetCurriculumStudents
} from 'projects/sb-api-client/src/api/grades.service';
import { GradeTypeNomsService } from 'projects/sb-api-client/src/api/gradeTypeNoms.service';
import {
  ScheduleLessonNomsService,
  ScheduleLessonNoms_GetNoms
} from 'projects/sb-api-client/src/api/scheduleLessonNoms.service';
import {
  SpecialNeedsGradeNomsService,
  SpecialNeedsGradeNoms_GetNomsByTerm
} from 'projects/sb-api-client/src/api/specialNeedsGradeNoms.service';
import { ClassBookType } from 'projects/sb-api-client/src/model/classBookType';
import { GradeType } from 'projects/sb-api-client/src/model/gradeType';
import { SchoolTerm } from 'projects/sb-api-client/src/model/schoolTerm';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  IShouldPreventLeave,
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { LocalStorageService } from 'projects/shared/services/local-storage.service';
import { assert } from 'projects/shared/utils/assert';
import { ClassBookInfoType, getTermFromDate, gradeTypeRequiresScheduleLesson } from 'projects/shared/utils/book';
import { formatNullableDate } from 'projects/shared/utils/date';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { combineLatest, from, Observable, of, Subject } from 'rxjs';
import { catchError, distinctUntilChanged, map, startWith, switchMap, takeUntil, tap } from 'rxjs/operators';
import { CLASS_BOOK_INFO } from '../book/book.component';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class GradesNewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    gradesService: GradesService,
    specialNeedsGradeNomsService: SpecialNeedsGradeNomsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const curriculumId = tryParseInt(route.snapshot.paramMap.get('curriculumId')) ?? throwParamError('curriculumId');

    this.resolve(GradesNewComponent, {
      schoolYear,
      instId,
      classBookId,
      curriculumId,
      classBookInfo: from(classBookInfo),
      students: gradesService.getCurriculumStudents({
        schoolYear,
        instId,
        classBookId,
        curriculumId
      }),
      curriculum: gradesService.getCurriculum({
        schoolYear,
        instId,
        classBookId,
        curriculumId
      }),
      specialNeedsGradeNoms: specialNeedsGradeNomsService.getNomsByTerm({
        schoolYear: schoolYear,
        instId: instId,
        term: ''
      })
    });
  }
}

type ScheduleLessonNomVO = ArrayElementType<ScheduleLessonNoms_GetNoms>;

@Component({
  selector: 'sb-grades-new',
  templateUrl: './grades-new.component.html',
  styleUrls: ['./grades-new.component.scss']
})
export class GradesNewComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    curriculumId: number;
    classBookInfo: ClassBookInfoType;
    students: Grades_GetCurriculumStudents;
    curriculum: Grades_GetCurriculum;
    specialNeedsGradeNoms: SpecialNeedsGradeNoms_GetNomsByTerm;
  };

  readonly destroyed$ = new Subject<void>();

  readonly SchoolTerm_TermOne = SchoolTerm.TermOne;
  readonly SchoolTerm_TermTwo = SchoolTerm.TermTwo;

  form!: UntypedFormGroup;
  showIndividualCurriculumStudentBanner = false;

  dateWithinSchoolYearAndTerm(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const date = control.value as Date;

      if (date == null) {
        return null;
      } else if (date < this.data.classBookInfo.schoolYearStartDateLimit) {
        return { beforeSchoolYearStartDate: true };
      } else if (date > this.data.classBookInfo.schoolYearEndDateLimit) {
        return { afterSchoolYearEndDate: true };
      } else if (
        date > this.data.classBookInfo.firstTermEndDate &&
        date < this.data.classBookInfo.secondTermStartDate
      ) {
        return { betweenTerms: true };
      }

      return null;
    };
  }

  userHasGradeTypeAccess(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const type = control.value as GradeType;

      if (!gradeTypeRequiresScheduleLesson(type) && !this.data.curriculum.hasCreateGradeWithoutScheduleLessonAccess) {
        return { cannotCreateGradeWithoutScheduleLesson: true };
      }

      return null;
    };
  }

  bookLocked(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const { type, date, scheduleLesson } = control.value as {
        type: GradeType | null;
        date: Date | null;
        scheduleLesson: ScheduleLessonNomVO | null;
      };

      if (type && date && gradeTypeRequiresScheduleLesson(type)) {
        if (this.data.classBookInfo.checkBookHasFutureEntryLock(date)) {
          return { bookLocked: 'Въвеждане на оценки за бъдещи дати е забранено' };
        } else if (this.data.classBookInfo.checkBookHasPastMonthLock(date)) {
          return {
            bookLocked: `Въвеждане на оценки преди ${formatNullableDate(
              this.data.classBookInfo.firstEditableMonthStartDate
            )} е забранено`
          };
        }
      }

      if (scheduleLesson?.isVerified) {
        return { bookLocked: 'Въвеждане на оценки в занятия проверени от директора е забранено' };
      }

      return null;
    };
  }

  hasComments = false;
  hasQualitativeGrades!: boolean;

  gradeTypeNomsService: NomServiceWithParams<GradeType, { instId: number; schoolYear: number }>;

  scheduleLessons: { id: ScheduleLessonNomVO; name: string }[] = [];

  students!: ReturnType<typeof mapStudents>;

  constructor(
    public localStorageService: LocalStorageService,
    private fb: UntypedFormBuilder,
    private gradesService: GradesService,
    private scheduleLessonNomsService: ScheduleLessonNomsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    gradeTypeNomsService: GradeTypeNomsService
  ) {
    this.gradeTypeNomsService = new NomServiceWithParams(gradeTypeNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      showFinalGradeType: this.data.classBookInfo.basicClassId !== 1,
      showTermGradeType: this.data.classBookInfo.bookType !== ClassBookType.Book_I_III,
      showGradeTypesWithoutScheduleLesson: true
    }));
  }

  ngOnInit() {
    this.hasQualitativeGrades = this.data.classBookInfo.bookType === ClassBookType.Book_I_III;

    const initialType = null;
    const initialDate = new Date();
    const initialTerm = getTermFromDate(this.data.classBookInfo, initialDate);
    const initialScheduleLesson = null;

    this.form = this.fb.group(
      {
        type: [initialType, [Validators.required, this.userHasGradeTypeAccess()]],
        date: [initialDate, [Validators.required, this.dateWithinSchoolYearAndTerm()]],
        term: [initialTerm, Validators.required],
        scheduleLesson: [initialScheduleLesson, Validators.required],
        studentGrades: this.fb.array([])
      },
      { validators: [this.bookLocked()] }
    );

    for (const student of this.data.students) {
      (this.form.get('studentGrades') as UntypedFormArray).push(
        this.fb.group({
          personId: [student.personId],
          decimalGrade: [null],
          qualitativeGrade: [null],
          specialGrade: [null],
          comment: [null]
        })
      );
    }

    // show comments warning if any of the grades has comments
    this.form.valueChanges
      .pipe(
        tap(() => {
          this.hasComments = this.form.get('studentGrades')?.value.filter((s: any) => s.comment).length > 0;
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();

    // update schedule lessons and term dropdowns on date and type changes
    combineLatest([
      (this.form.get('date')?.valueChanges as Observable<Date | null>).pipe(
        startWith(initialDate) // force prettier new line
      ),
      (this.form.get('type')?.valueChanges as Observable<GradeType | null>).pipe(
        startWith(initialType),
        map((type) => type == null || gradeTypeRequiresScheduleLesson(type)),
        distinctUntilChanged()
      )
    ])
      .pipe(
        // always sync the term control with the date
        // as we are using it even when not visible
        tap(([date]) => {
          const term = date && getTermFromDate(this.data.classBookInfo, date);
          if (term) {
            this.form.get('term')?.setValue(term);
          } else {
            this.form.get('term')?.reset();
          }
        }),
        // sync scheduleLesson validity on type changes
        tap(([, gradeTypeIsNullOrRequiresScheduleLesson]) => {
          if (gradeTypeIsNullOrRequiresScheduleLesson) {
            this.form.get('scheduleLesson')?.enable();
          } else {
            this.form.get('scheduleLesson')?.disable();
          }
        }),
        switchMap(([date, gradeTypeIsNullOrRequiresScheduleLesson]): Observable<ScheduleLessonNoms_GetNoms> => {
          if (date != null && gradeTypeIsNullOrRequiresScheduleLesson) {
            return this.scheduleLessonNomsService
              .getNoms({
                instId: this.data.instId,
                schoolYear: this.data.schoolYear,
                classBookId: this.data.classBookId,
                curriculumId: this.data.curriculumId,
                date
              })
              .pipe(
                catchError((err) => {
                  GlobalErrorHandler.instance.handleError(err);
                  return of([]);
                })
              );
          } else {
            return of([]);
          }
        }),
        tap((scheduleLessons) => {
          this.scheduleLessons = scheduleLessons.map((sl) => ({ id: sl, name: sl.name }));

          // if there is only one scheduleLesson select it
          if (this.scheduleLessons.length === 1) {
            this.form.get('scheduleLesson')?.setValue(this.scheduleLessons[0].id);
          } else {
            this.form.get('scheduleLesson')?.reset();
          }
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();

    // update visible students
    combineLatest([
      (this.form.get('term')?.valueChanges as Observable<SchoolTerm | null>).pipe(
        startWith(initialTerm) // force prettier new line
      ),
      (this.form.get('scheduleLesson')?.valueChanges as Observable<ScheduleLessonNomVO | null>).pipe(
        startWith(initialScheduleLesson),
        map((sl) => sl?.individualCurriculumPersonId ?? null),
        distinctUntilChanged()
      ),
      this.localStorageService.bookTransferredHidden$,
      this.localStorageService.bookNotEnrolledHidden$,
      this.localStorageService.bookGradelessHidden$
    ])
      .pipe(
        tap(
          ([
            schoolTerm,
            individualCurriculumPersonId,
            bookTransferredHidden,
            bookNotEnrolledHidden,
            bookGradelessHidden
          ]) => {
            this.students = mapStudents(
              this.data.students,
              this.studentGradesFormArray(),
              schoolTerm,
              individualCurriculumPersonId,
              bookTransferredHidden,
              bookNotEnrolledHidden,
              bookGradelessHidden
            );

            this.showIndividualCurriculumStudentBanner = individualCurriculumPersonId != null;

            this.data.students.forEach((student, i) => {
              const mappedStudent = this.students.find((s) => s.personId === student.personId);

              assert(
                mappedStudent == null || mappedStudent.studentFormIndex === i,
                'Mapped student formIndex not in sync'
              );

              if (mappedStudent) {
                this.studentForm(i).enable();
              } else {
                this.studentForm(i).disable();
              }
            });
          }
        ),
        takeUntil(this.destroyed$)
      )
      .subscribe();
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  get gradeTypeIsNullOrRequiresScheduleLesson() {
    const selectedGradeType = <GradeType | null>this.form.get('type')?.value;

    return selectedGradeType == null || gradeTypeRequiresScheduleLesson(selectedGradeType);
  }

  get gradeTypeIsFinal() {
    const selectedGradeType = <GradeType | null>this.form.get('type')?.value;

    return selectedGradeType != null && selectedGradeType === GradeType.Final;
  }

  studentGradesFormArray() {
    return this.form.get('studentGrades') as UntypedFormArray;
  }

  studentForm(i: number) {
    return this.studentGradesFormArray().at(i) as UntypedFormGroup;
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave(save: SaveToken) {
    const value = this.form.value;
    const curriculumId = this.data.curriculumId;
    const grade = {
      type: <GradeType>value.type,
      date: <Date>value.date,
      term: <SchoolTerm>value.term,
      scheduleLessonId: (<ScheduleLessonNomVO | null>value.scheduleLesson)?.scheduleLessonId,
      teacherAbsenceId: (<ScheduleLessonNomVO | null>value.scheduleLesson)?.teacherAbsenceId,
      students:
        (<Grades_CreateGradeRequestParams['createGradeCommand']['students']>value.studentGrades)?.filter(
          (s) => s.decimalGrade != null || s.qualitativeGrade != null || s.specialGrade != null
        ) ?? []
    };

    this.actionService
      .execute({
        httpAction: () =>
          this.gradesService
            .createGrade({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              curriculumId,
              createGradeCommand: grade
            })
            .toPromise()
            .then(() => {
              this.form.markAsPristine();
              this.router.navigate(['../'], { relativeTo: this.route });
            })
      })
      .then((success) => save.done(success));
  }
}

function mapStudents(
  students: GradesNewComponent['data']['students'],
  studentGradesFormArray: UntypedFormArray,
  schoolTerm: SchoolTerm | null,
  individualCurriculumPersonId: number | null,
  bookTransferredHidden: boolean,
  bookNotEnrolledHidden: boolean,
  bookGradelessHidden: boolean
) {
  return students
    .map((s, i) => {
      const studentFormValue = studentGradesFormArray.at(i).value;
      const hasSelectedGrade =
        studentFormValue.decimalGrade != null ||
        studentFormValue.qualitativeGrade != null ||
        studentFormValue.specialGrade != null ||
        studentFormValue.comment != null;

      const isGradeless =
        (schoolTerm === SchoolTerm.TermOne && s.withoutFirstTermGrade) ||
        (schoolTerm === SchoolTerm.TermTwo && s.withoutSecondTermGrade) ||
        (s.withoutFirstTermGrade && s.withoutSecondTermGrade);

      return {
        ...s,

        hasSelectedGrade,
        studentFormIndex: i,
        isGradeless,
        showInfoLink: !s.isTransferred,
        abnormalStatus: s.isTransferred
          ? 'ОТПИСАН'
          : s.notEnrolledInCurriculum
          ? 'НЕ ИЗУЧАВА ПРЕДМЕТА'
          : isGradeless
          ? 'ОСВОБОДЕН'
          : ''
      };
    })
    .filter(
      (s) =>
        (individualCurriculumPersonId == null || s.personId === individualCurriculumPersonId) &&
        (s.hasSelectedGrade ||
          ((!bookTransferredHidden || !s.isTransferred) &&
            (!bookNotEnrolledHidden || !s.notEnrolledInCurriculum) &&
            (!bookGradelessHidden || !s.isGradeless)))
    );
}
