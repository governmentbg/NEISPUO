import { Component, Inject, Input, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import {
  ClassBookTopicPlanItemsService,
  ClassBookTopicPlanItems_Get
} from 'projects/sb-api-client/src/api/classBookTopicPlanItems.service';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { throwNonNullableFieldError } from 'projects/shared/utils/various';

export type TopicPlansItemDialogData = {
  schoolYear: number;
  instId: number;
  classBookId: number;
  curriculumId: number;
  classBookTopicPlanItemId: number | null;
};

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class TopicPlansItemDialogSkeletonComponent extends SkeletonComponentBase {
  d!: TopicPlansItemDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: TopicPlansItemDialogData,
    classBookTopicPlanItemsService: ClassBookTopicPlanItemsService
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const classBookId = data.classBookId;
    const curriculumId = data.curriculumId;
    const classBookTopicPlanItemId = data.classBookTopicPlanItemId;

    if (classBookTopicPlanItemId) {
      this.resolve(TopicPlansItemDialogComponent, {
        schoolYear,
        instId,
        classBookId,
        curriculumId,
        classBookTopicPlanItemId,
        item: classBookTopicPlanItemsService.get({
          schoolYear,
          instId,
          classBookId,
          classBookTopicPlanItemId
        })
      });
    } else {
      this.resolve(TopicPlansItemDialogComponent, {
        schoolYear,
        instId,
        classBookId,
        curriculumId,
        classBookTopicPlanItemId,
        item: null
      });
    }
  }
}

@Component({
  selector: 'sb-topic-plans-item-dialog',
  templateUrl: './topic-plans-item-dialog.component.html'
})
export class TopicPlansItemDialogComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    curriculumId: number;
    classBookTopicPlanItemId: number | null;
    item: ClassBookTopicPlanItems_Get | null;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.nonNullable.group({
    number: [<number | null | undefined>null, Validators.required],
    title: [<string | null | undefined>null, Validators.required],
    taken: [false],
    note: [<string | null | undefined>null]
  });

  saving = false;

  constructor(
    private fb: FormBuilder,
    private classBookTopicPlanItemsService: ClassBookTopicPlanItemsService,
    private actionService: ActionService,
    private dialogRef: MatDialogRef<TopicPlansItemDialogComponent>
  ) {}

  ngOnInit(): void {
    if (this.data.item != null) {
      this.form.setValue({
        number: this.data.item.number,
        title: this.data.item.title,
        taken: this.data.item.taken,
        note: this.data.item.note
      });
    }
  }

  onSave() {
    if (this.form.invalid || this.saving) {
      return;
    }

    this.saving = true;

    const value = this.form.getRawValue();
    const item = {
      number: value.number ?? throwNonNullableFieldError<typeof value>('number'),
      title: value.title ?? throwNonNullableFieldError<typeof value>('title'),
      taken: value.taken,
      note: value.note
    };
    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.item == null) {
            return this.classBookTopicPlanItemsService
              .create({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                classBookId: this.data.classBookId,
                curriculumId: this.data.curriculumId,
                createClassBookTopicPlanItemCommand: item
              })
              .toPromise()
              .then(() => {
                this.dialogRef.close(true);
              });
          } else {
            return this.classBookTopicPlanItemsService
              .update({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                classBookId: this.data.classBookId,
                curriculumId: this.data.curriculumId,
                classBookTopicPlanItemId: this.data.classBookTopicPlanItemId!,
                updateClassBookTopicPlanItemCommand: item
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
