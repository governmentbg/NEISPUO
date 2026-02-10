import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faExclamationCircle as fasExclamationCircle } from '@fortawesome/pro-regular-svg-icons/faExclamationCircle';
import { faFilePdf as fasFilePdf } from '@fortawesome/pro-solid-svg-icons/faFilePdf';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import {
  ClassBooksAdminService,
  ClassBooksAdmin_GetClassBookPrints
} from 'projects/sb-api-client/src/api/classBooksAdmin.service';
import { ClassBookPrintStatus } from 'projects/sb-api-client/src/model/classBookPrintStatus';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Subject, Subscription, timer } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';
import { ClassBookAdminInfoType, CLASS_BOOK_ADMIN_INFO } from '../../book-admin-view.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class BookAdminClassBookPrintSkeletonComponent extends SkeletonComponentBase {
  constructor(@Inject(CLASS_BOOK_ADMIN_INFO) classBookAdminInfo: Promise<ClassBookAdminInfoType>) {
    super();

    this.resolve(BookAdminClassBookPrintComponent, {
      classBookAdminInfo: from(classBookAdminInfo)
    });
  }
}

@Component({
  selector: 'sb-book-admin-classbook-print',
  templateUrl: './book-admin-classbook-print.component.html'
})
export class BookAdminClassBookPrintComponent implements OnInit, OnDestroy {
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
  dataSource: TableDataSource<ClassBooksAdmin_GetClassBookPrints>;

  constructor(
    public route: ActivatedRoute,
    public router: Router,
    private classBooksAdminService: ClassBooksAdminService,
    private actionService: ActionService
  ) {
    this.schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    this.instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    this.classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      classBooksAdminService
        .getClassBookPrints({
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
    this.printing = true;
    this.actionService
      .execute({
        errorsMessage: 'Действието не може да бъде извършено:',
        httpAction: () =>
          this.classBooksAdminService
            .printClassBook({
              schoolYear: this.schoolYear,
              instId: this.instId,
              classBookId: this.classBookId
            })
            .toPromise()
      })
      .then((done) => {
        if (done) {
          this.dataSource.reload();
        }
      })
      .finally(() => (this.printing = false));
  }
}
