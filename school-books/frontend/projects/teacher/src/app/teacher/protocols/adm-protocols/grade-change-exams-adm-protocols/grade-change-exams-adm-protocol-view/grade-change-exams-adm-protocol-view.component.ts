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
  GradeChangeExamsAdmProtocolsService,
  GradeChangeExamsAdmProtocols_CreateRequestParams,
  GradeChangeExamsAdmProtocols_Get,
  GradeChangeExamsAdmProtocols_GetStudentAll
} from 'projects/sb-api-client/src/api/gradeChangeExamsAdmProtocols.service';
import { GradeChangeExamsAdmProtocolsWordService } from 'projects/sb-api-client/src/api/gradeChangeExamsAdmProtocolsWord.service';
import { InstitutionStudentNoms_GetNomsById } from 'projects/sb-api-client/src/api/institutionStudentNoms.service';
import { InstTeacherNomsService } from 'projects/sb-api-client/src/api/instTeacherNoms.service';
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
import { AddOrUpdateStudentGceAdmProtocolDialogSkeletonComponent } from '../add-or-update-student-gce-adm-protocol-dialog/add-or-update-student-gce-adm-protocol-dialog.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class GradeChangeExamsAdmProtocolViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    gradeChangeExamsAdmProtocolsService: GradeChangeExamsAdmProtocolsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const gradeChangeExamsAdmProtocolId = tryParseInt(route.snapshot.paramMap.get('gradeChangeExamsAdmProtocolId'));

    if (gradeChangeExamsAdmProtocolId) {
      this.resolve(GradeChangeExamsAdmProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        gradeChangeExamsAdmProtocol: gradeChangeExamsAdmProtocolsService.get({
          schoolYear,
          instId,
          gradeChangeExamsAdmProtocolId
        })
      });
    } else {
      this.resolve(GradeChangeExamsAdmProtocolViewComponent, {
        schoolYear,
        instId,
        institutionInfo: from(institutionInfo),
        gradeChangeExamsAdmProtocol: null
      });
    }
  }
}

type StudentNomVO = ArrayElementType<InstitutionStudentNoms_GetNomsById>['id'];
const PROTOCOL_NAME = '3-79A Протокол за допускане на учениците до изпити за промяна на оценката';

