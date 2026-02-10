import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import {
  InstitutionStudentNomsService,
  InstitutionStudentNoms_GetNomsById
} from 'projects/sb-api-client/src/api/institutionStudentNoms.service';
import {
  QualificationAcquisitionProtocolsService,
  QualificationAcquisitionProtocols_GetStudent
} from 'projects/sb-api-client/src/api/qualificationAcquisitionProtocols.service';
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

export type QualificationAcquisitionProtocolViewDialogData = {
  schoolYear: number;
  instId: number;
  qualificationAcquisitionProtocolId: number;
  studentKey: StudentNomVO | null;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class QualificationAcquisitionProtocolViewDialogSkeletonComponent extends SkeletonComponentBase {
  d!: QualificationAcquisitionProtocolViewDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: QualificationAcquisitionProtocolViewDialogData,
    qualificationAcquisitionProtocolsService: QualificationAcquisitionProtocolsService
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const qualificationAcquisitionProtocolId = data.qualificationAcquisitionProtocolId;
    const classId = data.studentKey?.classId;
    const personId = data.studentKey?.personId;

    if (classId && personId) {
      this.resolve(QualificationAcquisitionProtocolViewDialogComponent, {
        schoolYear,
        instId,
        qualificationAcquisitionProtocolId,
        student: qualificationAcquisitionProtocolsService.getStudent({
          schoolYear,
          instId,
          qualificationAcquisitionProtocolId,
          classId,
          personId
        })
      });
    } else {
      this.resolve(QualificationAcquisitionProtocolViewDialogComponent, {
        schoolYear,
        instId,
        qualificationAcquisitionProtocolId,
        student: null
      });
    }
  }
}

@Component({
  selector: 'sb-qualification-acquisition-protocol-student-view-dialog',
  templateUrl: './qualification-acquisition-protocol-student-view-dialog.component.html'
})
export class QualificationAcquisitionProtocolViewDialogComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    qualificationAcquisitionProtocolId: number;
    student: QualificationAcquisitionProtocols_GetStudent | null;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    studentKey: [null, Validators.required],
    examsPassed: [null],
    theoryPoints: [null],
    practicePoints: [null],
    averageDecimalGrade: [null]
  });

  institutionStudentNomsService: INomService<StudentNomVO, { schoolYear: number; instId: number }>;
  subjectNomsService: INomService<number, { schoolYear: number; instId: number; classBookId: number }>;
  saving = false;
  errors: string[] = [];
  private readonly destroyed$ = new Subject<void>();

  constructor(
    private fb: UntypedFormBuilder,
    private qualificationAcquisitionProtocolsService: QualificationAcquisitionProtocolsService,
    private actionService: ActionService,
    private dialogRef: MatDialogRef<QualificationAcquisitionProtocolViewDialogComponent>,
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
  }

  ngOnInit(): void {
    const student = this.data.student;
    if (student) {
      this.form.setValue({
        studentKey: <StudentNomVO>{ classId: student.classId, personId: student.personId },
        examsPassed: student.examsPassed,
        theoryPoints: student.theoryPoints,
        practicePoints: student.practicePoints,
        averageDecimalGrade: student.averageDecimalGrade
      });

      this.form.get('studentKey')?.disable();
    }

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
    const studentKey =
      this.data.student != null
        ? <StudentNomVO>{ classId: this.data.student.classId, personId: this.data.student.personId }
        : <StudentNomVO>value.studentKey;

    const examsPassed = <boolean>value.examsPassed ?? false;
    const theoryPoints = <number | null>value.theoryPoints;
    const practicePoints = <number | null>value.practicePoints;
    const averageDecimalGrade = <number | null>value.averageDecimalGrade;

    const student = {
      classId: studentKey.classId,
      personId: studentKey.personId,
      examsPassed,
      theoryPoints,
      practicePoints,
      averageDecimalGrade
    };
    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.student == null) {
            return this.qualificationAcquisitionProtocolsService
              .createStudent({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                qualificationAcquisitionProtocolId: this.data.qualificationAcquisitionProtocolId,
                createQualificationAcquisitionProtocolStudentCommand: student
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
          } else {
            return this.qualificationAcquisitionProtocolsService
              .updateStudent({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                qualificationAcquisitionProtocolId: this.data.qualificationAcquisitionProtocolId,
                updateQualificationAcquisitionProtocolStudentCommand: student
              })
              .toPromise()
              .then(() => {
                this.dialogRef.close(true);
              });
          }
        }
      })
      .finally(() => {
        this.saving = false;
      });
  }
}
