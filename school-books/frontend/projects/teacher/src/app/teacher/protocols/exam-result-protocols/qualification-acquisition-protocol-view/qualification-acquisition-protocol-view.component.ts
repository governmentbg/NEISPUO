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
  QualificationAcquisitionProtocolsService,
  QualificationAcquisitionProtocols_CreateRequestParams,
  QualificationAcquisitionProtocols_Get,
  QualificationAcquisitionProtocols_GetStudentAll
} from 'projects/sb-api-client/src/api/qualificationAcquisitionProtocols.service';
import { QualificationAcquisitionProtocolsWordService } from 'projects/sb-api-client/src/api/qualificationAcquisitionProtocolsWord.service';
import { QualificationAcquisitionProtocolTypeNomsService } from 'projects/sb-api-client/src/api/qualificationAcquisitionProtocolTypeNoms.service';
import { QualificationDegreeNomsService } from 'projects/sb-api-client/src/api/qualificationDegreeNoms.service';
import { QualificationAcquisitionProtocolType } from 'projects/sb-api-client/src/model/qualificationAcquisitionProtocolType';
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
import { enumFromStringValue, throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from } from 'rxjs';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';
import { QualificationAcquisitionProtocolViewDialogSkeletonComponent } from '../qualification-acquisition-protocol-student-view-dialog/qualification-acquisition-protocol-student-view-dialog.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class QualificationAcquisitionProtocolViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    qualificationAcquisitionProtocolsService: QualificationAcquisitionProtocolsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const qualificationAcquisitionProtocolId = tryParseInt(
      route.snapshot.paramMap.get('qualificationAcquisitionProtocolId')
    );

    const type = <QualificationAcquisitionProtocolType | null>route.snapshot.paramMap.get('type') ?? null;

    if (qualificationAcquisitionProtocolId) {
      this.resolve(QualificationAcquisitionProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        qualificationAcquisitionProtocol: qualificationAcquisitionProtocolsService.get({
          schoolYear,
          instId,
          qualificationAcquisitionProtocolId
        }),
        protocolType: type
      });
    } else {
      this.resolve(QualificationAcquisitionProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        qualificationAcquisitionProtocol: null,
        protocolType: type
      });
    }
  }
}

type StudentNomVO = ArrayElementType<InstitutionStudentNoms_GetNomsById>['id'];

