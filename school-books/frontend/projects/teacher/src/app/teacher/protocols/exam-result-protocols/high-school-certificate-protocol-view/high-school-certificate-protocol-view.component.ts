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
import {
  HighSchoolCertificateProtocolsService,
  HighSchoolCertificateProtocols_CreateRequestParams,
  HighSchoolCertificateProtocols_Get,
  HighSchoolCertificateProtocols_GetStudentAll
} from 'projects/sb-api-client/src/api/highSchoolCertificateProtocols.service';
import { HighSchoolCertificateProtocolStageNomsService } from 'projects/sb-api-client/src/api/highSchoolCertificateProtocolStageNoms.service';
import { HighSchoolCertificateProtocolsWordService } from 'projects/sb-api-client/src/api/highSchoolCertificateProtocolsWord.service';
import { InstTeacherNomsService } from 'projects/sb-api-client/src/api/instTeacherNoms.service';
import { HighSchoolCertificateProtocolStage } from 'projects/sb-api-client/src/model/highSchoolCertificateProtocolStage';
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
import { HighSchoolCertificateProtocolViewDialogSkeletonComponent } from '../high-school-certificate-protocol-student-view-dialog/high-school-certificate-protocol-student-view-dialog.component';
import { HighSchoolCertificateProtocolViewClassDialogSkeletonComponent } from '../high-school-certificate-protocol-view-class-dialog/high-school-certificate-protocol-view-class-dialog.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class HighSchoolCertificateProtocolViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    HighSchoolCertificateProtocolsService: HighSchoolCertificateProtocolsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const highSchoolCertificateProtocolId = tryParseInt(route.snapshot.paramMap.get('highSchoolCertificateProtocolId'));

    if (highSchoolCertificateProtocolId) {
      this.resolve(HighSchoolCertificateProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        highSchoolCertificateProtocol: HighSchoolCertificateProtocolsService.get({
          schoolYear,
          instId,
          highSchoolCertificateProtocolId
        })
      });
    } else {
      this.resolve(HighSchoolCertificateProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        highSchoolCertificateProtocol: null
      });
    }
  }
}

const PROTOCOL_NAME = '3-84 Протокол за удостоверяване на завършен гимназиален етап';

