import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormArray, UntypedFormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faArrowLeft as fasArrowLeft } from '@fortawesome/pro-solid-svg-icons/faArrowLeft';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTimes as fasTimes } from '@fortawesome/pro-solid-svg-icons/faTimes';
import { ClassBookCurriculumNomsService } from 'projects/sb-api-client/src/api/classBookCurriculumNoms.service';
import {
  ClassBooksService,
  ClassBooks_GetRemovedStudents,
  ClassBooks_GetStudents
} from 'projects/sb-api-client/src/api/classBooks.service';
import {
  GradeResultsService,
  GradeResults_GetAllEdit,
  GradeResults_UpdateRequestParams
} from 'projects/sb-api-client/src/api/gradeResults.service';
import { GradeResultType } from 'projects/sb-api-client/src/model/gradeResultType';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  IShouldPreventLeave,
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { LocalStorageService } from 'projects/shared/services/local-storage.service';
import { resolveWithRemovedStudents } from 'projects/shared/utils/book';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';
import { distinctUntilChanged, startWith, takeUntil, tap } from 'rxjs/operators';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class GradeResultsEditSkeletonComponent extends SkeletonComponentBase {
  constructor(classBooksService: ClassBooksService, gradeResultsService: GradeResultsService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');

    this.resolve(
      GradeResultsEditComponent,
      resolveWithRemovedStudents(
        classBooksService,
        (data) => data.students,
        (data) => data.gradeResults,
        {
          schoolYear,
          instId,
          classBookId,
          students: classBooksService.getStudents({
            schoolYear,
            instId,
            classBookId
          }),
          gradeResults: gradeResultsService.getAllEdit({
            schoolYear,
            instId,
            classBookId
          })
        }
      )
    );
  }
}

@Component({
  selector: 'sb-grade-results-edit',
  templateUrl: './grade-results-edit.component.html',
  styleUrls: ['./grade-results-edit.component.scss']
})
export class GradeResultsEditComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    students: ClassBooks_GetStudents;
    gradeResults: GradeResults_GetAllEdit;
    removedStudents: ClassBooks_GetRemovedStudents;
  };

  private readonly destroyed$ = new Subject<void>();
  readonly fasArrowLeft = fasArrowLeft;
  readonly fasCheck = fasCheck;
  readonly fasPlus = fasPlus;
  readonly fasTimes = fasTimes;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fadChevronCircleRight = fadChevronCircleRight;

  readonly form = this.fb.group({
    studentGrades: this.fb.array([])
  });

  students!: ReturnType<typeof mapStudents>;
  saveBtnDisabled = false;
  classBookCurriculumNomsService: INomService<number, { schoolYear: number; instId: number; classBookId: number }>;

  constructor(
    public localStorageService: LocalStorageService,
    private fb: UntypedFormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private gradeResultsService: GradeResultsService,
    private actionService: ActionService,
    classBookCurriculumNomsService: ClassBookCurriculumNomsService
  ) {
    this.classBookCurriculumNomsService = new NomServiceWithParams(classBookCurriculumNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId
    }));
  }

  ngOnInit(): void {
    this.students = mapStudents(this.data);

    for (const student of this.students) {
      const studentGradeForm = this.fb.group({
        personId: [student.personId],
        initialResultType: [student.gradeResult?.initialResultType],
        retakeExamCurriculumIds: [student.gradeResult?.retakeExamCurriculumIds, Validators.required],
        finalResultType: [student.gradeResult?.finalResultType]
      });

      this.studentGradesFormArray.push(studentGradeForm);

      const initialResultTypeControl = studentGradeForm.get('initialResultType') ?? throwError();
      const retakeExamCurriculumIdsControl = studentGradeForm.get('retakeExamCurriculumIds') ?? throwError();

      initialResultTypeControl.valueChanges
        .pipe(
          startWith(initialResultTypeControl.value),
          distinctUntilChanged(),
          tap((value) => {
            if (value === GradeResultType.MustRetakeExams) {
              retakeExamCurriculumIdsControl.enable();
            } else {
              retakeExamCurriculumIdsControl.disable();
            }
          }),
          takeUntil(this.destroyed$)
        )
        .subscribe();
    }
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  get studentGradesFormArray(): UntypedFormArray {
    return this.form.get('studentGrades') as UntypedFormArray;
  }

  gradeResultSelectedAt(i: number) {
    return this.studentGradesFormArray.at(i).get('initialResultType')?.value != null;
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave() {
    if (this.form.invalid || this.saveBtnDisabled) {
      return;
    }

    this.saveBtnDisabled = true;

    const students =
      <GradeResults_UpdateRequestParams['updateGradeResultsCommand']['students']>this.form.value.studentGrades ??
      throwError('studentGrades array is missing');

    for (const grResult of students) {
      if (grResult.initialResultType !== GradeResultType.MustRetakeExams) {
        grResult.retakeExamCurriculumIds = [];
        grResult.finalResultType = null;
      }
    }

    this.actionService
      .execute({
        httpAction: () =>
          this.gradeResultsService
            .update({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              updateGradeResultsCommand: { students }
            })
            .toPromise()
      })
      .then((done) => {
        if (done) {
          this.form.markAsPristine();
          this.router.navigate(['../'], { relativeTo: this.route });
        }
      })
      .finally(() => (this.saveBtnDisabled = false));
  }
}

function mapStudents(data: GradeResultsEditComponent['data']) {
  const gradeResultsMap = new Map(data.gradeResults.map((gr) => [gr.personId, gr]));

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
        hasSpecialNeedFirstGradeResult: false
      }))
    )
    .map((s) => {
      const gradeResult = gradeResultsMap.get(s.personId) ?? null;

      return {
        ...s,

        abnormalStatus: s.isRemoved ? 'ИЗТРИТ' : s.isTransferred ? 'ОТПИСАН' : '',

        gradeResult
      };
    });
}
