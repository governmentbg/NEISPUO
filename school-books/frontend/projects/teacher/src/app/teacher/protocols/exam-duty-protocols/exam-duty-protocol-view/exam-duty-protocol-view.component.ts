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
  ExamDutyProtocolsService,
  ExamDutyProtocols_CreateRequestParams,
  ExamDutyProtocols_Get,
  ExamDutyProtocols_GetStudentAll
} from 'projects/sb-api-client/src/api/examDutyProtocols.service';
import { ExamDutyProtocolsWordService } from 'projects/sb-api-client/src/api/examDutyProtocolsWord.service';
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
import { ExamDutyProtocolViewDialogSkeletonComponent } from '../exam-duty-protocol-student-view-dialog/exam-duty-protocol-student-view-dialog.component';
import { ExamDutyProtocolViewClassDialogSkeletonComponent } from '../exam-duty-protocol-view-class-dialog/exam-duty-protocol-view-class-dialog.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class ExamDutyProtocolViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    examDutyProtocolsService: ExamDutyProtocolsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const examDutyProtocolId = tryParseInt(route.snapshot.paramMap.get('examDutyProtocolId'));
    if (examDutyProtocolId) {
      this.resolve(ExamDutyProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        examDutyProtocol: examDutyProtocolsService.get({
          schoolYear,
          instId,
          examDutyProtocolId
        })
      });
    } else {
      this.resolve(ExamDutyProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        examDutyProtocol: null
      });
    }
  }
}

