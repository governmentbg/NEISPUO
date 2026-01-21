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
import { InstitutionStudentNoms_GetNomsById } from 'projects/sb-api-client/src/api/institutionStudentNoms.service';
import { InstTeacherNomsService } from 'projects/sb-api-client/src/api/instTeacherNoms.service';
import {
  StateExamsAdmProtocolsService,
  StateExamsAdmProtocols_CreateRequestParams,
  StateExamsAdmProtocols_Get,
  StateExamsAdmProtocols_GetStudentAll
} from 'projects/sb-api-client/src/api/stateExamsAdmProtocols.service';
import { StateExamsAdmProtocolsWordService } from 'projects/sb-api-client/src/api/stateExamsAdmProtocolsWord.service';
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
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from } from 'rxjs';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';
import { AddOrUpdateStudentSeAdmProtocolDialogSkeletonComponent } from '../add-or-update-student-se-adm-protocol-dialog/add-or-update-student-se-adm-protocol-dialog.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class StateExamsAdmProtocolViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    stateExamsAdmProtocolsService: StateExamsAdmProtocolsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const stateExamsAdmProtocolId = tryParseInt(route.snapshot.paramMap.get('stateExamsAdmProtocolId'));

    if (stateExamsAdmProtocolId) {
      this.resolve(StateExamsAdmProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        stateExamsAdmProtocol: stateExamsAdmProtocolsService.get({
          schoolYear,
          instId,
          stateExamsAdmProtocolId
        })
      });
    } else {
      this.resolve(StateExamsAdmProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        stateExamsAdmProtocol: null
      });
    }
  }
}

type StudentNomVO = ArrayElementType<InstitutionStudentNoms_GetNomsById>['id'];
const PROTOCOL_NAME =
  '3-79 Протокол за допускане на учениците до държавни изпити и за придобиване на средно образование и/или до държавен изпит за придобиване на професионална квалификация';

@Component({
  selector: 'sb-state-exams-adm-protocol-view',
  templateUrl: './state-exams-adm-protocol-view.component.html'
})
export class StateExamsAdmProtocolViewComponent implements OnInit, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    institutionInfo: InstitutionInfoType;
    stateExamsAdmProtocol: StateExamsAdmProtocols_Get | null;
  };

  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasPlus = fasPlus;
  readonly fasPencil = fasPencil;
  readonly fadChevronCircleRight = fadChevronCircleRight;
  readonly fasFileWord = fasFileWord;

  readonly form = this.fb.group({
    protocolName: [PROTOCOL_NAME],
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
  loadingTemplate = false;
  instTeacherNomsService: INomService<number, { instId: number; schoolYear: number }>;
  removingStud: { [key: string]: boolean } = {};
  dataSource!: TableDataSource<StateExamsAdmProtocols_GetStudentAll>;
  canEdit = false;
  canRemove = false;

  constructor(
    private fb: UntypedFormBuilder,
    private stateExamsAdmProtocolsService: StateExamsAdmProtocolsService,
    private stateExamsAdmProtocolsWordService: StateExamsAdmProtocolsWordService,
    instTeacherNomsService: InstTeacherNomsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private dialog: MatDialog
  ) {
    this.instTeacherNomsService = new NomServiceWithParams(instTeacherNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      this.stateExamsAdmProtocolsService.getStudentAll({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        stateExamsAdmProtocolId: this.data.stateExamsAdmProtocol!.stateExamsAdmProtocolId,
        offset,
        limit
      })
    );
  }

  ngOnInit(): void {
    const stateExamsAdmProtocol = this.data.stateExamsAdmProtocol;
    const protocolName = PROTOCOL_NAME;
    this.loadingTemplate = true;

    this.stateExamsAdmProtocolsWordService
      .downloadStateExamsAdmProtocolsWordTemplateFile({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        protocolId: 0
      })
      .toPromise()
      .then((url) => (this.downloadTemplateUrl = url))
      .finally(() => (this.loadingTemplate = false));

    if (stateExamsAdmProtocol != null) {
      this.loading = true;

      this.stateExamsAdmProtocolsWordService
        .downloadStateExamsAdmProtocolsWordFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          protocolId: stateExamsAdmProtocol.stateExamsAdmProtocolId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url))
        .finally(() => (this.loading = false));

      const {
        protocolNum,
        protocolDate,
        commissionMeetingDate,
        commissionNominationOrderNumber,
        commissionNominationOrderDate,
        examSession,
        directorPersonId,
        commissionChairman,
        commissionMembers
      } = stateExamsAdmProtocol;

      this.form.setValue({
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
    const formValue = <StateExamsAdmProtocols_CreateRequestParams['createStateExamsAdmProtocolCommand']>this.form.value;

    if (formValue.commissionMembers.indexOf(formValue.commissionChairman) > -1) {
      this.errors = ['Членовете на комисията не трябва да се повтарят'];
      save.done(false);
      return;
    }

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.stateExamsAdmProtocol == null) {
            return this.stateExamsAdmProtocolsService
              .create({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                createStateExamsAdmProtocolCommand: formValue
              })
              .toPromise()
              .then((stateExamsAdmProtocolId) => {
                this.form.markAsPristine();
                this.router.navigate(['../', stateExamsAdmProtocolId], { relativeTo: this.route });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              stateExamsAdmProtocolId: this.data.stateExamsAdmProtocol.stateExamsAdmProtocolId
            };
            return this.stateExamsAdmProtocolsService
              .update({
                updateStateExamsAdmProtocolCommand: formValue,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.stateExamsAdmProtocolsService.get(updateArgs).toPromise())
              .then((newStateExamsAdmProtocol) => {
                this.data.stateExamsAdmProtocol = newStateExamsAdmProtocol;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.stateExamsAdmProtocol) {
      throw new Error('onRemove requires a stateExamsAdmProtocol to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      stateExamsAdmProtocolId: this.data.stateExamsAdmProtocol.stateExamsAdmProtocolId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете протокола за допускане до държавни изпити?',
        errorsMessage: 'Не може да изтриете протокола за допускане до държавни изпити, защото:',
        httpAction: () => this.stateExamsAdmProtocolsService.remove(removeParams).toPromise()
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

  openAddOrUpdateStudentDialog(studentKey: StudentNomVO | null) {
    if (!this.data.stateExamsAdmProtocol) {
      throw new Error('openAddOrUpdateStudent requires a stateExamsAdmProtocol to have been loaded.');
    }

    openTypedDialog(this.dialog, AddOrUpdateStudentSeAdmProtocolDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        stateExamsAdmProtocolId: this.data.stateExamsAdmProtocol.stateExamsAdmProtocolId,
        studentKey: studentKey
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

  onRemoveStudent(studentKey: StudentNomVO | null) {
    if (!this.data.stateExamsAdmProtocol) {
      throw new Error('onRemoveStudent requires a stateExamsAdmProtocol to have been loaded.');
    }

    const removingKey = `${(<StudentNomVO>studentKey).classId}-${(<StudentNomVO>studentKey).personId}`;
    this.removingStud[removingKey] = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      stateExamsAdmProtocolId: this.data.stateExamsAdmProtocol.stateExamsAdmProtocolId,
      classId: (<StudentNomVO>studentKey).classId,
      personId: (<StudentNomVO>studentKey).personId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете ученика?',
        errorsMessage: 'Не може да изтриете студента, защото:',
        httpAction: () => this.stateExamsAdmProtocolsService.removeStudent(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          return this.dataSource.reload();
        }
      })
      .finally(() => {
        this.removingStud[removingKey] = false;
      });
  }
}
