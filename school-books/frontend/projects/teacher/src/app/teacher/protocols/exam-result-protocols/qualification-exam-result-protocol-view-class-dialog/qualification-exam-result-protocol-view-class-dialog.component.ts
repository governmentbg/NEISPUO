import { Component, Inject, Input } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { ClassGroupNomsService } from 'projects/sb-api-client/src/api/classGroupNoms.service';
import { QualificationExamResultProtocolsService } from 'projects/sb-api-client/src/api/qualificationExamResultProtocols.service';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';

export type QualificationExamResultProtocolViewClassDialogData = {
  schoolYear: number;
  instId: number;
  qualificationExamResultProtocolId: number;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class QualificationExamResultProtocolViewClassDialogSkeletonComponent extends SkeletonComponentBase {
  d!: QualificationExamResultProtocolViewClassDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: QualificationExamResultProtocolViewClassDialogData
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const qualificationExamResultProtocolId = data.qualificationExamResultProtocolId;

    this.resolve(QualificationExamResultProtocolViewClassDialogComponent, {
      schoolYear,
      instId,
      qualificationExamResultProtocolId
    });
  }
}

@Component({
  selector: 'sb-qualification-exam-result-protocol-view-class-dialog',
  templateUrl: './qualification-exam-result-protocol-view-class-dialog.component.html'
})
export class QualificationExamResultProtocolViewClassDialogComponent {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    qualificationExamResultProtocolId: number;
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
    private qualificationExamResultProtocolsService: QualificationExamResultProtocolsService,
    private actionService: ActionService,
    private dialogRef: MatDialogRef<QualificationExamResultProtocolViewClassDialogComponent>,
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
          return this.qualificationExamResultProtocolsService
            .addStudentsFromClass({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              qualificationExamResultProtocolId: this.data.qualificationExamResultProtocolId,
              addQualificationExamResultProtocolStudentsFromClassCommand: { classId: this.form.value.classId }
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
