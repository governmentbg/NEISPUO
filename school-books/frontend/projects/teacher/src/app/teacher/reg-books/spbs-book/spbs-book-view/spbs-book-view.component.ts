import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faPencil as fasPencil } from '@fortawesome/pro-solid-svg-icons/faPencil';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import {
  SpbsBookService,
  SpbsBook_Get,
  SpbsBook_GetAbsenceAll,
  SpbsBook_GetEscapeAll,
  SpbsBook_UpdateRequestParams
} from 'projects/sb-api-client/src/api/spbsBook.service';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Subject } from 'rxjs';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';
import { SpbsBookAbsenceViewDialogSkeletonComponent } from '../spbs-book-absence-view-dialog/spbs-book-absence-view-dialog.component';
import { SpbsBookEscapeViewDialogSkeletonComponent } from '../spbs-book-escape-view-dialog/spbs-book-escape-view-dialog.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class SpbsBookViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    spbsBookService: SpbsBookService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const spbsBookRecordId =
      tryParseInt(route.snapshot.paramMap.get('spbsBookRecordId')) ?? throwParamError('spbsBookRecordId');
    const recordSchoolYear = tryParseInt(route.snapshot.paramMap.get('recordSchoolYear')) ?? schoolYear;

    this.resolve(SpbsBookViewComponent, {
      schoolYear,
      instId,
      institutionInfo: from(institutionInfo),
      recordSchoolYear,
      spbsBookRecordId,
      spbsBookRecord: spbsBookService.get({
        schoolYear: recordSchoolYear,
        instId,
        spbsBookRecordId
      })
    });
  }
}

