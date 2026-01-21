import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { ClassBooksAdminService } from 'projects/sb-api-client/src/api/classBooksAdmin.service';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { TypedDialog } from 'projects/shared/utils/dialog';

export type BookAdminCurriculumItemDialogData = {
  schoolYear: number;
  instId: number;
  classBookId: number;
  curriculumId: number;
  subjectName: string;
  subjectTypeName: string;
  teacherNames: string;
  withoutGrade: boolean;
};

@Component({
  selector: 'sb-book-admin-curriculum-item-dialog',
  templateUrl: './book-admin-curriculum-item-dialog.component.html'
})
export class BookAdminCurriculumItemDialogComponent
  implements TypedDialog<BookAdminCurriculumItemDialogData, void>, OnInit
{
  d!: BookAdminCurriculumItemDialogData;
  r!: void;

  readonly fasFileExcel = fasFileExcel;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasCheck = fasCheck;
  readonly form = this.fb.group({
    subjectName: [{ value: null, disabled: true }],
    subjectTypeName: [{ value: null, disabled: true }],
    teacherNames: [{ value: null, disabled: true }],
    withoutGrade: [null]
  });

  loading = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: BookAdminCurriculumItemDialogData,
    private dialogRef: MatDialogRef<BookAdminCurriculumItemDialogComponent>,
    private classBooksAdminService: ClassBooksAdminService,
    private fb: UntypedFormBuilder
  ) {}

  update() {
    this.loading = true;
    const value = this.form.value;

    this.classBooksAdminService
      .updateCurriculumItem({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        updateClassBookCurriculumCommand: {
          curriculumId: this.data.curriculumId,
          withoutGrade: value.withoutGrade
        }
      })
      .toPromise()
      .then(() => {
        this.dialogRef.close(true);
      })
      .catch((err) => GlobalErrorHandler.instance.handleError(err))
      .finally(() => (this.loading = false));
  }

  ngOnInit() {
    this.form.setValue({
      subjectName: this.data.subjectName,
      subjectTypeName: this.data.subjectTypeName,
      teacherNames: this.data.teacherNames,
      withoutGrade: this.data.withoutGrade
    });
  }
}
