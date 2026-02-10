import { Component, Inject, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { SupportActivityTypeNomsService } from 'projects/sb-api-client/src/api/supportActivityTypeNoms.service';
import { SupportsService, Supports_GetActivity } from 'projects/sb-api-client/src/api/supports.service';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';

export type SupportViewDialogData = {
  schoolYear: number;
  instId: number;
  classBookId: number;
  supportId: number;
  supportActivityId: number | null;
};

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class SupportViewDialogSkeletonComponent extends SkeletonComponentBase {
  d!: SupportViewDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: SupportViewDialogData,
    supportsService: SupportsService
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const classBookId = data.classBookId;
    const supportId = data.supportId;
    const supportActivityId = data.supportActivityId;

    if (supportActivityId) {
      this.resolve(SupportViewDialogComponent, {
        schoolYear,
        instId,
        classBookId,
        supportId,
        supportActivityId,
        activity: supportsService.getActivity({
          schoolYear,
          instId,
          classBookId,
          supportId,
          supportActivityId
        })
      });
    } else {
      this.resolve(SupportViewDialogComponent, {
        schoolYear,
        instId,
        classBookId,
        supportId,
        supportActivityId,
        activity: null
      });
    }
  }
}

@Component({
  selector: 'sb-support-view-dialog',
  templateUrl: './support-view-dialog.component.html'
})
export class SupportViewDialogComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    supportId: number;
    supportActivityId: number | null;
    activity: Supports_GetActivity | null;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    supportActivityTypeId: [null, Validators.required],
    date: [null],
    target: [null],
    result: [null]
  });

  saving = false;
  supportActivityTypeNomsService: INomService<number, { instId: number; schoolYear: number }>;

  constructor(
    private fb: UntypedFormBuilder,
    private supportsService: SupportsService,
    private actionService: ActionService,
    private dialogRef: MatDialogRef<SupportViewDialogComponent>,
    supportActivityTypeNomsService: SupportActivityTypeNomsService
  ) {
    this.supportActivityTypeNomsService = new NomServiceWithParams(supportActivityTypeNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));
  }

  ngOnInit(): void {
    if (this.data.activity != null) {
      this.form.setValue({
        supportActivityTypeId: this.data.activity.supportActivityTypeId,
        date: this.data.activity.date,
        target: this.data.activity.target,
        result: this.data.activity.result
      });
    }
  }

  onSave() {
    if (this.form.invalid || this.saving) {
      return;
    }

    this.saving = true;

    const value = this.form.value;
    const activity = {
      supportActivityTypeId: <number>value.supportActivityTypeId,
      date: <Date | null>value.date,
      target: <string | null>value.target,
      result: <string | null>value.result
    };
    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.activity == null) {
            return this.supportsService
              .createSupportActivity({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                classBookId: this.data.classBookId,
                supportId: this.data.supportId,
                createSupportActivityCommand: activity
              })
              .toPromise()
              .then(() => {
                this.dialogRef.close(true);
              });
          } else {
            if (!this.data.supportActivityId) {
              throw new Error('onUpdate requires an activity to have been loaded.');
            }

            return this.supportsService
              .updateActivity({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                classBookId: this.data.classBookId,
                supportId: this.data.supportId,
                supportActivityId: this.data.supportActivityId,
                updateSupportActivityCommand: activity
              })
              .toPromise()
              .then(() => {
                this.dialogRef.close(true);
              });
          }
        }
      })
      .finally(() => {
        this.saving = false;
      });
  }
}