@Component({
  selector: 'sb-spbs-book-view',
  templateUrl: './spbs-book-view.component.html'
})
export class SpbsBookViewComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    institutionInfo: InstitutionInfoType;
    recordSchoolYear: number;
    spbsBookRecordId: number;
    spbsBookRecord: SpbsBook_Get;
  };

  private readonly destroyed$ = new Subject<void>();

  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasPlus = fasPlus;
  readonly fasPencil = fasPencil;
  readonly fadChevronCircleRight = fadChevronCircleRight;
  loading?: boolean;

  readonly form = this.fb.group({
    schoolYear: [null],
    recordNumber: [null],
    personalId: [null],
    firstName: [null],
    middleName: [null],
    lastName: [null],
    gender: [null],
    className: [null],
    birthPlaceCountry: [null],
    birthPlaceTown: [null],
    permanentTown: [null],
    permanentAddress: [null],
    sendingCommission: [null],
    sendingCommissionAddress: [null],
    sendingCommissionPhoneNumber: [null],
    inspectorNames: [null],
    inspectorAddress: [null],
    inspectorPhoneNumber: [null],
    courtDecisionNumber: [null],
    courtDecisionDate: [null],
    incommingLetterNumber: [null],
    incommingLetterDate: [null],
    incommingDate: [null],
    incommingDocNumber: [null],
    transferReason: [null],
    transferProtocolNumber: [null],
    transferProtocolDate: [null],
    transferLetterNumber: [null],
    transferLetterDate: [null],
    transferCertificateNumber: [null],
    transferCertificateDate: [null],
    transferMessageNumber: [null],
    transferMessageDate: [null]
  });

  removing = false;
  editable = false;
  escapesDataSource!: TableDataSource<SpbsBook_GetEscapeAll>;
  absencesDataSource!: TableDataSource<SpbsBook_GetAbsenceAll>;

  canEdit = false;

  canRemove = false;
  removingEscapeAct: { [key: number]: boolean } = {};
  removingAbsenceAct: { [key: number]: boolean } = {};

  constructor(
    private fb: UntypedFormBuilder,
    private spbsBookService: SpbsBookService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private dialog: MatDialog
  ) {
    this.escapesDataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      this.spbsBookService.getEscapeAll({
        schoolYear: this.data.recordSchoolYear,
        instId: this.data.instId,
        spbsBookRecordId: this.data.spbsBookRecordId,
        offset,
        limit
      })
    );

    this.absencesDataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      this.spbsBookService.getAbsenceAll({
        schoolYear: this.data.recordSchoolYear,
        instId: this.data.instId,
        spbsBookRecordId: this.data.spbsBookRecordId,
        offset,
        limit
      })
    );
  }

  ngOnInit(): void {
    this.loading = true;

    const spbsBookRecord = this.data.spbsBookRecord;
    if (spbsBookRecord != null) {
      const {
        studentPersonalInfo: {
          schoolYear,
          recordNumber,
          personalId,
          firstName,
          middleName,
          lastName,
          gender,
          className,
          birthPlaceCountry,
          birthPlaceTown,
          permanentTown,
          permanentAddress
        },
        sendingCommission,
        sendingCommissionAddress,
        sendingCommissionPhoneNumber,
        inspectorNames,
        inspectorAddress,
        inspectorPhoneNumber,
        movement: {
          courtDecisionNumber,
          courtDecisionDate,
          incommingLetterNumber,
          incommingLetterDate,
          incommingDate,
          incommingDocNumber,
          transferReason,
          transferProtocolNumber,
          transferProtocolDate,
          transferLetterNumber,
          transferLetterDate,
          transferCertificateNumber,
          transferCertificateDate,
          transferMessageNumber,
          transferMessageDate
        }
      } = spbsBookRecord;

      this.form.setValue({
        schoolYear,
        recordNumber,
        personalId,
        firstName,
        middleName,
        lastName,
        gender,
        className,
        birthPlaceCountry,
        birthPlaceTown,
        permanentTown,
        permanentAddress,
        sendingCommission,
        sendingCommissionAddress,
        sendingCommissionPhoneNumber,
        inspectorNames,
        inspectorAddress,
        inspectorPhoneNumber,
        courtDecisionNumber,
        courtDecisionDate,
        incommingLetterNumber,
        incommingLetterDate,
        incommingDate,
        incommingDocNumber,
        transferReason,
        transferProtocolNumber,
        transferProtocolDate,
        transferLetterNumber,
        transferLetterDate,
        transferCertificateNumber,
        transferCertificateDate,
        transferMessageNumber,
        transferMessageDate
      });
    }

    this.canEdit =
      this.data.institutionInfo.schoolYearAllowsModifications && this.data.institutionInfo.hasSpbsBookEditAccess;
    this.canRemove =
      this.data.institutionInfo.schoolYearAllowsModifications && this.data.institutionInfo.hasSpbsBookRemoveAccess;
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  onSave(save: SaveToken) {
    const formValue = <SpbsBook_UpdateRequestParams['updateSpbsBookRecordCommand']>this.form.value;

    this.actionService
      .execute({
        httpAction: () => {
          const updateArgs = {
            schoolYear: this.data.recordSchoolYear,
            instId: this.data.instId,
            spbsBookRecordId: this.data.spbsBookRecordId
          };
          return this.spbsBookService
            .update({
              updateSpbsBookRecordCommand: formValue,
              ...updateArgs
            })
            .toPromise()
            .then(() => this.spbsBookService.get(updateArgs).toPromise())
            .then((newSpbsBookRecord) => {
              this.data.spbsBookRecord = newSpbsBookRecord;
            });
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.spbsBookRecord) {
      throw new Error('onRemove requires a spbsBookRecord to have been loaded.');
    }
    this.removing = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      spbsBookRecordId: this.data.spbsBookRecordId
    };
    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете записа?',
        errorsMessage: 'Не може да изтриете записа, защото:',
        httpAction: () => this.spbsBookService.remove(removeParams).toPromise()
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

  openAddOrUpdateEscapeDialog(orderNum: number | null) {
    openTypedDialog(this.dialog, SpbsBookEscapeViewDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        recordSchoolYear: this.data.recordSchoolYear,
        spbsBookRecordId: this.data.spbsBookRecordId,
        orderNum
      }
    })
      .afterClosed()
      .toPromise()
      .then((ok) => {
        if (ok) {
          return this.escapesDataSource.reload();
        }

        return Promise.resolve();
      });
  }

  onRemoveEscape(orderNum: number) {
    if (!this.data.spbsBookRecord) {
      throw new Error('onRemoveEscape requires its spbsBookRecord to have been loaded.');
    }
    this.removingEscapeAct[orderNum] = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      spbsBookRecordId: this.data.spbsBookRecordId,
      orderNum: orderNum
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете бягството?',
        errorsMessage: 'Не може да изтриете бягството, защото:',
        httpAction: () => this.spbsBookService.removeEscape(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          return this.escapesDataSource.reload();
        }
      })
      .finally(() => {
        this.removingEscapeAct[orderNum] = false;
      });
  }

  openAddOrUpdateAbsenceDialog(orderNum: number | null) {
    openTypedDialog(this.dialog, SpbsBookAbsenceViewDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        recordSchoolYear: this.data.recordSchoolYear,
        spbsBookRecordId: this.data.spbsBookRecordId,
        orderNum
      }
    })
      .afterClosed()
      .toPromise()
      .then((ok) => {
        if (ok) {
          return this.absencesDataSource.reload();
        }

        return Promise.resolve();
      });
  }

  onRemoveAbsence(orderNum: number) {
    if (!this.data.spbsBookRecord) {
      throw new Error('onRemoveAbsence requires its spbsBookRecord to have been loaded.');
    }
    this.removingAbsenceAct[orderNum] = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      spbsBookRecordId: this.data.spbsBookRecordId,
      orderNum: orderNum
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете отсъствието?',
        errorsMessage: 'Не може да изтриете отсъствието, защото:',
        httpAction: () => this.spbsBookService.removeAbsence(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          return this.absencesDataSource.reload();
        }
      })
      .finally(() => {
        this.removingAbsenceAct[orderNum] = false;
      });
  }
}
