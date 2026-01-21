import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, NonNullableFormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import {
  ClassBooksAdminService,
  ClassBooksAdmin_GetStudentNumbers
} from 'projects/sb-api-client/src/api/classBooksAdmin.service';
import {
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { Subject } from 'rxjs';

export type BookAdminStudentNumbersDialogData = {
  schoolYear: number;
  instId: number;
  classBookId: number;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class BookAdminStudentNumbersDialogSkeletonComponent extends SkeletonComponentBase {
  d!: BookAdminStudentNumbersDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: BookAdminStudentNumbersDialogData,
    classBooksAdminService: ClassBooksAdminService
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const classBookId = data.classBookId;

    this.resolve(BookAdminStudentNumbersDialogComponent, {
      schoolYear,
      instId,
      classBookId,
      studentNumbers: classBooksAdminService.getStudentNumbers({
        schoolYear,
        instId,
        classBookId
      })
    });
  }
}

function createStudentForm(fb: NonNullableFormBuilder, personId: number, classNumber?: number | null) {
  return fb.group({
    personId: [personId],
    classNumber: [classNumber]
  });
}
type StudentForm = ReturnType<typeof createStudentForm>;

@Component({
  selector: 'sb-book-admin-student-numbers-dialog',
  templateUrl: './book-admin-student-numbers-dialog.component.html'
})
export class BookAdminStudentNumbersDialogComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    studentNumbers: ClassBooksAdmin_GetStudentNumbers;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly destroyed$ = new Subject<void>();

  readonly form = this.fb.nonNullable.group({
    studentNumbersArray: new FormArray<StudentForm>([])
  });

  saving = false;

  constructor(
    private fb: FormBuilder,
    private classBooksAdminService: ClassBooksAdminService,
    private actionService: ActionService,
    private dialogRef: MatDialogRef<BookAdminStudentNumbersDialogComponent>
  ) {}

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  ngOnInit(): void {
    const studentsForm = this.form.get('studentNumbersArray') as FormArray<StudentForm>;
    for (const student of this.data.studentNumbers) {
      studentsForm.push(createStudentForm(this.fb.nonNullable, student.personId, student.classNumber));
    }
  }

  onSave() {
    if (this.form.invalid || this.saving) {
      return;
    }

    this.saving = true;

    this.actionService
      .execute({
        httpAction: () => {
          return this.classBooksAdminService
            .updateStudentNumbers({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              updateClassBookStudentNumbersCommand: {
                students: this.form.getRawValue().studentNumbersArray
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
