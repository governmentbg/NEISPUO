import { Component, Inject, Input, OnDestroy } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { HighSchoolCertificateProtocolsService } from 'projects/sb-api-client/src/api/highSchoolCertificateProtocols.service';
import {
  InstitutionStudentNomsService,
  InstitutionStudentNoms_GetNomsById
} from 'projects/sb-api-client/src/api/institutionStudentNoms.service';
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

export type HighSchoolCertificateProtocolViewDialogData = {
  schoolYear: number;
  instId: number;
  highSchoolCertificateProtocolId: number;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class HighSchoolCertificateProtocolViewDialogSkeletonComponent extends SkeletonComponentBase {
  d!: HighSchoolCertificateProtocolViewDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: HighSchoolCertificateProtocolViewDialogData
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const highSchoolCertificateProtocolId = data.highSchoolCertificateProtocolId;

    this.resolve(HighSchoolCertificateProtocolViewDialogComponent, {
      schoolYear,
      instId,
      highSchoolCertificateProtocolId
    });
  }
}

@Component({
  selector: 'sb-high-school-certificate-protocol-student-view-dialog',
  templateUrl: './high-school-certificate-protocol-student-view-dialog.component.html'
})
export class HighSchoolCertificateProtocolViewDialogComponent implements OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    highSchoolCertificateProtocolId: number;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    studentKeys: [[], Validators.required]
  });

  institutionStudentNomsService: INomService<StudentNomVO, { schoolYear: number; instId: number }>;
  saving = false;
  errors: string[] = [];
  private readonly destroyed$ = new Subject<void>();

  constructor(
    private fb: UntypedFormBuilder,
    private highSchoolCertificateProtocolsService: HighSchoolCertificateProtocolsService,
    private actionService: ActionService,
    private dialogRef: MatDialogRef<HighSchoolCertificateProtocolViewDialogComponent>,
    institutionStudentNomsService: InstitutionStudentNomsService
  ) {
    this.institutionStudentNomsService = new NomServiceWithParams(institutionStudentNomsService, () => ({
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
          return this.highSchoolCertificateProtocolsService
            .createStudent({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              highSchoolCertificateProtocolId: this.data.highSchoolCertificateProtocolId,
              createHighSchoolCertificateProtocolStudentCommand: { students }
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