@Component({
  selector: 'sb-grade-change-exams-adm-protocol-view',
  templateUrl: './grade-change-exams-adm-protocol-view.component.html'
})
export class GradeChangeExamsAdmProtocolViewComponent implements OnInit, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    institutionInfo: InstitutionInfoType;
    gradeChangeExamsAdmProtocol: GradeChangeExamsAdmProtocols_Get | null;
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
  loadingTemplate?: boolean;
  instTeacherNomsService: INomService<number, { instId: number; schoolYear: number }>;
  removingStudent: { [key: string]: boolean } = {};
  dataSource!: TableDataSource<GradeChangeExamsAdmProtocols_GetStudentAll>;
  canEdit = false;
  canRemove = false;

  constructor(
    private fb: UntypedFormBuilder,
    private gradeChangeExamsAdmProtocolsService: GradeChangeExamsAdmProtocolsService,
    private gradeChangeExamsAdmProtocolsWordService: GradeChangeExamsAdmProtocolsWordService,
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
      this.gradeChangeExamsAdmProtocolsService.getStudentAll({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        gradeChangeExamsAdmProtocolId: this.data.gradeChangeExamsAdmProtocol!.gradeChangeExamsAdmProtocolId,
        offset,
        limit
      })
    );
  }

  ngOnInit(): void {
    const gradeChangeExamsAdmProtocol = this.data.gradeChangeExamsAdmProtocol;
    const protocolName = PROTOCOL_NAME;
    this.loadingTemplate = true;

    this.gradeChangeExamsAdmProtocolsWordService
      .downloadGradeChangeExamsAdmProtocolsWordTemplateFile({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        protocolId: 0
      })
      .toPromise()
      .then((url) => (this.downloadTemplateUrl = url))
      .finally(() => (this.loadingTemplate = false));

    if (gradeChangeExamsAdmProtocol != null) {
      this.loading = true;

      this.gradeChangeExamsAdmProtocolsWordService
        .downloadGradeChangeExamsAdmProtocolsWordFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          protocolId: gradeChangeExamsAdmProtocol.gradeChangeExamsAdmProtocolId
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
      } = gradeChangeExamsAdmProtocol;

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
    const formValue = <GradeChangeExamsAdmProtocols_CreateRequestParams['createGradeChangeExamsAdmProtocolCommand']>(
      this.form.value
    );

    if (formValue.commissionMembers.indexOf(formValue.commissionChairman) > -1) {
      this.errors = ['Членовете на комисията не трябва да се повтарят'];
      save.done(false);
      return;
    }

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.gradeChangeExamsAdmProtocol == null) {
            return this.gradeChangeExamsAdmProtocolsService
              .create({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                createGradeChangeExamsAdmProtocolCommand: formValue
              })
              .toPromise()
              .then((gradeChangeExamsAdmProtocolId) => {
                this.form.markAsPristine();
                this.router.navigate(['../', gradeChangeExamsAdmProtocolId], {
                  relativeTo: this.route
                });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              gradeChangeExamsAdmProtocolId: this.data.gradeChangeExamsAdmProtocol.gradeChangeExamsAdmProtocolId
            };
            return this.gradeChangeExamsAdmProtocolsService
              .update({
                updateGradeChangeExamsAdmProtocolCommand: formValue,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.gradeChangeExamsAdmProtocolsService.get(updateArgs).toPromise())
              .then((newGradeChangeExamsAdmProtocol) => {
                this.data.gradeChangeExamsAdmProtocol = newGradeChangeExamsAdmProtocol;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.gradeChangeExamsAdmProtocol) {
      throw new Error('onRemove requires a gradeChangeExamsAdmProtocol to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      gradeChangeExamsAdmProtocolId: this.data.gradeChangeExamsAdmProtocol.gradeChangeExamsAdmProtocolId
    };

    this.actionService
      .execute({
        confirmMessage:
          'Сигурни ли сте, че искате да изтриете протокола за допускане до изпити за промяна на оценката?',
        errorsMessage: 'Не може да изтриете протокола за допускане до изпити за промяна на оценката, защото:',
        httpAction: () => this.gradeChangeExamsAdmProtocolsService.remove(removeParams).toPromise()
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
    if (!this.data.gradeChangeExamsAdmProtocol) {
      throw new Error('openAddOrUpdateStudent requires a gradeChangeExamsAdmProtocol to have been loaded.');
    }

    openTypedDialog(this.dialog, AddOrUpdateStudentGceAdmProtocolDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        gradeChangeExamsAdmProtocolId: this.data.gradeChangeExamsAdmProtocol.gradeChangeExamsAdmProtocolId,
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
    if (!this.data.gradeChangeExamsAdmProtocol) {
      throw new Error('onRemoveStudent requires a gradeChangeExamsAdmProtocol to have been loaded.');
    }

    const removingKey = `${(<StudentNomVO>studentKey).classId}-${(<StudentNomVO>studentKey).personId}`;
    this.removingStudent[removingKey] = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      gradeChangeExamsAdmProtocolId: this.data.gradeChangeExamsAdmProtocol.gradeChangeExamsAdmProtocolId,
      classId: (<StudentNomVO>studentKey).classId,
      personId: (<StudentNomVO>studentKey).personId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете ученика?',
        errorsMessage: 'Не може да изтриете студента, защото:',
        httpAction: () => this.gradeChangeExamsAdmProtocolsService.removeStudent(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          return this.dataSource.reload();
        }
      })
      .finally(() => {
        this.removingStudent[removingKey] = false;
      });
  }
}
