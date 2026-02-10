import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faChalkboardTeacher as fadChalkboardTeacher } from '@fortawesome/pro-duotone-svg-icons/faChalkboardTeacher';
import { faCog as fadCog } from '@fortawesome/pro-duotone-svg-icons/faCog';
import { isValid, parse } from 'date-fns';
import { ClassBooksService } from 'projects/sb-api-client/src/api/classBooks.service';
import { GradesService } from 'projects/sb-api-client/src/api/grades.service';
import { GradeTypeNomsService } from 'projects/sb-api-client/src/api/gradeTypeNoms.service';
import { MyHourService, MyHour_GetTeacherLesson } from 'projects/sb-api-client/src/api/myHour.service';
import {
  SpecialNeedsGradeNomsService,
  SpecialNeedsGradeNoms_GetNomsByTerm
} from 'projects/sb-api-client/src/api/specialNeedsGradeNoms.service';
import { ClassBookType } from 'projects/sb-api-client/src/model/classBookType';
import { CreateGradeCommand } from 'projects/sb-api-client/src/model/createGradeCommand';
import { GradeType } from 'projects/sb-api-client/src/model/gradeType';
import { QualitativeGrade } from 'projects/sb-api-client/src/model/qualitativeGrade';
import { SchoolTerm } from 'projects/sb-api-client/src/model/schoolTerm';
import { SpecialNeedsGrade } from 'projects/sb-api-client/src/model/specialNeedsGrade';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  IShouldPreventLeave,
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { LocalStorageService } from 'projects/shared/services/local-storage.service';
import { assert } from 'projects/shared/utils/assert';
import { ClassBookInfoType, extendClassBookInfo, getTermFromDate } from 'projects/shared/utils/book';
import { formatDate } from 'projects/shared/utils/date';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { combineLatest, Subject } from 'rxjs';
import { map, takeUntil, tap } from 'rxjs/operators';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class AddGradesSkeletonComponent extends SkeletonComponentBase {
  constructor(
    route: ActivatedRoute,
    myHourService: MyHourService,
    classBooksService: ClassBooksService,
    specialNeedsGradeNomsService: SpecialNeedsGradeNomsService
  ) {
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

    this.resolve(AddGradesComponent, {
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
      specialNeedsGradeNoms: specialNeedsGradeNomsService.getNomsByTerm({
        schoolYear: schoolYear,
        instId: instId,
        term: ''
      })
    });
  }
}

type StudentGradeForm = {
  personId: FormControl<number>;
  decimalGrade: FormControl<number | null>;
  qualitativeGrade: FormControl<QualitativeGrade | null>;
  specialGrade: FormControl<SpecialNeedsGrade | null>;
  comment: FormControl<string | null>;
};

