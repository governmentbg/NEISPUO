import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormArray, UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { InstTeacherNomsService } from 'projects/sb-api-client/src/api/instTeacherNoms.service';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import { TypedDialog } from 'projects/shared/utils/dialog';
import { notEmpty, throwError } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';
import { MappedScheduleCurriculumGroup } from '../teacher-absence-view/teacher-absence-view.component';

export enum ReplType {
  Unset = 'Unset',
  EmptyHour = 'EmptyHour',
  NonSpecialist = 'NonSpecialist',
  Specialist = 'Specialist',
  ExtTeacher = 'ExtTeacher'
}

export type TeacherAbsenceViewDialogData = {
  schoolYear: number;
  instId: number;
  group: MappedScheduleCurriculumGroup | null;
  hours: {
    date: Date | null;
    scheduleLessonId: number | null;
    replType: ReplType;
    replTeacherPersonId?: number | null;
    isReadOnly: boolean;
    isValid: boolean;
    extReplTeacherName?: string | null;
    hasNoReplacementTeacher: boolean;
  }[];
  editable: boolean;
  singleHourMode: boolean;
};

export type TeacherAbsenceViewDialogResultHour = {
  scheduleLessonId: number;
  replType: ReplType;
  replTeacherPersonId?: number | null;
  replTeacherFirstName?: string | null;
  replTeacherLastName?: string | null;
  extReplTeacherName?: string | null;
};

export type TeacherAbsenceViewDialogResult = TeacherAbsenceViewDialogResultHour[];

@Component({
  selector: 'sb-teacher-absence-view-dialog.component',
  templateUrl: './teacher-absence-view-dialog.component.html'
})
export class TeacherAbsenceViewDialogComponent
  implements TypedDialog<TeacherAbsenceViewDialogData, TeacherAbsenceViewDialogResult>, OnInit, OnDestroy
{
  d!: TeacherAbsenceViewDialogData;
  r!: TeacherAbsenceViewDialogResult;

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly ReplTypeNonSpecialist = ReplType.NonSpecialist;
  readonly ReplTypeSpecialist = ReplType.Specialist;
  readonly ReplTypeExtTeacher = ReplType.ExtTeacher;

  readonly destroyed$ = new Subject<void>();

  readonly replTypes = [
    { id: ReplType.Unset, text: 'Без заместник - Учителя присъства' },
    { id: ReplType.EmptyHour, text: 'Без заместник - Свободен час' },
    { id: ReplType.NonSpecialist, text: 'Заместник неспециалист - Час по ГО' },
    { id: ReplType.Specialist, text: 'Заместник по предмета' },
    { id: ReplType.ExtTeacher, text: 'Външен лектор' }
  ];

  saving = false;
  instTeacherNomsService: INomService<number, { instId: number; schoolYear: number; includeNonPedagogical: boolean }>;

  readonly hasReadOnlyHours = this.data.hours.findIndex((h) => h.isReadOnly) !== -1;
  readonly hasNoReplacementHours = this.data.hours.findIndex((h) => h.hasNoReplacementTeacher) !== -1;
  readonly hasInvalidHours = this.data.hours.findIndex((h) => !h.isValid) !== -1;

  readonly form = this.fb.group({
    hours: this.fb.array(
      this.data.hours.map((h) => {
        return this.fb.group({
          scheduleLessonId: [h.scheduleLessonId],
          replType: [h.replType, Validators.required],
          replTeacherPersonId: [
            {
              value: h.replTeacherPersonId,
              disabled:
                h.replType === ReplType.Unset || h.replType === ReplType.EmptyHour || h.replType === ReplType.ExtTeacher
            },
            Validators.required
          ],
          extReplTeacherName: [h.extReplTeacherName]
        });
      })
    )
  });

  get hours(): UntypedFormArray {
    return this.form.get('hours') as UntypedFormArray;
  }

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: TeacherAbsenceViewDialogData,
    private dialogRef: MatDialogRef<TeacherAbsenceViewDialogComponent>,
    private fb: UntypedFormBuilder,
    private baseInstTeacherNomsService: InstTeacherNomsService
  ) {
    this.instTeacherNomsService = new NomServiceWithParams(this.baseInstTeacherNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear,
      includeNonPedagogical: true
    }));
  }

  ngOnInit() {
    for (const hourFormGroup of this.hours.controls) {
      hourFormGroup
        .get('replType')
        ?.valueChanges.pipe(
          tap((value) => {
            if (value !== ReplType.Unset && value !== ReplType.EmptyHour && value !== ReplType.ExtTeacher) {
              hourFormGroup.get('extReplTeacherName')?.disable();
              hourFormGroup.get('replTeacherPersonId')?.enable();
            } else if (value === ReplType.EmptyHour) {
              hourFormGroup.get('replTeacherPersonId')?.disable();
              hourFormGroup.get('extReplTeacherName')?.disable();
            } else {
              hourFormGroup.get('extReplTeacherName')?.enable();
              hourFormGroup.get('replTeacherPersonId')?.disable();
            }
          }),
          takeUntil(this.destroyed$)
        )
        .subscribe();
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
      replType: ReplType;
      replTeacherPersonId?: number | null;
      extReplTeacherName?: string | null;
    }[];

    const hoursTeacherPersonIds = hours.map((h) => h.replTeacherPersonId).filter(notEmpty);

    if (hoursTeacherPersonIds.length > 0) {
      this.saving = true;

      this.baseInstTeacherNomsService
        .getNomsById({
          instId: this.data.instId,
          schoolYear: this.data.schoolYear,
          ids: hours.map((h) => h.replTeacherPersonId).filter(notEmpty)
        })
        .toPromise()
        .then((instTeachers) => {
          const hoursWithTeacherNames: TeacherAbsenceViewDialogResult = hours.map((h) => {
            if (!h.replTeacherPersonId) {
              return h;
            }

            const teacher =
              instTeachers.find((t) => t.id === h.replTeacherPersonId) ?? throwError('Teacher must be available');

            return {
              ...h,
              replTeacherFirstName: teacher.firstName,
              replTeacherLastName: teacher.lastName
            };
          });

          this.dialogRef.close(hoursWithTeacherNames);
        })
        .finally(() => {
          this.saving = false;
        });
    } else {
      this.dialogRef.close(hours);
    }
  }
}
