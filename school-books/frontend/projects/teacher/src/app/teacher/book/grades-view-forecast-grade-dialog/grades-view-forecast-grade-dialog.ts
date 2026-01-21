import { Component, Inject } from '@angular/core';
import { UntypedFormControl } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { GradesService } from 'projects/sb-api-client/src/api/grades.service';
import { GradeCategory } from 'projects/sb-api-client/src/model/gradeCategory';
import { GradeType } from 'projects/sb-api-client/src/model/gradeType';
import { QualitativeGrade } from 'projects/sb-api-client/src/model/qualitativeGrade';
import { SchoolTerm } from 'projects/sb-api-client/src/model/schoolTerm';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { roundDecimalGrade } from 'projects/shared/utils/book';
import { TypedDialog } from 'projects/shared/utils/dialog';

export type GradesViewForecastGradeDialogData = {
  schoolYear: number;
  instId: number;
  classBookId: number;
  curriculumId: number;
  personId: number;
  subjectTypeIsProfilingSubject: boolean;
  category: GradeCategory;
  type: GradeType;
  term: SchoolTerm;
  decimalGrade?: number | null;
  qualitativeGrade?: QualitativeGrade | null;
};

@Component({
  selector: 'sb-grades-view-forecast-grade-dialog',
  templateUrl: './grades-view-forecast-grade-dialog.html'
})
export class GradesViewForecastGradeDialogComponent implements TypedDialog<GradesViewForecastGradeDialogData, boolean> {
  d!: GradesViewForecastGradeDialogData;
  r!: boolean;

  GRADE_CATEGORY_DECIMAL = GradeCategory.Decimal;
  GRADE_CATEGORY_QUALITATIVE = GradeCategory.Qualitative;

  GRADE_TYPE_TERM = GradeType.Term;

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  decimalGrade = new UntypedFormControl(
    this.data.decimalGrade != null
      ? this.data.subjectTypeIsProfilingSubject
        ? this.data.decimalGrade
        : roundDecimalGrade(this.data.decimalGrade)
      : null
  );
  qualitativeGrade = new UntypedFormControl(this.data.qualitativeGrade);
  loading = false;

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: GradesViewForecastGradeDialogData,
    private dialogRef: MatDialogRef<GradesViewForecastGradeDialogComponent>,
    private gradesService: GradesService
  ) {}

  createForecastGrade() {
    this.loading = true;

    this.gradesService
      .createForecastGrade({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        curriculumId: this.data.curriculumId,
        createForecastGradeCommand: {
          personId: this.data.personId,
          category: this.data.category,
          type: this.data.type,
          term: this.data.term,
          decimalGrade: this.decimalGrade.value as number,
          qualitativeGrade: this.qualitativeGrade.value as QualitativeGrade
        }
      })
      .toPromise()
      .then(() => {
        this.dialogRef.close(true);
      })
      .catch((err) => GlobalErrorHandler.instance.handleError(err))
      .finally(() => (this.loading = false));
  }
}
