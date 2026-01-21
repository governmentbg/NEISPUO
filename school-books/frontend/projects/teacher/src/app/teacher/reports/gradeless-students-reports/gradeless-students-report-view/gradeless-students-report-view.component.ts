import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faCalendarMinus as fadCalendarMinus } from '@fortawesome/pro-duotone-svg-icons/faCalendarMinus';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { ClassBookNomsService } from 'projects/sb-api-client/src/api/classBookNoms.service';
import {
  GradelessStudentsReportsService,
  GradelessStudentsReports_Get
} from 'projects/sb-api-client/src/api/gradelessStudentsReports.service';
import { ReportPeriodNomsService } from 'projects/sb-api-client/src/api/reportPeriodNoms.service';
import { GradelessStudentsReportsGetItemsVO } from 'projects/sb-api-client/src/model/gradelessStudentsReportsGetItemsVO';
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

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE,
  styleUrls: ['gradeless-students-report-view.component.scss']
})
export class GradelessStudentsReportViewSkeletonComponent extends SkeletonComponentBase {
  constructor(gradelessStudentsReportsService: GradelessStudentsReportsService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const gradelessStudentsReportId = tryParseInt(route.snapshot.paramMap.get('gradelessStudentsReportId'));
    if (gradelessStudentsReportId) {
      this.resolve(GradelessStudentsReportViewComponent, {
        schoolYear,
        instId,
        gradelessStudentsReport: gradelessStudentsReportsService.get({
          schoolYear,
          instId,
          gradelessStudentsReportId
        })
      });
    } else {
      this.resolve(GradelessStudentsReportViewComponent, {
        schoolYear,
        instId,
        gradelessStudentsReport: null
      });
    }
  }
}

@Component({
  selector: 'sb-gradeless-students-report-view',
  templateUrl: './gradeless-students-report-view.component.html'
})
export class GradelessStudentsReportViewComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    gradelessStudentsReport: GradelessStudentsReports_Get | null;
  };

  readonly fadCalendarMinus = fadCalendarMinus;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasFileExcel = fasFileExcel;

  readonly form = this.fb.nonNullable.group({
    onlyFinalGrades: this.fb.nonNullable.control<boolean>(false, Validators.required),
    period: this.fb.nonNullable.control<ReportPeriod | null | undefined>(null, Validators.required),
    classBookIds: this.fb.nonNullable.control<number[]>([]),
    classBookNames: this.fb.nonNullable.control<string | null | undefined>(null)
  });

  private readonly destroyed$ = new Subject<void>();
  removing = false;
  dataSource?: TableDataSource<TableResult<GradelessStudentsReportsGetItemsVO>>;

  downloadUrl?: string;
  reoportName?: string;

  reportPeriodNomsService: INomService<ReportPeriod, { instId: number; schoolYear: number }>;
  classBookNomsService: INomService<number, { instId: number; schoolYear: number }>;

  constructor(
    private fb: FormBuilder,
    private gradelessStudentsReportsService: GradelessStudentsReportsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    reportPeriodNomsService: ReportPeriodNomsService,
    classBookNomsService: ClassBookNomsService
  ) {
    this.classBookNomsService = new NomServiceWithParams(classBookNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      showPG: false,
      showCdo: false,
      showDplr: false
    }));
    this.reportPeriodNomsService = new NomServiceWithParams(reportPeriodNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  ngOnInit() {
    if (this.data.gradelessStudentsReport != null) {
      const gradelessStudentsReportId = this.data.gradelessStudentsReport.gradelessStudentsReportId;
      this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
        this.gradelessStudentsReportsService.getItems({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          gradelessStudentsReportId: gradelessStudentsReportId,
          offset,
          limit
        })
      );

      const { onlyFinalGrades, period, classBookNames, createDate } = this.data.gradelessStudentsReport;

      const gradeTypesText = onlyFinalGrades ? `${period !== ReportPeriod.WholeYear ? 'срочни' : 'годишни'}` : 'текущи';
      const periodTypeText =
        period === ReportPeriod.TermOne
          ? 'първи срок'
          : period === ReportPeriod.TermTwo
          ? 'втори срок'
          : 'цялата година';
      const periodText = onlyFinalGrades
        ? `${period !== ReportPeriod.WholeYear ? 'за ' + periodTypeText + ' ' : ''}`
        : 'за ' + periodTypeText + ' ';

      if (classBookNames) {
        this.reoportName = `Ученици без ${gradeTypesText} оценки от ${classBookNames} ${periodText}към дата ${formatDateTime(
          createDate
        )}`;
      } else {
        this.reoportName = `Ученици без ${gradeTypesText} оценки ${periodText}към дата ${formatDateTime(createDate)}`;
      }
      this.form.setValue({
        onlyFinalGrades,
        period,
        classBookIds: [],
        classBookNames
      });

      this.gradelessStudentsReportsService
        .downloadExcelFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          gradelessStudentsReportId: this.data.gradelessStudentsReport.gradelessStudentsReportId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url));
    }
  }

  onOnlyFinalGradesChanged() {
    this.form.get('period')?.setValue(null);
  }

  onSave(save: SaveToken) {
    const { onlyFinalGrades, period, classBookIds } = this.form.getRawValue();
    const gradelessStudentsReport = {
      onlyFinalGrades: onlyFinalGrades ?? throwError("'onlyFinalGrades' should not be null"),
      period: period ?? throwError("'period' should not be null"),
      classBookIds: classBookIds
    };

    this.actionService
      .execute({
        httpAction: () => {
          return this.gradelessStudentsReportsService
            .create({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              createGradelessStudentsReportCommand: gradelessStudentsReport
            })
            .toPromise()
            .then((newGradelessStudentsReportId) => {
              this.router.navigate(['../', newGradelessStudentsReportId], { relativeTo: this.route });
            });
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.gradelessStudentsReport) {
      throw new Error('onRemove requires a report to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      gradelessStudentsReportId: this.data.gradelessStudentsReport.gradelessStudentsReportId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете справката?',
        errorsMessage: 'Не може да изтриете справката, защото:',
        httpAction: () => this.gradelessStudentsReportsService.remove(removeParams).toPromise()
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
