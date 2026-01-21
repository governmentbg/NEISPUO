import { Component, Inject, OnDestroy } from '@angular/core';
import { UntypedFormArray, UntypedFormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { TypedDialog } from 'projects/shared/utils/dialog';
import { notEmpty } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';
import { MappedScheduleCurriculumGroup } from '../lecture-schedule-view/lecture-schedule-view.component';

export type LectureScheduleViewDialogData = {
  schoolYear: number;
  instId: number;
  group: MappedScheduleCurriculumGroup | null;
  hours: {
    date: Date | null;
    scheduleLessonId: number | null;
    isLectureHour: boolean;
    isValid: boolean;
  }[];
  editable: boolean;
};

export type LectureScheduleViewDialogResultHour = {
  scheduleLessonId: number;
  date: Date;
  isLectureHour: boolean;
};

export type LectureScheduleViewDialogResult = LectureScheduleViewDialogResultHour[];

@Component({
  selector: 'sb-lecture-schedule-view-dialog.component',
  templateUrl: './lecture-schedule-view-dialog.component.html'
})
export class LectureScheduleViewDialogComponent
  implements TypedDialog<LectureScheduleViewDialogData, LectureScheduleViewDialogResult>, OnDestroy
{
  d!: LectureScheduleViewDialogData;
  r!: LectureScheduleViewDialogResult;

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly destroyed$ = new Subject<void>();

  saving = false;

  readonly hasInvalidHours = this.data.hours.findIndex((h) => !h.isValid) !== -1;

  readonly form = this.fb.group({
    hours: this.fb.array(
      this.data.hours.map((h) => {
        return this.fb.group({
          scheduleLessonId: [h.scheduleLessonId],
          checked: [h.isLectureHour]
        });
      })
    )
  });

  allLectureScheduleHours = this.data.hours.every((h) => h.isLectureHour);

  get hours(): UntypedFormArray {
    return this.form.get('hours') as UntypedFormArray;
  }

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: LectureScheduleViewDialogData,
    private dialogRef: MatDialogRef<LectureScheduleViewDialogComponent>,
    private fb: UntypedFormBuilder
  ) {}

  toggleAllLectureScheduleHours() {
    this.allLectureScheduleHours = !this.allLectureScheduleHours;

    if (!this.allLectureScheduleHours) {
      this.hours.setValue(
        this.data.hours.map((h) => {
          return {
            scheduleLessonId: h.scheduleLessonId,
            checked: false
          };
        })
      );
    } else {
      this.hours.setValue(
        this.data.hours.map((h) => {
          return {
            scheduleLessonId: h.scheduleLessonId,
            checked: true
          };
        })
      );
    }
  }

  onIsLectureHourChanged() {
    const hours = this.form.value.hours as {
      scheduleLessonId: number;
      checked: boolean;
    }[];

    if (hours.every((h) => h.checked)) {
      this.allLectureScheduleHours = true;
    } else {
      this.allLectureScheduleHours = false;
    }
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  onSave() {
    if (this.form.invalid || this.saving) {
      return;
    }

    const hours = this.form.value.hours as {
      scheduleLessonId: number;
      checked: boolean;
    }[];

    const hoursScheduleLessonIds = hours.filter(notEmpty).map((h) => {
      const dataHour = this.data.hours.find((t) => t.scheduleLessonId === h.scheduleLessonId);
      return { scheduleLessonId: h.scheduleLessonId, date: dataHour!.date, isLectureHour: h.checked };
    });

    this.dialogRef.close(hoursScheduleLessonIds);
  }
}
