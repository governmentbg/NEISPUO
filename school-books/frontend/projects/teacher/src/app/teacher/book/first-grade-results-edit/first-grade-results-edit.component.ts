import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormArray, UntypedFormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faArrowLeft as fasArrowLeft } from '@fortawesome/pro-solid-svg-icons/faArrowLeft';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import {
  ClassBooksService,
  ClassBooks_GetRemovedStudents,
  ClassBooks_GetStudents
} from 'projects/sb-api-client/src/api/classBooks.service';
import {
  FirstGradeResultsService,
  FirstGradeResults_GetAll,
  FirstGradeResults_UpdateRequestParams
} from 'projects/sb-api-client/src/api/firstGradeResults.service';
import {
  SpecialNeedsGradeNomsService,
  SpecialNeedsGradeNoms_GetNomsByTerm
} from 'projects/sb-api-client/src/api/specialNeedsGradeNoms.service';
import {
  IShouldPreventLeave,
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { LocalStorageService } from 'projects/shared/services/local-storage.service';
import { resolveWithRemovedStudents } from 'projects/shared/utils/book';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';

export type FirstGradeResults = FirstGradeResults_UpdateRequestParams['updateFirstGradeResultCommand']['students'];

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class FirstGradeResultsEditSkeletonComponent extends SkeletonComponentBase {
  constructor(
    classBooksService: ClassBooksService,
    resultsService: FirstGradeResultsService,
    specialNeedsGradeNomsService: SpecialNeedsGradeNomsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');

    this.resolve(
      FirstGradeResultsEditComponent,
      resolveWithRemovedStudents(
        classBooksService,
        (data) => data.students,
        (data) => data.firstGradeResults,
        {
          schoolYear,
          instId,
          classBookId,
          students: classBooksService.getStudents({
            schoolYear,
            instId,
            classBookId
          }),
          firstGradeResults: resultsService.getAll({
            schoolYear,
            instId,
            classBookId
          }),
          specialNeedsGradeNoms: specialNeedsGradeNomsService.getNomsByTerm({
            schoolYear: schoolYear,
            instId: instId,
            term: ''
          })
        }
      )
    );
  }
}

@Component({
  selector: 'sb-first-grade-results-edit',
  templateUrl: './first-grade-results-edit.component.html'
})
export class FirstGradeResultsEditComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    students: ClassBooks_GetStudents;
    firstGradeResults: FirstGradeResults_GetAll;
    removedStudents: ClassBooks_GetRemovedStudents;
    specialNeedsGradeNoms: SpecialNeedsGradeNoms_GetNomsByTerm;
  };

  readonly fasArrowLeft = fasArrowLeft;
  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly destroyed$ = new Subject<void>();

  readonly form = this.fb.group({
    studentGrades: this.fb.array([])
  });

  saveBtnDisabled = false;
  students!: ReturnType<typeof mapStudents>;

  constructor(
    public localStorageService: LocalStorageService,
    private fb: UntypedFormBuilder,
    private resultsService: FirstGradeResultsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService
  ) {}

  ngOnInit() {
    this.students = mapStudents(this.data);

    for (const student of this.students) {
      this.studentGradesFormArray.push(
        this.fb.group({
          personId: [student.personId],
          qualitativeGrade: [student.firstGradeResult?.qualitativeGrade],
          specialGrade: [student.firstGradeResult?.specialGrade]
        })
      );
    }
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  get studentGradesFormArray(): UntypedFormArray {
    return this.form.get('studentGrades') as UntypedFormArray;
  }

  firstGradeResultSelectedAt(i: number) {
    return this.studentGradesFormArray.at(i).get('qualitativeGrade')?.value != null;
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave() {
    if (this.form.invalid || this.saveBtnDisabled) {
      return;
    }

    this.saveBtnDisabled = true;

    this.actionService
      .execute({
        httpAction: () =>
          this.resultsService
            .update({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              updateFirstGradeResultCommand: {
                students: <FirstGradeResults>this.form.value.studentGrades
              }
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

function mapStudents(data: FirstGradeResultsEditComponent['data']) {
  const firstGradeResultsMap = new Map(data.firstGradeResults.map((fgr) => [fgr.personId, fgr]));

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
      const firstGradeResult = firstGradeResultsMap.get(s.personId) ?? null;

      return {
        ...s,

        abnormalStatus: s.isRemoved ? 'ИЗТРИТ' : s.isTransferred ? 'ОТПИСАН' : '',

        firstGradeResult
      };
    });
}
