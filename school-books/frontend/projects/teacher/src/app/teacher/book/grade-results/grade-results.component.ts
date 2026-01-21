import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faPencil as fasPencil } from '@fortawesome/pro-solid-svg-icons/faPencil';
import {
  ClassBooksService,
  ClassBooks_GetRemovedStudents,
  ClassBooks_GetStudents
} from 'projects/sb-api-client/src/api/classBooks.service';
import { GradeResultsService, GradeResults_GetAll } from 'projects/sb-api-client/src/api/gradeResults.service';
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
export class GradeResultsSkeletonComponent extends SkeletonComponentBase {
  constructor(
    classBooksService: ClassBooksService,
    gradeResultsService: GradeResultsService,
    route: ActivatedRoute,
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');

    this.resolve(
      GradeResultsComponent,
      resolveWithRemovedStudents(
        classBooksService,
        (data) => data.students,
        (data) => data.gradeResults,
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
          gradeResults: gradeResultsService.getAll({
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
  selector: 'sb-grade-results',
  templateUrl: './grade-results.component.html'
})
export class GradeResultsComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    classBookInfo: ClassBookInfoType;
    students: ClassBooks_GetStudents;
    gradeResults: GradeResults_GetAll;
    removedStudents: ClassBooks_GetRemovedStudents;
  };

  readonly fasPencil = fasPencil;

  readonly destroyed$ = new Subject<void>();

  studentGradeResults!: ReturnType<typeof getStudentGradeResults>;
  showTransferredStudentsBanner = false;
  canEdit = false;

  constructor(private localStorageService: LocalStorageService) {}

  ngOnInit() {
    this.showTransferredStudentsBanner = this.data.students.find((s) => s.isTransferred) != null;
    this.localStorageService.bookTransferredHidden$
      .pipe(
        tap((bookTransferredHidden) => {
          this.studentGradeResults = getStudentGradeResults(this.data, bookTransferredHidden);
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
    this.canEdit = this.data.classBookInfo.bookAllowsModifications && this.data.classBookInfo.hasEditGradeResultsAccess;
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }
}

function getStudentGradeResults(data: GradeResultsComponent['data'], bookTransferredHidden: boolean) {
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
      const gradeResult = gradeResultsMap.get(s.personId);

      return {
        ...s,

        showInfoLink: !s.isTransferred && !s.isRemoved,
        abnormalStatus: s.isRemoved ? 'ИЗТРИТ' : s.isTransferred ? 'ОТПИСАН' : '',

        gradeResult
      };
    })
    .filter((s) => !bookTransferredHidden || !s.isTransferred || s.gradeResult != null);
}
