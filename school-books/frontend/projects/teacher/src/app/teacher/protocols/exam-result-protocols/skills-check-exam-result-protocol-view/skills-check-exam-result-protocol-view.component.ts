import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
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
  SkillsCheckExamResultProtocolsService,
  SkillsCheckExamResultProtocols_CreateRequestParams,
  SkillsCheckExamResultProtocols_Get,
  SkillsCheckExamResultProtocols_GetEvaluatorAll
} from 'projects/sb-api-client/src/api/skillsCheckExamResultProtocols.service';
import { SkillsCheckExamResultProtocolsWordService } from 'projects/sb-api-client/src/api/skillsCheckExamResultProtocolsWord.service';
import { SubjectNomsService } from 'projects/sb-api-client/src/api/subjectNoms.service';
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
import { from, Subject } from 'rxjs';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';
import { SkillsCheckExamResultProtocolViewDialogSkeletonComponent } from '../skills-check-exam-result-protocol-evaluator-view-dialog/skills-check-exam-result-protocol-evaluator-view.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class SkillsCheckExamResultProtocolViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    skillsCheckExamResultProtocolsService: SkillsCheckExamResultProtocolsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const skillsCheckExamResultProtocolId = tryParseInt(route.snapshot.paramMap.get('skillsCheckExamResultProtocolId'));
    if (skillsCheckExamResultProtocolId) {
      this.resolve(SkillsCheckExamResultProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        skillsCheckExamResultProtocol: skillsCheckExamResultProtocolsService.get({
          schoolYear,
          instId,
          skillsCheckExamResultProtocolId
        })
      });
    } else {
      this.resolve(SkillsCheckExamResultProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        skillsCheckExamResultProtocol: null
      });
    }
  }
}

@Component({
  selector: 'sb-skills-check-exam-result-protocol-view',
  templateUrl: './skills-check-exam-result-protocol-view.component.html'
})
export class SkillsCheckExamResultProtocolViewComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    institutionInfo: InstitutionInfoType;
    skillsCheckExamResultProtocol: SkillsCheckExamResultProtocols_Get | null;
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
  protocolName = '3-80 Протокол за резултатите от изпита за проверка на способностите';

  readonly form = this.fb.group({
    protocolName: [this.protocolName],
    subjectId: [null, Validators.required],
    protocolNumber: [null],
    date: [null],
    studentsCapacity: [null, Validators.required]
  });

  removing = false;
  editable = false;
  subjectNomsService: INomService<number, { instId: number; schoolYear: number }>;
  removingAct: { [key: number]: boolean } = {};
  dataSource!: TableDataSource<SkillsCheckExamResultProtocols_GetEvaluatorAll>;
  canEdit = false;
  canRemove = false;

  constructor(
    private fb: UntypedFormBuilder,
    private skillsCheckExamResultProtocolsService: SkillsCheckExamResultProtocolsService,
    subjectNomsService: SubjectNomsService,
    private skillsCheckExamResultProtocolsWordService: SkillsCheckExamResultProtocolsWordService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private dialog: MatDialog
  ) {
    this.subjectNomsService = new NomServiceWithParams(subjectNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      this.skillsCheckExamResultProtocolsService.getEvaluatorAll({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        skillsCheckExamResultProtocolId: this.data.skillsCheckExamResultProtocol?.skillsCheckExamResultProtocolId ?? 0,
        offset,
        limit
      })
    );
  }

  ngOnInit(): void {
    const skillsCheckExamResultProtocol = this.data.skillsCheckExamResultProtocol;
    const protocolName = this.protocolName;

    this.loadingTemplate = true;

    this.skillsCheckExamResultProtocolsWordService
      .downloadSkillsCheckExamResultProtocolsWordTemplateFile({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        protocolId: 0
      })
      .toPromise()
      .then((url) => (this.downloadTemplateUrl = url))
      .finally(() => (this.loadingTemplate = false));

    if (skillsCheckExamResultProtocol != null) {
      this.loading = true;

      this.skillsCheckExamResultProtocolsWordService
        .downloadSkillsCheckExamResultProtocolsWordFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          protocolId: skillsCheckExamResultProtocol.skillsCheckExamResultProtocolId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url))
        .finally(() => (this.loading = false));

      const { subjectId, protocolNumber, date, studentsCapacity } = skillsCheckExamResultProtocol;

      this.form.setValue({
        protocolName,
        subjectId,
        protocolNumber,
        date,
        studentsCapacity
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
    const formValue = <
      SkillsCheckExamResultProtocols_CreateRequestParams['createSkillsCheckExamResultProtocolCommand']
    >this.form.value;

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.skillsCheckExamResultProtocol == null) {
            return this.skillsCheckExamResultProtocolsService
              .create({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                createSkillsCheckExamResultProtocolCommand: formValue
              })
              .toPromise()
              .then((newSkillsCheckExamResultProtocolId) => {
                this.form.markAsPristine();
                this.router.navigate(['../skillsCheck', newSkillsCheckExamResultProtocolId], {
                  relativeTo: this.route
                });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.skillsCheckExamResultProtocol.schoolYear,
              instId: this.data.instId,
              skillsCheckExamResultProtocolId: this.data.skillsCheckExamResultProtocol.skillsCheckExamResultProtocolId
            };
            return this.skillsCheckExamResultProtocolsService
              .update({
                updateSkillsCheckExamResultProtocolCommand: formValue,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.skillsCheckExamResultProtocolsService.get(updateArgs).toPromise())
              .then((newSkillsCheckExamResultProtocol) => {
                this.data.skillsCheckExamResultProtocol = newSkillsCheckExamResultProtocol;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.skillsCheckExamResultProtocol) {
      throw new Error('Protocol is not created.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      skillsCheckExamResultProtocolId: this.data.skillsCheckExamResultProtocol.skillsCheckExamResultProtocolId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете протокола?',
        errorsMessage: 'Не може да изтриете протокола, защото:',
        httpAction: () => this.skillsCheckExamResultProtocolsService.remove(removeParams).toPromise()
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

  openAddOrUpdateEvaluatorDialog(skillsCheckExamResultProtocolEvaluatorId: number | null) {
    if (!this.data.skillsCheckExamResultProtocol) {
      throw new Error('onAddOrUpdate an evaluator requires a protocol to have been loaded.');
    }
    openTypedDialog(this.dialog, SkillsCheckExamResultProtocolViewDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        skillsCheckExamResultProtocolId: this.data.skillsCheckExamResultProtocol.skillsCheckExamResultProtocolId,
        skillsCheckExamResultProtocolEvaluatorId: skillsCheckExamResultProtocolEvaluatorId
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

  onRemoveEvaluator(skillsCheckExamResultProtocolEvaluatorId: number) {
    if (!this.data.skillsCheckExamResultProtocol) {
      throw new Error('Removing evaluator requires a protocol to have been loaded.');
    }
    this.removing = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      skillsCheckExamResultProtocolId: this.data.skillsCheckExamResultProtocol.skillsCheckExamResultProtocolId,
      skillsCheckExamResultProtocolEvaluatorId: skillsCheckExamResultProtocolEvaluatorId
    };
    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете оценителя?',
        errorsMessage: 'Не може да изтриете оценителя, защото:',
        httpAction: () => this.skillsCheckExamResultProtocolsService.removeEvaluator(removeParams).toPromise()
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
