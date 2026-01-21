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
import { BasicClassNomsService } from 'projects/sb-api-client/src/api/basicClassNoms.service';
import { InstTeacherNomsService } from 'projects/sb-api-client/src/api/instTeacherNoms.service';
import {
  NvoExamDutyProtocolsService,
  NvoExamDutyProtocols_CreateRequestParams,
  NvoExamDutyProtocols_Get,
  NvoExamDutyProtocols_GetStudentAll
} from 'projects/sb-api-client/src/api/nvoExamDutyProtocols.service';
import { NvoExamDutyProtocolsWordService } from 'projects/sb-api-client/src/api/nvoExamDutyProtocolsWord.service';
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
import { NvoExamDutyProtocolStudentViewDialogSkeletonComponent } from '../nvo-exam-duty-protocol-student-view-dialog/nvo-exam-duty-protocol-student-view-dialog.component';
import { NvoExamDutyProtocolViewClassDialogSkeletonComponent } from '../nvo-exam-duty-protocol-view-class-dialog/nvo-exam-duty-protocol-view-class-dialog.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class NvoExamDutyProtocolViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    nvoExamDutyProtocolsService: NvoExamDutyProtocolsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const nvoExamDutyProtocolId = tryParseInt(route.snapshot.paramMap.get('nvoExamDutyProtocolId'));
    if (nvoExamDutyProtocolId) {
      this.resolve(NvoExamDutyProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        nvoExamDutyProtocol: nvoExamDutyProtocolsService.get({
          schoolYear,
          instId,
          nvoExamDutyProtocolId
        })
      });
    } else {
      this.resolve(NvoExamDutyProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        nvoExamDutyProtocol: null
      });
    }
  }
}

