import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faCalendarMinus as fadCalendarMinus } from '@fortawesome/pro-duotone-svg-icons/faCalendarMinus';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import {
  AbsencesByStudentsReportsService,
  AbsencesByStudentsReports_Get
} from 'projects/sb-api-client/src/api/absencesByStudentsReports.service';
import { ClassBookNomsService } from 'projects/sb-api-client/src/api/classBookNoms.service';
import { ReportPeriodNomsService } from 'projects/sb-api-client/src/api/reportPeriodNoms.service';
import { AbsencesByStudentsReportsGetItemsVO } from 'projects/sb-api-client/src/model/absencesByStudentsReportsGetItemsVO';
import { ReportPeriod } from 'projects/sb-api-client/src/model/reportPeriod';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TableDataSource, TableResult } from 'projects/shared/components/table/table-datasource';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { formatDateTime } from 'projects/shared/utils/date';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class AbsencesByStudentsReportViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    absencesByStudentsReportsService: AbsencesByStudentsReportsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const absencesByStudentsReportId = tryParseInt(route.snapshot.paramMap.get('absencesByStudentsReportId'));
    if (absencesByStudentsReportId) {
      this.resolve(AbsencesByStudentsReportViewComponent, {
        schoolYear,
        instId,
        absencesByStudentsReport: absencesByStudentsReportsService.get({
          schoolYear,
          instId,
          absencesByStudentsReportId
        })
      });
    } else {
      this.resolve(AbsencesByStudentsReportViewComponent, {
        schoolYear,
        instId,
        absencesByStudentsReport: null
      });
    }
  }
}

@Component({
  selector: 'sb-absences-by-students-report-view',
  templateUrl: './absences-by-students-report-view.component.html'
})
export class AbsencesByStudentsReportViewComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    absencesByStudentsReport: AbsencesByStudentsReports_Get | null;
  };

  readonly fadCalendarMinus = fadCalendarMinus;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasFileExcel = fasFileExcel;

  readonly form = this.fb.nonNullable.group({
    period: this.fb.nonNullable.control<ReportPeriod | null | undefined>(null, Validators.required),
    classBookIds: this.fb.nonNullable.control<number[]>([]),
    classBookNames: this.fb.nonNullable.control<string | null | undefined>(null)
  });

  private readonly destroyed$ = new Subject<void>();
  removing = false;
  dataSource?: TableDataSource<TableResult<AbsencesByStudentsReportsGetItemsVO>>;

  downloadUrl?: string;
  reoportName?: string;

  reportPeriodNomsService: INomService<ReportPeriod, { instId: number; schoolYear: number }>;
  classBookNomsService: INomService<number, { instId: number; schoolYear: number }>;

  constructor(
    private fb: FormBuilder,
    private absencesByStudentsReportsService: AbsencesByStudentsReportsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    reportPeriodNomsService: ReportPeriodNomsService,
    classBookNomsService: ClassBookNomsService
  ) {
    this.reportPeriodNomsService = new NomServiceWithParams(reportPeriodNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));
    this.classBookNomsService = new NomServiceWithParams(classBookNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      showPG: false,
      showDplr: false
    }));
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  ngOnInit() {
    if (this.data.absencesByStudentsReport != null) {
      const absencesByStudentsReportId = this.data.absencesByStudentsReport.absencesByStudentsReportId;
      this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
        this.absencesByStudentsReportsService.getItems({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          absencesByStudentsReportId: absencesByStudentsReportId,
          offset,
          limit
        })
      );

      const { period, classBookNames, createDate } = this.data.absencesByStudentsReport;
      const periodText =
        period === ReportPeriod.TermOne
          ? 'първи срок'
          : period === ReportPeriod.TermTwo
          ? 'втори срок'
          : 'цялата година';
      if (classBookNames) {
        this.reoportName = `Отсъствия по ученици от ${classBookNames} за ${periodText} към дата ${formatDateTime(
          createDate
        )}`;
      } else {
        this.reoportName = `Отсъствия по ученици за ${periodText} към дата ${formatDateTime(createDate)}`;
      }

      this.form.setValue({
        period,
        classBookIds: [],
        classBookNames
      });

      this.absencesByStudentsReportsService
        .downloadExcelFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          absencesByStudentsReportId: this.data.absencesByStudentsReport.absencesByStudentsReportId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url));
    }
  }

  onSave(save: SaveToken) {
    const { period, classBookIds } = this.form.getRawValue();
    const absencesByStudentsReport = {
      period: period ?? throwError("'period' should not be null"),
      classBookIds: classBookIds
    };

    this.actionService
      .execute({
        httpAction: () => {
          return this.absencesByStudentsReportsService
            .create({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              createAbsencesByStudentsReportCommand: absencesByStudentsReport
            })
            .toPromise()
            .then((newAbsencesByStudentsReportId) => {
              this.router.navigate(['../', newAbsencesByStudentsReportId], { relativeTo: this.route });
            });
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.absencesByStudentsReport) {
      throw new Error('onRemove requires a report to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      absencesByStudentsReportId: this.data.absencesByStudentsReport.absencesByStudentsReportId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете справката?',
        errorsMessage: 'Не може да изтриете справката, защото:',
        httpAction: () => this.absencesByStudentsReportsService.remove(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          this.router.navigate(['../'], { relativeTo: this.route });
        }
      })
      .finally(() => {
        this.removing = false;
      });
  }
}
