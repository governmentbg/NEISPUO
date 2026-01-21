import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faCalendarMinus as fadCalendarMinus } from '@fortawesome/pro-duotone-svg-icons/faCalendarMinus';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import {
  SessionStudentsReportsService,
  SessionStudentsReports_Get
} from 'projects/sb-api-client/src/api/sessionStudentsReports.service';
import { SessionStudentsReportsGetItemsVO } from 'projects/sb-api-client/src/model/sessionStudentsReportsGetItemsVO';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TableDataSource, TableResult } from 'projects/shared/components/table/table-datasource';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Subject } from 'rxjs';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class SessionStudentsReportViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    sessionStudentsReportsService: SessionStudentsReportsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const sessionStudentsReportId = tryParseInt(route.snapshot.paramMap.get('sessionStudentsReportId'));
    if (sessionStudentsReportId) {
      this.resolve(SessionStudentsReportViewComponent, {
        schoolYear,
        instId,
        sessionStudentsReport: sessionStudentsReportsService.get({
          schoolYear,
          instId,
          sessionStudentsReportId
        }),
        institutionInfo: from(institutionInfo)
      });
    }
  }
}

@Component({
  selector: 'sb-session-students-report-view',
  templateUrl: './session-students-report-view.component.html'
})
export class SessionStudentsReportViewComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    sessionStudentsReport: SessionStudentsReports_Get | null;
    institutionInfo: InstitutionInfoType;
  };

  readonly fadCalendarMinus = fadCalendarMinus;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasFileExcel = fasFileExcel;

  readonly form = this.fb.nonNullable.group({});

  private readonly destroyed$ = new Subject<void>();
  removing = false;
  dataSource?: TableDataSource<TableResult<SessionStudentsReportsGetItemsVO>>;

  downloadUrl?: string;

  constructor(
    private fb: FormBuilder,
    private sessionStudentsReportsService: SessionStudentsReportsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService
  ) {}

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  ngOnInit() {
    if (this.data.sessionStudentsReport) {
      const sessionStudentsReportId = this.data.sessionStudentsReport.sessionStudentsReportId;
      this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
        this.sessionStudentsReportsService.getItems({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          sessionStudentsReportId: sessionStudentsReportId,
          offset,
          limit
        })
      );
    }

    if (this.data.sessionStudentsReport != null) {
      this.sessionStudentsReportsService
        .downloadExcelFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          sessionStudentsReportId: this.data.sessionStudentsReport.sessionStudentsReportId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url));
    }
  }

  onRemove() {
    if (!this.data.sessionStudentsReport) {
      throw new Error('onRemove requires a report to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      sessionStudentsReportId: this.data.sessionStudentsReport.sessionStudentsReportId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете справката?',
        errorsMessage: 'Не може да изтриете справката, защото:',
        httpAction: () => this.sessionStudentsReportsService.remove(removeParams).toPromise()
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
