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
import { InstTeacherNomsService } from 'projects/sb-api-client/src/api/instTeacherNoms.service';
import { QualificationDegreeNomsService } from 'projects/sb-api-client/src/api/qualificationDegreeNoms.service';
import {
  QualificationExamResultProtocolsService,
  QualificationExamResultProtocols_CreateRequestParams,
  QualificationExamResultProtocols_Get,
  QualificationExamResultProtocols_GetStudentAll
} from 'projects/sb-api-client/src/api/qualificationExamResultProtocols.service';
import { QualificationExamResultProtocolsWordService } from 'projects/sb-api-client/src/api/qualificationExamResultProtocolsWord.service';
import { QualificationExamTypeNomsService } from 'projects/sb-api-client/src/api/qualificationExamTypeNoms.service';
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
import { QualificationExamResultProtocolViewDialogSkeletonComponent } from '../qualification-exam-result-protocol-student-view-dialog/qualification-exam-result-protocol-student-view-dialog.component';
import { QualificationExamResultProtocolViewClassDialogSkeletonComponent } from '../qualification-exam-result-protocol-view-class-dialog/qualification-exam-result-protocol-view-class-dialog.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class QualificationExamResultProtocolViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    qualificationExamResultProtocolsService: QualificationExamResultProtocolsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const qualificationExamResultProtocolId = tryParseInt(
      route.snapshot.paramMap.get('qualificationExamResultProtocolId')
    );
    if (qualificationExamResultProtocolId) {
      this.resolve(QualificationExamResultProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        qualificationExamResultProtocol: qualificationExamResultProtocolsService.get({
          schoolYear,
          instId,
          qualificationExamResultProtocolId
        })
      });
    } else {
      this.resolve(QualificationExamResultProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        qualificationExamResultProtocol: null
      });
    }
  }
}

