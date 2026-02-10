import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faCalendarMinus as fadCalendarMinus } from '@fortawesome/pro-duotone-svg-icons/faCalendarMinus';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import {
  StudentsAtRiskOfDroppingOutReportsService,
  StudentsAtRiskOfDroppingOutReports_Get
} from 'projects/sb-api-client/src/api/studentsAtRiskOfDroppingOutReports.service';
import { StudentsAtRiskOfDroppingOutReportsGetItemsVO } from 'projects/sb-api-client/src/model/studentsAtRiskOfDroppingOutReportsGetItemsVO';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TableDataSource, TableResult } from 'projects/shared/components/table/table-datasource';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Subject } from 'rxjs';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class StudentsAtRiskOfDroppingOutReportViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    studentsAtRiskOfDroppingOutReportsService: StudentsAtRiskOfDroppingOutReportsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const studentsAtRiskOfDroppingOutReportId = tryParseInt(
      route.snapshot.paramMap.get('studentsAtRiskOfDroppingOutReportId')
    );
    if (studentsAtRiskOfDroppingOutReportId) {
      this.resolve(StudentsAtRiskOfDroppingOutReportViewComponent, {
        schoolYear,
        instId,
        studentsAtRiskOfDroppingOutReport: studentsAtRiskOfDroppingOutReportsService.get({
          schoolYear,
          instId,
          studentsAtRiskOfDroppingOutReportId
        }),
        institutionInfo: from(institutionInfo)
      });
    } else {
      this.resolve(StudentsAtRiskOfDroppingOutReportViewComponent, {
        schoolYear,
        instId,
        studentsAtRiskOfDroppingOutReport: null,
        institutionInfo: from(institutionInfo)
      });
    }
  }
}

@Component({
  selector: 'sb-students-at-risk-of-dropping-out-report-view',
  templateUrl: './students-at-risk-of-dropping-out-report-view.component.html'
})
export class StudentsAtRiskOfDroppingOutReportViewComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    studentsAtRiskOfDroppingOutReport: StudentsAtRiskOfDroppingOutReports_Get | null;
    institutionInfo: InstitutionInfoType;
  };

  readonly fadCalendarMinus = fadCalendarMinus;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasFileExcel = fasFileExcel;

  readonly form = this.fb.nonNullable.group({
    reportDate: this.fb.nonNullable.control<Date>(new Date(), Validators.required)
  });

  private readonly destroyed$ = new Subject<void>();
  removing = false;
  dataSource?: TableDataSource<TableResult<StudentsAtRiskOfDroppingOutReportsGetItemsVO>>;

  downloadUrl?: string;

  constructor(
    private fb: FormBuilder,
    private studentsAtRiskOfDroppingOutReportsService: StudentsAtRiskOfDroppingOutReportsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService
  ) {}

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  ngOnInit() {
    if (this.data.studentsAtRiskOfDroppingOutReport) {
      const studentsAtRiskOfDroppingOutReportId =
        this.data.studentsAtRiskOfDroppingOutReport.studentsAtRiskOfDroppingOutReportId;
      this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
        this.studentsAtRiskOfDroppingOutReportsService.getItems({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          studentsAtRiskOfDroppingOutReportId: studentsAtRiskOfDroppingOutReportId,
          offset,
          limit
        })
      );

      const { reportDate } = this.data.studentsAtRiskOfDroppingOutReport;
      this.form.get('reportDate')?.setValue(reportDate);
    }

    if (this.data.studentsAtRiskOfDroppingOutReport != null) {
      this.studentsAtRiskOfDroppingOutReportsService
        .downloadExcelFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          studentsAtRiskOfDroppingOutReportId:
            this.data.studentsAtRiskOfDroppingOutReport.studentsAtRiskOfDroppingOutReportId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url));
    }
  }

  onSave(save: SaveToken) {
    const { reportDate } = this.form.getRawValue();
    const studentsAtRiskOfDroppingOutReport = {
      reportDate: reportDate ?? throwError("'reportDate' should not be null")
    };

    this.actionService
      .execute({
        httpAction: () => {
          return this.studentsAtRiskOfDroppingOutReportsService
            .create({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              createStudentsAtRiskOfDroppingOutReportCommand: studentsAtRiskOfDroppingOutReport
            })
            .toPromise()
            .then((newStudentsAtRiskOfDroppingOutReportId) => {
              this.router.navigate(['../', newStudentsAtRiskOfDroppingOutReportId], { relativeTo: this.route });
            });
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.studentsAtRiskOfDroppingOutReport) {
      throw new Error('onRemove requires a report to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      studentsAtRiskOfDroppingOutReportId:
        this.data.studentsAtRiskOfDroppingOutReport.studentsAtRiskOfDroppingOutReportId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете справката?',
        errorsMessage: 'Не може да изтриете справката, защото:',
        httpAction: () => this.studentsAtRiskOfDroppingOutReportsService.remove(removeParams).toPromise()
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
