import { Component, Inject, Input } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { AbsenceReasonNomsService } from 'projects/sb-api-client/src/api/absenceReasonNoms.service';
import { AbsencesService } from 'projects/sb-api-client/src/api/absences.service';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';

export type AbsenceExcuseDialogData = {
  schoolYear: number;
  instId: number;
  classBookId: number;
  absenceId: number;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class AbsenceExcuseDialogSkeletonComponent extends SkeletonComponentBase {
  d!: AbsenceExcuseDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: AbsenceExcuseDialogData
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const classBookId = data.classBookId;
    const absenceId = data.absenceId;

    this.resolve(AbsenceExcuseDialogComponent, {
      schoolYear,
      instId,
      classBookId,
      absenceId
    });
  }
}

@Component({
  selector: 'sb-absence-excuse-dialog',
  templateUrl: './absence-excuse-dialog.component.html'
})
export class AbsenceExcuseDialogComponent {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    absenceId: number;
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
    private absencesService: AbsencesService,
    private dialogRef: MatDialogRef<AbsenceExcuseDialogComponent>,
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
      excusedReasonId: <number | null>value.excusedReasonId,
      excusedReasonComment: <string | null>value.excusedReasonComment
    };
    this.actionService
      .execute({
        httpAction: () => {
          return this.absencesService
            .excuseAbsence({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              absenceId: this.data.absenceId,
              excuseAbsenceCommand: excusedAbsence
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
