import { Component, Inject, Input } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { ClassBookStudentNomsService } from 'projects/sb-api-client/src/api/classBookStudentNoms.service';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';

export type ScheduleIndividualDialogData = {
  schoolYear: number;
  instId: number;
  classBookId: number;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class ScheduleIndividualDialogSkeletonComponent extends SkeletonComponentBase {
  d!: ScheduleIndividualDialogData;
  r!: number;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: ScheduleIndividualDialogData
  ) {
    super();

    const { schoolYear, instId, classBookId } = data;

    this.resolve(ScheduleIndividualDialogComponent, {
      schoolYear,
      instId,
      classBookId
    });
  }
}

@Component({
  selector: 'sb-schedule-individual-dialog',
  templateUrl: './schedule-individual-dialog.component.html'
})
export class ScheduleIndividualDialogComponent {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    studentId: [null, Validators.required]
  });

  saving = false;

  classBookStudentNomsService: INomService<number, { schoolYear: number; instId: number; classBookId: number }>;

  constructor(
    private fb: UntypedFormBuilder,
    classBookStudentNomsService: ClassBookStudentNomsService,
    private dialogRef: MatDialogRef<ScheduleIndividualDialogComponent>
  ) {
    this.classBookStudentNomsService = new NomServiceWithParams(classBookStudentNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      showOnlyWithIndividualCurriculum: true
    }));
  }

  onSave() {
    if (this.form.invalid || this.saving) {
      return;
    }

    this.saving = true;

    this.dialogRef.close(this.form.value.studentId);
  }
}
