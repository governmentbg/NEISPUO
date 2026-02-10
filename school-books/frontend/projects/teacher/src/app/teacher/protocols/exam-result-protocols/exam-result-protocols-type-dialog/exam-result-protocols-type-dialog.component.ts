import { Component, Inject, Input } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { ExamResultProtocolTypeNomsService } from 'projects/sb-api-client/src/api/examResultProtocolTypeNoms.service';
import { ExamResultProtocolType } from 'projects/sb-api-client/src/model/examResultProtocolType';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';

export type ExamResultProtocolsTypeDialogData = {
  schoolYear: number;
  instId: number;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class ExamResultProtocolsTypeDialogSkeletonComponent extends SkeletonComponentBase {
  d!: ExamResultProtocolsTypeDialogData;
  r!: ExamResultProtocolType;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: ExamResultProtocolsTypeDialogData
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;

    this.resolve(ExamResultProtocolsTypeDialogComponent, { schoolYear, instId });
  }
}

@Component({
  selector: 'sb-exam-result-protocols-type-dialog',
  templateUrl: './exam-result-protocols-type-dialog.component.html'
})
export class ExamResultProtocolsTypeDialogComponent {
  @Input() data!: {
    schoolYear: number;
    instId: number;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    examResultProtocolType: [null, Validators.required]
  });

  saving = false;
  examResultProtocolTypeNomsService: INomService<ExamResultProtocolType, { instId: number; schoolYear: number }>;

  constructor(
    private fb: UntypedFormBuilder,
    examResultProtocolTypeNomsService: ExamResultProtocolTypeNomsService,
    private dialogRef: MatDialogRef<ExamResultProtocolsTypeDialogComponent>
  ) {
    this.examResultProtocolTypeNomsService = new NomServiceWithParams(examResultProtocolTypeNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));
  }

  onSave() {
    if (this.form.invalid || this.saving) {
      return;
    }

    const value = this.form.value;
    this.dialogRef.close(<ExamResultProtocolType>value.examResultProtocolType);
  }
}