@Component({
  selector: 'sb-qualification-acquisition-protocol-view',
  templateUrl: './qualification-acquisition-protocol-view.component.html'
})
export class QualificationAcquisitionProtocolViewComponent implements OnInit, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    institutionInfo: InstitutionInfoType;
    qualificationAcquisitionProtocol: QualificationAcquisitionProtocols_Get | null;
    protocolType: QualificationAcquisitionProtocolType | null;
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

  readonly form = this.fb.group({
    protocolName: [null],
    profession: [null, Validators.required],
    speciality: [null, Validators.required],
    qualificationDegreeId: [null, Validators.required],
    protocolNumber: [null],
    protocolDate: [null],
    date: [null, Validators.required],
    commissionNominationOrderNumber: [null, Validators.required],
    commissionNominationOrderDate: [null, Validators.required],
    directorPersonId: [null, Validators.required],
    commissionChairman: [null, Validators.required],
    commissionMembers: [null, Validators.required]
  });

  errors: string[] = [];

  removing = false;
  editable = false;
  instTeacherNomsService: INomService<number, { instId: number; schoolYear: number }>;
  protocolTypeNomsService: INomService<QualificationAcquisitionProtocolType, { instId: number; schoolYear: number }>;
  qualificationDegreeNomsService: INomService<number, { instId: number; schoolYear: number }>;
  removingAct: { [key: number]: boolean } = {};
  dataSource!: TableDataSource<QualificationAcquisitionProtocols_GetStudentAll>;
  canEdit = false;
  canRemove = false;

  constructor(
    private fb: UntypedFormBuilder,
    private qualificationAcquisitionProtocolsService: QualificationAcquisitionProtocolsService,
    instTeacherNomsService: InstTeacherNomsService,
    protocolTypeNomsService: QualificationAcquisitionProtocolTypeNomsService,
    qualificationDegreeNomsService: QualificationDegreeNomsService,
    private qualificationAcquisitionProtocolsWordService: QualificationAcquisitionProtocolsWordService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private dialog: MatDialog
  ) {
    this.instTeacherNomsService = new NomServiceWithParams(instTeacherNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.protocolTypeNomsService = new NomServiceWithParams(protocolTypeNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.qualificationDegreeNomsService = new NomServiceWithParams(qualificationDegreeNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      this.qualificationAcquisitionProtocolsService.getStudentAll({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        qualificationAcquisitionProtocolId:
          this.data.qualificationAcquisitionProtocol!.qualificationAcquisitionProtocolId,
        offset,
        limit
      })
    );
  }

  ngOnInit(): void {
    this.loadingTemplate = true;

    const protocolType = enumFromStringValue(
      QualificationAcquisitionProtocolType,
      this.data.protocolType ??
        this.data.qualificationAcquisitionProtocol?.protocolType ??
        throwError('protocol type should not be null')
    );

    this.qualificationAcquisitionProtocolsWordService
      .downloadQualificationAcquisitionProtocolsWordTemplateFile({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        protocolId: 0,
        protocolType: protocolType!
      })
      .toPromise()
      .then((url) => (this.downloadTemplateUrl = url))
      .finally(() => (this.loadingTemplate = false));

    const qualificationAcquisitionProtocol = this.data.qualificationAcquisitionProtocol;
    if (qualificationAcquisitionProtocol != null) {
      this.loading = true;

      this.qualificationAcquisitionProtocolsWordService
        .downloadQualificationAcquisitionProtocolsWordFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          protocolId: qualificationAcquisitionProtocol.qualificationAcquisitionProtocolId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url))
        .finally(() => (this.loading = false));

      const {
        profession,
        speciality,
        qualificationDegreeId,
        protocolNumber,
        protocolDate,
        date,
        commissionNominationOrderNumber,
        commissionNominationOrderDate,
        directorPersonId,
        commissionChairman,
        commissionMembers
      } = qualificationAcquisitionProtocol;

      const protocolName = this.getProtocolName(qualificationAcquisitionProtocol.protocolType);
      this.form.setValue({
        protocolName,
        profession,
        speciality,
        qualificationDegreeId,
        protocolNumber,
        protocolDate,
        date,
        commissionNominationOrderNumber,
        commissionNominationOrderDate,
        directorPersonId,
        commissionChairman,
        commissionMembers
      });
    } else {
      const protocolName = this.getProtocolName(this.data.protocolType);
      this.form.setValue({
        protocolName,
        profession: null,
        speciality: null,
        qualificationDegreeId: null,
        protocolNumber: null,
        protocolDate: null,
        date: null,
        commissionNominationOrderNumber: null,
        commissionNominationOrderDate: null,
        directorPersonId: this.data.institutionInfo.directorPersonId,
        commissionChairman: null,
        commissionMembers: null
      });
    }

    this.canEdit = this.data.institutionInfo.hasProtocolsEditAccess;
    this.canRemove = this.data.institutionInfo.hasProtocolsRemoveAccess;
  }

  private getProtocolName(protocolType: QualificationAcquisitionProtocolType | null): string {
    switch (protocolType) {
      case QualificationAcquisitionProtocolType.QualificationAcquisition:
        return '3-81В Протокол за придобиване на професионална квалификация';
      case QualificationAcquisitionProtocolType.QualificationAcquisitionExamGrades:
        return '3-81В ПОО Протокол за оценките от държавен изпит за придобиване на степен на професионална квалификация';
      case QualificationAcquisitionProtocolType.QualificationAcquisitionStateExamGrades:
        return '3-81В ПОО Протокол за придобиване на професионална квалификация по част от професия';
      default:
        throw new Error('Unknown protocolType');
    }
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave(save: SaveToken) {
    const formValue = <
      QualificationAcquisitionProtocols_CreateRequestParams['createQualificationAcquisitionProtocolCommand']
    >this.form.value;

    const protocolType =
      this.data.qualificationAcquisitionProtocol != null
        ? this.data.qualificationAcquisitionProtocol.protocolType
        : this.data.protocolType;

    formValue.protocolType =
      protocolType === QualificationAcquisitionProtocolType.QualificationAcquisition ||
      protocolType === QualificationAcquisitionProtocolType.QualificationAcquisitionExamGrades ||
      protocolType === QualificationAcquisitionProtocolType.QualificationAcquisitionStateExamGrades
        ? protocolType
        : throwError('Unknown protocolType');

    if (formValue.commissionMembers.indexOf(formValue.commissionChairman) > -1) {
      this.errors = ['Членовете на комисията не трябва да се повтарят'];
      save.done(false);
      return;
    }

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.qualificationAcquisitionProtocol == null) {
            return this.qualificationAcquisitionProtocolsService
              .create({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                createQualificationAcquisitionProtocolCommand: formValue
              })
              .toPromise()
              .then((newQualificationAcquisitionProtocolId) => {
                this.form.markAsPristine();
                this.router.navigate(['../qualificationAcquisition', newQualificationAcquisitionProtocolId], {
                  relativeTo: this.route
                });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              qualificationAcquisitionProtocolId:
                this.data.qualificationAcquisitionProtocol.qualificationAcquisitionProtocolId
            };
            return this.qualificationAcquisitionProtocolsService
              .update({
                updateQualificationAcquisitionProtocolCommand: formValue,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.qualificationAcquisitionProtocolsService.get(updateArgs).toPromise())
              .then((newQualificationAcquisitionProtocol) => {
                this.data.qualificationAcquisitionProtocol = newQualificationAcquisitionProtocol;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.qualificationAcquisitionProtocol) {
      throw new Error('Removing a student equires a protocol to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      qualificationAcquisitionProtocolId: this.data.qualificationAcquisitionProtocol.qualificationAcquisitionProtocolId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете протокола?',
        errorsMessage: 'Не може да изтриете протокола, защото:',
        httpAction: () => this.qualificationAcquisitionProtocolsService.remove(removeParams).toPromise()
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
    if (!this.data.qualificationAcquisitionProtocol) {
      throw new Error('onAddOrUpdate a student requires a protocol to have been loaded.');
    }
    openTypedDialog(this.dialog, QualificationAcquisitionProtocolViewDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        qualificationAcquisitionProtocolId:
          this.data.qualificationAcquisitionProtocol.qualificationAcquisitionProtocolId,
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

  onRemoveStudent(classId: number, personId: number) {
    if (!this.data.qualificationAcquisitionProtocol) {
      throw new Error('Removing student requires a protocol to have been loaded.');
    }
    this.removing = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      qualificationAcquisitionProtocolId: this.data.qualificationAcquisitionProtocol.qualificationAcquisitionProtocolId,
      classId: classId,
      personId: personId
    };
    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете ученика?',
        errorsMessage: 'Не може да изтриете ученика, защото:',
        httpAction: () => this.qualificationAcquisitionProtocolsService.removeStudent(removeParams).toPromise()
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
