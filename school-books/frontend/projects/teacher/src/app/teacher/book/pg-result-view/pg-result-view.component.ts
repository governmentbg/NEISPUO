import { Component, Inject, Input, OnInit } from '@angular/core';
import { AbstractControl, UntypedFormBuilder, ValidationErrors, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ClassBookStudentNomsService } from 'projects/sb-api-client/src/api/classBookStudentNoms.service';
import { ClassBookSubjectNomsService } from 'projects/sb-api-client/src/api/classBookSubjectNoms.service';
import { PgResultsService, PgResults_Get } from 'projects/sb-api-client/src/api/pgResults.service';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  IShouldPreventLeave,
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { ClassBookInfoType } from 'projects/shared/utils/book';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from } from 'rxjs';
import { CLASS_BOOK_INFO } from '../book/book.component';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class PgResultViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    pgResultsService: PgResultsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const pgResultId = tryParseInt(route.snapshot.paramMap.get('pgResultId'));

    if (pgResultId) {
      this.resolve(PgResultViewComponent, {
        schoolYear,
        instId,
        classBookId,
        classBookInfo: from(classBookInfo),
        pgResult: pgResultsService.get({
          schoolYear,
          instId,
          classBookId,
          pgResultId: pgResultId
        })
      });
    } else {
      this.resolve(PgResultViewComponent, {
        schoolYear,
        instId,
        classBookId,
        classBookInfo: from(classBookInfo),
        pgResult: null
      });
    }
  }
}

@Component({
  selector: 'sb-pg-result-view',
  templateUrl: './pg-result-view.component.html'
})
export class PgResultViewComponent implements OnInit, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    classBookInfo: ClassBookInfoType;
    pgResult: PgResults_Get | null;
  };

  readonly form = this.fb.group(
    {
      studentId: [null, Validators.required],
      startSchoolYearResult: [null],
      subjectId: [null],
      endSchoolYearResult: [null]
    },
    {
      validators: [
        (control: AbstractControl): ValidationErrors | null => {
          const { startSchoolYearResult, endSchoolYearResult } = control.value as {
            startSchoolYearResult: string | null;
            endSchoolYearResult: string | null;
          };
          if (startSchoolYearResult || endSchoolYearResult) {
            return null;
          }
          return { startEndError: true };
        }
      ]
    }
  );

  classBookStudentNomsService: INomService<number, { schoolYear: number; instId: number; classBookId: number }>;
  classBookSubjectNomsService: INomService<number, { schoolYear: number; instId: number; classBookId: number }>;

  constructor(
    private fb: UntypedFormBuilder,
    private pgResultsService: PgResultsService,
    classBookStudentNomsService: ClassBookStudentNomsService,
    classBookSubjectNomsService: ClassBookSubjectNomsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService
  ) {
    this.classBookStudentNomsService = new NomServiceWithParams(classBookStudentNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId
    }));

    this.classBookSubjectNomsService = new NomServiceWithParams(classBookSubjectNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      withWriteAccess: true
    }));
  }

  ngOnInit() {
    if (this.data.pgResult != null) {
      this.form.setValue({
        studentId: this.data.pgResult.personId,
        subjectId: this.data.pgResult.subjectId,
        startSchoolYearResult: this.data.pgResult.startSchoolYearResult,
        endSchoolYearResult: this.data.pgResult.endSchoolYearResult
      });
    }
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave(save: SaveToken) {
    const value = this.form.value;
    const pgResult = {
      personId: <number>value.studentId,
      subjectId: <number>value.subjectId,
      startSchoolYearResult: <string | null>value.startSchoolYearResult,
      endSchoolYearResult: <string | null>value.endSchoolYearResult
    };
    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.pgResult == null) {
            return this.pgResultsService
              .createPgResult({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                classBookId: this.data.classBookId,
                createPgResultCommand: pgResult
              })
              .toPromise()
              .then(() => {
                this.form.markAsPristine();
                this.router.navigate(['../'], { relativeTo: this.route });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              pgResultId: this.data.pgResult.pgResultId,
              subjectId: pgResult.subjectId
            };
            return this.pgResultsService
              .update({
                updatePgResultCommand: {
                  startSchoolYearResult: pgResult.startSchoolYearResult,
                  endSchoolYearResult: pgResult.endSchoolYearResult
                },
                ...updateArgs
              })
              .toPromise()
              .then(() => this.pgResultsService.get(updateArgs).toPromise())
              .then(() => {
                this.form.markAsPristine();
                this.router.navigate(['../'], { relativeTo: this.route });
              });
          }
        }
      })
      .then((success) => save.done(success));
  }
}
