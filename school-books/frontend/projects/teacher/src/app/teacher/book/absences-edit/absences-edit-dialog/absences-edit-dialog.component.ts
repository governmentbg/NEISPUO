import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faChalkboardTeacher as fadChalkboardTeacher } from '@fortawesome/pro-duotone-svg-icons/faChalkboardTeacher';
import { faCog as fadCog } from '@fortawesome/pro-duotone-svg-icons/faCog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTimes as fasTimes } from '@fortawesome/pro-solid-svg-icons/faTimes';
import { add, isAfter } from 'date-fns';
import { ClassBooks_GetStudents } from 'projects/sb-api-client/src/api/classBooks.service';
import { ClassBookStudentNoms_GetNomsByTerm } from 'projects/sb-api-client/src/api/classBookStudentNoms.service';
import { AbsenceType } from 'projects/sb-api-client/src/model/absenceType';
import { ClassBookType } from 'projects/sb-api-client/src/model/classBookType';
import { AbsenceChip } from 'projects/shared/components/absence-chips/absence-chips.component';
import {
  IShouldPreventLeave,
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { LocalStorageService } from 'projects/shared/services/local-storage.service';
import { assert } from 'projects/shared/utils/assert';
import { ClassBookInfoType, UNDO_INTERVAL_IN_MINUTES } from 'projects/shared/utils/book';
import { formatDate, isExpired } from 'projects/shared/utils/date';
import { expiredAt } from 'projects/shared/utils/rxjs';
import { combineLatest, merge, of, Subject } from 'rxjs';
import { filter, take, takeUntil, tap } from 'rxjs/operators';

export type AbsencesEditDialogData = {
  schoolYear: number;
  instId: number;
  classBookId: number;
  date: Date;
  hourNumber: number;
  scheduleLessonId: number;
  classBookInfo: ClassBookInfoType;
  students: ClassBooks_GetStudents;
  individualCurriculumStudents: ClassBookStudentNoms_GetNomsByTerm;
  absenceChips: AbsenceChip[];
  lateChips: AbsenceChip[];
  absences: AbsencesEditDialogDataAbsence[];
  isDplrAbsenceMode: boolean | null;
};

export type AbsencesEditDialogDataAbsence = {
  absenceId: number;
  personId: number;
  type: AbsenceType;
  createDate: Date;
  hasUndoAccess: boolean;
};

export type AbsencesEditDialogResult = {
  absenceChips: AbsenceChip[];
  lateChips: AbsenceChip[];
  chipsForSelection: AbsenceChip[];
  absencesForRemoval: AbsencesEditDialogResultAbsenceForRemoval[];
};

export type AbsencesEditDialogResultAbsenceForRemoval = {
  personId: number;
  classNumber: number;
  isLate: boolean;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class AbsencesEditDialogSkeletonComponent extends SkeletonComponentBase {
  d!: AbsencesEditDialogData;
  r!: AbsencesEditDialogResult;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: AbsencesEditDialogData
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const classBookId = data.classBookId;
    const date = data.date;
    const hourNumber = data.hourNumber;
    const scheduleLessonId = data.scheduleLessonId;
    const classBookInfo = data.classBookInfo;
    const students = data.students;
    const individualCurriculumStudents = data.individualCurriculumStudents;
    const absenceChips = data.absenceChips;
    const lateChips = data.lateChips;
    const absences = data.absences;
    const isDplrAbsenceMode = data.isDplrAbsenceMode;

    this.resolve(AbsencesEditDialogComponent, {
      schoolYear,
      instId,
      classBookId,
      date,
      hourNumber,
      scheduleLessonId,
      classBookInfo,
      students,
      individualCurriculumStudents,
      absenceChips,
      lateChips,
      absences,
      isDplrAbsenceMode
    });
  }
}

type StudentAbsenceForm = {
  personId: FormControl<number>;
  absenceType: FormControl<AbsenceType | null>;
};

@Component({
  selector: 'sb-absences-edit-dialog',
  templateUrl: './absences-edit-dialog.component.html'
})
export class AbsencesEditDialogComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    date: Date;
    hourNumber: number;
    scheduleLessonId: number;
    classBookInfo: ClassBookInfoType;
    students: ClassBooks_GetStudents;
    individualCurriculumStudents: ClassBookStudentNoms_GetNomsByTerm;
    absenceChips: AbsenceChip[];
    lateChips: AbsenceChip[];
    absences: AbsencesEditDialogDataAbsence[];
    isDplrAbsenceMode: boolean | null;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fadChalkboardTeacher = fadChalkboardTeacher;
  readonly fadCog = fadCog;
  readonly fasTimes = fasTimes;

  readonly ABSENCE_TYPE_LATE = AbsenceType.Late;
  readonly ABSENCE_TYPE_UNEXCUSED = AbsenceType.Unexcused;
  readonly ABSENCE_TYPE_EXCUSED = AbsenceType.Excused;
  readonly ABSENCE_TYPE_DPLR_ABSENCE = AbsenceType.DplrAbsence;
  readonly ABSENCE_TYPE_DPLR_ATTENDANCE = AbsenceType.DplrAttendance;

  readonly CLASS_BOOK_TYPE_BOOK_DPLR = ClassBookType.Book_DPLR;

  readonly destroyed$ = new Subject<void>();

  saving = false;
  hasComments = false;
  templateData!: ReturnType<typeof getTemplateData>;
  form!: FormGroup<{
    students: FormArray<FormGroup<StudentAbsenceForm>>;
  }>;
  hourDescriptionShort = '';
  absencesUndoBannerHidden = false;
  isDplr = false;
  absencesKindText = '';

  constructor(
    private fb: FormBuilder,
    public errorStateMatcher: ErrorStateMatcher,
    public localStorageService: LocalStorageService,
    private dialogRef: MatDialogRef<AbsencesEditDialogComponent>
  ) {}

  ngOnInit() {
    this.hourDescriptionShort = `${formatDate(this.data.date)} час #${this.data.hourNumber}`;
    this.isDplr = this.data.classBookInfo.bookType === ClassBookType.Book_DPLR;
    this.absencesKindText = !this.isDplr
      ? 'Можете да правите промени по отсъствията в рамките на 1ч от въвеждането им.'
      : 'Можете да правите промени по присъствията/отсъствията в рамките на 1ч от въвеждането им.';

    this.absencesUndoBannerHidden = this.localStorageService.getAbsencesUndoBannerHidden();
    this.localStorageService.absencesUndoBannerHidden$
      .pipe(
        filter((absencesUndoBannerHidden) => absencesUndoBannerHidden),
        tap(() => this.hideAbsencesUndoBanner()),
        take(1),
        takeUntil(
          merge(
            // unsubscribe if the user hides the banner himself
            this.localStorageService.absencesUndoBannerHidden$.pipe(
              filter((absencesUndoBannerHidden) => absencesUndoBannerHidden)
            ),
            this.destroyed$
          )
        )
      )
      .subscribe();

    this.form = this.fb.group({
      students: this.fb.nonNullable.array<FormGroup<StudentAbsenceForm>>([])
    });

    for (const student of this.data.students) {
      const currentAbsence = this.data.absenceChips.find((a) => a.classNumber === student.classNumber);
      const currentLateAbsence = this.data.lateChips.find((a) => a.classNumber === student.classNumber);

      let absenceType: AbsenceType | null = null;

      if (currentAbsence) {
        absenceType =
          this.data.isDplrAbsenceMode === true
            ? AbsenceType.DplrAbsence
            : this.data.isDplrAbsenceMode === false
            ? AbsenceType.DplrAttendance
            : currentAbsence.excused
            ? AbsenceType.Excused
            : AbsenceType.Unexcused;
      } else if (currentLateAbsence) {
        absenceType = AbsenceType.Late;
      }

      this.studentsFormArray().push(
        this.fb.group<StudentAbsenceForm>({
          personId: this.fb.nonNullable.control(student.personId),
          absenceType: this.fb.control<AbsenceType | null>(absenceType)
        })
      );
    }

    // update visible students
    combineLatest([
      this.localStorageService.bookTransferredHidden$,
      this.localStorageService.bookNotEnrolledHidden$,
      this.localStorageService.bookGradelessHidden$
    ])
      .pipe(
        tap(([bookTransferredHidden, bookNotEnrolledHidden, bookGradelessHidden]) => {
          this.templateData = getTemplateData(
            this.data,
            this.studentsFormArray(),
            bookTransferredHidden,
            bookNotEnrolledHidden,
            bookGradelessHidden,
            this.absencesKindText
          );

          this.data.students.forEach((student, i) => {
            const mappedStudent = this.templateData.students.find((s) => s?.personId === student.personId);

            assert(
              mappedStudent == null || mappedStudent.studentFormIndex === i,
              'Mapped student formIndex not in sync'
            );

            if (mappedStudent) {
              this.studentForm(i).enable();
            } else {
              this.studentForm(i).disable();
            }
          });
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  studentsFormArray() {
    return this.form.get('students') as FormArray<FormGroup<StudentAbsenceForm>>;
  }

  studentForm(i: number) {
    return this.studentsFormArray().at(i) as FormGroup<StudentAbsenceForm>;
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  hideAbsencesUndoBanner() {
    this.absencesUndoBannerHidden = true;
    this.localStorageService.setAbsencesUndoBannerHidden(true);
  }

  getAbsenceType(studentFormIndex: number) {
    return this.studentForm(studentFormIndex).value.absenceType;
  }

  setAbsenceType(studentFormIndex: number, absenceType: AbsenceType | null) {
    this.studentForm(studentFormIndex).patchValue({ absenceType });
  }

  onSave() {
    const absenceChips: AbsenceChip[] = [];
    const lateChips: AbsenceChip[] = [];
    const chipsForSelection: AbsenceChip[] = [];
    const absencesForRemoval: AbsencesEditDialogResultAbsenceForRemoval[] = [];

    const studentClassNumbersMap = new Map(this.data.students.map((s) => [s.personId, s.classNumber]));

    for (const student of this.form.value.students ?? []) {
      assert(student.personId);

      const existingAbsence = this.data.absences.find((a) => a.personId === student.personId);
      const studentClassNumber = studentClassNumbersMap.get(student.personId);
      const existingAbsenceChip = this.data.absenceChips.find((a) => a.classNumber === studentClassNumber);
      const existingLateChip = this.data.lateChips.find((a) => a.classNumber === studentClassNumber);

      const currentAbsenceId = existingAbsenceChip?.absenceId ?? existingAbsenceChip?.absenceId;

      if (
        existingAbsence != null &&
        (!existingAbsence.hasUndoAccess ||
          isExpired(add(existingAbsence.createDate, { minutes: UNDO_INTERVAL_IN_MINUTES })) ||
          existingAbsence.type === student.absenceType)
      ) {
        continue;
      }

      if (
        student.absenceType == null &&
        studentClassNumber &&
        !currentAbsenceId &&
        (existingAbsenceChip || existingLateChip)
      ) {
        absencesForRemoval.push({
          personId: student.personId,
          classNumber: studentClassNumber,
          isLate: existingLateChip != null
        });
      } else if (student.absenceType === AbsenceType.Late && currentAbsenceId) {
        lateChips.push({
          classNumber: studentClassNumber,
          excused: false,
          scheduleLessonId: this.data.scheduleLessonId
        });
      } else if (student.absenceType != null && !existingAbsenceChip && !existingLateChip) {
        (student.absenceType === AbsenceType.Late ? lateChips : absenceChips).push({
          classNumber: studentClassNumber,
          excused: student.absenceType === AbsenceType.Excused,
          scheduleLessonId: this.data.scheduleLessonId
        });
      } else if (existingAbsenceChip) {
        if (existingAbsenceChip?.excused !== (student.absenceType === AbsenceType.Excused))
          chipsForSelection.push({
            classNumber: studentClassNumber,
            excused: student.absenceType === AbsenceType.Unexcused,
            scheduleLessonId: this.data.scheduleLessonId
          });
      }
    }

    this.dialogRef.close({ absenceChips, lateChips, chipsForSelection, absencesForRemoval });
  }
}

function getTemplateData(
  data: AbsencesEditDialogComponent['data'],
  studentsFormArray: FormArray<FormGroup<StudentAbsenceForm>>,
  bookTransferredHidden: boolean,
  bookNotEnrolledHidden: boolean,
  bookGradelessHidden: boolean,
  absencesKindText: string
) {
  const students = data.students
    .map((s, i) => {
      const studentFormValue = studentsFormArray.at(i).value;
      const hasSelectedAbsence = studentFormValue.absenceType != null;
      const currentAbsence = data.absences.find((a) => a.personId === s.personId);
      const currentAbsenceCanUndo = currentAbsence?.hasUndoAccess ?? false;
      const currentAbsenceUndoExpired$ =
        currentAbsence != null
          ? expiredAt(add(currentAbsence.createDate, { minutes: UNDO_INTERVAL_IN_MINUTES }))
          : of(true);

      let tooltipText = '';
      let convertToLateMode = false;

      if (currentAbsence) {
        if (currentAbsence.type === AbsenceType.Excused && currentAbsence.hasUndoAccess) {
          if (currentAbsence.absenceId) {
            if (studentFormValue.absenceType === AbsenceType.Late) {
              tooltipText =
                'Ученикът вече има въведено извинено отсъствие, което не може да бъде конвертирано в закъснение. Ако все пак искате да въведете закъснение изтрийте отсъствието.';
            } else {
              tooltipText = 'Ученикът вече има въведено отсъствие';
            }
          }
        } else if (
          currentAbsence.type === AbsenceType.Unexcused &&
          currentAbsence.hasUndoAccess &&
          !isAfter(new Date(), add(currentAbsence.createDate, { minutes: UNDO_INTERVAL_IN_MINUTES }))
        ) {
          convertToLateMode = true;
        } else {
          tooltipText = 'Ученикът вече има въведено отсъствие';
        }
      }

      const individualCurriculumStudent = data.individualCurriculumStudents.find((a) => a.id === s.personId);

      return {
        ...s,
        convertToLateMode,
        individualCurriculumStudent,
        hasCurrentAbsence: currentAbsence != null,
        currentAbsenceType: currentAbsence?.type,
        currentAbsenceCanUndo,
        currentAbsenceUndoExpired$,
        hasSelectedAbsence,
        studentFormIndex: i,
        abnormalStatus: s.isTransferred ? 'ОТПИСАН' : '',
        tooltipText
      };
    })
    .filter(
      (s) =>
        (s.individualCurriculumStudent == null || s.personId === s.individualCurriculumStudent.id) &&
        (s.hasSelectedAbsence || !bookTransferredHidden || !s.isTransferred)
    );

  return {
    students
  };
}
