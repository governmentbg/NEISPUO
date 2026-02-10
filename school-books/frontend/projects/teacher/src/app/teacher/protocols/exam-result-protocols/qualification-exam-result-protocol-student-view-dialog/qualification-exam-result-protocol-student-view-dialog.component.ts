import { Component, Inject, Input, OnDestroy } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import {
  InstitutionStudentNomsService,
  InstitutionStudentNoms_GetNomsById
} from 'projects/sb-api-client/src/api/institutionStudentNoms.service';
import { QualificationExamResultProtocolsService } from 'projects/sb-api-client/src/api/qualificationExamResultProtocols.service';
import { SubjectNomsService } from 'projects/sb-api-client/src/api/subjectNoms.service';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { ArrayElementType } from 'projects/shared/utils/type';
import { Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';

type StudentNomVO = ArrayElementType<InstitutionStudentNoms_GetNomsById>['id'];

export type QualificationExamResultProtocolViewDialogData = {
  schoolYear: number;
  instId: number;
  qualificationExamResultProtocolId: number;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class QualificationExamResultProtocolViewDialogSkeletonComponent extends SkeletonComponentBase {
  d!: QualificationExamResultProtocolViewDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: QualificationExamResultProtocolViewDialogData
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const qualificationExamResultProtocolId = data.qualificationExamResultProtocolId;

    this.resolve(QualificationExamResultProtocolViewDialogComponent, {
      schoolYear,
      instId,
      qualificationExamResultProtocolId
    });
  }
}

@Component({
  selector: 'sb-qualification-exam-result-protocol-student-view-dialog',
  templateUrl: './qualification-exam-result-protocol-student-view-dialog.component.html'
})
export class QualificationExamResultProtocolViewDialogComponent implements OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    qualificationExamResultProtocolId: number;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    studentKeys: [[], Validators.required]
  });

  institutionStudentNomsService: INomService<StudentNomVO, { schoolYear: number; instId: number }>;
  subjectNomsService: INomService<number, { schoolYear: number; instId: number; classBookId: number }>;
  saving = false;
  errors: string[] = [];
  private readonly destroyed$ = new Subject<void>();

  constructor(
    private fb: UntypedFormBuilder,
    private qualificationExamResultProtocolsService: QualificationExamResultProtocolsService,
    private actionService: ActionService,
    private dialogRef: MatDialogRef<QualificationExamResultProtocolViewDialogComponent>,
    institutionStudentNomsService: InstitutionStudentNomsService,
    subjectNomsService: SubjectNomsService
  ) {
    this.institutionStudentNomsService = new NomServiceWithParams(institutionStudentNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));

    this.subjectNomsService = new NomServiceWithParams(subjectNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));

    this.form.valueChanges
      .pipe(
        tap(() => {
          this.errors = [];
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  onSave() {
    if (this.form.invalid || this.saving) {
      return;
    }
    this.saving = true;

    const value = this.form.value;

    const students = <StudentNomVO[] | null>value.studentKeys ?? [];

    this.actionService
      .execute({
        httpAction: () => {
          return this.qualificationExamResultProtocolsService
            .createStudent({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              qualificationExamResultProtocolId: this.data.qualificationExamResultProtocolId,
              createQualificationExamResultProtocolStudentCommand: { students }
            })
            .toPromise()
            .then(() => {
              this.dialogRef.close(true);
            })
            .catch((err) => {
              if (err.status === 400 && err.error?.errorMessages?.length > 0) {
                this.errors = err.error.errorMessages;
                this.saving = false;
                return;
              }
              // let the action service present the error
              return Promise.reject(err);
            });
        }
      })
      .finally(() => {
        this.saving = false;
      });
  }
}
