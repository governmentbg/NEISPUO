import { Component, Inject, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faFileWord as fasFileWord } from '@fortawesome/pro-solid-svg-icons/faFileWord';
import { faPencil as fasPencil } from '@fortawesome/pro-solid-svg-icons/faPencil';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { ClassGroupNomsService } from 'projects/sb-api-client/src/api/classGroupNoms.service';
import { EduFormNomsService } from 'projects/sb-api-client/src/api/eduFormNoms.service';
import {
  ExamResultProtocolsService,
  ExamResultProtocols_CreateRequestParams,
  ExamResultProtocols_Get,
  ExamResultProtocols_GetStudentAll
} from 'projects/sb-api-client/src/api/examResultProtocols.service';
import { ExamResultProtocolsWordService } from 'projects/sb-api-client/src/api/examResultProtocolsWord.service';
import { ExamSubTypeNomsService } from 'projects/sb-api-client/src/api/examSubTypeNoms.service';
import { ExamTypeNomsService } from 'projects/sb-api-client/src/api/examTypeNoms.service';
import { InstTeacherNomsService } from 'projects/sb-api-client/src/api/instTeacherNoms.service';
import { SubjectNomsService } from 'projects/sb-api-client/src/api/subjectNoms.service';
import { SubjectTypeNomsService } from 'projects/sb-api-client/src/api/subjectTypeNoms.service';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  IShouldPreventLeave,
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from } from 'rxjs';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';
import { ExamResultProtocolViewDialogSkeletonComponent } from '../exam-result-protocol-student-view-dialog/exam-result-protocol-student-view-dialog.component';
import { ExamResultProtocolViewClassDialogSkeletonComponent } from '../exam-result-protocol-view-class-dialog/exam-result-protocol-view-class-dialog.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class ExamResultProtocolViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    examResultProtocolsService: ExamResultProtocolsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const examResultProtocolId = tryParseInt(route.snapshot.paramMap.get('examResultProtocolId'));
    if (examResultProtocolId) {
      this.resolve(ExamResultProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        examResultProtocol: examResultProtocolsService.get({
          schoolYear,
          instId,
          examResultProtocolId
        })
      });
    } else {
      this.resolve(ExamResultProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        examResultProtocol: null
      });
    }
  }
}

