import { Component, Inject } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { AbsenceReasonNomsService } from 'projects/sb-api-client/src/api/absenceReasonNoms.service';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import { TypedDialog } from 'projects/shared/utils/dialog';

export type AbsencesExcuseDialogData = {
  schoolYear: number;
  instId: number;
};

export type AbsencesExcuseDialogResult = {
  excusedReasonId: number;
  excusedReasonComment: string | null;
};

@Component({
  selector: 'sb-absences-excuse-dialog',
  templateUrl: './absences-excuse-dialog.component.html'
})
export class AbsencesExcuseDialogComponent
  implements TypedDialog<AbsencesExcuseDialogData, AbsencesExcuseDialogResult>
{
  d!: AbsencesExcuseDialogData;
  r!: AbsencesExcuseDialogResult;

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    excusedReasonId: [null, Validators.required],
    excusedReasonComment: [null]
  });

  saving = false;
  absenceReasonNomsService: INomService<number, { instId: number; schoolYear: number }>;

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: AbsencesExcuseDialogData,
    private fb: UntypedFormBuilder,
    private dialogRef: MatDialogRef<AbsencesExcuseDialogComponent>,
    absenceReasonNomsService: AbsenceReasonNomsService
  ) {
    this.absenceReasonNomsService = new NomServiceWithParams(absenceReasonNomsService, () => ({
      schoolYear: data.schoolYear,
      instId: data.instId
    }));
  }

  onSave() {
    if (this.form.invalid || this.saving) {
      return;
    }

    this.saving = true;

    const value = this.form.value;
    const result = {
      excusedReasonId: <number | null>value.excusedReasonId,
      excusedReasonComment: <string | null>value.excusedReasonComment
    };

    this.dialogRef.close(result);
  }
}
