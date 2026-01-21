import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faGraduationCap as fadGraduationCap } from '@fortawesome/pro-duotone-svg-icons/faGraduationCap';
import { faCalendarTimes as farCalendarTimes } from '@fortawesome/pro-regular-svg-icons/faCalendarTimes';
import { faBookOpen as fasBookOpen } from '@fortawesome/pro-solid-svg-icons/faBookOpen';
import { faPencil as fasPencil } from '@fortawesome/pro-solid-svg-icons/faPencil';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import {
  ClassBooksService,
  ClassBooks_GetRemovedStudents,
  ClassBooks_GetStudents
} from 'projects/sb-api-client/src/api/classBooks.service';
import {
  PgResultsService,
  PgResults_GetAllForClassBook,
  PgResults_GetAllForStudent
} from 'projects/sb-api-client/src/api/pgResults.service';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { LocalStorageService } from 'projects/shared/services/local-storage.service';
import { ClassBookInfoType, resolveWithRemovedStudents } from 'projects/shared/utils/book';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';
import { CLASS_BOOK_INFO } from '../book/book.component';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class PgResultsSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    classBooksService: ClassBooksService,
    pgResultsService: PgResultsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');

    this.resolve(
      PgResultsComponent,
      resolveWithRemovedStudents(
        classBooksService,
        (data) => data.students,
        (data) => data.pgResults,
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
          pgResults: pgResultsService.getAllForClassBook({
            schoolYear,
            instId,
            classBookId
          })
        }
      )
    );
  }
}

type PgResult = ArrayElementType<PgResults_GetAllForStudent> & {
  removing: boolean;
  viewing: boolean;
  canEdit: boolean;
  canRemove: boolean;
};

@Component({
  selector: 'sb-pg-results',
  templateUrl: './pg-results.component.html'
})
export class PgResultsComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    classBookInfo: ClassBookInfoType;
    students: ClassBooks_GetStudents;
    pgResults: PgResults_GetAllForClassBook;
    removedStudents: ClassBooks_GetRemovedStudents;
  };

  readonly fasBookOpen = fasBookOpen;
  readonly fadGraduationCap = fadGraduationCap;
  readonly fasPlus = fasPlus;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasTrashAlt = fasTrashAlt;
  readonly farCalendarTimes = farCalendarTimes;
  readonly fasPencil = fasPencil;

  readonly destroyed$ = new Subject<void>();

  studentPgResults!: ReturnType<typeof getStudentPgResults>;
  showTransferredStudentsBanner = false;

  selectedStudentPersonId?: number;
  selectedPgResults?: PgResult[];
  loadingPgResults = false;

  canCreate = false;

  constructor(
    private actionService: ActionService,
    private pgResultsService: PgResultsService,
    private route: ActivatedRoute,
    private router: Router,
    private localStorageService: LocalStorageService
  ) {}

  ngOnInit() {
    this.showTransferredStudentsBanner = this.data.students.find((s) => s.isTransferred) != null;
    this.localStorageService.bookTransferredHidden$
      .pipe(
        tap((bookTransferredHidden) => {
          this.studentPgResults = getStudentPgResults(this.data, bookTransferredHidden);
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
    this.canCreate = this.data.classBookInfo.bookAllowsModifications && this.data.classBookInfo.hasCreatePgResultAccess;
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  reloadPgResults() {
    return this.pgResultsService
      .getAllForClassBook({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId
      })
      .toPromise()
      .then((pgResults) => {
        this.selectedStudentPersonId = undefined;
        this.selectedPgResults = undefined;

        this.data.pgResults = pgResults;
        this.studentPgResults = getStudentPgResults(this.data, this.localStorageService.getBookTransferredHidden());
      })
      .catch((err) => GlobalErrorHandler.instance.handleError(err));
  }

  viewPgResults(studentPersonId: number) {
    this.selectedPgResults = undefined;

    if (this.selectedStudentPersonId === studentPersonId) {
      this.selectedStudentPersonId = undefined;

      return;
    }

    this.selectedStudentPersonId = studentPersonId;
    this.loadingPgResults = true;

    return this.pgResultsService
      .getAllForStudent({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        personId: this.selectedStudentPersonId
      })
      .toPromise()
      .then((pgResults) => {
        this.selectedPgResults = pgResults.map((r) => ({
          ...r,
          removing: false,
          viewing: false,
          canEdit: this.data.classBookInfo.bookAllowsModifications && r.hasEditAccess,
          canRemove: this.data.classBookInfo.bookAllowsModifications && r.hasRemoveAccess
        }));
      })
      .catch((err) => {
        this.selectedStudentPersonId = undefined;

        GlobalErrorHandler.instance.handleError(err);
      })
      .finally(() => {
        this.loadingPgResults = false;
      });
  }

  editPgResult(pgResult: PgResult) {
    this.router.navigate(['./', pgResult.pgResultId], { relativeTo: this.route });
  }

  removePgResult(pgResult: PgResult) {
    pgResult.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      pgResultId: pgResult.pgResultId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете резултата?',
        errorsMessage: 'Не може да изтриете резултата, защото:',
        httpAction: () => this.pgResultsService.remove(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          return this.reloadPgResults();
        }

        return Promise.resolve();
      })
      .finally(() => {
        pgResult.removing = false;
      });
  }
}

function getStudentPgResults(data: PgResultsComponent['data'], bookTransferredHidden: boolean) {
  const pgResultsMap = new Map(
    data.pgResults.map((r) => [
      r.personId,
      {
        pgResultsCount: r.count
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
      const pgResults = pgResultsMap.get(s.personId);

      return {
        ...s,

        showInfoLink: !s.isTransferred && !s.isRemoved,
        abnormalStatus: s.isRemoved ? 'ИЗТРИТ' : s.isTransferred ? 'ОТПИСАН' : '',

        pgResultsCount: pgResults?.pgResultsCount ?? 0
      };
    });

  if (bookTransferredHidden) {
    students = students.filter((s) => !s.isTransferred || s.pgResultsCount);
  }

  const totals = students.reduce(
    (totals, s) => {
      totals.pgResultsCountTotal += s.pgResultsCount;

      return totals;
    },
    {
      pgResultsCountTotal: 0
    }
  );

  return {
    students,
    totals
  };
}
