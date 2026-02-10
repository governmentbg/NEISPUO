import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faCalendarMinus as fadCalendarMinus } from '@fortawesome/pro-duotone-svg-icons/faCalendarMinus';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { ClassBookNomsService } from 'projects/sb-api-client/src/api/classBookNoms.service';
import {
  FinalGradePointAverageByClassesReportsService,
  FinalGradePointAverageByClassesReports_Get
} from 'projects/sb-api-client/src/api/finalGradePointAverageByClassesReports.service';
import { ReportPeriodNomsService } from 'projects/sb-api-client/src/api/reportPeriodNoms.service';
import { FinalGradePointAverageByClassesReportsGetItemsVO } from 'projects/sb-api-client/src/model/finalGradePointAverageByClassesReportsGetItemsVO';
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
export class FinalGradePointAverageByClassesReportViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    finalGradePointAverageByClassesReportsService: FinalGradePointAverageByClassesReportsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const finalGradePointAverageByClassesReportId = tryParseInt(
      route.snapshot.paramMap.get('finalGradePointAverageByClassesReportId')
    );
    if (finalGradePointAverageByClassesReportId) {
      this.resolve(FinalGradePointAverageByClassesReportViewComponent, {
        schoolYear,
        instId,
        finalGradePointAverageByClassesReport: finalGradePointAverageByClassesReportsService.get({
          schoolYear,
          instId,
          finalGradePointAverageByClassesReportId
        })
      });
    } else {
      this.resolve(FinalGradePointAverageByClassesReportViewComponent, {
        schoolYear,
        instId,
        finalGradePointAverageByClassesReport: null
      });
    }
  }
}

@Component({
  selector: 'sb-final-grade-point-average-by-classes-report-view',
  templateUrl: './final-grade-point-average-by-classes-report-view.component.html'
})
export class FinalGradePointAverageByClassesReportViewComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    finalGradePointAverageByClassesReport: FinalGradePointAverageByClassesReports_Get | null;
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
  dataSource?: TableDataSource<TableResult<FinalGradePointAverageByClassesReportsGetItemsVO>>;

  downloadUrl?: string;
  reportName?: string;

  reportPeriodNomsService: INomService<ReportPeriod, { instId: number; schoolYear: number }>;
  classBookNomsService: INomService<number, { instId: number; schoolYear: number }>;

  constructor(
    private fb: FormBuilder,
    private finalGradePointAverageByClassesReportsService: FinalGradePointAverageByClassesReportsService,
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
      showDplr: false
    }));
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  ngOnInit() {
    if (this.data.finalGradePointAverageByClassesReport != null) {
      const finalGradePointAverageByClassesReportId =
        this.data.finalGradePointAverageByClassesReport.finalGradePointAverageByClassesReportId;
      this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
        this.finalGradePointAverageByClassesReportsService.getItems({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          finalGradePointAverageByClassesReportId: finalGradePointAverageByClassesReportId,
          offset,
          limit
        })
      );

      const { period, classBookNames, createDate } = this.data.finalGradePointAverageByClassesReport;
      const periodText =
        period === ReportPeriod.TermOne
          ? 'първи срок'
          : period === ReportPeriod.TermTwo
          ? 'втори срок'
          : 'цялата година';
      if (classBookNames) {
        this.reportName = `Среден успех от срочни/годишни оценки по класове от ${classBookNames} за ${periodText} към дата ${formatDateTime(
          createDate
        )}`;
      } else {
        this.reportName = `Среден успех от срочни/годишни оценки по класове за ${periodText} към дата ${formatDateTime(
          createDate
        )}`;
      }

      this.form.setValue({
        period,
        classBookIds: [],
        classBookNames
      });

      this.finalGradePointAverageByClassesReportsService
        .downloadExcelFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          finalGradePointAverageByClassesReportId:
            this.data.finalGradePointAverageByClassesReport.finalGradePointAverageByClassesReportId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url));
    }
  }

  onSave(save: SaveToken) {
    const { period, classBookIds } = this.form.getRawValue();
    const finalGradePointAverageByClassesReport = {
      period: period ?? throwError("'period' should not be null"),
      classBookIds: classBookIds
    };

    this.actionService
      .execute({
        httpAction: () => {
          return this.finalGradePointAverageByClassesReportsService
            .create({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              createFinalGradePointAverageByClassesReportCommand: finalGradePointAverageByClassesReport
            })
            .toPromise()
            .then((newFinalGradePointAverageByClassesReportId) => {
              this.router.navigate(['../', newFinalGradePointAverageByClassesReportId], { relativeTo: this.route });
            });
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.finalGradePointAverageByClassesReport) {
      throw new Error('onRemove requires a report to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      finalGradePointAverageByClassesReportId:
        this.data.finalGradePointAverageByClassesReport.finalGradePointAverageByClassesReportId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете справката?',
        errorsMessage: 'Не може да изтриете справката, защото:',
        httpAction: () => this.finalGradePointAverageByClassesReportsService.remove(removeParams).toPromise()
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
