import { Component, Inject, Input } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { ClassGroupNomsService } from 'projects/sb-api-client/src/api/classGroupNoms.service';
import { ExamResultProtocolsService } from 'projects/sb-api-client/src/api/examResultProtocols.service';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';

export type ExamResultProtocolViewClassDialogData = {
  schoolYear: number;
  instId: number;
  examResultProtocolId: number;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class ExamResultProtocolViewClassDialogSkeletonComponent extends SkeletonComponentBase {
  d!: ExamResultProtocolViewClassDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: ExamResultProtocolViewClassDialogData
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const examResultProtocolId = data.examResultProtocolId;

    this.resolve(ExamResultProtocolViewClassDialogComponent, {
      schoolYear,
      instId,
      examResultProtocolId
    });
  }
}

@Component({
  selector: 'sb-exam-result-protocol-view-class-dialog',
  templateUrl: './exam-result-protocol-view-class-dialog.component.html'
})
export class ExamResultProtocolViewClassDialogComponent {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    examResultProtocolId: number;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    classId: [null, Validators.required]
  });

  classGroupNomsService!: INomService<number, { instId: number; schoolYear: number }>;
  saving = false;
  errors: string[] = [];

  constructor(
    private fb: UntypedFormBuilder,
    private examResultProtocolsService: ExamResultProtocolsService,
    private actionService: ActionService,
    private dialogRef: MatDialogRef<ExamResultProtocolViewClassDialogComponent>,
    classGroupNomsService: ClassGroupNomsService
  ) {
    this.classGroupNomsService = new NomServiceWithParams(classGroupNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));
  }

  onSave() {
    if (this.form.invalid || this.saving) {
      return;
    }
    this.saving = true;

    this.actionService
      .execute({
        httpAction: () => {
          return this.examResultProtocolsService
            .addStudentsFromClass({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              examResultProtocolId: this.data.examResultProtocolId,
              addExamResultProtocolStudentsFromClassCommand: { classId: this.form.value.classId }
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
