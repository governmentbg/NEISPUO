import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faBookOpen as fasBookOpen } from '@fortawesome/pro-solid-svg-icons/faBookOpen';
import { faCalendarAlt as fasCalendarAlt } from '@fortawesome/pro-solid-svg-icons/faCalendarAlt';
import { faEye as fasEye } from '@fortawesome/pro-solid-svg-icons/faEye';
import { faPencil as fasPencil } from '@fortawesome/pro-solid-svg-icons/faPencil';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faStarHalfAlt as fasStarHalfAlt } from '@fortawesome/pro-solid-svg-icons/faStarHalfAlt';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import {
  ClassBooksService,
  ClassBooks_GetRemovedStudents,
  ClassBooks_GetStudents
} from 'projects/sb-api-client/src/api/classBooks.service';
import {
  RemarksService,
  Remarks_GetAllForClassBook,
  Remarks_GetAllForStudentAndType
} from 'projects/sb-api-client/src/api/remarks.service';
import { RemarkType } from 'projects/sb-api-client/src/model/remarkType';
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
export class RemarksSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    classBooksService: ClassBooksService,
    remarksService: RemarksService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');

    this.resolve(
      RemarksComponent,
      resolveWithRemovedStudents(
        classBooksService,
        (data) => data.students,
        (data) => data.remarks,
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
          remarks: remarksService.getAllForClassBook({
            schoolYear,
            instId,
            classBookId
          })
        }
      )
    );
  }
}

type Remark = ArrayElementType<Remarks_GetAllForStudentAndType> & {
  removing: boolean;
  viewing: boolean;
  canEdit: boolean;
  canRemove: boolean;
};

@Component({
  selector: 'sb-remarks',
  templateUrl: './remarks.component.html'
})
export class RemarksComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    classBookInfo: ClassBookInfoType;
    students: ClassBooks_GetStudents;
    remarks: Remarks_GetAllForClassBook;
    removedStudents: ClassBooks_GetRemovedStudents;
  };

  readonly fasBookOpen = fasBookOpen;
  readonly fasCalendarAlt = fasCalendarAlt;
  readonly fasPlus = fasPlus;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasStarHalfAlt = fasStarHalfAlt;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasPencil = fasPencil;
  readonly fasEye = fasEye;

  readonly REMARK_TYPE_BAD = RemarkType.Bad;
  readonly REMARK_TYPE_GOOD = RemarkType.Good;

  readonly destroyed$ = new Subject<void>();

  studentRemarks!: ReturnType<typeof getStudentRemarks>;
  showTransferredStudentsBanner = false;

  selectedStudentPersonId?: number;
  selectedType?: RemarkType;
  selectedRemarks?: Remark[];
  loadingRemarks = false;

  canCreate = false;

  constructor(
    private actionService: ActionService,
    private remarksService: RemarksService,
    private route: ActivatedRoute,
    private router: Router,
    private localStorageService: LocalStorageService
  ) {}

  ngOnInit() {
    this.showTransferredStudentsBanner = this.data.students.find((s) => s.isTransferred) != null;
    this.localStorageService.bookTransferredHidden$
      .pipe(
        tap((bookTransferredHidden) => {
          this.studentRemarks = getStudentRemarks(this.data, bookTransferredHidden);
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
    this.canCreate = this.data.classBookInfo.bookAllowsModifications;
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  reloadRemarks() {
    return this.remarksService
      .getAllForClassBook({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId
      })
      .toPromise()
      .then((remarks) => {
        this.selectedStudentPersonId = undefined;
        this.selectedRemarks = undefined;

        this.data.remarks = remarks;
        this.studentRemarks = getStudentRemarks(this.data, this.localStorageService.getBookTransferredHidden());
      })
      .catch((err) => GlobalErrorHandler.instance.handleError(err));
  }

  viewRemarks(studentPersonId: number, type: RemarkType) {
    this.selectedRemarks = undefined;

    if (this.selectedStudentPersonId === studentPersonId && this.selectedType === type) {
      this.selectedStudentPersonId = undefined;
      this.selectedType = undefined;

      return;
    }

    this.selectedStudentPersonId = studentPersonId;
    this.selectedType = type;
    this.loadingRemarks = true;

    return this.remarksService
      .getAllForStudentAndType({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        personId: this.selectedStudentPersonId,
        type: this.selectedType
      })
      .toPromise()
      .then((remarks) => {
        this.selectedRemarks = remarks.map((r) => ({
          ...r,
          removing: false,
          viewing: false,
          canEdit: this.data.classBookInfo.bookAllowsModifications && r.hasEditAccess,
          canRemove: this.data.classBookInfo.bookAllowsModifications && r.hasRemoveAccess
        }));
      })
      .catch((err) => {
        this.selectedStudentPersonId = undefined;
        this.selectedType = undefined;

        GlobalErrorHandler.instance.handleError(err);
      })
      .finally(() => {
        this.loadingRemarks = false;
      });
  }

  editRemark(remark: Remark) {
    this.router.navigate(['./', remark.remarkId, { type: remark.type }], { relativeTo: this.route });
  }

  removeRemark(remark: Remark) {
    remark.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      curriculumId: remark.curriculumId,
      remarkId: remark.remarkId
    };

    this.actionService
      .execute({
        confirmMessage: `Сигурни ли сте, че искате да изтриете ${
          remark.type === RemarkType.Bad ? 'забележката' : 'похвалата'
        }?`,
        errorsMessage: `Не може да изтриете ${remark.type === RemarkType.Bad ? 'забележката' : 'похвалата'}, защото:`,
        httpAction: () => this.remarksService.remove(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          return this.reloadRemarks();
        }

        return Promise.resolve();
      })
      .finally(() => {
        remark.removing = false;
      });
  }
}

function getStudentRemarks(data: RemarksComponent['data'], bookTransferredHidden: boolean) {
  const remarksMap = new Map(
    data.remarks.map((r) => [
      r.personId,
      {
        badRemarksCount: r.badRemarksCount,
        goodRemarksCount: r.goodRemarksCount
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
      const remarks = remarksMap.get(s.personId);

      return {
        ...s,

        showInfoLink: !s.isTransferred && !s.isRemoved,
        abnormalStatus: s.isRemoved ? 'ИЗТРИТ' : s.isTransferred ? 'ОТПИСАН' : '',

        badRemarksCount: remarks?.badRemarksCount ?? 0,
        goodRemarksCount: remarks?.goodRemarksCount ?? 0
      };
    });

  if (bookTransferredHidden) {
    students = students.filter((s) => !s.isTransferred || s.badRemarksCount || s.goodRemarksCount);
  }

  const totals = students.reduce(
    (totals, s) => {
      totals.badRemarksCountTotal += s.badRemarksCount;
      totals.goodRemarksCountTotal += s.goodRemarksCount;

      return totals;
    },
    {
      badRemarksCountTotal: 0,
      goodRemarksCountTotal: 0
    }
  );

  return {
    students,
    totals
  };
}
