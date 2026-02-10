import { Component, Inject, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import {
  SpbsBookService,
  SpbsBook_CreateAbsenceRequestParams,
  SpbsBook_GetAbsence,
  SpbsBook_UpdateAbsenceRequestParams
} from 'projects/sb-api-client/src/api/spbsBook.service';
import {
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';

export type SpbsBookAbsenceViewDialogData = {
  schoolYear: number;
  instId: number;
  recordSchoolYear: number;
  spbsBookRecordId: number;
  orderNum?: number | null;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class SpbsBookAbsenceViewDialogSkeletonComponent extends SkeletonComponentBase {
  d!: SpbsBookAbsenceViewDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: SpbsBookAbsenceViewDialogData,
    spbsBookService: SpbsBookService
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const spbsBookRecordId = data.spbsBookRecordId;
    const recordSchoolYear = data.recordSchoolYear;
    const orderNum = data.orderNum;

    if (orderNum == null) {
      this.resolve(SpbsBookAbsenceViewDialogComponent, {
        schoolYear,
        instId,
        recordSchoolYear,
        spbsBookRecordId,
        orderNum,
        spbsBookRecordAbsence: null
      });
    } else {
      this.resolve(SpbsBookAbsenceViewDialogComponent, {
        schoolYear,
        instId,
        recordSchoolYear,
        spbsBookRecordId,
        orderNum,
        spbsBookRecordAbsence: spbsBookService.getAbsence({
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
  selector: 'sb-spbs-book-absence-view-dialog',
  templateUrl: './spbs-book-absence-view-dialog.component.html'
})
export class SpbsBookAbsenceViewDialogComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    recordSchoolYear: number;
    spbsBookRecordId: number;
    orderNum?: number | null;
    spbsBookRecordAbsence: SpbsBook_GetAbsence | null;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    absenceDate: [null, Validators.required],
    absenceReason: [null, Validators.required]
  });

  saving = false;

  constructor(
    private fb: UntypedFormBuilder,
    private spbsBookService: SpbsBookService,
    private actionService: ActionService,
    private dialogRef: MatDialogRef<SpbsBookAbsenceViewDialogComponent>
  ) {}

  ngOnInit(): void {
    const spbsBookRecordAbsence = this.data.spbsBookRecordAbsence;
    if (spbsBookRecordAbsence) {
      const { absenceDate, absenceReason } = spbsBookRecordAbsence;

      this.form.setValue({
        absenceDate,
        absenceReason
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
              .createAbsence({
                schoolYear: this.data.recordSchoolYear,
                instId: this.data.instId,
                spbsBookRecordId: this.data.spbsBookRecordId,
                createSpbsBookRecordAbsenceCommand: <
                  SpbsBook_CreateAbsenceRequestParams['createSpbsBookRecordAbsenceCommand']
                >this.form.value
              })
              .toPromise()
              .then(() => {
                this.dialogRef.close(true);
              });
          } else {
            return this.spbsBookService
              .updateAbsence({
                schoolYear: this.data.recordSchoolYear,
                instId: this.data.instId,
                spbsBookRecordId: this.data.spbsBookRecordId,
                orderNum: this.data.orderNum,
                updateSpbsBookRecordAbsenceCommand: <
                  SpbsBook_UpdateAbsenceRequestParams['updateSpbsBookRecordAbsenceCommand']
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