@Component({
  selector: 'sb-exam-duty-protocol-view',
  templateUrl: './exam-duty-protocol-view.component.html'
})
export class ExamDutyProtocolViewComponent implements OnInit, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    institutionInfo: InstitutionInfoType;
    examDutyProtocol: ExamDutyProtocols_Get | null;
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
  protocolName = '3-82 Протокол за дежурство при провеждане на изпит';

  readonly form = this.fb.group({
    protocolName: [this.protocolName],
    protocolNumber: [null],
    protocolDate: [null],
    classIds: [null, Validators.required],
    eduFormId: [null],
    subjectId: [null, Validators.required],
    subjectTypeId: [null, Validators.required],
    protocolExamTypeId: [null, Validators.required],
    protocolExamSubTypeId: [null, Validators.required],
    orderNumber: [null, Validators.required],
    orderDate: [null, Validators.required],
    date: [null, Validators.required],
    sessionType: [null],
    groupNum: [null],
    supervisorPersonIds: [null]
  });

  classGroupNomsService!: INomService<number, { instId: number; schoolYear: number }>;

  removing = false;
  editable = false;
  eduFormNomsService: INomService<number, { instId: number; schoolYear: number }>;
  instTeacherNomsService: INomService<number, { instId: number; schoolYear: number }>;
  subjectNomsService: INomService<number, { instId: number; schoolYear: number }>;
  subjectTypeNomsService: INomService<number, { instId: number; schoolYear: number }>;
  examTypeNomsService: INomService<number, { instId: number; schoolYear: number }>;
  examSubTypeNomsService: INomService<number, { instId: number; schoolYear: number }>;
  removingAct: { [key: number]: boolean } = {};
  dataSource!: TableDataSource<ExamDutyProtocols_GetStudentAll>;
  canEdit = false;
  canRemove = false;

  constructor(
    private fb: UntypedFormBuilder,
    private examDutyProtocolsService: ExamDutyProtocolsService,
    instTeacherNomsService: InstTeacherNomsService,
    eduFormNomsService: EduFormNomsService,
    subjectNomsService: SubjectNomsService,
    subjectTypeNomsService: SubjectTypeNomsService,
    examTypeNomsService: ExamTypeNomsService,
    examSubTypeNomsService: ExamSubTypeNomsService,
    private examDutyProtocolsWordService: ExamDutyProtocolsWordService,
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
      schoolYear: this.data.schoolYear,
      includeNonPedagogical: true
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
      this.examDutyProtocolsService.getStudentAll({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        examDutyProtocolId: this.data.examDutyProtocol!.examDutyProtocolId,
        offset,
        limit
      })
    );
  }

  ngOnInit(): void {
    this.loading = true;
    this.loadingTemplate = true;

    this.examDutyProtocolsWordService
      .downloadExamDutyProtocolsWordTemplateFile({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        protocolId: 0
      })
      .toPromise()
      .then((url) => (this.downloadTemplateUrl = url))
      .finally(() => (this.loadingTemplate = false));

    this.examDutyProtocolsWordService
      .downloadExamDutyProtocolsWordFile({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        protocolId: this.data.examDutyProtocol?.examDutyProtocolId ?? 0
      })
      .toPromise()
      .then((url) => (this.downloadUrl = url))
      .finally(() => (this.loading = false));

    const examDutyProtocol = this.data.examDutyProtocol;
    const protocolName = this.protocolName;
    if (examDutyProtocol != null) {
      const {
        protocolNumber,
        protocolDate,
        classIds,
        eduFormId,
        subjectId,
        subjectTypeId,
        orderNumber,
        orderDate,
        date,
        sessionType,
        protocolExamTypeId,
        protocolExamSubTypeId,
        groupNum,
        supervisorPersonIds
      } = examDutyProtocol;

      this.form.setValue({
        protocolNumber,
        protocolDate,
        protocolName,
        classIds,
        eduFormId,
        subjectId,
        subjectTypeId,
        orderNumber,
        orderDate,
        date,
        sessionType,
        protocolExamTypeId,
        protocolExamSubTypeId,
        groupNum,
        supervisorPersonIds
      });
    }

    this.canEdit = this.data.institutionInfo.hasProtocolsEditAccess;
    this.canRemove = this.data.institutionInfo.hasProtocolsRemoveAccess;
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave(save: SaveToken) {
    const formValue = <ExamDutyProtocols_CreateRequestParams['createExamDutyProtocolCommand']>this.form.value;

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.examDutyProtocol == null) {
            return this.examDutyProtocolsService
              .create({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                createExamDutyProtocolCommand: formValue
              })
              .toPromise()
              .then((newExamDutyProtocolId) => {
                this.form.markAsPristine();
                this.router.navigate(['../', newExamDutyProtocolId], { relativeTo: this.route });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              examDutyProtocolId: this.data.examDutyProtocol.examDutyProtocolId
            };
            return this.examDutyProtocolsService
              .update({
                updateExamDutyProtocolCommand: formValue,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.examDutyProtocolsService.get(updateArgs).toPromise())
              .then((newExamDutyProtocol) => {
                this.data.examDutyProtocol = newExamDutyProtocol;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.examDutyProtocol) {
      throw new Error('Removing a student equires a protocol to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      examDutyProtocolId: this.data.examDutyProtocol.examDutyProtocolId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете протокола?',
        errorsMessage: 'Не може да изтриете протокола, защото:',
        httpAction: () => this.examDutyProtocolsService.remove(removeParams).toPromise()
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
    if (!this.data.examDutyProtocol) {
      throw new Error('onAddOrUpdate a student requires a stateExamsAdmProtocol to have been loaded.');
    }
    openTypedDialog(this.dialog, ExamDutyProtocolViewDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        examDutyProtocolId: this.data.examDutyProtocol.examDutyProtocolId
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
    if (!this.data.examDutyProtocol) {
      throw new Error('onAdd a whole class requires a protocol to have been loaded.');
    }
    openTypedDialog(this.dialog, ExamDutyProtocolViewClassDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        examDutyProtocolId: this.data.examDutyProtocol.examDutyProtocolId
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
    if (!this.data.examDutyProtocol) {
      throw new Error('Removing student requires a protocol to have been loaded.');
    }
    this.removing = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      examDutyProtocolId: this.data.examDutyProtocol.examDutyProtocolId,
      classId: classId,
      personId: personId
    };
    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете ученика?',
        errorsMessage: 'Не може да изтриете ученика, защото:',
        httpAction: () => this.examDutyProtocolsService.removeStudent(removeParams).toPromise()
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
