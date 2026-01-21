import { Component, Inject, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import {
  SpbsBookService,
  SpbsBook_CreateEscapeRequestParams,
  SpbsBook_GetEscape,
  SpbsBook_UpdateEscapeRequestParams
} from 'projects/sb-api-client/src/api/spbsBook.service';
import {
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { getTimeFormatHint, hourRegex } from 'projects/shared/utils/date';

export type SpbsBookEscapeViewDialogData = {
  schoolYear: number;
  instId: number;
  recordSchoolYear: number;
  spbsBookRecordId: number;
  orderNum?: number | null;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class SpbsBookEscapeViewDialogSkeletonComponent extends SkeletonComponentBase {
  d!: SpbsBookEscapeViewDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: SpbsBookEscapeViewDialogData,
    spbsBookService: SpbsBookService
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const spbsBookRecordId = data.spbsBookRecordId;
    const recordSchoolYear = data.recordSchoolYear;
    const orderNum = data.orderNum;

    if (orderNum == null) {
      this.resolve(SpbsBookEscapeViewDialogComponent, {
        schoolYear,
        instId,
        recordSchoolYear,
        spbsBookRecordId,
        orderNum,
        spbsBookRecordEscape: null
      });
    } else {
      this.resolve(SpbsBookEscapeViewDialogComponent, {
        schoolYear,
        instId,
        recordSchoolYear,
        spbsBookRecordId,
        orderNum,
        spbsBookRecordEscape: spbsBookService.getEscape({
          schoolYear: recordSchoolYear,
          instId,
          spbsBookRecordId,
          orderNum
        })
      });
    }
  }
}

@Component({
  selector: 'sb-spbs-book-escape-view-dialog',
  templateUrl: './spbs-book-escape-view-dialog.component.html'
})
export class SpbsBookEscapeViewDialogComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    recordSchoolYear: number;
    spbsBookRecordId: number;
    orderNum?: number | null;
    spbsBookRecordEscape: SpbsBook_GetEscape | null;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly timeFormatHint = getTimeFormatHint();

  readonly form = this.fb.group({
    escapeDate: [null, Validators.required],
    escapeTime: [null, [Validators.required, Validators.pattern(hourRegex)]],
    policeNotificationDate: [null, Validators.required],
    policeNotificationTime: [null, [Validators.required, Validators.pattern(hourRegex)]],
    policeLetterNumber: [null, Validators.required],
    policeLetterDate: [null, Validators.required],
    returnDate: [null]
  });

  saving = false;

  constructor(
    private fb: UntypedFormBuilder,
    private spbsBookService: SpbsBookService,
    private actionService: ActionService,
    private dialogRef: MatDialogRef<SpbsBookEscapeViewDialogComponent>
  ) {}

  ngOnInit(): void {
    const spbsBookRecordEscape = this.data.spbsBookRecordEscape;
    if (spbsBookRecordEscape) {
      const {
        escapeDate,
        escapeTime,
        policeNotificationDate,
        policeNotificationTime,
        policeLetterNumber,
        policeLetterDate,
        returnDate
      } = spbsBookRecordEscape;

      this.form.setValue({
        escapeDate,
        escapeTime,
        policeNotificationDate,
        policeNotificationTime,
        policeLetterNumber,
        policeLetterDate,
        returnDate
      });
    }
  }

  onSave() {
    if (this.form.invalid || this.saving) {
      return;
    }
    this.saving = true;

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.orderNum == null) {
            return this.spbsBookService
              .createEscape({
                schoolYear: this.data.recordSchoolYear,
                instId: this.data.instId,
                spbsBookRecordId: this.data.spbsBookRecordId,
                createSpbsBookRecordEscapeCommand: <
                  SpbsBook_CreateEscapeRequestParams['createSpbsBookRecordEscapeCommand']
                >this.form.value
              })
              .toPromise()
              .then(() => {
                this.dialogRef.close(true);
              });
          } else {
            return this.spbsBookService
              .updateEscape({
                schoolYear: this.data.recordSchoolYear,
                instId: this.data.instId,
                spbsBookRecordId: this.data.spbsBookRecordId,
                orderNum: this.data.orderNum,
                updateSpbsBookRecordEscapeCommand: <
                  SpbsBook_UpdateEscapeRequestParams['updateSpbsBookRecordEscapeCommand']
                >this.form.value
              })
              .toPromise()
              .then(() => {
                this.dialogRef.close(true);
              });
          }
        }
      })
      .finally(() => {
        this.saving = false;
      });
  }
}