@Component({
  selector: 'sb-exam-result-protocol-view',
  templateUrl: './exam-result-protocol-view.component.html'
})
export class ExamResultProtocolViewComponent implements OnInit, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    institutionInfo: InstitutionInfoType;
    examResultProtocol: ExamResultProtocols_Get | null;
  };

  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasPlus = fasPlus;
  readonly fasPencil = fasPencil;
  readonly fadChevronCircleRight = fadChevronCircleRight;
  fasFileWord = fasFileWord;
  downloadUrl?: string;
  downloadTemplateUrl?: string;
  loading?: boolean;
  loadingTemplate?: boolean;
  protocolName = '3-80 Протокол за резултата от писмен, устен или практически изпит';

  readonly form = this.fb.group({
    protocolName: [this.protocolName],
    classIds: [null, Validators.required],
    eduFormId: [null],
    subjectId: [null, Validators.required],
    subjectTypeId: [null, Validators.required],
    protocolExamTypeId: [null, Validators.required],
    protocolExamSubTypeId: [null, Validators.required],
    protocolNumber: [null],
    protocolDate: [null],
    date: [null, Validators.required],
    sessionType: [null],
    groupNum: [null],
    commissionNominationOrderNumber: [null, Validators.required],
    commissionNominationOrderDate: [null, Validators.required],
    commissionChairman: [null, Validators.required],
    commissionMembers: [null, Validators.required]
  });

  errors: string[] = [];

  removing = false;
  editable = false;
  eduFormNomsService: INomService<number, { instId: number; schoolYear: number }>;
  instTeacherNomsService: INomService<number, { instId: number; schoolYear: number }>;
  subjectNomsService: INomService<number, { instId: number; schoolYear: number }>;
  subjectTypeNomsService: INomService<number, { instId: number; schoolYear: number }>;
  examTypeNomsService: INomService<number, { instId: number; schoolYear: number }>;
  examSubTypeNomsService: INomService<number, { instId: number; schoolYear: number }>;
  classGroupNomsService: INomService<number, { instId: number; schoolYear: number }>;
  removingAct: { [key: number]: boolean } = {};
  dataSource!: TableDataSource<ExamResultProtocols_GetStudentAll>;
  canEdit = false;
  canRemove = false;

  constructor(
    private fb: UntypedFormBuilder,
    private examResultProtocolsService: ExamResultProtocolsService,
    instTeacherNomsService: InstTeacherNomsService,
    eduFormNomsService: EduFormNomsService,
    subjectNomsService: SubjectNomsService,
    subjectTypeNomsService: SubjectTypeNomsService,
    examTypeNomsService: ExamTypeNomsService,
    examSubTypeNomsService: ExamSubTypeNomsService,
    private examResultProtocolsWordService: ExamResultProtocolsWordService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private dialog: MatDialog,
    classGroupNomsService: ClassGroupNomsService
  ) {
    this.eduFormNomsService = new NomServiceWithParams(eduFormNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.instTeacherNomsService = new NomServiceWithParams(instTeacherNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.subjectNomsService = new NomServiceWithParams(subjectNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.subjectTypeNomsService = new NomServiceWithParams(subjectTypeNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.examTypeNomsService = new NomServiceWithParams(examTypeNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.examSubTypeNomsService = new NomServiceWithParams(examSubTypeNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.classGroupNomsService = new NomServiceWithParams(classGroupNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      this.examResultProtocolsService.getStudentAll({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        examResultProtocolId: this.data.examResultProtocol!.examResultProtocolId,
        offset,
        limit
      })
    );
  }

  ngOnInit(): void {
    this.loadingTemplate = true;

    this.examResultProtocolsWordService
      .downloadExamResultProtocolsWordTemplateFile({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        protocolId: 0
      })
      .toPromise()
      .then((url) => (this.downloadTemplateUrl = url))
      .finally(() => (this.loadingTemplate = false));

    const examResultProtocol = this.data.examResultProtocol;
    const protocolName = this.protocolName;
    if (examResultProtocol != null) {
      this.loading = true;

      this.examResultProtocolsWordService
        .downloadExamResultProtocolsWordFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          protocolId: examResultProtocol.examResultProtocolId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url))
        .finally(() => (this.loading = false));

      const {
        classIds,
        eduFormId,
        subjectId,
        subjectTypeId,
        protocolNumber,
        protocolDate,
        date,
        sessionType,
        protocolExamTypeId,
        protocolExamSubTypeId,
        groupNum,
        commissionNominationOrderNumber,
        commissionNominationOrderDate,
        commissionChairman,
        commissionMembers
      } = examResultProtocol;

      this.form.setValue({
        protocolName,
        classIds,
        eduFormId,
        subjectId,
        subjectTypeId,
        protocolNumber,
        protocolDate,
        date,
        sessionType,
        protocolExamTypeId,
        protocolExamSubTypeId,
        groupNum,
        commissionNominationOrderNumber,
        commissionNominationOrderDate,
        commissionChairman,
        commissionMembers
      });
    }

    this.canEdit = this.data.institutionInfo.hasProtocolsEditAccess;
    this.canRemove = this.data.institutionInfo.hasProtocolsRemoveAccess;
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave(save: SaveToken) {
    const formValue = <ExamResultProtocols_CreateRequestParams['createExamResultProtocolCommand']>this.form.value;

    if (formValue.commissionMembers.indexOf(formValue.commissionChairman) > -1) {
      this.errors = ['Членовете на комисията не трябва да се повтарят'];
      save.done(false);
      return;
    }

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.examResultProtocol == null) {
            return this.examResultProtocolsService
              .create({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                createExamResultProtocolCommand: formValue
              })
              .toPromise()
              .then((newExamResultProtocolId) => {
                this.form.markAsPristine();
                this.router.navigate(['../', newExamResultProtocolId], {
                  relativeTo: this.route
                });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              examResultProtocolId: this.data.examResultProtocol.examResultProtocolId
            };
            return this.examResultProtocolsService
              .update({
                updateExamResultProtocolCommand: formValue,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.examResultProtocolsService.get(updateArgs).toPromise())
              .then((newExamResultProtocol) => {
                this.data.examResultProtocol = newExamResultProtocol;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.examResultProtocol) {
      throw new Error('Removing a student equires a protocol to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      examResultProtocolId: this.data.examResultProtocol.examResultProtocolId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете протокола?',
        errorsMessage: 'Не може да изтриете протокола, защото:',
        httpAction: () => this.examResultProtocolsService.remove(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          this.router.navigate(['../'], { relativeTo: this.route });
        }
      })
      .finally(() => {
        this.removing = false;
      });
  }

  openAddStudentDialog() {
    if (!this.data.examResultProtocol) {
      throw new Error('onAddOrUpdate a student requires a stateExamsAdmProtocol to have been loaded.');
    }
    openTypedDialog(this.dialog, ExamResultProtocolViewDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        examResultProtocolId: this.data.examResultProtocol.examResultProtocolId
      }
    })
      .afterClosed()
      .toPromise()
      .then((ok) => {
        if (ok) {
          return this.dataSource.reload();
        }

        return Promise.resolve();
      });
  }

  openAddWholeClassDialog() {
    if (!this.data.examResultProtocol) {
      throw new Error('onAdd a whole class requires a protocol to have been loaded.');
    }
    openTypedDialog(this.dialog, ExamResultProtocolViewClassDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        examResultProtocolId: this.data.examResultProtocol.examResultProtocolId
      }
    })
      .afterClosed()
      .toPromise()
      .then((ok) => {
        if (ok) {
          return this.dataSource.reload();
        }

        return Promise.resolve();
      });
  }

  onRemoveStudent(classId: number, personId: number) {
    if (!this.data.examResultProtocol) {
      throw new Error('Removing student requires a protocol to have been loaded.');
    }
    this.removing = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      examResultProtocolId: this.data.examResultProtocol.examResultProtocolId,
      classId: classId,
      personId: personId
    };
    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете ученика?',
        errorsMessage: 'Не може да изтриете ученика, защото:',
        httpAction: () => this.examResultProtocolsService.removeStudent(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          return this.dataSource.reload();
        }
      })
      .finally(() => {
        this.removing = false;
      });
  }
}
