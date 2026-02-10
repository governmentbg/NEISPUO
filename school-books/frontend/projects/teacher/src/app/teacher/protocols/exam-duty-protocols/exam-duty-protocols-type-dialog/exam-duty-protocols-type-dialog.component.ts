import { Component, Inject, Input } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { ExamDutyProtocolTypeNomsService } from 'projects/sb-api-client/src/api/examDutyProtocolTypeNoms.service';
import { ExamDutyProtocolType } from 'projects/sb-api-client/src/model/examDutyProtocolType';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';

export type ExamDutyProtocolsTypeDialogData = {
  schoolYear: number;
  instId: number;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class ExamDutyProtocolsTypeDialogSkeletonComponent extends SkeletonComponentBase {
  d!: ExamDutyProtocolsTypeDialogData;
  r!: ExamDutyProtocolType;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: ExamDutyProtocolsTypeDialogData
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;

    this.resolve(ExamDutyProtocolsTypeDialogComponent, { schoolYear, instId });
  }
}

@Component({
  selector: 'sb-exam-duty-protocols-type-dialog',
  templateUrl: './exam-duty-protocols-type-dialog.component.html'
})
export class ExamDutyProtocolsTypeDialogComponent {
  @Input() data!: {
    schoolYear: number;
    instId: number;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    examDutyProtocolType: [null, Validators.required]
  });

  saving = false;
  examDutyProtocolTypeNomsService: INomService<ExamDutyProtocolType, { instId: number; schoolYear: number }>;

  constructor(
    private fb: UntypedFormBuilder,
    examDutyProtocolTypeNomsService: ExamDutyProtocolTypeNomsService,
    private dialogRef: MatDialogRef<ExamDutyProtocolsTypeDialogComponent>
  ) {
    this.examDutyProtocolTypeNomsService = new NomServiceWithParams(examDutyProtocolTypeNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));
  }

  onSave() {
    if (this.form.invalid || this.saving) {
      return;
    }

    const value = this.form.value;
    this.dialogRef.close(<ExamDutyProtocolType>value.examDutyProtocolType);
  }
}
