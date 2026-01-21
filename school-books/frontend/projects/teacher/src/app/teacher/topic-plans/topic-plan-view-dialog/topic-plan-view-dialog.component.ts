import { Component, Inject, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { TopicPlanItemsService, TopicPlanItems_GetItem } from 'projects/sb-api-client/src/api/topicPlanItems.service';
import {
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';

export type TopicPlanViewDialogData = {
  schoolYear: number;
  instId: number;
  topicPlanId: number;
  topicPlanItemId: number | null;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class TopicPlanViewDialogSkeletonComponent extends SkeletonComponentBase {
  d!: TopicPlanViewDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: TopicPlanViewDialogData,
    topicPlanItemsService: TopicPlanItemsService
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const topicPlanId = data.topicPlanId;
    const topicPlanItemId = data.topicPlanItemId;

    if (topicPlanItemId) {
      this.resolve(TopicPlanViewDialogComponent, {
        schoolYear,
        instId,
        topicPlanId,
        topicPlanItemId,
        item: topicPlanItemsService.getItem({
          schoolYear,
          instId,
          topicPlanItemId
        })
      });
    } else {
      this.resolve(TopicPlanViewDialogComponent, {
        schoolYear,
        instId,
        topicPlanId,
        topicPlanItemId,
        item: null
      });
    }
  }
}

@Component({
  selector: 'sb-topic-plan-view-dialog',
  templateUrl: './topic-plan-view-dialog.component.html'
})
export class TopicPlanViewDialogComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    topicPlanId: number;
    topicPlanItemId: number | null;
    item: TopicPlanItems_GetItem | null;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    number: [null, Validators.required],
    title: [null, Validators.required],
    note: [null]
  });

  saving = false;

  constructor(
    private fb: UntypedFormBuilder,
    private topicPlanItemsService: TopicPlanItemsService,
    private actionService: ActionService,
    private dialogRef: MatDialogRef<TopicPlanViewDialogComponent>
  ) {}

  ngOnInit(): void {
    if (this.data.item != null) {
      this.form.setValue({
        number: this.data.item.number,
        title: this.data.item.title,
        note: this.data.item.note
      });
    }
  }

  onSave() {
    if (this.form.invalid || this.saving) {
      return;
    }

    this.saving = true;

    const value = this.form.value;

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.item == null) {
            const topicPlanItem = {
              topicPlanId: this.data.topicPlanId,
              number: <number>value.number,
              title: <string>value.title,
              note: <string | null>value.note
            };
            return this.topicPlanItemsService
              .create({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                createTopicPlanItemCommand: topicPlanItem
              })
              .toPromise()
              .then(() => {
                this.dialogRef.close(true);
              });
          } else {
            if (!this.data.topicPlanItemId) {
              throw new Error('onUpdate requires an item to have been loaded.');
            }

            const topicPlanItem = {
              number: <number>value.number,
              title: <string>value.title,
              note: <string | null>value.note
            };

            return this.topicPlanItemsService
              .update({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                topicPlanItemId: this.data.topicPlanItemId,
                updateTopicPlanItemCommand: topicPlanItem
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
