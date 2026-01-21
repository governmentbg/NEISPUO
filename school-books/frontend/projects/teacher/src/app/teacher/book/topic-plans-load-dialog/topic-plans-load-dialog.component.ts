import { Component, Inject } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { ClassBookTopicPlanItemsService } from 'projects/sb-api-client/src/api/classBookTopicPlanItems.service';
import { TopicPlanNomsService } from 'projects/sb-api-client/src/api/topicPlanNoms.service';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { TypedDialog } from 'projects/shared/utils/dialog';

export type TopicPlansLoadDialogData = {
  schoolYear: number;
  instId: number;
  classBookId: number;
  curriculumId: number;
};

@Component({
  selector: 'sb-topic-plans-load-dialog',
  templateUrl: './topic-plans-load-dialog.component.html'
})
export class TopicPlansLoadDialogComponent implements TypedDialog<TopicPlansLoadDialogData, boolean> {
  d!: TopicPlansLoadDialogData;
  r!: boolean;

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  loading = false;

  topicPlanNomsService: INomService<number, { instId: number; schoolYear: number }>;

  readonly form = this.fb.group({
    topicPlanId: [null, Validators.required]
  });

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: TopicPlansLoadDialogData,
    private fb: UntypedFormBuilder,
    private dialogRef: MatDialogRef<TopicPlansLoadDialogComponent>,
    private classBookTopicPlanItemsService: ClassBookTopicPlanItemsService,
    private actionService: ActionService,
    topicPlanNomsService: TopicPlanNomsService
  ) {
    this.topicPlanNomsService = new NomServiceWithParams(topicPlanNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));
  }

  load() {
    this.loading = true;

    this.actionService
      .execute({
        httpAction: () => {
          return this.classBookTopicPlanItemsService
            .loadFromTopicPlan({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              curriculumId: this.data.curriculumId,
              topicPlanId: this.form.value.topicPlanId
            })
            .toPromise()
            .then(() => {
              this.dialogRef.close(true);
            });
        }
      })
      .finally(() => {
        this.loading = false;
      });
  }
}
