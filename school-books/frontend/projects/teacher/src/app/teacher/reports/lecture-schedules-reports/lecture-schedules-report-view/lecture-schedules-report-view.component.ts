import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faCalendarStar as fadCalendarStar } from '@fortawesome/pro-duotone-svg-icons/faCalendarStar';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { eachMonthOfInterval, endOfMonth, min } from 'date-fns';
import { InstTeacherNomsService } from 'projects/sb-api-client/src/api/instTeacherNoms.service';
import { LectureSchedulesReportPeriodNomsService } from 'projects/sb-api-client/src/api/lectureSchedulesReportPeriodNoms.service';
import {
  LectureSchedulesReportsService,
  LectureSchedulesReports_Get
} from 'projects/sb-api-client/src/api/lectureSchedulesReports.service';
import { LectureSchedulesReportPeriod } from 'projects/sb-api-client/src/model/lectureSchedulesReportPeriod';
import { LectureSchedulesReportsGetItemsVO } from 'projects/sb-api-client/src/model/lectureSchedulesReportsGetItemsVO';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TableDataSource, TableResult } from 'projects/shared/components/table/table-datasource';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { AuthService } from 'projects/shared/services/auth.service';
import { formatMonth, parseMonth } from 'projects/shared/utils/date';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';
import { LectureSchedulesMode } from 'src/app/teacher/lecture-schedules/lecture-schedules/lecture-schedules.component';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';

const SCHOOL_YEAR_START_MONTH = 9; // September
const SCHOOL_YEAR_END_MONTH = 7; // July

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class LectureSchedulesReportViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    lectureSchedulesReportsService: LectureSchedulesReportsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const lectureSchedulesReportId = tryParseInt(route.snapshot.paramMap.get('lectureSchedulesReportId'));
    if (lectureSchedulesReportId) {
      this.resolve(LectureSchedulesReportViewComponent, {
        schoolYear,
        instId,
        lectureSchedulesReport: lectureSchedulesReportsService.get({
          schoolYear,
          instId,
          lectureSchedulesReportId
        }),
        institutionInfo: from(institutionInfo)
      });
    } else {
      this.resolve(LectureSchedulesReportViewComponent, {
        schoolYear,
        instId,
        lectureSchedulesReport: null,
        institutionInfo: from(institutionInfo)
      });
    }
  }
}

