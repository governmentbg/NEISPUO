import { KeyValue } from '@angular/common';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faCalendarMinus as fadCalendarMinus } from '@fortawesome/pro-duotone-svg-icons/faCalendarMinus';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { ClassBookNomsService } from 'projects/sb-api-client/src/api/classBookNoms.service';
import {
  DateAbsencesReportsService,
  DateAbsencesReports_Get,
  DateAbsencesReports_GetItems
} from 'projects/sb-api-client/src/api/dateAbsencesReports.service';
import { ShiftNomsService } from 'projects/sb-api-client/src/api/shiftNoms.service';
import { DateAbsencesReportsGetItemsVOHour } from 'projects/sb-api-client/src/model/dateAbsencesReportsGetItemsVOHour';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { formatDate, formatDateTime } from 'projects/shared/utils/date';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class DateAbsencesReportViewSkeletonComponent extends SkeletonComponentBase {
  constructor(dateAbsencesReportsService: DateAbsencesReportsService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const dateAbsencesReportId = tryParseInt(route.snapshot.paramMap.get('dateAbsencesReportId'));
    if (dateAbsencesReportId) {
      this.resolve(DateAbsencesReportViewComponent, {
        schoolYear,
        instId,
        dateAbsencesReport: dateAbsencesReportsService.get({
          schoolYear,
          instId,
          dateAbsencesReportId
        }),
        dateAbsencesReportItems: dateAbsencesReportsService.getItems({
          schoolYear,
          instId,
          dateAbsencesReportId
        })
      });
    } else {
      this.resolve(DateAbsencesReportViewComponent, {
        schoolYear,
        instId,
        dateAbsencesReport: null,
        dateAbsencesReportItems: null
      });
    }
  }
}

@Component({
  selector: 'sb-date-absences-report-view',
  templateUrl: './date-absences-report-view.component.html'
})
export class DateAbsencesReportViewComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    dateAbsencesReport: DateAbsencesReports_Get | null;
    dateAbsencesReportItems: DateAbsencesReports_GetItems | null;
  };

  readonly fadCalendarMinus = fadCalendarMinus;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasFileExcel = fasFileExcel;

  readonly form = this.fb.nonNullable.group({
    reportDate: this.fb.nonNullable.control<Date>(new Date(), Validators.required),
    isUnited: this.fb.nonNullable.control<boolean>(false, Validators.required),
    classBookIds: this.fb.nonNullable.control<number[]>([]),
    classBookNames: this.fb.nonNullable.control<string | null | undefined>(null),
    shiftIds: this.fb.nonNullable.control<number[]>([]),
    shiftNames: this.fb.nonNullable.control<string | null | undefined>(null)
  });

  private readonly destroyed$ = new Subject<void>();
  removing = false;

  downloadUrl?: string;
  reportName?: string;

  classBookNomsService: INomService<number, { instId: number; schoolYear: number }>;
  shiftNomsService: INomService<number, { instId: number; schoolYear: number }>;

  items!: ReturnType<typeof mapItems>;

  originalOrder = (
    a: KeyValue<string, { shiftName: string; hoursNumbers: number[] }>,
    b: KeyValue<string, { shiftName: string; hoursNumbers: number[] }>
  ): number => {
    return 0;
  };

  constructor(
    private fb: FormBuilder,
    private dateAbsencesReportsService: DateAbsencesReportsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    classBookNomsService: ClassBookNomsService,
    shiftNomsService: ShiftNomsService
  ) {
    this.classBookNomsService = new NomServiceWithParams(classBookNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      showPG: true,
      showDplr: true
    }));

    this.shiftNomsService = new NomServiceWithParams(shiftNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  ngOnInit() {
    if (this.data.dateAbsencesReport != null) {
      const { reportDate, isUnited, classBookNames, shiftNames, createDate } = this.data.dateAbsencesReport;
      this.reportName = `Отсъстващи за ${formatDate(reportDate)} към дата ${formatDateTime(createDate)}`;

      this.form.setValue({
        reportDate,
        isUnited,
        classBookIds: [],
        classBookNames,
        shiftIds: [],
        shiftNames
      });

      this.dateAbsencesReportsService
        .downloadExcelFile({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          dateAbsencesReportId: this.data.dateAbsencesReport.dateAbsencesReportId
        })
        .toPromise()
        .then((url) => (this.downloadUrl = url));

      this.items = mapItems(this.data);
    }
  }

  onSave(save: SaveToken) {
    const { reportDate, isUnited, classBookIds, shiftIds } = this.form.getRawValue();
    const dateAbsencesReport = {
      reportDate,
      isUnited,
      classBookIds: classBookIds,
      shiftIds: shiftIds
    };

    this.actionService
      .execute({
        httpAction: () => {
          return this.dateAbsencesReportsService
            .create({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              createDateAbsencesReportCommand: dateAbsencesReport
            })
            .toPromise()
            .then((newDateAbsencesReportId) => {
              this.router.navigate(['../', newDateAbsencesReportId], { relativeTo: this.route });
            });
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.dateAbsencesReport) {
      throw new Error('onRemove requires a report to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      dateAbsencesReportId: this.data.dateAbsencesReport.dateAbsencesReportId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете справката?',
        errorsMessage: 'Не може да изтриете справката, защото:',
        httpAction: () => this.dateAbsencesReportsService.remove(removeParams).toPromise()
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

function mapItems(data: DateAbsencesReportViewComponent['data']) {
  const itemsByShifts = data
    .dateAbsencesReportItems!.flatMap((i) => i.hours)
    .reduce((acc, item) => {
      const key = item.shiftId?.toString() ?? '';
      const value = acc.get(key) ?? [];
      value.push(item);
      acc.set(key, value);
      return acc;
    }, new Map<string, DateAbsencesReportsGetItemsVOHour[]>());

  const headerShifts = new Map<string, { shiftName: string; hoursNumbers: number[] }>();

  itemsByShifts.forEach((value, key) => {
    const shiftName = value[0].shiftName ?? '';
    const hoursNumbers = Array.from(new Set(value.map((item) => item.hourNumber)));
    headerShifts.set(key, { shiftName, hoursNumbers });
  });

  const headerHourNumbers = Array.from(headerShifts.values()).flatMap((i) => i.hoursNumbers);

  const absencesAggregatedCount: Record<string, number> = {};
  for (const shift in itemsByShifts) {
    const hours = itemsByShifts.get(shift)!.map((h) => h.hourNumber);
    for (const hour of [...new Set(hours)]) {
      absencesAggregatedCount[`${shift}-${hour}`] = 0;
    }
  }
  for (const item of data.dateAbsencesReportItems!) {
    for (const hour of item.hours) {
      const key = `${hour.shiftId ?? -1}-${hour.hourNumber}`;
      absencesAggregatedCount[key] = (absencesAggregatedCount[key] || 0) + hour.absenceStudentCount;
    }
  }

  return {
    headerShifts,
    headerHourNumbers,
    items: data.dateAbsencesReportItems!,
    totals: Object.entries(absencesAggregatedCount).map(([k, v]) => ({
      count: v
    }))
  };
}
