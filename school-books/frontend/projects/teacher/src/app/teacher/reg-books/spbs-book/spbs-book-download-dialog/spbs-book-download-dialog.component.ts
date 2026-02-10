import { Component, Inject, OnDestroy } from '@angular/core';
import { UntypedFormControl } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import {
  SchoolYearNomsService,
  SchoolYearNoms_GetNomsByTerm
} from 'projects/sb-api-client/src/api/schoolYearNoms.service';
import { SpbsBookExcelService } from 'projects/sb-api-client/src/api/spbsBookExcel.service';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { TypedDialog } from 'projects/shared/utils/dialog';
import { Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';

export type SpbsBookDownloadDialogData = {
  schoolYear: number;
  instId: number;
};

@Component({
  selector: 'sb-spbs-book-download-dialog',
  templateUrl: './spbs-book-download-dialog.component.html'
})
export class SpbsBookDownloadDialogComponent implements TypedDialog<SpbsBookDownloadDialogData, void>, OnDestroy {
  d!: SpbsBookDownloadDialogData;
  r!: void;

  private readonly destroyed$ = new Subject<void>();

  fasFileExcel = fasFileExcel;
  fasSpinnerThird = fasSpinnerThird;

  selectedYear = new UntypedFormControl('');
  schoolYears: SchoolYearNoms_GetNomsByTerm = [];
  downloadUrl?: string;
  spinning = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: SpbsBookDownloadDialogData,
    schoolYearNomsService: SchoolYearNomsService,
    spbsBookExcelService: SpbsBookExcelService
  ) {
    schoolYearNomsService
      .getNomsByTerm({ schoolYear: this.data.schoolYear, instId: this.data.instId })
      .toPromise()
      .then((r) => (this.schoolYears = r))
      .catch((err) => GlobalErrorHandler.instance.handleError(err));

    this.selectedYear.valueChanges
      .pipe(
        tap((selectedValue) => {
          if (selectedValue != null) {
            this.spinning = true;
            this.downloadUrl = undefined;

            spbsBookExcelService
              .downloadSpbsBookExcelFile({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                recordSchoolYear: selectedValue
              })
              .toPromise()
              .then((url) => (this.downloadUrl = url))
              .catch((err) => GlobalErrorHandler.instance.handleError(err))
              .finally(() => (this.spinning = false));
          } else {
            this.downloadUrl = undefined;
            this.spinning = false;
          }
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }
}
