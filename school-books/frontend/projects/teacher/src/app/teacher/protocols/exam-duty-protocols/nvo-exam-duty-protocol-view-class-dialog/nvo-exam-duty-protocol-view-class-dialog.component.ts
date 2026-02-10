import { Component, Inject, Input } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { ClassGroupNomsService } from 'projects/sb-api-client/src/api/classGroupNoms.service';
import { NvoExamDutyProtocolsService } from 'projects/sb-api-client/src/api/nvoExamDutyProtocols.service';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';

export type NvoExamDutyProtocolViewClassDialogData = {
  schoolYear: number;
  instId: number;
  nvoExamDutyProtocolId: number;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class NvoExamDutyProtocolViewClassDialogSkeletonComponent extends SkeletonComponentBase {
  d!: NvoExamDutyProtocolViewClassDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: NvoExamDutyProtocolViewClassDialogData
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const nvoExamDutyProtocolId = data.nvoExamDutyProtocolId;

    this.resolve(NvoExamDutyProtocolViewClassDialogComponent, {
      schoolYear,
      instId,
      nvoExamDutyProtocolId
    });
  }
}

@Component({
  selector: 'sb-nvo-exam-duty-protocol-view-class-dialog',
  templateUrl: './nvo-exam-duty-protocol-view-class-dialog.component.html'
})
export class NvoExamDutyProtocolViewClassDialogComponent {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    nvoExamDutyProtocolId: number;
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
    private nvoExamDutyProtocolsService: NvoExamDutyProtocolsService,
    private actionService: ActionService,
    private dialogRef: MatDialogRef<NvoExamDutyProtocolViewClassDialogComponent>,
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
          return this.nvoExamDutyProtocolsService
            .addStudentsFromClass({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              nvoExamDutyProtocolId: this.data.nvoExamDutyProtocolId,
              addNvoExamDutyProtocolStudentsFromClassCommand: { classId: this.form.value.classId }
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
