import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import {
  ClassBooksAdminService,
  ClassBooksAdmin_GetStudent
} from 'projects/sb-api-client/src/api/classBooksAdmin.service';
import { ClassBookType } from 'projects/sb-api-client/src/model/classBookType';
import { INomService, NomServiceFromItems } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwError } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';

export type BookAdminStudentDialogData = {
  schoolYear: number;
  instId: number;
  classBookId: number;
  personId: number;
  bookType: ClassBookType;
  showSpecialNeedFirstGradeResult: boolean;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class BookAdminStudentDialogSkeletonComponent extends SkeletonComponentBase {
  d!: BookAdminStudentDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: BookAdminStudentDialogData,
    classBooksAdminService: ClassBooksAdminService
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const classBookId = data.classBookId;
    const personId = data.personId;
    const bookType = data.bookType;
    const showSpecialNeedFirstGradeResult = data.showSpecialNeedFirstGradeResult;

    this.resolve(BookAdminStudentDialogComponent, {
      schoolYear,
      instId,
      classBookId,
      studentInfo: classBooksAdminService.getStudent({
        schoolYear,
        instId,
        classBookId,
        personId
      }),
      bookType,
      showSpecialNeedFirstGradeResult
    });
  }
}

@Component({
  selector: 'sb-book-admin-student-dialog',
  templateUrl: './book-admin-student-dialog.component.html'
})
export class BookAdminStudentDialogComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    studentInfo: ClassBooksAdmin_GetStudent;
    bookType: ClassBookType;
    showSpecialNeedFirstGradeResult: boolean;
  };

  private readonly destroyed$ = new Subject<void>();

  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasCheck = fasCheck;

  readonly hasSpecialNeedsControl = new FormControl(false, {
    nonNullable: true
  });
  readonly hasCarriedAbsencesControl = new FormControl(false, {
    nonNullable: true
  });

  readonly form = this.fb.nonNullable.group({
    studentFullName: this.fb.nonNullable.control<string | null | undefined>({ value: null, disabled: true }),
    classNumber: this.fb.nonNullable.control<string | null | undefined>(null),
    curriculumIds: this.fb.nonNullable.control<number[]>([]),
    hasSpecialNeedFirstGradeResult: this.fb.nonNullable.control<boolean | null | undefined>(null),
    activities: this.fb.nonNullable.control<string | null | undefined>(null),
    speciality: this.fb.nonNullable.control<string | null | undefined>({ value: null, disabled: true }),
    firstTermExcusedCount: this.fb.nonNullable.control<number | null | undefined>(0),
    firstTermUnexcusedCount: this.fb.nonNullable.control<number | null | undefined>(0),
    firstTermLateCount: this.fb.nonNullable.control<number | null | undefined>(0),
    secondTermExcusedCount: this.fb.nonNullable.control<number | null | undefined>(0),
    secondTermUnexcusedCount: this.fb.nonNullable.control<number | null | undefined>(0),
    secondTermLateCount: this.fb.nonNullable.control<number | null | undefined>(0)
  });

  curriculumNomsService!: INomService<number, { instId: number; schoolYear: number; classBookId: number }>;

  saving = false;
  activitiesFieldName?: string | null;
  hasActivities = false;
  hasSpeciality = false;
  hasGradelessCurriculums = false;
  hasSpecialNeedsCurriculums = false;
  hasCarriedAbsences = false;
  allCurriculumsWithSpecialNeeds = false;
  gradelessCurriculums!: Array<
    ArrayElementType<ClassBooksAdmin_GetStudent['gradelessCurriculumIds']> & { curriculumName: string }
  >;

  constructor(
    private fb: FormBuilder,
    private classBooksAdminService: ClassBooksAdminService,
    private actionService: ActionService,
    private dialogRef: MatDialogRef<BookAdminStudentDialogComponent>
  ) {}

  ngOnInit() {
    this.curriculumNomsService = new NomServiceFromItems<number>(
      this.data.studentInfo.studentCurriculums.map((c) => ({ id: c.curriculumId, name: c.curriculumName }))
    );

    this.hasSpecialNeedsControl.setValue(
      this.data.studentInfo.specialNeedsCurriculumIds.length > 0 || this.data.studentInfo.hasSpecialNeedFirstGradeResult
    );
    this.hasCarriedAbsencesControl.setValue(this.data.studentInfo.carriedAbsences != null);
    this.allCurriculumsWithSpecialNeeds =
      this.data.studentInfo.studentCurriculums.length === this.data.studentInfo.specialNeedsCurriculumIds.length;

    this.hasActivities =
      this.data.bookType === ClassBookType.Book_PG ||
      this.data.bookType === ClassBookType.Book_I_III ||
      this.data.bookType === ClassBookType.Book_IV;

    this.hasGradelessCurriculums = this.hasSpecialNeedsCurriculums =
      this.data.bookType === ClassBookType.Book_I_III ||
      this.data.bookType === ClassBookType.Book_IV ||
      this.data.bookType === ClassBookType.Book_V_XII ||
      this.data.bookType === ClassBookType.Book_CSOP;

    this.hasSpeciality = this.data.bookType === ClassBookType.Book_V_XII;

    this.hasCarriedAbsences =
      this.data.bookType === ClassBookType.Book_I_III ||
      this.data.bookType === ClassBookType.Book_IV ||
      this.data.bookType === ClassBookType.Book_V_XII;

    this.activitiesFieldName =
      this.data.bookType === ClassBookType.Book_PG
        ? 'Дейности по чл. 19'
        : this.data.bookType === ClassBookType.Book_I_III || this.data.bookType === ClassBookType.Book_IV
        ? 'Дейности по интереси'
        : '';

    this.form.setValue({
      studentFullName: this.data.studentInfo.studentFullName,
      classNumber: this.data.studentInfo.classNumber != null ? this.data.studentInfo.classNumber.toString() : null,
      curriculumIds: this.data.studentInfo.gradelessCurriculumIds.map((c) => c.curriculumId),
      hasSpecialNeedFirstGradeResult: this.data.studentInfo.hasSpecialNeedFirstGradeResult,
      activities: this.data.studentInfo.activities,
      speciality: this.data.studentInfo.speciality,
      firstTermExcusedCount: this.data.studentInfo.carriedAbsences?.firstTermExcusedCount ?? 0,
      firstTermUnexcusedCount: this.data.studentInfo.carriedAbsences?.firstTermUnexcusedCount ?? 0,
      firstTermLateCount: this.data.studentInfo.carriedAbsences?.firstTermLateCount ?? 0,
      secondTermExcusedCount: this.data.studentInfo.carriedAbsences?.secondTermExcusedCount ?? 0,
      secondTermUnexcusedCount: this.data.studentInfo.carriedAbsences?.secondTermUnexcusedCount ?? 0,
      secondTermLateCount: this.data.studentInfo.carriedAbsences?.secondTermLateCount ?? 0
    });

    this.syncCurriculums();

    (this.form.get('curriculumIds') ?? throwError('Missing curriculumIds control')).valueChanges
      .pipe(
        tap(() => {
          this.syncCurriculums();
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  toggleAllCurriculumsWithSpecialNeeds() {
    this.allCurriculumsWithSpecialNeeds = !this.allCurriculumsWithSpecialNeeds;

    if (!this.allCurriculumsWithSpecialNeeds) {
      this.data.studentInfo.specialNeedsCurriculumIds = [];
    } else {
      this.data.studentInfo.specialNeedsCurriculumIds = this.data.studentInfo.studentCurriculums.map(
        (c) => c.curriculumId
      );
    }
  }

  toggleCurriculum(curriculumId: number) {
    const curriculumIdIndex = this.data.studentInfo.specialNeedsCurriculumIds.indexOf(curriculumId);

    if (curriculumIdIndex > -1) {
      this.data.studentInfo.specialNeedsCurriculumIds.splice(curriculumIdIndex, 1);
    } else {
      this.data.studentInfo.specialNeedsCurriculumIds.push(curriculumId);
    }

    this.allCurriculumsWithSpecialNeeds =
      this.data.studentInfo.studentCurriculums.length === this.data.studentInfo.specialNeedsCurriculumIds.length;
  }

  syncCurriculums() {
    const curriculumIds = this.form.getRawValue().curriculumIds;

    this.gradelessCurriculums = curriculumIds.map((curriculumId) => {
      const curriculum =
        this.data.studentInfo.studentCurriculums.find(
          (cc: { curriculumId: number }) => cc.curriculumId === curriculumId
        ) ?? throwError('Missing curriculum');

      const dataGc = this.data.studentInfo.gradelessCurriculumIds.find(
        (gc: { curriculumId: number }) => gc.curriculumId === curriculumId
      );
      return {
        curriculumId,
        withoutFirstTermGrade: !!dataGc && dataGc.withoutFirstTermGrade,
        withoutSecondTermGrade: !!dataGc && dataGc.withoutSecondTermGrade,
        withoutFinalGrade: !!dataGc && dataGc.withoutFinalGrade,
        curriculumName: curriculum.curriculumName
      };
    });

    this.data.studentInfo.gradelessCurriculumIds = curriculumIds.map((curriculumId) => {
      const curriculum =
        this.data.studentInfo.studentCurriculums.find(
          (cc: { curriculumId: number }) => cc.curriculumId === curriculumId
        ) ?? throwError('Missing curriculum');
      const dataGc = this.data.studentInfo.gradelessCurriculumIds.find(
        (gc: { curriculumId: number }) => gc.curriculumId === curriculumId
      );
      return (
        dataGc || {
          curriculumId: curriculumId,
          withoutFirstTermGrade: false,
          withoutSecondTermGrade: false,
          withoutFinalGrade: false,
          curriculumName: curriculum.curriculumName
        }
      );
    });
  }

  toggleGc(
    curriculumId: number,
    firstTermGrade: boolean | null,
    secondTermGrade: boolean | null,
    finalGrade: boolean | null
  ) {
    const dataGc = this.data.studentInfo.gradelessCurriculumIds.find((gc) => gc.curriculumId === curriculumId);
    const gc =
      this.gradelessCurriculums.find((gc) => gc.curriculumId === curriculumId) ?? throwError('Missing curriculum');
    if (dataGc != null && firstTermGrade) {
      dataGc.withoutFirstTermGrade = !dataGc.withoutFirstTermGrade;
      gc.withoutFirstTermGrade = !gc.withoutFirstTermGrade;
    }
    if (dataGc != null && secondTermGrade) {
      dataGc.withoutSecondTermGrade = !dataGc.withoutSecondTermGrade;
      gc.withoutSecondTermGrade = !gc.withoutSecondTermGrade;
    }
    if (dataGc != null && finalGrade) {
      // prettier-ignore
      dataGc.withoutFinalGrade = !dataGc.withoutFinalGrade;
      gc.withoutFinalGrade = !gc.withoutFinalGrade;
    }
  }

  onSave() {
    if (this.form.invalid || this.saving) {
      return;
    }
    this.saving = true;

    const specialNeedsCurriculums = this.hasSpecialNeedsControl.value
      ? this.data.studentInfo.specialNeedsCurriculumIds
      : [];

    const hasSpecialNeedFirstGradeResult = this.hasSpecialNeedsControl.value
      ? this.form.value.hasSpecialNeedFirstGradeResult
      : false;

    this.actionService
      .execute({
        httpAction: () => {
          return this.classBooksAdminService
            .updateStudent({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              updateClassBookStudentCommand: {
                personId: this.data.studentInfo.personId,
                classNumber: this.form.value.classNumber != null ? parseInt(this.form.value.classNumber) : null,
                specialNeedCurriculumIds: specialNeedsCurriculums,
                hasSpecialNeedFirstGradeResult: hasSpecialNeedFirstGradeResult,
                gradelessCurriculums: this.data.studentInfo.gradelessCurriculumIds,
                activities: this.form.value.activities,
                carriedAbsences: this.hasCarriedAbsencesControl.value
                  ? {
                      firstTermExcusedCount: this.form.value.firstTermExcusedCount,
                      firstTermUnexcusedCount: this.form.value.firstTermUnexcusedCount,
                      firstTermLateCount: this.form.value.firstTermLateCount,
                      secondTermExcusedCount: this.form.value.secondTermExcusedCount,
                      secondTermUnexcusedCount: this.form.value.secondTermUnexcusedCount,
                      secondTermLateCount: this.form.value.secondTermLateCount
                    }
                  : null
              }
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
