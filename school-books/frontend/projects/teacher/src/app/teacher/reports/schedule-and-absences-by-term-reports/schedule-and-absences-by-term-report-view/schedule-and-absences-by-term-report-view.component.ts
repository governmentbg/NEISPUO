import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faCalendarMinus as fadCalendarMinus } from '@fortawesome/pro-duotone-svg-icons/faCalendarMinus';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { ClassBookNomsService } from 'projects/sb-api-client/src/api/classBookNoms.service';
import {
  ScheduleAndAbsencesByTermReportsService,
  ScheduleAndAbsencesByTermReports_Get,
  ScheduleAndAbsencesByTermReports_GetWeeks
} from 'projects/sb-api-client/src/api/scheduleAndAbsencesByTermReports.service';
import { SchoolTermNomsService } from 'projects/sb-api-client/src/api/schoolTermNoms.service';
import { SchoolTerm } from 'projects/sb-api-client/src/model/schoolTerm';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { formatDateTime } from 'projects/shared/utils/date';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class ScheduleAndAbsencesByTermReportViewSkeletonComponent extends SkeletonComponentBase {
  constructor(scheduleAndAbsencesByTermReportsService: ScheduleAndAbsencesByTermReportsService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const scheduleAndAbsencesByTermReportId = tryParseInt(
      route.snapshot.paramMap.get('scheduleAndAbsencesByTermReportId')
    );
    if (scheduleAndAbsencesByTermReportId) {
      this.resolve(ScheduleAndAbsencesByTermReportViewComponent, {
        schoolYear,
        instId,
        scheduleAndAbsencesByTermReport: scheduleAndAbsencesByTermReportsService.get({
          schoolYear,
          instId,
          scheduleAndAbsencesByTermReportId
        }),
        scheduleAndAbsencesByTermReportWeeks: scheduleAndAbsencesByTermReportsService.getWeeks({
          schoolYear,
          instId,
          scheduleAndAbsencesByTermReportId
        })
      });
    } else {
      this.resolve(ScheduleAndAbsencesByTermReportViewComponent, {
        schoolYear,
        instId,
        scheduleAndAbsencesByTermReport: null,
        scheduleAndAbsencesByTermReportWeeks: null
      });
    }
  }
}

@Component({
  selector: 'sb-schedule-and-absences-by-term-report-view',
  templateUrl: './schedule-and-absences-by-term-report-view.component.html',
  styleUrls: ['../../reports.scss']
})
export class ScheduleAndAbsencesByTermReportViewComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    scheduleAndAbsencesByTermReport: ScheduleAndAbsencesByTermReports_Get | null;
    scheduleAndAbsencesByTermReportWeeks: ScheduleAndAbsencesByTermReports_GetWeeks | null;
  };

  readonly fadCalendarMinus = fadCalendarMinus;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasFileExcel = fasFileExcel;

  readonly form = this.fb.nonNullable.group({
    term: this.fb.nonNullable.control<SchoolTerm | null | undefined>(null, Validators.required),
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

  constructor(
    private fb: FormBuilder,
    private scheduleAndAbsencesByTermReportsService: ScheduleAndAbsencesByTermReportsService,
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
      showPG: true,
      showDplr: true,
      showCsop: false
    }));
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  ngOnInit() {
    if (this.data.scheduleAndAbsencesByTermReport != null) {
      const { isDPLR, term, classBookName, createDate } = this.data.scheduleAndAbsencesByTermReport;
      const periodText = term === SchoolTerm.TermOne ? 'първи срок' : term === SchoolTerm.TermTwo ? 'втори срок' : null;
      this.reportName = `Отсъствия/теми за ${periodText} към дата ${formatDateTime(createDate)}`;
      this.isDPLR = isDPLR;

      this.form.setValue({
        term,
        classBookId: -1,
        classBookName
      });

      this.scheduleAndAbsencesByTermReportsService
        .downloadExcelFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          scheduleAndAbsencesByTermReportId: this.data.scheduleAndAbsencesByTermReport.scheduleAndAbsencesByTermReportId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url));
    }
  }

  onSave(save: SaveToken) {
    const { term, classBookId } = this.form.getRawValue();
    const scheduleAndAbsencesByTermReport = {
      term: term ?? throwError("'term' should not be null"),
      classBookId: classBookId ?? throwError("'classBookId' should not be null")
    };

    this.actionService
      .execute({
        httpAction: () => {
          return this.scheduleAndAbsencesByTermReportsService
            .create({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              createScheduleAndAbsencesByTermReportCommand: scheduleAndAbsencesByTermReport
            })
            .toPromise()
            .then((newScheduleAndAbsencesByTermReportId) => {
              this.router.navigate(['../', newScheduleAndAbsencesByTermReportId], { relativeTo: this.route });
            });
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.scheduleAndAbsencesByTermReport) {
      throw new Error('onRemove requires a report to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      scheduleAndAbsencesByTermReportId: this.data.scheduleAndAbsencesByTermReport.scheduleAndAbsencesByTermReportId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете справката?',
        errorsMessage: 'Не може да изтриете справката, защото:',
        httpAction: () => this.scheduleAndAbsencesByTermReportsService.remove(removeParams).toPromise()
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
