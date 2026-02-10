import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import {
  InstitutionCurriculumNomsService,
  InstitutionCurriculumNoms_GetNomsById
} from 'projects/sb-api-client/src/api/institutionCurriculumNoms.service';
import {
  InstitutionStudentNomsService,
  InstitutionStudentNoms_GetNomsById
} from 'projects/sb-api-client/src/api/institutionStudentNoms.service';
import {
  StateExamsAdmProtocolsService,
  StateExamsAdmProtocols_GetStudent
} from 'projects/sb-api-client/src/api/stateExamsAdmProtocols.service';
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
type CurriculumNomVO = ArrayElementType<InstitutionCurriculumNoms_GetNomsById>['id'];

export type StateExamsAdmProtocolViewDialogData = {
  schoolYear: number;
  instId: number;
  stateExamsAdmProtocolId: number;
  studentKey: StudentNomVO | null;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class AddOrUpdateStudentSeAdmProtocolDialogSkeletonComponent extends SkeletonComponentBase {
  d!: StateExamsAdmProtocolViewDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: StateExamsAdmProtocolViewDialogData,
    stateExamsAdmProtocolsService: StateExamsAdmProtocolsService
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const stateExamsAdmProtocolId = data.stateExamsAdmProtocolId;
    const classId = data.studentKey?.classId;
    const personId = data.studentKey?.personId;

    if (classId && personId) {
      this.resolve(AddOrUpdateStudentSeAdmProtocolDialogComponent, {
        schoolYear,
        instId,
        stateExamsAdmProtocolId,
        student: stateExamsAdmProtocolsService.getStudent({
          schoolYear,
          instId,
          stateExamsAdmProtocolId,
          classId,
          personId
        })
      });
    } else {
      this.resolve(AddOrUpdateStudentSeAdmProtocolDialogComponent, {
        schoolYear,
        instId,
        stateExamsAdmProtocolId,
        student: null
      });
    }
  }
}

@Component({
  selector: 'sb-add-or-update-student-se-adm-protocol-dialog',
  templateUrl: './add-or-update-student-se-adm-protocol-dialog.component.html'
})
export class AddOrUpdateStudentSeAdmProtocolDialogComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    stateExamsAdmProtocolId: number;
    student: StateExamsAdmProtocols_GetStudent | null;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    studentKey: [null, Validators.required],
    hasFirstMandatorySubject: [null],
    secondMandatorySubjectKey: [null],
    additionalSubjectKeys: [null],
    qualificationSubjectKeys: [null]
  });

  institutionStudentNomsService: INomService<StudentNomVO, { schoolYear: number; instId: number }>;
  institutionCurriculumNomsService: INomService<CurriculumNomVO, { schoolYear: number; instId: number }>;
  saving = false;
  errors: string[] = [];
  private readonly destroyed$ = new Subject<void>();

  constructor(
    private fb: UntypedFormBuilder,
    private stateExamsAdmProtocolsService: StateExamsAdmProtocolsService,
    private actionService: ActionService,
    private dialogRef: MatDialogRef<AddOrUpdateStudentSeAdmProtocolDialogComponent>,
    institutionStudentNomsService: InstitutionStudentNomsService,
    institutionCurriculumNomsService: InstitutionCurriculumNomsService
  ) {
    this.institutionStudentNomsService = new NomServiceWithParams(institutionStudentNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      showOnlyLastGrade: true
    }));

    this.institutionCurriculumNomsService = new NomServiceWithParams(institutionCurriculumNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));
  }

  ngOnInit(): void {
    const student = this.data.student;
    if (student) {
      this.form.setValue({
        studentKey: <StudentNomVO>{ classId: student.classId, personId: student.personId },
        hasFirstMandatorySubject: student.hasFirstMandatorySubject,
        secondMandatorySubjectKey:
          <CurriculumNomVO | null>student.secondMandatorySubject != null
            ? {
                subjectId: student.secondMandatorySubject?.subjectId,
                subjectTypeId: student.secondMandatorySubject?.subjectTypeId
              }
            : null,
        additionalSubjectKeys: <CurriculumNomVO[]>student.additionalSubjects,
        qualificationSubjectKeys: <CurriculumNomVO[]>student.qualificationSubjects
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
    this.errors = [];

    const value = this.form.value;
    const hasFirstMandatorySubject = <boolean | null>value.hasFirstMandatorySubject ?? false;
    const secondMandatorySubject = <CurriculumNomVO | null>value.secondMandatorySubjectKey;
    const additionalSubjects = <CurriculumNomVO[] | null>value.additionalSubjectKeys ?? [];
    const qualificationSubjects = <CurriculumNomVO[] | null>value.qualificationSubjectKeys ?? [];
    const studentKey =
      this.data.student != null
        ? <StudentNomVO>{ classId: this.data.student.classId, personId: this.data.student.personId }
        : <StudentNomVO>value.studentKey;

    if (
      !hasFirstMandatorySubject &&
      !secondMandatorySubject &&
      additionalSubjects.length === 0 &&
      qualificationSubjects.length === 0
    ) {
      this.errors = ['Ученикa не може да бъде добавян към протокола без нито един изпит'];
      this.saving = false;
      return;
    }

    const student = {
      classId: studentKey.classId,
      personId: studentKey.personId,
      hasFirstMandatorySubject,
      secondMandatorySubject: secondMandatorySubject,
      additionalSubjects,
      qualificationSubjects
    };
    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.student == null) {
            return this.stateExamsAdmProtocolsService
              .createStudent({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                stateExamsAdmProtocolId: this.data.stateExamsAdmProtocolId,
                createStateExamsAdmProtocolStudentCommand: student
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
            return this.stateExamsAdmProtocolsService
              .updateStudent({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                stateExamsAdmProtocolId: this.data.stateExamsAdmProtocolId,
                updateStateExamsAdmProtocolStudentCommand: student
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
