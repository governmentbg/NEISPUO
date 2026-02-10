import { Component, Inject, Input, OnDestroy } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import {
  InstitutionStudentNomsService,
  InstitutionStudentNoms_GetNomsById
} from 'projects/sb-api-client/src/api/institutionStudentNoms.service';
import { NvoExamDutyProtocolsService } from 'projects/sb-api-client/src/api/nvoExamDutyProtocols.service';
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

export type NvoExamDutyProtocolStudentViewDialogData = {
  schoolYear: number;
  instId: number;
  nvoExamDutyProtocolId: number;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class NvoExamDutyProtocolStudentViewDialogSkeletonComponent extends SkeletonComponentBase {
  d!: NvoExamDutyProtocolStudentViewDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: NvoExamDutyProtocolStudentViewDialogData
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const nvoExamDutyProtocolId = data.nvoExamDutyProtocolId;

    this.resolve(NvoExamDutyProtocolStudentViewDialogComponent, {
      schoolYear,
      instId,
      nvoExamDutyProtocolId
    });
  }
}

@Component({
  selector: 'sb-nvo-exam-duty-protocol-student-view-dialog',
  templateUrl: './nvo-exam-duty-protocol-student-view-dialog.component.html'
})
export class NvoExamDutyProtocolStudentViewDialogComponent implements OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    nvoExamDutyProtocolId: number;
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
    private nvoExamDutyProtocolsService: NvoExamDutyProtocolsService,
    private actionService: ActionService,
    private dialogRef: MatDialogRef<NvoExamDutyProtocolStudentViewDialogComponent>,
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
          return this.nvoExamDutyProtocolsService
            .createStudent({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              nvoExamDutyProtocolId: this.data.nvoExamDutyProtocolId,
              createNvoExamDutyProtocolStudentCommand: { students }
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
