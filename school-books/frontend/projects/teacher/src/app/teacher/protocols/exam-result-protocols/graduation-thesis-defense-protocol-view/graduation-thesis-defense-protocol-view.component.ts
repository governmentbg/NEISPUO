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
import {
  GraduationThesisDefenseProtocolsService,
  GraduationThesisDefenseProtocols_CreateRequestParams,
  GraduationThesisDefenseProtocols_Get
} from 'projects/sb-api-client/src/api/graduationThesisDefenseProtocols.service';
import { GraduationThesisDefenseProtocolsWordService } from 'projects/sb-api-client/src/api/graduationThesisDefenseProtocolsWord.service';
import { InstTeacherNomsService } from 'projects/sb-api-client/src/api/instTeacherNoms.service';
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
export class GraduationThesisDefenseProtocolViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    graduationThesisDefenseProtocolsService: GraduationThesisDefenseProtocolsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const graduationThesisDefenseProtocolId = tryParseInt(
      route.snapshot.paramMap.get('graduationThesisDefenseProtocolId')
    );
    if (graduationThesisDefenseProtocolId) {
      this.resolve(GraduationThesisDefenseProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        graduationThesisDefenseProtocol: graduationThesisDefenseProtocolsService.get({
          schoolYear,
          instId,
          graduationThesisDefenseProtocolId
        })
      });
    } else {
      this.resolve(GraduationThesisDefenseProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        graduationThesisDefenseProtocol: null
      });
    }
  }
}

@Component({
  selector: 'sb-graduation-thesis-defense-protocol-view',
  templateUrl: './graduation-thesis-defense-protocol-view.component.html'
})
export class GraduationThesisDefenseProtocolViewComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    institutionInfo: InstitutionInfoType;
    graduationThesisDefenseProtocol: GraduationThesisDefenseProtocols_Get | null;
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
  protocolName =
    '3-81Д Протокол на комисията за оценяване на изпит чрез защитa на дипломен проект - част по теория на професията';

  readonly form = this.fb.group({
    protocolName: [this.protocolName],
    protocolNumber: [null],
    protocolDate: [null],
    sessionType: [null],
    eduFormId: [null],
    commissionMeetingDate: [null, Validators.required],
    directorOrderNumber: [null, Validators.required],
    directorOrderDate: [null, Validators.required],
    directorPersonId: [null, Validators.required],
    commissionChairman: [null, Validators.required],
    commissionMembers: [null, Validators.required],
    section1StudentsCapacity: [null, Validators.required],
    section2StudentsCapacity: [null, Validators.required],
    section3StudentsCapacity: [null, Validators.required],
    section4StudentsCapacity: [null, Validators.required]
  });

  errors: string[] = [];
  removing = false;
  editable = false;
  eduFormNomsService: INomService<number, { instId: number; schoolYear: number }>;
  instTeacherNomsService: INomService<number, { instId: number; schoolYear: number }>;
  removingAct: { [key: number]: boolean } = {};
  canEdit = false;
  canRemove = false;

  constructor(
    private fb: UntypedFormBuilder,
    private graduationThesisDefenseProtocolsService: GraduationThesisDefenseProtocolsService,
    instTeacherNomsService: InstTeacherNomsService,
    eduFormNomsService: EduFormNomsService,
    private graduationThesisDefenseProtocolsWordService: GraduationThesisDefenseProtocolsWordService,
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
      schoolYear: this.data.schoolYear
    }));
  }

  ngOnInit(): void {
    const graduationThesisDefenseProtocol = this.data.graduationThesisDefenseProtocol;
    const protocolName = this.protocolName;

    this.loadingTemplate = true;

    this.graduationThesisDefenseProtocolsWordService
      .downloadGraduationThesisDefenseProtocolsWordTemplateFile({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        protocolId: 0
      })
      .toPromise()
      .then((url) => (this.downloadTemplateUrl = url))
      .finally(() => (this.loadingTemplate = false));

    if (graduationThesisDefenseProtocol != null) {
      this.loading = true;

      this.graduationThesisDefenseProtocolsWordService
        .downloadGraduationThesisDefenseProtocolsWordFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          protocolId: graduationThesisDefenseProtocol.graduationThesisDefenseProtocolId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url))
        .finally(() => (this.loading = false));

      const {
        protocolNumber,
        protocolDate,
        sessionType,
        eduFormId,
        commissionMeetingDate,
        directorOrderNumber,
        directorOrderDate,
        directorPersonId,
        commissionChairman,
        commissionMembers,
        section1StudentsCapacity,
        section2StudentsCapacity,
        section3StudentsCapacity,
        section4StudentsCapacity
      } = graduationThesisDefenseProtocol;

      this.form.setValue({
        protocolName,
        protocolNumber,
        protocolDate,
        sessionType,
        eduFormId,
        commissionMeetingDate,
        directorOrderNumber,
        directorOrderDate,
        directorPersonId,
        commissionChairman,
        commissionMembers,
        section1StudentsCapacity,
        section2StudentsCapacity,
        section3StudentsCapacity,
        section4StudentsCapacity
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
    const formValue = <
      GraduationThesisDefenseProtocols_CreateRequestParams['createGraduationThesisDefenseProtocolCommand']
    >this.form.value;

    if (formValue.commissionMembers.indexOf(formValue.commissionChairman) > -1) {
      this.errors = ['Членовете на комисията не трябва да се повтарят'];
      save.done(false);
      return;
    }

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.graduationThesisDefenseProtocol == null) {
            return this.graduationThesisDefenseProtocolsService
              .create({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                createGraduationThesisDefenseProtocolCommand: formValue
              })
              .toPromise()
              .then((newGraduationThesisDefenseProtocolId) => {
                this.form.markAsPristine();
                this.router.navigate(['../graduationThesisDefense', newGraduationThesisDefenseProtocolId], {
                  relativeTo: this.route
                });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.graduationThesisDefenseProtocol.schoolYear,
              instId: this.data.instId,
              graduationThesisDefenseProtocolId:
                this.data.graduationThesisDefenseProtocol.graduationThesisDefenseProtocolId
            };
            return this.graduationThesisDefenseProtocolsService
              .update({
                updateGraduationThesisDefenseProtocolCommand: formValue,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.graduationThesisDefenseProtocolsService.get(updateArgs).toPromise())
              .then((newGraduationThesisDefenseProtocol) => {
                this.data.graduationThesisDefenseProtocol = newGraduationThesisDefenseProtocol;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.graduationThesisDefenseProtocol) {
      throw new Error('Protocol is not created.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      graduationThesisDefenseProtocolId: this.data.graduationThesisDefenseProtocol.graduationThesisDefenseProtocolId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете протокола?',
        errorsMessage: 'Не може да изтриете протокола, защото:',
        httpAction: () => this.graduationThesisDefenseProtocolsService.remove(removeParams).toPromise()
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
