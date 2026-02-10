import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faPencil as fasPencil } from '@fortawesome/pro-solid-svg-icons/faPencil';
import {
  ClassBooksService,
  ClassBooks_GetRemovedStudents,
  ClassBooks_GetStudents
} from 'projects/sb-api-client/src/api/classBooks.service';
import {
  FirstGradeResultsService,
  FirstGradeResults_GetAll
} from 'projects/sb-api-client/src/api/firstGradeResults.service';
import { GradeCategory } from 'projects/sb-api-client/src/model/gradeCategory';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { LocalStorageService } from 'projects/shared/services/local-storage.service';
import { ClassBookInfoType, resolveWithRemovedStudents } from 'projects/shared/utils/book';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';
import { CLASS_BOOK_INFO } from '../book/book.component';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class FirstGradeResultsSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    classBooksService: ClassBooksService,
    FirstGradeResultsService: FirstGradeResultsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');

    this.resolve(
      FirstGradeResultsComponent,
      resolveWithRemovedStudents(
        classBooksService,
        (data) => data.students,
        (data) => data.firstGradeResults,
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
          firstGradeResults: FirstGradeResultsService.getAll({
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
  selector: 'sb-first-grade-results',
  templateUrl: './first-grade-results.component.html'
})
export class FirstGradeResultsComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    classBookInfo: ClassBookInfoType;
    students: ClassBooks_GetStudents;
    firstGradeResults: FirstGradeResults_GetAll;
    removedStudents: ClassBooks_GetRemovedStudents;
  };

  readonly fasPencil = fasPencil;

  readonly destroyed$ = new Subject<void>();

  gradeResults!: ReturnType<typeof getStudentFirstGradeResults>;
  showTransferredStudentsBanner = false;
  canEdit = false;

  constructor(private localStorageService: LocalStorageService) {}

  ngOnInit() {
    this.showTransferredStudentsBanner = this.data.students.find((s) => s.isTransferred) != null;
    this.localStorageService.bookTransferredHidden$
      .pipe(
        tap((bookTransferredHidden) => {
          this.gradeResults = getStudentFirstGradeResults(this.data, bookTransferredHidden);
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
    this.canEdit =
      this.data.classBookInfo.bookAllowsModifications && this.data.classBookInfo.hasEditFirstGradeResultsAccess;
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }
}

function getStudentFirstGradeResults(data: FirstGradeResultsComponent['data'], bookTransferredHidden: boolean) {
  const firstGradeResultsMap = new Map(
    data.firstGradeResults.map((a) => [
      a.personId,
      { qualitativeGrade: a.qualitativeGrade, specialGrade: a.specialGrade }
    ])
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
        hasSpecialNeedFirstGradeResult: false
      }))
    )
    .map((s) => {
      const grade = firstGradeResultsMap.get(s.personId);

      return {
        ...s,

        showInfoLink: !s.isTransferred && !s.isRemoved,
        abnormalStatus: s.isRemoved ? 'ИЗТРИТ' : s.isTransferred ? 'ОТПИСАН' : '',

        grade: {
          category: grade?.qualitativeGrade != null ? GradeCategory.Qualitative : GradeCategory.SpecialNeeds,
          qualitativeGrade: grade?.qualitativeGrade,
          specialGrade: grade?.specialGrade
        }
      };
    })
    .filter(
      (s) =>
        !bookTransferredHidden || !s.isTransferred || s.grade.qualitativeGrade != null || s.grade.specialGrade != null
    );
}