@Component({
  selector: 'sb-lecture-schedules-report-view',
  templateUrl: './lecture-schedules-report-view.component.html'
})
export class LectureSchedulesReportViewComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    lectureSchedulesReport: LectureSchedulesReports_Get | null;
    institutionInfo: InstitutionInfoType;
  };

  readonly fadCalendarStar = fadCalendarStar;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasFileExcel = fasFileExcel;

  readonly form = this.fb.nonNullable.group({
    period: this.fb.nonNullable.control<LectureSchedulesReportPeriod | null | undefined>(null, Validators.required),
    month: this.fb.nonNullable.control<string | null | undefined>(null, Validators.required),
    teacherPersonId: this.fb.nonNullable.control<number | null | undefined>(null),
    teacherPersonName: this.fb.nonNullable.control<string | null | undefined>(null)
  });

  lectureSchedulesReportPeriodNomsService: INomService<
    LectureSchedulesReportPeriod,
    { instId: number; schoolYear: number }
  >;

  private readonly destroyed$ = new Subject<void>();
  removing = false;
  dataSource?: TableDataSource<TableResult<LectureSchedulesReportsGetItemsVO>>;
  instTeacherNomsService: INomService<number, { instId: number; schoolYear: number }>;
  monthsSelect: { id: string; name: string }[] = [];

  lectureSchedulesMode?: LectureSchedulesMode;
  downloadUrl?: string;

  constructor(
    private fb: FormBuilder,
    private lectureSchedulesReportsService: LectureSchedulesReportsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private authService: AuthService,
    lectureSchedulesReportPeriodNomsService: LectureSchedulesReportPeriodNomsService,
    instTeacherNomsService: InstTeacherNomsService
  ) {
    this.instTeacherNomsService = new NomServiceWithParams(instTeacherNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear,
      includeNotActiveTeachers: true
    }));

    this.lectureSchedulesReportPeriodNomsService = new NomServiceWithParams(
      lectureSchedulesReportPeriodNomsService,
      () => ({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId
      })
    );
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  ngOnInit() {
    if (this.data.lectureSchedulesReport) {
      const lectureSchedulesReportId = this.data.lectureSchedulesReport.lectureSchedulesReportId;
      this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
        this.lectureSchedulesReportsService.getItems({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          lectureSchedulesReportId: lectureSchedulesReportId,
          offset,
          limit
        })
      );
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

    if (this.data.lectureSchedulesReport != null) {
      this.lectureSchedulesReportsService
        .downloadExcelFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          lectureSchedulesReportId: this.data.lectureSchedulesReport.lectureSchedulesReportId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url));

      const { period, year, month, teacherPersonName } = this.data.lectureSchedulesReport;

      Promise.resolve().then(() => {
        this.form.setValue({
          period,
          month: year != null && month != null ? formatMonth(new Date(year, month - 1, 1)) : null,
          teacherPersonId: null,
          teacherPersonName
        });
      });

      this.lectureSchedulesMode = this.data.institutionInfo.hasLectureSchedulesReportAdminCreateAccess
        ? LectureSchedulesMode.All
        : LectureSchedulesMode.My;
    } else {
      this.form.get('month')?.disable();
      this.form
        .get('period')
        ?.valueChanges.pipe(
          tap((value) => {
            if (value === LectureSchedulesReportPeriod.Month) {
              this.form.get('month')?.enable();
            } else {
              this.form.get('month')?.setValue(null);
              this.form.get('month')?.disable();
            }
          }),
          takeUntil(this.destroyed$)
        )
        .subscribe();

      const personId = this.authService.userPersonId;
      if (!this.data.institutionInfo.hasLectureSchedulesReportAdminCreateAccess && personId != null) {
        this.form.get('teacherPersonId')?.setValue(personId);
        this.form.get('teacherPersonId')?.disable();
      }
    }
  }

  onSave(save: SaveToken) {
    if (this.data.schoolYear === 2022 && (this.periodIsTermOrWholeYear || this.monthIsFeb23OrEarlier)) {
      save.done(false);
      return;
    }

    const { period, month: m, teacherPersonId } = this.form.getRawValue();
    const month =
      period === LectureSchedulesReportPeriod.Month ? parseMonth(m ?? throwError("'month' should not be null")) : null;
    const lectureSchedulesReport = {
      period: period ?? throwError("'period' should not be null"),
      year: month?.getFullYear(),
      month: month ? month.getMonth() + 1 : null,
      teacherPersonId
    };

    this.actionService
      .execute({
        httpAction: () => {
          return this.lectureSchedulesReportsService
            .create({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              createLectureSchedulesReportCommand: lectureSchedulesReport
            })
            .toPromise()
            .then((newLectureSchedulesReportId) => {
              this.router.navigate(['../', newLectureSchedulesReportId], { relativeTo: this.route });
            });
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.lectureSchedulesReport) {
      throw new Error('onRemove requires a report to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      lectureSchedulesReportId: this.data.lectureSchedulesReport.lectureSchedulesReportId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете справката?',
        errorsMessage: 'Не може да изтриете справката, защото:',
        httpAction: () => this.lectureSchedulesReportsService.remove(removeParams).toPromise()
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

  get periodIsTermOrWholeYear() {
    const period = this.form.get('period')?.value;
    return (
      period === LectureSchedulesReportPeriod.TermOne ||
      period === LectureSchedulesReportPeriod.TermTwo ||
      period === LectureSchedulesReportPeriod.WholeYear
    );
  }

  get monthIsFeb23OrEarlier() {
    const month = this.form.get('month')?.value;
    if (!month) {
      return false;
    }

    return parseMonth(month) < new Date(2023, 2, 1);
  }

  isTotalHoursTakenRow(index: number, item: LectureSchedulesReportsGetItemsVO): boolean {
    return item.totalRow != null;
  }
}
