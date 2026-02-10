import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faCalendarMinus as fadCalendarMinus } from '@fortawesome/pro-duotone-svg-icons/faCalendarMinus';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { ClassBookNomsService } from 'projects/sb-api-client/src/api/classBookNoms.service';
import {
  FinalGradePointAverageByStudentsReportsService,
  FinalGradePointAverageByStudentsReports_Get
} from 'projects/sb-api-client/src/api/finalGradePointAverageByStudentsReports.service';
import { ReportPeriodNomsService } from 'projects/sb-api-client/src/api/reportPeriodNoms.service';
import { FinalGradePointAverageByStudentsReportsGetItemsVO } from 'projects/sb-api-client/src/model/finalGradePointAverageByStudentsReportsGetItemsVO';
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
export class FinalGradePointAverageByStudentsReportViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    finalGradePointAverageByStudentsReportsService: FinalGradePointAverageByStudentsReportsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const finalGradePointAverageByStudentsReportId = tryParseInt(
      route.snapshot.paramMap.get('finalGradePointAverageByStudentsReportId')
    );
    if (finalGradePointAverageByStudentsReportId) {
      this.resolve(FinalGradePointAverageByStudentsReportViewComponent, {
        schoolYear,
        instId,
        finalGradePointAverageByStudentsReport: finalGradePointAverageByStudentsReportsService.get({
          schoolYear,
          instId,
          finalGradePointAverageByStudentsReportId
        })
      });
    } else {
      this.resolve(FinalGradePointAverageByStudentsReportViewComponent, {
        schoolYear,
        instId,
        finalGradePointAverageByStudentsReport: null
      });
    }
  }
}

@Component({
  selector: 'sb-final-grade-point-average-by-students-report-view',
  templateUrl: './final-grade-point-average-by-students-report-view.component.html'
})
export class FinalGradePointAverageByStudentsReportViewComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    finalGradePointAverageByStudentsReport: FinalGradePointAverageByStudentsReports_Get | null;
  };

  readonly fadCalendarMinus = fadCalendarMinus;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasFileExcel = fasFileExcel;

  readonly form = this.fb.nonNullable.group({
    period: this.fb.nonNullable.control<ReportPeriod | null | undefined>(null, Validators.required),
    classBookIds: this.fb.nonNullable.control<number[]>([]),
    classBookNames: this.fb.nonNullable.control<string | null | undefined>(null),
    minimumGradePointAverage: this.fb.nonNullable.control<number | null | undefined>(5.5, [
      Validators.min(2),
      Validators.max(6)
    ])
  });

  private readonly destroyed$ = new Subject<void>();
  removing = false;
  dataSource?: TableDataSource<TableResult<FinalGradePointAverageByStudentsReportsGetItemsVO>>;

  downloadUrl?: string;
  reportName?: string;

  reportPeriodNomsService: INomService<ReportPeriod, { instId: number; schoolYear: number }>;
  classBookNomsService: INomService<number, { instId: number; schoolYear: number }>;

  constructor(
    private fb: FormBuilder,
    private finalGradePointAverageByStudentsReportsService: FinalGradePointAverageByStudentsReportsService,
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
      showCdo: false,
      showDplr: false,
      showCsop: false
    }));
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  ngOnInit() {
    if (this.data.finalGradePointAverageByStudentsReport != null) {
      const finalGradePointAverageByStudentsReportId =
        this.data.finalGradePointAverageByStudentsReport.finalGradePointAverageByStudentsReportId;
      this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
        this.finalGradePointAverageByStudentsReportsService.getItems({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          finalGradePointAverageByStudentsReportId: finalGradePointAverageByStudentsReportId,
          offset,
          limit
        })
      );

      const { period, classBookNames, minimumGradePointAverage, createDate } =
        this.data.finalGradePointAverageByStudentsReport;
      const periodText =
        period === ReportPeriod.TermOne
          ? 'първи срок'
          : period === ReportPeriod.TermTwo
          ? 'втори срок'
          : 'цялата година';
      const periodType = period === ReportPeriod.WholeYear ? 'годишни' : 'срочни';
      const minimalGradeValue =
        minimumGradePointAverage != null ? `с успех по-голям или равен на ${minimumGradePointAverage.toFixed(2)}` : '';
      if (classBookNames) {
        this.reportName = `Среден успех от ${periodType} оценки по ученици ${minimalGradeValue} от ${classBookNames} за ${periodText} към дата ${formatDateTime(
          createDate
        )}`;
      } else {
        this.reportName = `Среден успех от ${periodType} оценки по ученици ${minimalGradeValue} за ${periodText} към дата ${formatDateTime(
          createDate
        )}`;
      }

      this.form.setValue({
        period,
        classBookIds: [],
        classBookNames,
        minimumGradePointAverage
      });

      this.finalGradePointAverageByStudentsReportsService
        .downloadExcelFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          finalGradePointAverageByStudentsReportId:
            this.data.finalGradePointAverageByStudentsReport.finalGradePointAverageByStudentsReportId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url));
    }
  }

  onSave(save: SaveToken) {
    const { period, classBookIds, minimumGradePointAverage } = this.form.getRawValue();
    const finalGradePointAverageByStudentsReport = {
      period: period ?? throwError("'period' should not be null"),
      classBookIds: classBookIds,
      minimumGradePointAverage: minimumGradePointAverage
    };

    this.actionService
      .execute({
        httpAction: () => {
          return this.finalGradePointAverageByStudentsReportsService
            .create({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              createFinalGradePointAverageByStudentsReportCommand: finalGradePointAverageByStudentsReport
            })
            .toPromise()
            .then((newFinalGradePointAverageByStudentsReportId) => {
              this.router.navigate(['../', newFinalGradePointAverageByStudentsReportId], { relativeTo: this.route });
            });
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.finalGradePointAverageByStudentsReport) {
      throw new Error('onRemove requires a report to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      finalGradePointAverageByStudentsReportId:
        this.data.finalGradePointAverageByStudentsReport.finalGradePointAverageByStudentsReportId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете справката?',
        errorsMessage: 'Не може да изтриете справката, защото:',
        httpAction: () => this.finalGradePointAverageByStudentsReportsService.remove(removeParams).toPromise()
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
