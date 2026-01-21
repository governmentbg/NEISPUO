import { Component, Inject } from '@angular/core';
import { UntypedFormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { SchoolYearNomsService } from 'projects/sb-api-client/src/api/schoolYearNoms.service';
import { SpbsBookService, SpbsBook_GetAll } from 'projects/sb-api-client/src/api/spbsBook.service';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { deepEqual, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';
import { SpbsBookDownloadDialogComponent } from '../spbs-book-download-dialog/spbs-book-download-dialog.component';
import { SpbsBookNewDialogSkeletonComponent } from '../spbs-book-new-dialog/spbs-book-new-dialog.component';

@Component({
  selector: 'sb-spbs-book',
  templateUrl: './spbs-book.component.html'
})
export class SpbsBookComponent {
  private schoolYear: number;
  private instId: number;

  readonly fasPlus = fasPlus;
  readonly fadChevronCircleRight = fadChevronCircleRight;
  readonly fasFileExcel = fasFileExcel;

  dataSource: TableDataSource<SpbsBook_GetAll>;
  schoolYearNomsService: INomService<number, { schoolYear: number; instId: number }>;
  searchForm = this.fb.group({
    recordSchoolYear: null,
    recordNumber: null,
    studentName: null,
    personalId: null
  });
  canCreate = false;

  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    private spbsBookService: SpbsBookService,
    private actionService: ActionService,
    schoolYearNomsService: SchoolYearNomsService,
    private router: Router,
    private route: ActivatedRoute,
    private fb: UntypedFormBuilder,
    private dialog: MatDialog
  ) {
    this.schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    this.instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      spbsBookService.getAll({
        schoolYear: this.schoolYear,
        instId: this.instId,
        offset,
        limit,
        ...this.searchForm.value
      })
    );

    this.searchForm.valueChanges
      .pipe(
        debounceTime(200),
        distinctUntilChanged((a, b) => deepEqual(a, b))
      )
      .subscribe(() => {
        this.dataSource.reload();
      });

    this.schoolYearNomsService = new NomServiceWithParams(schoolYearNomsService, () => ({
      schoolYear: this.schoolYear,
      instId: this.instId
    }));

    // the promise should be resolved by this stage,
    // so no need for a skeleton screen
    institutionInfo.then((institutionInfo) => {
      this.canCreate = institutionInfo.schoolYearAllowsModifications && institutionInfo.hasSpbsBookCreateAccess;
    });
  }

  openDownloadDialog(): void {
    openTypedDialog(this.dialog, SpbsBookDownloadDialogComponent, {
      data: {
        schoolYear: this.schoolYear,
        instId: this.instId
      }
    });
  }

  openAddDialog(): void {
    openTypedDialog(this.dialog, SpbsBookNewDialogSkeletonComponent, {
      data: {
        schoolYear: this.schoolYear,
        instId: this.instId
      }
    })
      .afterClosed()
      .toPromise()
      .then((student) => {
        if (student) {
          this.actionService.execute({
            httpAction: () => {
              return this.spbsBookService
                .create({
                  schoolYear: this.schoolYear,
                  instId: this.instId,
                  createSpbsBookRecordCommand: student
                })
                .toPromise()
                .then((spbsBookRecordId) => {
                  this.router.navigate([this.schoolYear, spbsBookRecordId], { relativeTo: this.route });
                });
            }
          });
        }

        return Promise.resolve();
      });
  }
}