@Component({
  selector: 'sb-add-grades',
  templateUrl: './add-grades.component.html',
  styleUrls: ['./add-grades.component.scss']
})
export class AddGradesComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    date: Date;
    scheduleLessonId: number;
    classBookInfo: ClassBookInfoType;
    teacherLesson: MyHour_GetTeacherLesson;
    specialNeedsGradeNoms: SpecialNeedsGradeNoms_GetNomsByTerm;
  };

  readonly fadChalkboardTeacher = fadChalkboardTeacher;
  readonly fadCog = fadCog;

  readonly destroyed$ = new Subject<void>();

  saving = false;
  hasComments = false;
  gradeTypeNomsService: NomServiceWithParams<GradeType, { instId: number; schoolYear: number }>;
  templateData!: ReturnType<typeof getTemplateData>;
  form!: FormGroup<{
    type: FormControl<GradeType | null>;
    students: FormArray<FormGroup<StudentGradeForm>>;
  }>;
  hourDescriptionShort = '';
  hourDescription = '';

  constructor(
    public errorStateMatcher: ErrorStateMatcher,
    public localStorageService: LocalStorageService,
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private gradesService: GradesService,
    gradeTypeNomsService: GradeTypeNomsService
  ) {
    this.gradeTypeNomsService = new NomServiceWithParams(gradeTypeNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      showFinalGradeType: false,
      showTermGradeType: false,
      showGradeTypesWithoutScheduleLesson: false
    }));
  }

  ngOnInit() {
    this.hourDescriptionShort = `${formatDate(this.data.date)} час #${this.data.teacherLesson.hourNumber}`;
    this.hourDescription = `${this.hourDescriptionShort} (${this.data.teacherLesson.startTime} - ${this.data.teacherLesson.endTime})`;

    this.form = this.fb.group({
      type: this.fb.control<GradeType | null>(null, {
        validators: [Validators.required]
      }),
      students: this.fb.nonNullable.array<FormGroup<StudentGradeForm>>([])
    });

    for (const student of this.data.teacherLesson.students) {
      this.studentsFormArray().push(
        this.fb.group<StudentGradeForm>({
          personId: this.fb.nonNullable.control(student.personId),
          decimalGrade: this.fb.control<number | null>(null),
          qualitativeGrade: this.fb.control<QualitativeGrade | null>(null),
          specialGrade: this.fb.control<SpecialNeedsGrade | null>(null),
          comment: this.fb.control<string | null>(null)
        })
      );
    }

    // update visible students
    combineLatest([
      this.localStorageService.bookTransferredHidden$,
      this.localStorageService.bookNotEnrolledHidden$,
      this.localStorageService.bookGradelessHidden$
    ])
      .pipe(
        tap(([bookTransferredHidden, bookNotEnrolledHidden, bookGradelessHidden]) => {
          this.templateData = getTemplateData(
            this.data,
            this.studentsFormArray(),
            bookTransferredHidden,
            bookNotEnrolledHidden,
            bookGradelessHidden
          );

          this.data.teacherLesson.students.forEach((student, i) => {
            const mappedStudent = this.templateData.students.find((s) => s.personId === student.personId);

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
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();

    // show comments warning if any of the grades has comments
    this.form.valueChanges
      .pipe(
        tap(() => {
          this.hasComments = this.studentsFormArray().value.filter((s: any) => s.comment).length > 0;
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  studentsFormArray() {
    return this.form.get('students') as FormArray<FormGroup<StudentGradeForm>>;
  }

  studentForm(i: number) {
    return this.studentsFormArray().at(i) as FormGroup<StudentGradeForm>;
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave(save: SaveToken) {
    const createGradeCommand: CreateGradeCommand = {
      date: this.data.date,
      term: this.templateData.schoolTerm,
      scheduleLessonId: this.data.scheduleLessonId,
      teacherAbsenceId: this.data.teacherLesson.teacherAbsenceId,
      type: <GradeType>this.form.value.type,
      students:
        (<CreateGradeCommand['students']>this.form.value.students)?.filter(
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
              curriculumId: this.data.teacherLesson.curriculumId,
              createGradeCommand
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

function getTemplateData(
  data: AddGradesComponent['data'],
  studentsFormArray: FormArray<FormGroup<StudentGradeForm>>,
  bookTransferredHidden: boolean,
  bookNotEnrolledHidden: boolean,
  bookGradelessHidden: boolean
) {
  const schoolTerm = getTermFromDate(data.classBookInfo, data.date) ?? throwError('Date outside school term');

  const hasQualitativeGrades = data.classBookInfo.bookType === ClassBookType.Book_I_III;

  const students = data.teacherLesson.students
    .map((s, i) => {
      const studentFormValue = studentsFormArray.at(i).value;
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
        (data.teacherLesson.individualCurriculumPersonId == null ||
          s.personId === data.teacherLesson.individualCurriculumPersonId) &&
        (s.hasSelectedGrade ||
          ((!bookTransferredHidden || !s.isTransferred) &&
            (!bookNotEnrolledHidden || !s.notEnrolledInCurriculum) &&
            (!bookGradelessHidden || !s.isGradeless)))
    );

  return {
    schoolTerm,
    hasQualitativeGrades,
    students
  };
}
