import { Component, Inject, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { TopicsService, Topics_GetAdditionalActivity } from 'projects/sb-api-client/src/api/topics.service';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';

export type AdditionalActivityDialogData = {
  schoolYear: number;
  instId: number;
  classBookId: number;
  year: number;
  weekNumber: number;
  additionalActivityId: number | null;
};

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class AdditionalActivityDialogSkeletonComponent extends SkeletonComponentBase {
  d!: AdditionalActivityDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: AdditionalActivityDialogData,
    topicsService: TopicsService
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const classBookId = data.classBookId;
    const year = data.year;
    const weekNumber = data.weekNumber;
    const additionalActivityId = data.additionalActivityId;

    if (additionalActivityId) {
      this.resolve(AdditionalActivityDialogComponent, {
        schoolYear,
        instId,
        classBookId,
        year,
        weekNumber,
        additionalActivity: topicsService.getAdditionalActivity({
          schoolYear,
          instId,
          classBookId,
          additionalActivityId
        })
      });
    } else {
      this.resolve(AdditionalActivityDialogComponent, {
        schoolYear,
        instId,
        classBookId,
        year,
        weekNumber,
        additionalActivity: null
      });
    }
  }
}

@Component({
  selector: 'sb-additional-activity-dialog',
  templateUrl: './additional-activity-dialog.component.html'
})
export class AdditionalActivityDialogComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    year: number;
    weekNumber: number;
    additionalActivity: Topics_GetAdditionalActivity | null;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    activity: [null, Validators.required]
  });

  saving = false;

  constructor(
    private fb: UntypedFormBuilder,
    private topicsService: TopicsService,
    private actionService: ActionService,
    private dialogRef: MatDialogRef<AdditionalActivityDialogSkeletonComponent>
  ) {}

  ngOnInit(): void {
    if (this.data.additionalActivity) {
      this.form.setValue({
        activity: this.data.additionalActivity.activity
      });
    }
  }

  onSave() {
    if (this.form.invalid || this.saving) {
      return;
    }

    this.saving = true;

    const value = this.form.value;
    const additionalActivity = {
      activity: <string>value.activity
    };
    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.additionalActivity == null) {
            return this.topicsService
              .createAdditionalActivity({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                classBookId: this.data.classBookId,
                createAdditionalActivityCommand: {
                  ...additionalActivity,
                  year: this.data.year,
                  weekNumber: this.data.weekNumber
                }
              })
              .toPromise()
              .then(() => {
                this.dialogRef.close(true);
              });
          } else {
            return this.topicsService
              .updateAdditionalActivity({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                classBookId: this.data.classBookId,
                additionalActivityId: this.data.additionalActivity.additionalActivityId,
                updateAdditionalActivityCommand: additionalActivity
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
