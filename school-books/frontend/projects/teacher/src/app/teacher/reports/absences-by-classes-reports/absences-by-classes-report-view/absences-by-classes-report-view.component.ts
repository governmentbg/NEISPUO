import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faCalendarMinus as fadCalendarMinus } from '@fortawesome/pro-duotone-svg-icons/faCalendarMinus';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import {
  AbsencesByClassesReportsService,
  AbsencesByClassesReports_Get
} from 'projects/sb-api-client/src/api/absencesByClassesReports.service';
import { ClassBookNomsService } from 'projects/sb-api-client/src/api/classBookNoms.service';
import { ReportPeriodNomsService } from 'projects/sb-api-client/src/api/reportPeriodNoms.service';
import { AbsencesByClassesReportsGetItemsVO } from 'projects/sb-api-client/src/model/absencesByClassesReportsGetItemsVO';
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
export class AbsencesByClassesReportViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    absencesByClassesReportsService: AbsencesByClassesReportsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const absencesByClassesReportId = tryParseInt(route.snapshot.paramMap.get('absencesByClassesReportId'));
    if (absencesByClassesReportId) {
      this.resolve(AbsencesByClassesReportViewComponent, {
        schoolYear,
        instId,
        absencesByClassesReport: absencesByClassesReportsService.get({
          schoolYear,
          instId,
          absencesByClassesReportId
        })
      });
    } else {
      this.resolve(AbsencesByClassesReportViewComponent, {
        schoolYear,
        instId,
        absencesByClassesReport: null
      });
    }
  }
}

@Component({
  selector: 'sb-absences-by-classes-report-view',
  templateUrl: './absences-by-classes-report-view.component.html'
})
export class AbsencesByClassesReportViewComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    absencesByClassesReport: AbsencesByClassesReports_Get | null;
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
  dataSource?: TableDataSource<TableResult<AbsencesByClassesReportsGetItemsVO>>;

  downloadUrl?: string;
  reportName?: string;

  reportPeriodNomsService: INomService<ReportPeriod, { instId: number; schoolYear: number }>;
  classBookNomsService: INomService<number, { instId: number; schoolYear: number }>;

  constructor(
    private fb: FormBuilder,
    private absencesByClassesReportsService: AbsencesByClassesReportsService,
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
    if (this.data.absencesByClassesReport != null) {
      const absencesByClassesReportId = this.data.absencesByClassesReport.absencesByClassesReportId;
      this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
        this.absencesByClassesReportsService.getItems({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          absencesByClassesReportId: absencesByClassesReportId,
          offset,
          limit
        })
      );

      const { period, classBookNames, createDate } = this.data.absencesByClassesReport;
      const periodText =
        period === ReportPeriod.TermOne
          ? 'първи срок'
          : period === ReportPeriod.TermTwo
          ? 'втори срок'
          : 'цялата година';
      if (classBookNames) {
        this.reportName = `Отсъствия по класове от ${classBookNames} за ${periodText} към дата ${formatDateTime(
          createDate
        )}`;
      } else {
        this.reportName = `Отсъствия по класове за ${periodText} към дата ${formatDateTime(createDate)}`;
      }

      this.form.setValue({
        period,
        classBookIds: [],
        classBookNames
      });

      this.absencesByClassesReportsService
        .downloadExcelFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          absencesByClassesReportId: this.data.absencesByClassesReport.absencesByClassesReportId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url));
    }
  }

  onSave(save: SaveToken) {
    const { period, classBookIds } = this.form.getRawValue();
    const absencesByClassesReport = {
      period: period ?? throwError("'period' should not be null"),
      classBookIds: classBookIds
    };

    this.actionService
      .execute({
        httpAction: () => {
          return this.absencesByClassesReportsService
            .create({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              createAbsencesByClassesReportCommand: absencesByClassesReport
            })
            .toPromise()
            .then((newAbsencesByClassesReportId) => {
              this.router.navigate(['../', newAbsencesByClassesReportId], { relativeTo: this.route });
            });
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.absencesByClassesReport) {
      throw new Error('onRemove requires a report to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      absencesByClassesReportId: this.data.absencesByClassesReport.absencesByClassesReportId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете справката?',
        errorsMessage: 'Не може да изтриете справката, защото:',
        httpAction: () => this.absencesByClassesReportsService.remove(removeParams).toPromise()
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
