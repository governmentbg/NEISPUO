import { Component, Inject, Input } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { ClassGroupNomsService } from 'projects/sb-api-client/src/api/classGroupNoms.service';
import { ExamDutyProtocolsService } from 'projects/sb-api-client/src/api/examDutyProtocols.service';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';

export type ExamDutyProtocolViewClassDialogData = {
  schoolYear: number;
  instId: number;
  examDutyProtocolId: number;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class ExamDutyProtocolViewClassDialogSkeletonComponent extends SkeletonComponentBase {
  d!: ExamDutyProtocolViewClassDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: ExamDutyProtocolViewClassDialogData
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const examDutyProtocolId = data.examDutyProtocolId;

    this.resolve(ExamDutyProtocolViewClassDialogComponent, {
      schoolYear,
      instId,
      examDutyProtocolId
    });
  }
}

@Component({
  selector: 'sb-exam-duty-protocol-view-class-dialog',
  templateUrl: './exam-duty-protocol-view-class-dialog.component.html'
})
export class ExamDutyProtocolViewClassDialogComponent {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    examDutyProtocolId: number;
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
    private examDutyProtocolsService: ExamDutyProtocolsService,
    private actionService: ActionService,
    private dialogRef: MatDialogRef<ExamDutyProtocolViewClassDialogComponent>,
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
          return this.examDutyProtocolsService
            .addStudentsFromClass({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              examDutyProtocolId: this.data.examDutyProtocolId,
              addExamDutyProtocolStudentsFromClassCommand: { classId: this.form.value.classId }
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