@Component({
  selector: 'sb-qualification-exam-result-protocol-view',
  templateUrl: './qualification-exam-result-protocol-view.component.html'
})
export class QualificationExamResultProtocolViewComponent implements OnInit, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    institutionInfo: InstitutionInfoType;
    qualificationExamResultProtocol: QualificationExamResultProtocols_Get | null;
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
  protocolName = '3-80 ПОО Протокол за резултата от изпит за професионална квалификация';

  readonly form = this.fb.group({
    protocolName: [this.protocolName],
    classIds: [null, Validators.required],
    eduFormId: [null],
    profession: [null, Validators.required],
    speciality: [null, Validators.required],
    qualificationDegreeId: [null, Validators.required],
    qualificationExamTypeId: [null, Validators.required],
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
  examTypeNomsService: INomService<number, { instId: number; schoolYear: number }>;
  qualificationDegreeNomsService: INomService<number, { instId: number; schoolYear: number }>;
  classGroupNomsService: INomService<number, { instId: number; schoolYear: number }>;
  removingAct: { [key: number]: boolean } = {};
  dataSource!: TableDataSource<QualificationExamResultProtocols_GetStudentAll>;
  canEdit = false;
  canRemove = false;

  constructor(
    private fb: UntypedFormBuilder,
    private qualificationExamResultProtocolsService: QualificationExamResultProtocolsService,
    instTeacherNomsService: InstTeacherNomsService,
    eduFormNomsService: EduFormNomsService,
    examTypeNomsService: QualificationExamTypeNomsService,
    qualificationDegreeNomsService: QualificationDegreeNomsService,
    private qualificationExamResultProtocolsWordService: QualificationExamResultProtocolsWordService,
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

    this.examTypeNomsService = new NomServiceWithParams(examTypeNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.qualificationDegreeNomsService = new NomServiceWithParams(qualificationDegreeNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.classGroupNomsService = new NomServiceWithParams(classGroupNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      this.qualificationExamResultProtocolsService.getStudentAll({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        qualificationExamResultProtocolId: this.data.qualificationExamResultProtocol!.qualificationExamResultProtocolId,
        offset,
        limit
      })
    );
  }

  ngOnInit(): void {
    this.loadingTemplate = true;

    this.qualificationExamResultProtocolsWordService
      .downloadQualificationExamResultProtocolsWordTemplateFile({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        protocolId: 0
      })
      .toPromise()
      .then((url) => (this.downloadTemplateUrl = url))
      .finally(() => (this.loadingTemplate = false));

    const qualificationExamResultProtocol = this.data.qualificationExamResultProtocol;
    const protocolName = this.protocolName;
    if (qualificationExamResultProtocol != null) {
      this.loading = true;

      this.qualificationExamResultProtocolsWordService
        .downloadQualificationExamResultProtocolsWordFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          protocolId: qualificationExamResultProtocol.qualificationExamResultProtocolId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url))
        .finally(() => (this.loading = false));

      const {
        classIds,
        eduFormId,
        profession,
        speciality,
        qualificationDegreeId,
        protocolNumber,
        protocolDate,
        date,
        sessionType,
        qualificationExamTypeId,
        groupNum,
        commissionNominationOrderNumber,
        commissionNominationOrderDate,
        commissionChairman,
        commissionMembers
      } = qualificationExamResultProtocol;

      this.form.setValue({
        protocolName,
        classIds,
        eduFormId,
        profession,
        speciality,
        qualificationDegreeId,
        protocolNumber,
        protocolDate,
        date,
        sessionType,
        qualificationExamTypeId,
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
    const formValue = <
      QualificationExamResultProtocols_CreateRequestParams['createQualificationExamResultProtocolCommand']
    >this.form.value;

    if (formValue.commissionMembers.indexOf(formValue.commissionChairman) > -1) {
      this.errors = ['Членовете на комисията не трябва да се повтарят'];
      save.done(false);
      return;
    }

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.qualificationExamResultProtocol == null) {
            return this.qualificationExamResultProtocolsService
              .create({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                createQualificationExamResultProtocolCommand: formValue
              })
              .toPromise()
              .then((newQualificationExamResultProtocolId) => {
                this.form.markAsPristine();
                this.router.navigate(['../qualification', newQualificationExamResultProtocolId], {
                  relativeTo: this.route
                });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              qualificationExamResultProtocolId:
                this.data.qualificationExamResultProtocol.qualificationExamResultProtocolId
            };
            return this.qualificationExamResultProtocolsService
              .update({
                updateQualificationExamResultProtocolCommand: formValue,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.qualificationExamResultProtocolsService.get(updateArgs).toPromise())
              .then((newQualificationExamResultProtocol) => {
                this.data.qualificationExamResultProtocol = newQualificationExamResultProtocol;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.qualificationExamResultProtocol) {
      throw new Error('Removing a student equires a protocol to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      qualificationExamResultProtocolId: this.data.qualificationExamResultProtocol.qualificationExamResultProtocolId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете протокола?',
        errorsMessage: 'Не може да изтриете протокола, защото:',
        httpAction: () => this.qualificationExamResultProtocolsService.remove(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          this.router.navigate(['../../'], { relativeTo: this.route });
        }
      })
      .finally(() => {
        this.removing = false;
      });
  }

  openAddStudentDialog() {
    if (!this.data.qualificationExamResultProtocol) {
      throw new Error('onAddOrUpdate a student requires a stateExamsAdmProtocol to have been loaded.');
    }
    openTypedDialog(this.dialog, QualificationExamResultProtocolViewDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        qualificationExamResultProtocolId: this.data.qualificationExamResultProtocol.qualificationExamResultProtocolId
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
    if (!this.data.qualificationExamResultProtocol) {
      throw new Error('onAdd a whole class requires a protocol to have been loaded.');
    }
    openTypedDialog(this.dialog, QualificationExamResultProtocolViewClassDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        qualificationExamResultProtocolId: this.data.qualificationExamResultProtocol.qualificationExamResultProtocolId
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
    if (!this.data.qualificationExamResultProtocol) {
      throw new Error('Removing student requires a protocol to have been loaded.');
    }
    this.removing = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      qualificationExamResultProtocolId: this.data.qualificationExamResultProtocol.qualificationExamResultProtocolId,
      classId: classId,
      personId: personId
    };
    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете ученика?',
        errorsMessage: 'Не може да изтриете ученика, защото:',
        httpAction: () => this.qualificationExamResultProtocolsService.removeStudent(removeParams).toPromise()
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