@Component({
  selector: 'sb-nvo-exam-duty-protocol-view',
  templateUrl: './nvo-exam-duty-protocol-view.component.html'
})
export class NvoExamDutyProtocolViewComponent implements OnInit, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    institutionInfo: InstitutionInfoType;
    nvoExamDutyProtocol: NvoExamDutyProtocols_Get | null;
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
  protocolName = '3-82 Протокол за дежурство при провеждане на писмен изпит от НВО';

  readonly form = this.fb.group({
    protocolName: [this.protocolName],
    protocolNumber: [null],
    protocolDate: [null],
    basicClassId: [null, Validators.required],
    subjectId: [null, Validators.required],
    subjectTypeId: [null, Validators.required],
    date: [null, Validators.required],
    roomNumber: [null],
    directorPersonId: [null, Validators.required],
    supervisorPersonIds: [null, Validators.required]
  });

  removing = false;
  editable = false;
  instTeacherNomsService: INomService<number, { instId: number; schoolYear: number }>;
  subjectNomsService: INomService<number, { instId: number; schoolYear: number }>;
  subjectTypeNomsService: INomService<number, { instId: number; schoolYear: number }>;
  basicClassNomsService!: INomService<number, { instId: number; schoolYear: number }>;
  removingAct: { [key: number]: boolean } = {};
  dataSource!: TableDataSource<NvoExamDutyProtocols_GetStudentAll>;

  canEdit = false;
  canRemove = false;

  constructor(
    private fb: UntypedFormBuilder,
    private nvoExamDutyProtocolsService: NvoExamDutyProtocolsService,
    instTeacherNomsService: InstTeacherNomsService,
    subjectNomsService: SubjectNomsService,
    subjectTypeNomsService: SubjectTypeNomsService,
    basicClassNomsService: BasicClassNomsService,
    private nvoExamDutyProtocolsWordService: NvoExamDutyProtocolsWordService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private dialog: MatDialog
  ) {
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

    this.basicClassNomsService = new NomServiceWithParams(basicClassNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      this.nvoExamDutyProtocolsService.getStudentAll({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        nvoExamDutyProtocolId: this.data.nvoExamDutyProtocol!.nvoExamDutyProtocolId,
        offset,
        limit
      })
    );
  }

  ngOnInit(): void {
    this.loading = true;
    this.loadingTemplate = true;

    this.nvoExamDutyProtocolsWordService
      .downloadNvoExamDutyProtocolsWordTemplateFile({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        protocolId: 0
      })
      .toPromise()
      .then((url) => (this.downloadTemplateUrl = url))
      .finally(() => (this.loadingTemplate = false));

    this.nvoExamDutyProtocolsWordService
      .downloadNvoExamDutyProtocolsWordFile({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        protocolId: this.data.nvoExamDutyProtocol?.nvoExamDutyProtocolId ?? 0
      })
      .toPromise()
      .then((url) => (this.downloadUrl = url))
      .finally(() => (this.loading = false));

    const nvoExamDutyProtocol = this.data.nvoExamDutyProtocol;
    const protocolName = this.protocolName;
    if (nvoExamDutyProtocol != null) {
      const {
        protocolNumber,
        protocolDate,
        basicClassId,
        subjectId,
        subjectTypeId,
        date,
        roomNumber,
        directorPersonId,
        supervisorPersonIds
      } = nvoExamDutyProtocol;

      this.form.setValue({
        protocolName,
        protocolNumber,
        protocolDate,
        basicClassId,
        subjectId,
        subjectTypeId,
        date,
        roomNumber,
        directorPersonId,
        supervisorPersonIds
      });
    } else if (this.data.institutionInfo.directorPersonId != null) {
      this.form.controls['directorPersonId'].setValue(this.data.institutionInfo.directorPersonId);
    }

    this.canEdit = this.data.institutionInfo.hasProtocolsEditAccess;
    this.canRemove = this.data.institutionInfo.hasProtocolsRemoveAccess;
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave(save: SaveToken) {
    const formValue = <NvoExamDutyProtocols_CreateRequestParams['createNvoExamDutyProtocolCommand']>this.form.value;

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.nvoExamDutyProtocol == null) {
            return this.nvoExamDutyProtocolsService
              .create({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                createNvoExamDutyProtocolCommand: formValue
              })
              .toPromise()
              .then((newExamDutyProtocolId) => {
                this.form.markAsPristine();
                this.router.navigate(['../nvo', newExamDutyProtocolId], {
                  relativeTo: this.route
                });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              nvoExamDutyProtocolId: this.data.nvoExamDutyProtocol.nvoExamDutyProtocolId
            };
            return this.nvoExamDutyProtocolsService
              .update({
                updateNvoExamDutyProtocolCommand: formValue,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.nvoExamDutyProtocolsService.get(updateArgs).toPromise())
              .then((newExamDutyProtocol) => {
                this.data.nvoExamDutyProtocol = newExamDutyProtocol;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.nvoExamDutyProtocol) {
      throw new Error('Removing a student equires a protocol to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      nvoExamDutyProtocolId: this.data.nvoExamDutyProtocol.nvoExamDutyProtocolId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете протокола?',
        errorsMessage: 'Не може да изтриете протокола, защото:',
        httpAction: () => this.nvoExamDutyProtocolsService.remove(removeParams).toPromise()
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
    if (!this.data.nvoExamDutyProtocol) {
      throw new Error('onAddOrUpdate a student requires a protocol to have been loaded.');
    }
    openTypedDialog(this.dialog, NvoExamDutyProtocolStudentViewDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        nvoExamDutyProtocolId: this.data.nvoExamDutyProtocol.nvoExamDutyProtocolId
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
    if (!this.data.nvoExamDutyProtocol) {
      throw new Error('onAdd a whole class requires a protocol to have been loaded.');
    }
    openTypedDialog(this.dialog, NvoExamDutyProtocolViewClassDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        nvoExamDutyProtocolId: this.data.nvoExamDutyProtocol.nvoExamDutyProtocolId
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
    if (!this.data.nvoExamDutyProtocol) {
      throw new Error('Removing student requires a protocol to have been loaded.');
    }
    this.removing = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      nvoExamDutyProtocolId: this.data.nvoExamDutyProtocol.nvoExamDutyProtocolId,
      classId: classId,
      personId: personId
    };
    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете ученика?',
        errorsMessage: 'Не може да изтриете ученика, защото:',
        httpAction: () => this.nvoExamDutyProtocolsService.removeStudent(removeParams).toPromise()
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
