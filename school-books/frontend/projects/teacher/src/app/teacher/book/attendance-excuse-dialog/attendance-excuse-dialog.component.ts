import { Component, Inject, Input } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { AbsenceReasonNomsService } from 'projects/sb-api-client/src/api/absenceReasonNoms.service';
import { AttendancesService } from 'projects/sb-api-client/src/api/attendances.service';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';

export type AttendanceExcuseDialogData = {
  schoolYear: number;
  instId: number;
  classBookId: number;
  attendanceId: number;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class AttendanceExcuseDialogSkeletonComponent extends SkeletonComponentBase {
  d!: AttendanceExcuseDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: AttendanceExcuseDialogData
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const classBookId = data.classBookId;
    const attendanceId = data.attendanceId;

    this.resolve(AttendanceExcuseDialogComponent, {
      schoolYear,
      instId,
      classBookId,
      attendanceId
    });
  }
}

@Component({
  selector: 'sb-attendance-excuse-dialog',
  templateUrl: './attendance-excuse-dialog.component.html'
})
export class AttendanceExcuseDialogComponent {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    attendanceId: number;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    excusedReasonId: [null, Validators.required],
    excusedReasonComment: [null]
  });

  saving = false;
  absenceReasonNomsService: INomService<number, { instId: number; schoolYear: number }>;

  constructor(
    private fb: UntypedFormBuilder,
    private actionService: ActionService,
    private attendancesService: AttendancesService,
    private dialogRef: MatDialogRef<AttendanceExcuseDialogComponent>,
    absenceReasonNomsService: AbsenceReasonNomsService
  ) {
    this.absenceReasonNomsService = new NomServiceWithParams(absenceReasonNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));
  }

  onSave() {
    if (this.form.invalid || this.saving) {
      return;
    }

    this.saving = true;

    const value = this.form.value;
    const excusedAbsence = {
      attendanceId: this.data.attendanceId,
      excusedReasonId: <number | null>value.excusedReasonId,
      excusedReasonComment: <string | null>value.excusedReasonComment
    };
    this.actionService
      .execute({
        httpAction: () => {
          return this.attendancesService
            .excuseAttendance({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              excuseAttendanceCommand: excusedAbsence
            })
            .toPromise()
            .then(() => {
              this.dialogRef.close(true);
            });
        }
      })
      .finally(() => {
        this.saving = false;
      });
  }
}
