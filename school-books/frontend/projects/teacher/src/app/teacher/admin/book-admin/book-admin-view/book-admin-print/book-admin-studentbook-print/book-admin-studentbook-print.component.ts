import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { faExclamationCircle as fasExclamationCircle } from '@fortawesome/pro-regular-svg-icons/faExclamationCircle';
import { faFilePdf as fasFilePdf } from '@fortawesome/pro-solid-svg-icons/faFilePdf';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import {
  ClassBooksAdminService,
  ClassBooksAdmin_GetClassBookStudentPrints
} from 'projects/sb-api-client/src/api/classBooksAdmin.service';
import { ClassBookPrintStatus } from 'projects/sb-api-client/src/model/classBookPrintStatus';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Subject, Subscription, timer } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';
import { ClassBookAdminInfoType, CLASS_BOOK_ADMIN_INFO } from '../../book-admin-view.component';
import { BookAdminStudentBookPrintStudentDialogSkeletonComponent } from '../book-admin-studentbook-print-student-dialog/book-admin-studentbook-print-student-dialog.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class BookAdminStudentBookPrintSkeletonComponent extends SkeletonComponentBase {
  constructor(@Inject(CLASS_BOOK_ADMIN_INFO) classBookAdminInfo: Promise<ClassBookAdminInfoType>) {
    super();

    this.resolve(BookAdminStudentBookPrintComponent, {
      classBookAdminInfo: from(classBookAdminInfo)
    });
  }
}

@Component({
  selector: 'sb-book-admin-studentbook-print',
  templateUrl: './book-admin-studentbook-print.component.html'
})
export class BookAdminStudentBookPrintComponent implements OnInit, OnDestroy {
  @Input() data!: {
    classBookAdminInfo: ClassBookAdminInfoType;
  };

  readonly destroyed$ = new Subject<void>();
  readonly fasFilePdf = fasFilePdf;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasExclamationCircle = fasExclamationCircle;
  readonly ClassBookPrintStatus = ClassBookPrintStatus;
  schoolYear: number;
  instId: number;
  classBookId: number;
  canPrint = false;
  printing = false;

  pendingReload?: Subscription | null;
  dataSource: TableDataSource<ClassBooksAdmin_GetClassBookStudentPrints>;

  constructor(
    public route: ActivatedRoute,
    public router: Router,
    private classBooksAdminService: ClassBooksAdminService,
    private dialog: MatDialog,
    private actionService: ActionService
  ) {
    this.schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    this.instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    this.classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      classBooksAdminService
        .getClassBookStudentPrints({
          schoolYear: this.schoolYear,
          instId: this.instId,
          classBookId: this.classBookId,
          offset,
          limit
        })
        .pipe(
          // reload the page in 5sec if there is a Pending print
          tap((prints) => {
            if (this.pendingReload) {
              this.pendingReload.unsubscribe();
            }

            if (prints.result.find((p) => p.status === ClassBookPrintStatus.Pending)) {
              // to make the spinner feel smooth even with the reloads(which replace the icon element)
              // use a reloading timer that is a multiple of the fa-spin's animation-duration of 2s
              this.pendingReload = timer(4000)
                .pipe(takeUntil(this.destroyed$))
                .subscribe(() => {
                  this.dataSource.reloadPage(true);
                });
            }
          })
        )
    );
  }

  ngOnInit() {
    // no 'bookAllowsModifications' check as printing should be posible
    // even if there is a CBExtProvider
    this.canPrint =
      !this.data.classBookAdminInfo.schoolYearIsFinalized &&
      !this.data.classBookAdminInfo.isFinalized &&
      this.data.classBookAdminInfo.hasCreatePrintAccess;
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  startPrint() {
    openTypedDialog(this.dialog, BookAdminStudentBookPrintStudentDialogSkeletonComponent, {
      data: {
        schoolYear: this.schoolYear,
        instId: this.instId,
        classBookId: this.classBookId
      }
    })
      .afterClosed()
      .toPromise()
      .then((studentId) => {
        if (!studentId) return;

        this.printing = true;
        this.actionService
          .execute({
            errorsMessage: 'Действието не може да бъде извършено:',
            httpAction: () =>
              this.classBooksAdminService
                .printClassBookStudent({
                  schoolYear: this.schoolYear,
                  instId: this.instId,
                  classBookId: this.classBookId,
                  personId: studentId
                })
                .toPromise()
          })
          .then((done) => {
            if (done) {
              this.dataSource.reload();
            }
          })
          .finally(() => (this.printing = false));
      });
  }
}
