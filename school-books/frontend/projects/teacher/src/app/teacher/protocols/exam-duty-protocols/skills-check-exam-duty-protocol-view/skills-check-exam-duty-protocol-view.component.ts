import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faFileWord as fasFileWord } from '@fortawesome/pro-solid-svg-icons/faFileWord';
import { faPencil as fasPencil } from '@fortawesome/pro-solid-svg-icons/faPencil';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { InstTeacherNomsService } from 'projects/sb-api-client/src/api/instTeacherNoms.service';
import {
  SkillsCheckExamDutyProtocolsService,
  SkillsCheckExamDutyProtocols_CreateRequestParams,
  SkillsCheckExamDutyProtocols_Get
} from 'projects/sb-api-client/src/api/skillsCheckExamDutyProtocols.service';
import { SkillsCheckExamDutyProtocolsWordService } from 'projects/sb-api-client/src/api/skillsCheckExamDutyProtocolsWord.service';
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
export class SkillsCheckExamDutyProtocolViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    skillsCheckExamDutyProtocolsService: SkillsCheckExamDutyProtocolsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const skillsCheckExamDutyProtocolId = tryParseInt(route.snapshot.paramMap.get('skillsCheckExamDutyProtocolId'));
    if (skillsCheckExamDutyProtocolId) {
      this.resolve(SkillsCheckExamDutyProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        skillsCheckExamDutyProtocol: skillsCheckExamDutyProtocolsService.get({
          schoolYear,
          instId,
          skillsCheckExamDutyProtocolId
        })
      });
    } else {
      this.resolve(SkillsCheckExamDutyProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        skillsCheckExamDutyProtocol: null
      });
    }
  }
}

@Component({
  selector: 'sb-skills-check-exam-duty-protocol-view',
  templateUrl: './skills-check-exam-duty-protocol-view.component.html'
})
export class SkillsCheckExamDutyProtocolViewComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    institutionInfo: InstitutionInfoType;
    skillsCheckExamDutyProtocol: SkillsCheckExamDutyProtocols_Get | null;
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
  protocolName = '3-82 Протокол за дежурство при провеждане на изпит за проверка на способностите';

  readonly form = this.fb.group({
    protocolName: [this.protocolName],
    protocolNumber: [null],
    protocolDate: [null],
    subjectId: [null, Validators.required],
    subjectTypeId: [null, Validators.required],
    date: [null, Validators.required],
    directorPersonId: [null, Validators.required],
    studentsCapacity: [null, Validators.required],
    supervisorPersonIds: [null, Validators.required]
  });

  removing = false;
  editable = false;
  instTeacherNomsService: INomService<number, { instId: number; schoolYear: number }>;
  subjectNomsService: INomService<number, { instId: number; schoolYear: number }>;
  subjectTypeNomsService: INomService<number, { instId: number; schoolYear: number }>;
  removingAct: { [key: number]: boolean } = {};
  canEdit = false;
  canRemove = false;

  constructor(
    private fb: UntypedFormBuilder,
    private skillsCheckExamDutyProtocolsService: SkillsCheckExamDutyProtocolsService,
    instTeacherNomsService: InstTeacherNomsService,
    subjectNomsService: SubjectNomsService,
    subjectTypeNomsService: SubjectTypeNomsService,
    private skillsCheckExamDutyProtocolsWordService: SkillsCheckExamDutyProtocolsWordService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService
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
  }

  ngOnInit(): void {
    const skillsCheckExamDutyProtocol = this.data.skillsCheckExamDutyProtocol;
    const protocolName = this.protocolName;

    this.loadingTemplate = true;

    this.skillsCheckExamDutyProtocolsWordService
      .downloadSkillsCheckExamDutyProtocolsWordTemplateFile({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        protocolId: 0
      })
      .toPromise()
      .then((url) => (this.downloadTemplateUrl = url))
      .finally(() => (this.loadingTemplate = false));

    if (skillsCheckExamDutyProtocol != null) {
      this.loading = true;

      this.skillsCheckExamDutyProtocolsWordService
        .downloadSkillsCheckExamDutyProtocolsWordFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          protocolId: skillsCheckExamDutyProtocol.skillsCheckExamDutyProtocolId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url))
        .finally(() => (this.loading = false));

      const {
        subjectId,
        subjectTypeId,
        protocolNumber,
        protocolDate,
        date,
        directorPersonId,
        studentsCapacity,
        supervisorPersonIds
      } = skillsCheckExamDutyProtocol;

      this.form.setValue({
        protocolName,
        protocolNumber,
        protocolDate,
        subjectId,
        subjectTypeId,
        date,
        directorPersonId,
        studentsCapacity,
        supervisorPersonIds
      });
    } else if (this.data.institutionInfo.directorPersonId != null) {
      this.form.controls['directorPersonId'].setValue(this.data.institutionInfo.directorPersonId);
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
    const formValue = <SkillsCheckExamDutyProtocols_CreateRequestParams['createSkillsCheckExamDutyProtocolCommand']>(
      this.form.value
    );

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.skillsCheckExamDutyProtocol == null) {
            return this.skillsCheckExamDutyProtocolsService
              .create({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                createSkillsCheckExamDutyProtocolCommand: formValue
              })
              .toPromise()
              .then((newSkillsCheckExamDutyProtocolId) => {
                this.form.markAsPristine();
                this.router.navigate(['../skillsCheck', newSkillsCheckExamDutyProtocolId], {
                  relativeTo: this.route
                });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.skillsCheckExamDutyProtocol.schoolYear,
              instId: this.data.instId,
              skillsCheckExamDutyProtocolId: this.data.skillsCheckExamDutyProtocol.skillsCheckExamDutyProtocolId
            };
            return this.skillsCheckExamDutyProtocolsService
              .update({
                updateSkillsCheckExamDutyProtocolCommand: formValue,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.skillsCheckExamDutyProtocolsService.get(updateArgs).toPromise())
              .then((newSkillsCheckExamDutyProtocol) => {
                this.data.skillsCheckExamDutyProtocol = newSkillsCheckExamDutyProtocol;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.skillsCheckExamDutyProtocol) {
      throw new Error('Protocol is not created.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      skillsCheckExamDutyProtocolId: this.data.skillsCheckExamDutyProtocol.skillsCheckExamDutyProtocolId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете протокола?',
        errorsMessage: 'Не може да изтриете протокола, защото:',
        httpAction: () => this.skillsCheckExamDutyProtocolsService.remove(removeParams).toPromise()
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
