import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faCalendarWeek as fadCalendarWeek } from '@fortawesome/pro-duotone-svg-icons/faCalendarWeek';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { eachMonthOfInterval, endOfMonth, min } from 'date-fns';
import { InstTeacherNomsService } from 'projects/sb-api-client/src/api/instTeacherNoms.service';
import { MissingTopicsReportPeriodNomsService } from 'projects/sb-api-client/src/api/missingTopicsReportPeriodNoms.service';
import {
  MissingTopicsReportsService,
  MissingTopicsReports_Get,
  MissingTopicsReports_GetItems
} from 'projects/sb-api-client/src/api/missingTopicsReports.service';
import { MissingTopicsReportPeriod } from 'projects/sb-api-client/src/model/missingTopicsReportPeriod';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { AuthService } from 'projects/shared/services/auth.service';
import { formatMonth, parseMonth } from 'projects/shared/utils/date';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';

const SCHOOL_YEAR_START_MONTH = 9; // September
const SCHOOL_YEAR_END_MONTH = 7; // July

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class MissingTopicsReportViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    missingTopicsReportsService: MissingTopicsReportsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const missingTopicsReportId = tryParseInt(route.snapshot.paramMap.get('missingTopicsReportId'));
    if (missingTopicsReportId) {
      this.resolve(MissingTopicsReportViewComponent, {
        schoolYear,
        instId,
        missingTopicsReport: missingTopicsReportsService.get({
          schoolYear,
          instId,
          missingTopicsReportId
        }),
        institutionInfo: from(institutionInfo)
      });
    } else {
      this.resolve(MissingTopicsReportViewComponent, {
        schoolYear,
        instId,
        missingTopicsReport: null,
        institutionInfo: from(institutionInfo)
      });
    }
  }
}

@Component({
  selector: 'sb-missing-topics-report-view',
  templateUrl: './missing-topics-report-view.component.html'
})
export class MissingTopicsReportViewComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    missingTopicsReport: MissingTopicsReports_Get | null;
    institutionInfo: InstitutionInfoType;
  };

  readonly fadCalendarWeek = fadCalendarWeek;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasFileExcel = fasFileExcel;

  readonly form = this.fb.nonNullable.group({
    period: this.fb.nonNullable.control<MissingTopicsReportPeriod | null | undefined>(null, Validators.required),
    selectedMonth: this.fb.nonNullable.control<string | null | undefined>(null, Validators.required),
    teacherPersonId: this.fb.nonNullable.control<number | null | undefined>(null)
  });

  missingTopicsReportPeriodNomsService: INomService<MissingTopicsReportPeriod, { instId: number; schoolYear: number }>;
  downloadUrl?: string;

  private readonly destroyed$ = new Subject<void>();
  removing = false;
  dataSource?: TableDataSource<MissingTopicsReports_GetItems>;
  instTeacherNomsService: INomService<number, { instId: number; schoolYear: number }>;
  monthsSelect: { id: string; name: string }[] = [];

  constructor(
    private fb: FormBuilder,
    private missingTopicsReportsService: MissingTopicsReportsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private authService: AuthService,
    missingTopicsReportPeriodNomsService: MissingTopicsReportPeriodNomsService,
    instTeacherNomsService: InstTeacherNomsService
  ) {
    this.instTeacherNomsService = new NomServiceWithParams(instTeacherNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear,
      includeNotActiveTeachers: true
    }));

    this.missingTopicsReportPeriodNomsService = new NomServiceWithParams(missingTopicsReportPeriodNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  ngOnInit() {
    if (this.data.missingTopicsReport != null) {
      const missingTopicsReportId = this.data.missingTopicsReport.missingTopicsReportId;
      this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
        this.missingTopicsReportsService.getItems({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          missingTopicsReportId: missingTopicsReportId,
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

    if (this.data.missingTopicsReport != null) {
      this.missingTopicsReportsService
        .downloadExcelFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          missingTopicsReportId: this.data.missingTopicsReport.missingTopicsReportId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url));
    }

    if (this.data.missingTopicsReport != null) {
      const { period, year, month, teacherPersonId } = this.data.missingTopicsReport;

      Promise.resolve().then(() => {
        this.form.setValue({
          period,
          selectedMonth: year != null && month != null ? formatMonth(new Date(year, month - 1, 1)) : null,
          teacherPersonId
        });
      });
    } else {
      this.form.get('selectedMonth')?.disable();
      this.form
        .get('period')
        ?.valueChanges.pipe(
          tap((value) => {
            if (value === MissingTopicsReportPeriod.Month) {
              this.form.get('selectedMonth')?.enable();
            } else {
              this.form.get('selectedMonth')?.setValue(null);
              this.form.get('selectedMonth')?.disable();
            }
          }),
          takeUntil(this.destroyed$)
        )
        .subscribe();

      const personId = this.authService.userPersonId;
      if (!this.data.institutionInfo.hasMissingTopicsReportAdminCreateAccess && personId != null) {
        this.form.get('teacherPersonId')?.setValue(personId);
        this.form.get('teacherPersonId')?.disable();
      }
    }
  }

  onSave(save: SaveToken) {
    const { period, selectedMonth, teacherPersonId } = this.form.getRawValue();
    const month =
      period === MissingTopicsReportPeriod.Month
        ? parseMonth(selectedMonth ?? throwError("'selectedMonth' should not be null"))
        : null;
    const missingTopicsReport = {
      period: period ?? throwError("'period' should not be null"),
      year: month?.getFullYear(),
      month: month ? month.getMonth() + 1 : null,
      teacherPersonId
    };

    this.actionService
      .execute({
        httpAction: () => {
          return this.missingTopicsReportsService
            .create({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              createMissingTopicsReportCommand: missingTopicsReport
            })
            .toPromise()
            .then((newMissingTopicsReportId) => {
              this.router.navigate(['../', newMissingTopicsReportId], { relativeTo: this.route });
            });
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.missingTopicsReport) {
      throw new Error('onRemove requires a report to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      missingTopicsReportId: this.data.missingTopicsReport.missingTopicsReportId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете справката?',
        errorsMessage: 'Не може да изтриете справката, защото:',
        httpAction: () => this.missingTopicsReportsService.remove(removeParams).toPromise()
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
