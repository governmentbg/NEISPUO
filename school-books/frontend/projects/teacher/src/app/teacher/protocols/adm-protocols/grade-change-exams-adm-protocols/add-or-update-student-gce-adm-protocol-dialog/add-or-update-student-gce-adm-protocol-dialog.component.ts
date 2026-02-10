import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import {
  GradeChangeExamsAdmProtocolsService,
  GradeChangeExamsAdmProtocols_GetStudent
} from 'projects/sb-api-client/src/api/gradeChangeExamsAdmProtocols.service';
import {
  InstitutionCurriculumNomsService,
  InstitutionCurriculumNoms_GetNomsById
} from 'projects/sb-api-client/src/api/institutionCurriculumNoms.service';
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
type CurriculumNomVO = ArrayElementType<InstitutionCurriculumNoms_GetNomsById>['id'];

export type GradeChangeExamsAdmProtocolViewDialogData = {
  schoolYear: number;
  instId: number;
  gradeChangeExamsAdmProtocolId: number;
  studentKey: StudentNomVO | null;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class AddOrUpdateStudentGceAdmProtocolDialogSkeletonComponent extends SkeletonComponentBase {
  d!: GradeChangeExamsAdmProtocolViewDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: GradeChangeExamsAdmProtocolViewDialogData,
    gradeChangeExamsAdmProtocolsService: GradeChangeExamsAdmProtocolsService
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const gradeChangeExamsAdmProtocolId = data.gradeChangeExamsAdmProtocolId;
    const classId = data.studentKey?.classId;
    const personId = data.studentKey?.personId;

    if (classId && personId) {
      this.resolve(AddOrUpdateStudentGceAdmProtocolDialogComponent, {
        schoolYear,
        instId,
        gradeChangeExamsAdmProtocolId,
        student: gradeChangeExamsAdmProtocolsService.getStudent({
          schoolYear,
          instId,
          gradeChangeExamsAdmProtocolId,
          classId,
          personId
        })
      });
    } else {
      this.resolve(AddOrUpdateStudentGceAdmProtocolDialogComponent, {
        schoolYear,
        instId,
        gradeChangeExamsAdmProtocolId,
        student: null
      });
    }
  }
}

@Component({
  selector: 'sb-add-or-update-student-gce-adm-protocol-dialog',
  templateUrl: './add-or-update-student-gce-adm-protocol-dialog.component.html'
})
export class AddOrUpdateStudentGceAdmProtocolDialogComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    gradeChangeExamsAdmProtocolId: number;
    student: GradeChangeExamsAdmProtocols_GetStudent | null;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    studentKey: [null, Validators.required],
    subjectKeys: [null, Validators.required]
  });

  institutionStudentNomsService: INomService<StudentNomVO, { schoolYear: number; instId: number }>;
  institutionCurriculumNomsService: INomService<CurriculumNomVO, { schoolYear: number; instId: number }>;
  saving = false;
  errors: string[] = [];
  private readonly destroyed$ = new Subject<void>();

  constructor(
    private fb: UntypedFormBuilder,
    private gradeChangeExamsAdmProtocolsService: GradeChangeExamsAdmProtocolsService,
    private actionService: ActionService,
    private dialogRef: MatDialogRef<AddOrUpdateStudentGceAdmProtocolDialogComponent>,
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
        subjectKeys: <CurriculumNomVO[]>student.subjects
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
    const studentKey =
      this.data.student != null
        ? <StudentNomVO>{ classId: this.data.student.classId, personId: this.data.student.personId }
        : <StudentNomVO>value.studentKey;

    const student = {
      classId: studentKey.classId,
      personId: studentKey.personId,
      subjects: value.subjectKeys
    };

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.student == null) {
            return this.gradeChangeExamsAdmProtocolsService
              .createStudent({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                gradeChangeExamsAdmProtocolId: this.data.gradeChangeExamsAdmProtocolId,
                createGradeChangeExamsAdmProtocolStudentCommand: student
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
            return this.gradeChangeExamsAdmProtocolsService
              .updateStudent({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                gradeChangeExamsAdmProtocolId: this.data.gradeChangeExamsAdmProtocolId,
                updateGradeChangeExamsAdmProtocolStudentCommand: student
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
