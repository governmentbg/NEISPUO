import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faCalendarMinus as fadCalendarMinus } from '@fortawesome/pro-duotone-svg-icons/faCalendarMinus';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { eachMonthOfInterval, endOfMonth, min } from 'date-fns';
import { ClassBookNomsService } from 'projects/sb-api-client/src/api/classBookNoms.service';
import {
  ScheduleAndAbsencesByMonthReportsService,
  ScheduleAndAbsencesByMonthReports_Get,
  ScheduleAndAbsencesByMonthReports_GetWeeks
} from 'projects/sb-api-client/src/api/scheduleAndAbsencesByMonthReports.service';
import { SchoolTermNomsService } from 'projects/sb-api-client/src/api/schoolTermNoms.service';
import { SchoolTerm } from 'projects/sb-api-client/src/model/schoolTerm';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { formatDateTime, formatMonth, parseMonth } from 'projects/shared/utils/date';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';

const SCHOOL_YEAR_START_MONTH = 9; // September
const SCHOOL_YEAR_END_MONTH = 7; // July

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class ScheduleAndAbsencesByMonthReportViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    scheduleAndAbsencesByMonthReportsService: ScheduleAndAbsencesByMonthReportsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const scheduleAndAbsencesByMonthReportId = tryParseInt(
      route.snapshot.paramMap.get('scheduleAndAbsencesByMonthReportId')
    );
    if (scheduleAndAbsencesByMonthReportId) {
      this.resolve(ScheduleAndAbsencesByMonthReportViewComponent, {
        schoolYear,
        instId,
        scheduleAndAbsencesByMonthReport: scheduleAndAbsencesByMonthReportsService.get({
          schoolYear,
          instId,
          scheduleAndAbsencesByMonthReportId
        }),
        scheduleAndAbsencesByMonthReportWeeks: scheduleAndAbsencesByMonthReportsService.getWeeks({
          schoolYear,
          instId,
          scheduleAndAbsencesByMonthReportId
        })
      });
    } else {
      this.resolve(ScheduleAndAbsencesByMonthReportViewComponent, {
        schoolYear,
        instId,
        scheduleAndAbsencesByMonthReport: null,
        scheduleAndAbsencesByMonthReportWeeks: null
      });
    }
  }
}

@Component({
  selector: 'sb-schedule-and-absences-by-month-report-view',
  templateUrl: './schedule-and-absences-by-month-report-view.component.html',
  styleUrls: ['../../reports.scss']
})
export class ScheduleAndAbsencesByMonthReportViewComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    scheduleAndAbsencesByMonthReport: ScheduleAndAbsencesByMonthReports_Get | null;
    scheduleAndAbsencesByMonthReportWeeks: ScheduleAndAbsencesByMonthReports_GetWeeks | null;
  };

  readonly fadCalendarMinus = fadCalendarMinus;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasFileExcel = fasFileExcel;

  readonly form = this.fb.nonNullable.group({
    month: this.fb.nonNullable.control<string | null | undefined>(null, Validators.required),
    classBookId: this.fb.nonNullable.control<number | null | undefined>(null, Validators.required),
    classBookName: this.fb.nonNullable.control<string | null | undefined>(null)
  });

  private readonly destroyed$ = new Subject<void>();
  removing = false;

  downloadUrl?: string;
  reportName?: string;
  isDPLR = false;

  schoolTermNomsService: INomService<SchoolTerm, { instId: number; schoolYear: number }>;
  classBookNomsService: INomService<number, { instId: number; schoolYear: number }>;
  monthsSelect: { id: string; name: string }[] = [];

  constructor(
    private fb: FormBuilder,
    private scheduleAndAbsencesByMonthReportsService: ScheduleAndAbsencesByMonthReportsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    schoolTermNomsService: SchoolTermNomsService,
    classBookNomsService: ClassBookNomsService
  ) {
    this.schoolTermNomsService = new NomServiceWithParams(schoolTermNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));
    this.classBookNomsService = new NomServiceWithParams(classBookNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      showPG: false,
      showDplr: true,
      showCsop: false
    }));
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  ngOnInit() {
    if (this.data.scheduleAndAbsencesByMonthReport != null) {
      const { isDPLR, year, month, classBookName, createDate } = this.data.scheduleAndAbsencesByMonthReport;
      const monthString = formatMonth(new Date(year, month - 1, 1));

      this.reportName = `Отсъствия/теми за ${monthString} към дата ${formatDateTime(createDate)}`;
      this.isDPLR = isDPLR;

      this.form.setValue({
        month: monthString,
        classBookId: -1,
        classBookName
      });

      this.scheduleAndAbsencesByMonthReportsService
        .downloadExcelFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          scheduleAndAbsencesByMonthReportId:
            this.data.scheduleAndAbsencesByMonthReport.scheduleAndAbsencesByMonthReportId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url));
    }

    this.monthsSelect = eachMonthOfInterval({
      start: new Date(this.data.schoolYear, SCHOOL_YEAR_START_MONTH - 1, 1),
      end: min([endOfMonth(new Date(this.data.schoolYear + 1, SCHOOL_YEAR_END_MONTH - 1, 1)), new Date()])
    })
      .map((d) => formatMonth(d))
      .map((m) => ({
        id: m,
        name: m
      }));
  }

  onSave(save: SaveToken) {
    const { month: m, classBookId } = this.form.getRawValue();
    const month = parseMonth(m ?? throwError("'month' should not be null"));

    const scheduleAndAbsencesByMonthReport = {
      year: month?.getFullYear() ?? throwError("'year' should not be null"),
      month: month ? month.getMonth() + 1 : throwError("'month' should not be null"),
      classBookId: classBookId ?? throwError("'classBookId' should not be null")
    };

    this.actionService
      .execute({
        httpAction: () => {
          return this.scheduleAndAbsencesByMonthReportsService
            .create({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              createScheduleAndAbsencesByMonthReportCommand: scheduleAndAbsencesByMonthReport
            })
            .toPromise()
            .then((newScheduleAndAbsencesByMonthReportId) => {
              this.router.navigate(['../', newScheduleAndAbsencesByMonthReportId], { relativeTo: this.route });
            });
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.scheduleAndAbsencesByMonthReport) {
      throw new Error('onRemove requires a report to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      scheduleAndAbsencesByMonthReportId: this.data.scheduleAndAbsencesByMonthReport.scheduleAndAbsencesByMonthReportId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете справката?',
        errorsMessage: 'Не може да изтриете справката, защото:',
        httpAction: () => this.scheduleAndAbsencesByMonthReportsService.remove(removeParams).toPromise()
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