@Component({
  selector: 'sb-high-school-certificate-protocol-view',
  templateUrl: './high-school-certificate-protocol-view.component.html'
})
export class HighSchoolCertificateProtocolViewComponent implements OnInit, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    institutionInfo: InstitutionInfoType;
    highSchoolCertificateProtocol: HighSchoolCertificateProtocols_Get | null;
  };

  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasPlus = fasPlus;
  readonly fasPencil = fasPencil;
  readonly fadChevronCircleRight = fadChevronCircleRight;
  readonly fasFileWord = fasFileWord;

  readonly form = this.fb.group({
    protocolName: [PROTOCOL_NAME],
    stage: [null, Validators.required],
    protocolNum: [null],
    protocolDate: [null],
    commissionMeetingDate: [null, Validators.required],
    commissionNominationOrderNumber: [null, Validators.required],
    commissionNominationOrderDate: [null, Validators.required],
    examSession: [null],
    directorPersonId: [null, Validators.required],
    commissionChairman: [null, Validators.required],
    commissionMembers: [null, Validators.required]
  });

  errors: string[] = [];
  removing = false;
  editable = false;
  downloadUrl?: string;
  downloadTemplateUrl?: string;
  loading = false;
  loadingTemplate?: boolean;
  stageNomsService: INomService<HighSchoolCertificateProtocolStage, { instId: number; schoolYear: number }>;
  instTeacherNomsService: INomService<number, { instId: number; schoolYear: number }>;
  removingStud: { [key: string]: boolean } = {};
  dataSource!: TableDataSource<HighSchoolCertificateProtocols_GetStudentAll>;
  canEdit = false;
  canRemove = false;

  constructor(
    private fb: UntypedFormBuilder,
    private highSchoolCertificateProtocolsService: HighSchoolCertificateProtocolsService,
    private highSchoolCertificateProtocolsWordService: HighSchoolCertificateProtocolsWordService,
    stageNomsService: HighSchoolCertificateProtocolStageNomsService,
    instTeacherNomsService: InstTeacherNomsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private dialog: MatDialog
  ) {
    this.stageNomsService = new NomServiceWithParams(stageNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.instTeacherNomsService = new NomServiceWithParams(instTeacherNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      this.highSchoolCertificateProtocolsService.getStudentAll({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        highSchoolCertificateProtocolId: this.data.highSchoolCertificateProtocol!.highSchoolCertificateProtocolId,
        offset,
        limit
      })
    );
  }

  ngOnInit(): void {
    const highSchoolCertificateProtocol = this.data.highSchoolCertificateProtocol;
    const protocolName = PROTOCOL_NAME;
    this.loadingTemplate = true;

    this.highSchoolCertificateProtocolsWordService
      .downloadHighSchoolCertificateProtocolsWordTemplateFile({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        protocolId: 0
      })
      .toPromise()
      .then((url) => (this.downloadTemplateUrl = url))
      .finally(() => (this.loadingTemplate = false));

    if (highSchoolCertificateProtocol != null) {
      this.loading = true;

      this.highSchoolCertificateProtocolsWordService
        .downloadHighSchoolCertificateProtocolsWordFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          protocolId: highSchoolCertificateProtocol.highSchoolCertificateProtocolId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url))
        .finally(() => (this.loading = false));

      const {
        stage,
        protocolNum,
        protocolDate,
        commissionMeetingDate,
        commissionNominationOrderNumber,
        commissionNominationOrderDate,
        examSession,
        directorPersonId,
        commissionChairman,
        commissionMembers
      } = highSchoolCertificateProtocol;

      this.form.setValue({
        stage,
        protocolName,
        protocolNum,
        protocolDate,
        commissionMeetingDate,
        commissionNominationOrderNumber,
        commissionNominationOrderDate,
        examSession,
        directorPersonId,
        commissionChairman,
        commissionMembers
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
    const formValue = <
      HighSchoolCertificateProtocols_CreateRequestParams['createHighSchoolCertificateProtocolCommand']
    >this.form.value;

    if (formValue.commissionMembers.indexOf(formValue.commissionChairman) > -1) {
      this.errors = ['Членовете на комисията не трябва да се повтарят'];
      save.done(false);
      return;
    }

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.highSchoolCertificateProtocol == null) {
            return this.highSchoolCertificateProtocolsService
              .create({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                createHighSchoolCertificateProtocolCommand: formValue
              })
              .toPromise()
              .then((highSchoolCertificateProtocolId) => {
                this.form.markAsPristine();
                this.router.navigate(['../highSchoolCertificate', highSchoolCertificateProtocolId], {
                  relativeTo: this.route
                });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              highSchoolCertificateProtocolId: this.data.highSchoolCertificateProtocol.highSchoolCertificateProtocolId
            };
            return this.highSchoolCertificateProtocolsService
              .update({
                updateHighSchoolCertificateProtocolCommand: formValue,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.highSchoolCertificateProtocolsService.get(updateArgs).toPromise())
              .then((newHighSchoolCertificateProtocol) => {
                this.data.highSchoolCertificateProtocol = newHighSchoolCertificateProtocol;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.highSchoolCertificateProtocol) {
      throw new Error('onRemove requires a HighSchoolCertificateProtocol to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      highSchoolCertificateProtocolId: this.data.highSchoolCertificateProtocol.highSchoolCertificateProtocolId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете протокола?',
        errorsMessage: 'Не може да изтриете протокола, защото:',
        httpAction: () => this.highSchoolCertificateProtocolsService.remove(removeParams).toPromise()
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
    if (!this.data.highSchoolCertificateProtocol) {
      throw new Error('onAdd a student requires a protocol to have been loaded.');
    }
    openTypedDialog(this.dialog, HighSchoolCertificateProtocolViewDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        highSchoolCertificateProtocolId: this.data.highSchoolCertificateProtocol.highSchoolCertificateProtocolId
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
    if (!this.data.highSchoolCertificateProtocol) {
      throw new Error('onAdd a whole class requires a protocol to have been loaded.');
    }
    openTypedDialog(this.dialog, HighSchoolCertificateProtocolViewClassDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        highSchoolCertificateProtocolId: this.data.highSchoolCertificateProtocol.highSchoolCertificateProtocolId
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
    if (!this.data.highSchoolCertificateProtocol) {
      throw new Error('Removing student requires a protocol to have been loaded.');
    }
    this.removing = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      highSchoolCertificateProtocolId: this.data.highSchoolCertificateProtocol.highSchoolCertificateProtocolId,
      classId: classId,
      personId: personId
    };
    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете ученика?',
        errorsMessage: 'Не може да изтриете ученика, защото:',
        httpAction: () => this.highSchoolCertificateProtocolsService.removeStudent(removeParams).toPromise()
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
