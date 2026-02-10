import { Component, Inject, Input } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import {
  InstitutionStudentNomsService,
  InstitutionStudentNoms_GetNomsById
} from 'projects/sb-api-client/src/api/institutionStudentNoms.service';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ArrayElementType } from 'projects/shared/utils/type';

type StudentNomVO = ArrayElementType<InstitutionStudentNoms_GetNomsById>['id'];

export type SpbsBookNewDialogData = {
  schoolYear: number;
  instId: number;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class SpbsBookNewDialogSkeletonComponent extends SkeletonComponentBase {
  d!: SpbsBookNewDialogData;
  r!: StudentNomVO;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: SpbsBookNewDialogData
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;

    this.resolve(SpbsBookNewDialogComponent, { schoolYear, instId });
  }
}

@Component({
  selector: 'sb-spbs-book-new-dialog',
  templateUrl: './spbs-book-new-dialog.component.html'
})
export class SpbsBookNewDialogComponent {
  @Input() data!: {
    schoolYear: number;
    instId: number;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    studentKey: [null, Validators.required]
  });

  saving = false;

  institutionStudentNomsService: INomService<StudentNomVO, { schoolYear: number; instId: number }>;

  constructor(
    private fb: UntypedFormBuilder,
    institutionStudentNomsService: InstitutionStudentNomsService,
    private dialogRef: MatDialogRef<SpbsBookNewDialogComponent>
  ) {
    this.institutionStudentNomsService = new NomServiceWithParams(institutionStudentNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));
  }

  onSave() {
    if (this.form.invalid || this.saving) {
      return;
    }
    this.saving = true;

    const value = this.form.value;

    const student = {
      classId: (<StudentNomVO>value.studentKey).classId,
      personId: (<StudentNomVO>value.studentKey).personId
    };

    this.dialogRef.close(student);
  }
}
