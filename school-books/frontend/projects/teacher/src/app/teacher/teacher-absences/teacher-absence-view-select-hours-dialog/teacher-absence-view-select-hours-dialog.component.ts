import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faUsersClass as fasUsersClass } from '@fortawesome/pro-solid-svg-icons/faUsersClass';
import { InstTeacherNomsService } from 'projects/sb-api-client/src/api/instTeacherNoms.service';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import { TypedDialog } from 'projects/shared/utils/dialog';
import { throwError } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';
import { MappedSchedule, MappedScheduleCurriculumGroup } from '../teacher-absence-view/teacher-absence-view.component';

export enum ReplType {
  Unset = 'Unset',
  EmptyHour = 'EmptyHour',
  NonSpecialist = 'NonSpecialist',
  Specialist = 'Specialist',
  ExtTeacher = 'ExtTeacher'
}

export type TeacherAbsenceViewSelectHoursDialogData = {
  schoolYear: number;
  instId: number;
  mappedSchedule: MappedSchedule;
};

export type TeacherAbsenceViewSelectHoursDialogResult = {
  replType: ReplType;
  replTeacherPersonId?: number | null;
  replTeacherFirstName?: string | null;
  replTeacherLastName?: string | null;
  extReplTeacherName?: string | null;
  scheduleLessonIds: number[];
};

@Component({
  selector: 'sb-teacher-absence-view-select-hours-dialog.component',
  templateUrl: './teacher-absence-view-select-hours-dialog.component.html',
  styleUrls: ['../teacher-absence-view/teacher-absence-view.component.scss']
})
export class TeacherAbsenceViewSelectHoursDialogComponent
  implements
    TypedDialog<TeacherAbsenceViewSelectHoursDialogData, TeacherAbsenceViewSelectHoursDialogResult>,
    OnInit,
    OnDestroy
{
  d!: TeacherAbsenceViewSelectHoursDialogData;
  r!: TeacherAbsenceViewSelectHoursDialogResult;

  readonly fasCheck = fasCheck;
  readonly fasUsersClass = fasUsersClass;
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

  scheduleLessonIds: number[] = [];

  saving = false;
  instTeacherNomsService: INomService<number, { instId: number; schoolYear: number; includeNonPedagogical: boolean }>;

  readonly form = this.fb.group({
    replType: [ReplType.Unset, Validators.required],
    replTeacherPersonId: [null, Validators.required],
    extReplTeacherName: [null, Validators.required]
  });

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: TeacherAbsenceViewSelectHoursDialogData,
    private dialogRef: MatDialogRef<TeacherAbsenceViewSelectHoursDialogComponent>,
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
    this.form.get('replTeacherPersonId')?.disable();
    this.form.get('extReplTeacherName')?.disable();

    this.form
      .get('replType')
      ?.valueChanges.pipe(
        tap((value) => {
          if (value === ReplType.NonSpecialist || value === ReplType.Specialist) {
            this.form.get('extReplTeacherName')?.disable();
            this.form.get('replTeacherPersonId')?.enable();
          } else if (value === ReplType.Unset || value === ReplType.EmptyHour) {
            this.form.get('replTeacherPersonId')?.disable();
            this.form.get('extReplTeacherName')?.disable();
          } else if (value === ReplType.ExtTeacher) {
            this.form.get('extReplTeacherName')?.enable();
            this.form.get('replTeacherPersonId')?.disable();
          }
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  toggleScheduleLessonSelected(scheduleLessonId: number) {
    const index = this.scheduleLessonIds.indexOf(scheduleLessonId);

    if (index === -1) {
      this.scheduleLessonIds.push(scheduleLessonId);
    } else {
      this.scheduleLessonIds.splice(index, 1);
    }
  }

  checkShowColumns(group: MappedScheduleCurriculumGroup) {
    return !group.isReplHour && group.curriculumHours.some((h) => !h.isInUse);
  }

  checkIsHourUsed(group: MappedScheduleCurriculumGroup, scheduleLessonId: number) {
    return group.absenceHours.some((a) => a.scheduleLessonId === scheduleLessonId);
  }

  onSave() {
    if (this.form.invalid || this.saving) {
      return;
    }

    const value = this.form.value as {
      replType: ReplType;
      replTeacherPersonId?: number | null;
      extReplTeacherName?: string | null;
    };

    if (value.replType === ReplType.Unset || value.replType === ReplType.EmptyHour) {
      this.dialogRef.close({
        replType: value.replType,
        scheduleLessonIds: this.scheduleLessonIds
      });
    } else if (value.replTeacherPersonId) {
      this.saving = true;

      this.baseInstTeacherNomsService
        .getNomsById({
          instId: this.data.instId,
          schoolYear: this.data.schoolYear,
          ids: [value.replTeacherPersonId!]
        })
        .toPromise()
        .then((instTeachers) => {
          const teacher =
            instTeachers.find((t) => t.id === value.replTeacherPersonId) ?? throwError('Teacher must be available');

          this.dialogRef.close({
            replType: value.replType,
            replTeacherPersonId: value.replTeacherPersonId,
            extReplTeacherName: null,
            replTeacherFirstName: teacher.firstName,
            replTeacherLastName: teacher.lastName,
            scheduleLessonIds: this.scheduleLessonIds
          });
        })
        .finally(() => {
          this.saving = false;
        });
    } else if (value.extReplTeacherName) {
      this.dialogRef.close({
        replType: value.replType,
        replTeacherPersonId: null,
        extReplTeacherName: value.extReplTeacherName,
        replTeacherFirstName: null,
        replTeacherLastName: null,
        scheduleLessonIds: this.scheduleLessonIds
      });
    } else {
      this.dialogRef.close(null);
    }
  }
}
