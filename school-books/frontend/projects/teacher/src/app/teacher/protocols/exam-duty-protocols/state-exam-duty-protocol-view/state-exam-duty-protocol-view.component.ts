import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faFileWord as fasFileWord } from '@fortawesome/pro-solid-svg-icons/faFileWord';
import { faPencil as fasPencil } from '@fortawesome/pro-solid-svg-icons/faPencil';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { EduFormNomsService } from 'projects/sb-api-client/src/api/eduFormNoms.service';
import { InstTeacherNomsService } from 'projects/sb-api-client/src/api/instTeacherNoms.service';
import {
  StateExamDutyProtocolsService,
  StateExamDutyProtocols_CreateRequestParams,
  StateExamDutyProtocols_Get
} from 'projects/sb-api-client/src/api/stateExamDutyProtocols.service';
import { StateExamDutyProtocolsWordService } from 'projects/sb-api-client/src/api/stateExamDutyProtocolsWord.service';
import { SubjectNomsService } from 'projects/sb-api-client/src/api/subjectNoms.service';
import { SubjectTypeNomsService } from 'projects/sb-api-client/src/api/subjectTypeNoms.service';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  IShouldPreventLeave,
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Subject } from 'rxjs';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class StateExamDutyProtocolViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    stateExamDutyProtocolsService: StateExamDutyProtocolsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const stateExamDutyProtocolId = tryParseInt(route.snapshot.paramMap.get('stateExamDutyProtocolId'));
    if (stateExamDutyProtocolId) {
      this.resolve(StateExamDutyProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        stateExamDutyProtocol: stateExamDutyProtocolsService.get({
          schoolYear,
          instId,
          stateExamDutyProtocolId
        })
      });
    } else {
      this.resolve(StateExamDutyProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        stateExamDutyProtocol: null
      });
    }
  }
}

@Component({
  selector: 'sb-state-exam-duty-protocol-view',
  templateUrl: './state-exam-duty-protocol-view.component.html'
})
export class StateExamDutyProtocolViewComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    institutionInfo: InstitutionInfoType;
    stateExamDutyProtocol: StateExamDutyProtocols_Get | null;
  };

  private readonly destroyed$ = new Subject<void>();

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
  protocolName = '3-82 ДЗИ Протокол за дежурство при провеждане на писмен държавен зрелостен изпит';

  readonly form = this.fb.group({
    protocolName: [this.protocolName],
    protocolNumber: [null],
    protocolDate: [null],
    eduFormId: [null],
    subjectId: [null, Validators.required],
    subjectTypeId: [null, Validators.required],
    orderNumber: [null, Validators.required],
    orderDate: [null, Validators.required],
    date: [null, Validators.required],
    sessionType: [null],
    roomNumber: [null],
    modulesCount: [null, Validators.required],
    supervisorPersonIds: [null, Validators.required]
  });

  removing = false;
  editable = false;
  eduFormNomsService: INomService<number, { instId: number; schoolYear: number }>;
  instTeacherNomsService: INomService<number, { instId: number; schoolYear: number }>;
  subjectNomsService: INomService<number, { instId: number; schoolYear: number }>;
  subjectTypeNomsService: INomService<number, { instId: number; schoolYear: number }>;
  removingAct: { [key: number]: boolean } = {};
  canEdit = false;
  canRemove = false;

  constructor(
    private fb: UntypedFormBuilder,
    private stateExamDutyProtocolsService: StateExamDutyProtocolsService,
    instTeacherNomsService: InstTeacherNomsService,
    eduFormNomsService: EduFormNomsService,
    subjectNomsService: SubjectNomsService,
    subjectTypeNomsService: SubjectTypeNomsService,
    private stateExamDutyProtocolsWordService: StateExamDutyProtocolsWordService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService
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
  }

  ngOnInit(): void {
    const stateExamDutyProtocol = this.data.stateExamDutyProtocol;
    const protocolName = this.protocolName;

    this.loadingTemplate = true;

    this.stateExamDutyProtocolsWordService
      .downloadStateExamDutyProtocolsWordTemplateFile({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        protocolId: 0
      })
      .toPromise()
      .then((url) => (this.downloadTemplateUrl = url))
      .finally(() => (this.loadingTemplate = false));

    if (stateExamDutyProtocol != null) {
      this.loading = true;

      this.stateExamDutyProtocolsWordService
        .downloadStateExamDutyProtocolsWordFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          protocolId: stateExamDutyProtocol.stateExamDutyProtocolId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url))
        .finally(() => (this.loading = false));

      const {
        protocolNumber,
        protocolDate,
        eduFormId,
        subjectId,
        subjectTypeId,
        orderNumber,
        orderDate,
        date,
        sessionType,
        roomNumber,
        modulesCount,
        supervisorPersonIds
      } = stateExamDutyProtocol;

      this.form.setValue({
        protocolNumber,
        protocolDate,
        protocolName,
        eduFormId,
        subjectId,
        subjectTypeId,
        orderNumber,
        orderDate,
        date,
        sessionType,
        roomNumber,
        modulesCount,
        supervisorPersonIds
      });
    }

    this.canEdit = this.data.institutionInfo.hasProtocolsEditAccess;
    this.canRemove = this.data.institutionInfo.hasProtocolsRemoveAccess;
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave(save: SaveToken) {
    const formValue = <StateExamDutyProtocols_CreateRequestParams['createStateExamDutyProtocolCommand']>this.form.value;

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.stateExamDutyProtocol == null) {
            return this.stateExamDutyProtocolsService
              .create({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                createStateExamDutyProtocolCommand: formValue
              })
              .toPromise()
              .then((newStateExamDutyProtocolId) => {
                this.form.markAsPristine();
                this.router.navigate(['../state', newStateExamDutyProtocolId], {
                  relativeTo: this.route
                });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.stateExamDutyProtocol.schoolYear,
              instId: this.data.instId,
              stateExamDutyProtocolId: this.data.stateExamDutyProtocol.stateExamDutyProtocolId
            };
            return this.stateExamDutyProtocolsService
              .update({
                updateStateExamDutyProtocolCommand: formValue,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.stateExamDutyProtocolsService.get(updateArgs).toPromise())
              .then((newStateExamDutyProtocol) => {
                this.data.stateExamDutyProtocol = newStateExamDutyProtocol;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.stateExamDutyProtocol) {
      throw new Error('Protocol is not created.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      stateExamDutyProtocolId: this.data.stateExamDutyProtocol.stateExamDutyProtocolId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете протокола?',
        errorsMessage: 'Не може да изтриете протокола, защото:',
        httpAction: () => this.stateExamDutyProtocolsService.remove(removeParams).toPromise()
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
}
