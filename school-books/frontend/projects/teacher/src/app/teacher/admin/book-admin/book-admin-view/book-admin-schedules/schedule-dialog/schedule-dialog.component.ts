import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormControl,
  UntypedFormArray,
  UntypedFormBuilder,
  UntypedFormControl,
  UntypedFormGroup,
  ValidationErrors,
  Validators
} from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCalendarCheck as farCalendarCheck } from '@fortawesome/pro-regular-svg-icons/faCalendarCheck';
import { faCalendarMinus as farCalendarMinus } from '@fortawesome/pro-regular-svg-icons/faCalendarMinus';
import { faCalendarPlus as farCalendarPlus } from '@fortawesome/pro-regular-svg-icons/faCalendarPlus';
import { faTrashAlt as farTrashAlt } from '@fortawesome/pro-regular-svg-icons/faTrashAlt';
import { faAngleLeft as fasAngleLeft } from '@fortawesome/pro-solid-svg-icons/faAngleLeft';
import { faAngleRight as fasAngleRight } from '@fortawesome/pro-solid-svg-icons/faAngleRight';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrash as fasTrash } from '@fortawesome/pro-solid-svg-icons/faTrash';
import {
  eachDayOfInterval,
  eachWeekOfInterval,
  endOfWeek,
  format,
  getISODay,
  getISOWeek,
  getISOWeekYear,
  max,
  min,
  startOfWeek
} from 'date-fns';
import { bg } from 'date-fns/locale';
import { ClassBookCurriculumNomsService } from 'projects/sb-api-client/src/api/classBookCurriculumNoms.service';
import {
  SchedulesService,
  Schedules_Get,
  Schedules_GetOffDates,
  Schedules_GetSchoolYearSettings,
  Schedules_GetUsedDatesWeeks
} from 'projects/sb-api-client/src/api/schedules.service';
import { ShiftNomsService } from 'projects/sb-api-client/src/api/shiftNoms.service';
import { ShiftsService, Shifts_Get } from 'projects/sb-api-client/src/api/shifts.service';
import { CreateScheduleCommand } from 'projects/sb-api-client/src/model/createScheduleCommand';
import { SchoolTerm } from 'projects/sb-api-client/src/model/schoolTerm';
import {
  INomService,
  NomServiceFromItems,
  NomServiceWithParams
} from 'projects/shared/components/nom-select/nom-service';
import { ShiftFormValue } from 'projects/shared/components/shift-form/shift-form.component';
import {
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { TypedDialog } from 'projects/shared/utils/dialog';
import { dayNames } from 'projects/shared/utils/schedule';
import { Nullable } from 'projects/shared/utils/type';
import { throwError } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';
import { distinctUntilChanged, map, shareReplay, switchMap, takeUntil, tap } from 'rxjs/operators';

export type ScheduleDialogData = {
  schoolYear: number;
  instId: number;
  classBookId: number;
  isIndividualSchedule: boolean;
  personId: number | null;
  scheduleId: number | null;
  isDG: boolean;
};

function defaultIfNullOrEmpty<T>(arr: T[] | null | undefined, defaultArr: T[]) {
  return arr?.length ? arr : defaultArr;
}

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class ScheduleDialogSkeletonComponent
  extends SkeletonComponentBase
  implements TypedDialog<ScheduleDialogData, boolean>
{
  d!: ScheduleDialogData;
  r!: boolean;

  constructor(
    schedulesService: SchedulesService,
    shiftsService: ShiftsService,
    @Inject(MAT_DIALOG_DATA)
    data: ScheduleDialogData
  ) {
    super();

    const { schoolYear, instId, classBookId, isIndividualSchedule, personId, scheduleId, isDG } = data;

    const schedule$ = scheduleId
      ? schedulesService
          .get({
            schoolYear,
            instId,
            classBookId,
            scheduleId
          })
          .pipe(shareReplay(1))
      : null;

    this.resolve(ScheduleDialogComponent, {
      schoolYear,
      instId,
      classBookId,
      isIndividualSchedule: schedule$ ? schedule$.pipe(map((s) => s.isIndividualSchedule)) : isIndividualSchedule,
      personId: schedule$ ? schedule$.pipe(map((s) => s.personId)) : personId,
      scheduleId,
      isDG,
      schoolYearSettings: schedulesService.getSchoolYearSettings({
        schoolYear,
        instId,
        classBookId
      }),
      usedDatesWeeks: schedule$
        ? schedule$.pipe(
            switchMap((s) =>
              schedulesService.getUsedDatesWeeks({
                schoolYear,
                instId,
                classBookId,
                isIndividualSchedule: isIndividualSchedule,
                personId: s.personId,
                exceptScheduleId: scheduleId
              })
            )
          )
        : schedulesService.getUsedDatesWeeks({
            schoolYear,
            instId,
            classBookId,
            isIndividualSchedule: isIndividualSchedule,
            personId: personId,
            exceptScheduleId: null
          }),
      offDates: schedulesService.getOffDates({
        schoolYear,
        instId,
        classBookId
      }),
      schedule: schedule$,
      shift: schedule$
        ? schedule$.pipe(
            switchMap((s) =>
              shiftsService.get({
                schoolYear,
                instId,
                shiftId: s.shiftId
              })
            )
          )
        : null
    });
  }
}

type WeekModel = {
  disabled: boolean;
  readOnly: boolean;
  selected: boolean;
  year: number;
  weekNumber: number;
  startDate: Date;
  startDateDisplay: string;
  endDate: Date;
  endDateDisplay: string;
  monthAbbr: string;
};

type Period = { startDate: Date; endDate: Date };

function isSamePeriod(p1: Period | null, p2: Period | null): boolean {
  return (
    !!p1 && !!p2 && p1.startDate.getTime() === p2.startDate.getTime() && p1.endDate.getTime() === p2.endDate.getTime()
  );
}

class TermNomsService extends NomServiceFromItems<SchoolTerm | 0> {
  constructor() {
    super([
      { id: 0, name: 'Цялата учебна година' },
      { id: SchoolTerm.TermOne, name: 'Първи срок' },
      { id: SchoolTerm.TermTwo, name: 'Втори срок' }
    ]);
  }
}

@Component({
  selector: 'sb-schedule-dialog',
  templateUrl: './schedule-dialog.component.html'
})
export class ScheduleDialogComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    isIndividualSchedule: boolean;
    personId: number | null | undefined;
    scheduleId: number | null;
    isDG: boolean;
    schoolYearSettings: Schedules_GetSchoolYearSettings;
    usedDatesWeeks: Schedules_GetUsedDatesWeeks;
    offDates: Schedules_GetOffDates;
    schedule: Schedules_Get | null;
    shift: Shifts_Get | null;
  };

  private readonly destroyed$ = new Subject<void>();

  readonly fasAngleRight = fasAngleRight;
  readonly fasAngleLeft = fasAngleLeft;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasCheck = fasCheck;
  readonly fasPlus = fasPlus;
  readonly fasTrash = fasTrash;
  readonly farTrashAlt = farTrashAlt;
  readonly farCalendarPlus = farCalendarPlus;
  readonly farCalendarMinus = farCalendarMinus;
  readonly farCalendarCheck = farCalendarCheck;

  readonly scheduleDaysSet = new Set([1, 2, 3, 4, 5]);
  readonly termNomsService = new TermNomsService();
  readonly Object = Object;
  readonly dayNames = dayNames;

  shiftNomsService!: INomService<number, { instId: number; schoolYear: number }>;
  periodFinishing = false;
  saving = false;
  currentStep = 1;
  errors: string[] = [];

  weeks!: WeekModel[];
  weeksByYear!: { [key: string]: WeekModel[] };
  fullyUsedWeeks!: Date[];
  readOnlyDates!: Date[];
  hasReadOnlyWeeks!: boolean;
  hasFullyUsedWeeks!: boolean;
  usedHoursTableDownloadUrls: string[] | null = null;
  shift?: Omit<Shifts_Get, 'shiftId' | 'name' | 'isAdhoc'> | null;
  shiftId?: number | null;

  termControl!: UntypedFormControl;
  includesWeekendControl!: UntypedFormControl;
  shiftIdControl!: UntypedFormControl;
  hasAdhocShiftControl!: UntypedFormControl;
  periodForm!: UntypedFormGroup;
  shiftForm!: FormControl<ShiftFormValue>;
  shiftFormSubmitted = false;
  showIndividualCurriculumsWarning = false;
  individualCurriculumsWarningMessage = '';

  get period() {
    return this.periodForm.value as Nullable<Period>;
  }

  get term() {
    return this.termControl.value as SchoolTerm | 0 | null;
  }

  getDayForm(dayIndex: number) {
    return (<UntypedFormArray>this.daysForm.get('days')).at(dayIndex);
  }

  days!: {
    day: number;
    hours: {
      hourNumber: number;
      startTime: string;
      endTime: string;
      groups: { curriculumId?: number | null; isReadOnly: boolean; location?: string | null }[];
    }[];
    hasReadOnlyHours: boolean;
  }[];
  daysForm!: UntypedFormGroup;
  classBookCurriculumNomsService: INomService<number, { schoolYear: number; instId: number; classBookId: number }>;

  constructor(
    private fb: UntypedFormBuilder,
    private schedulesService: SchedulesService,
    private shiftsService: ShiftsService,
    private dialogRef: MatDialogRef<ScheduleDialogSkeletonComponent>,
    shiftNomsService: ShiftNomsService,
    classBookCurriculumNomsService: ClassBookCurriculumNomsService
  ) {
    this.shiftNomsService = new NomServiceWithParams(shiftNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));
    this.classBookCurriculumNomsService = new NomServiceWithParams(classBookCurriculumNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      individualCurriculumPersonId: this.data.isIndividualSchedule ? this.data.personId : null,
      excludeIndividualCurriculums: !this.data.isIndividualSchedule
    }));
  }

  ngOnInit() {
    this.initializeHelperData();
    this.initializeFormControls();
    this.fillFormControls();
    if (!this.data.schedule) {
      this.selectAll();
    } else {
      Promise.all(
        Array.from({ length: 7 }, (_, i) =>
          this.schedulesService
            .downloadScheduleUsedHoursTableExcelFile({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              scheduleId: this.data.scheduleId!,
              day: i + 1
            })
            .toPromise()
        )
      ).then((urls) => {
        this.usedHoursTableDownloadUrls = urls;
      });
    }

    this.termControl.valueChanges
      .pipe(
        distinctUntilChanged(),
        tap((term: SchoolTerm | 0 | null) => {
          if (term != null) {
            // changing the term will reset the period if
            // one can be determined
            const period = this.getPeriodFromTerm(term);
            if (period) {
              const { startDate, endDate } = period;
              this.periodForm.setValue({ startDate, endDate }, { emitEvent: false });
            }

            this.selectAll();
          }
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();

    this.periodForm.valueChanges
      .pipe(
        distinctUntilChanged(),
        tap((period: Nullable<Period>) => {
          this.deselectAll();
          this.selectAllInternal(period);
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  initializeHelperData() {
    this.weeks = eachWeekOfInterval(
      {
        start: this.data.schoolYearSettings.schoolYearStartDateLimit,
        end: this.data.schoolYearSettings.schoolYearEndDateLimit
      },
      { weekStartsOn: 1 }
    ).map((startDate) => {
      const year = getISOWeekYear(startDate);
      const week = getISOWeek(startDate);
      const endDate = endOfWeek(startDate, { weekStartsOn: 1 });

      const disabled =
        this.data.usedDatesWeeks.weeks.findIndex(
          (uw) => uw.year === year && uw.weekNumber === week && !uw.isPartiallyUsed
        ) > -1;

      let readOnly;
      let selected = false;
      if (this.data.schedule) {
        const dw = this.data.schedule.weeks.find((dw) => dw.year === year && dw.weekNumber === week);
        readOnly = !!dw && dw.isReadOnly;
        selected = !!dw;
      } else {
        readOnly = false;
      }

      return {
        disabled,
        readOnly,
        selected,
        year: year,
        weekNumber: week,
        startDate,
        startDateDisplay: format(startDate, 'dd.MM', { locale: bg }),
        endDate,
        endDateDisplay: format(endDate, 'dd.MM', { locale: bg }),
        monthAbbr: format(endDate, 'MMM', { locale: bg })
      };
    });

    this.weeksByYear = this.weeks.reduce((acc, w) => {
      const year = w.startDate.getFullYear();
      acc[year] = acc[year] || [];
      acc[year].push(w);
      return acc;
    }, {} as { [key: string]: WeekModel[] });

    this.readOnlyDates = this.data.schedule?.readOnlyDates || [];
    this.hasReadOnlyWeeks = this.weeks.findIndex((w) => w.readOnly) !== -1;
    this.hasFullyUsedWeeks = this.weeks.filter((w) => w.disabled).length > 0;
    this.shift = this.data.shift;
    this.shiftId = this.data.shift?.shiftId;
  }

  initializeFormControls() {
    this.termControl = new UntypedFormControl(null, Validators.required);
    this.includesWeekendControl = new UntypedFormControl(false, Validators.required);
    this.shiftIdControl = new UntypedFormControl(null, Validators.required);
    this.hasAdhocShiftControl = new UntypedFormControl(false);
    this.periodForm = this.fb.group(
      {
        startDate: [null, Validators.required],
        endDate: [null, Validators.required]
      },
      {
        validators: [
          (control: AbstractControl): ValidationErrors | null => {
            const { startDate, endDate } = control.value as Nullable<Period>;

            let error: { dateErrors: string[] } | null = null;
            if (startDate && endDate && startDate > endDate) {
              error = error || { dateErrors: [] };
              error.dateErrors.push('Началната дата трябва да е преди крайната.');
            }

            const term = this.term;
            if (term && startDate && startDate < this.getTermStart(term)) {
              error = error || { dateErrors: [] };
              error.dateErrors.push('Началната дата трябва да е след началото на срока.');
            }

            if (term && endDate && endDate > this.getTermEnd(term)) {
              error = error || { dateErrors: [] };
              error.dateErrors.push('Крайната дата трябва да е преди края на срока.');
            }

            if (!error && startDate && endDate) {
              const period = { startDate, endDate };
              if (!isSamePeriod(period, this.expandToCoverReadOnlyDates(period))) {
                error = { dateErrors: ['Избрания период не обхваща всички използвани дати в разписанието.'] };
              }
            }

            return error;
          }
        ]
      }
    );

    const shift = this.data.shift;
    const shiftFormValue: ShiftFormValue = {
      isMultiday: shift?.isMultiday ?? false,
      days: shift?.days ?? [
        {
          day: 1,
          hours: [{ hourNumber: 1, startTime: '07:30', endTime: '08:10' }]
        }
      ]
    };
    this.shiftForm = this.fb.nonNullable.control(shiftFormValue);
  }

  fillFormControls() {
    let term: SchoolTerm | 0;
    let includesWeekend: boolean;
    let period: Nullable<Period>;
    if (!this.data.schedule) {
      const offDatesSet = new Set<Date>(this.data.offDates);

      const unusedSubPeriodInFirstTerm = this.shrinkToFirstUnusedSubPeriod({
        startDate: this.data.schoolYearSettings.schoolYearStartDate,
        endDate: this.data.schoolYearSettings.firstTermEndDate
      });

      const defaultTerm = this.data.isDG
        ? 0
        : unusedSubPeriodInFirstTerm != null &&
          this.getDatesInPeriod(unusedSubPeriodInFirstTerm).filter((d) => !offDatesSet.has(d)).length > 0
        ? SchoolTerm.TermOne
        : SchoolTerm.TermTwo;

      const termPeriod = this.getPeriodFromTerm(defaultTerm);
      const defaultStartDate = termPeriod && termPeriod.startDate;
      const defaultEndDate = termPeriod && termPeriod.endDate;

      term = defaultTerm;
      includesWeekend = false;
      period = { startDate: defaultStartDate, endDate: defaultEndDate };
    } else {
      term = this.data.schedule.term ?? 0;
      includesWeekend = this.data.schedule.includesWeekend;
      period = { startDate: this.data.schedule.startDate, endDate: this.data.schedule.endDate };
    }
    this.termControl.setValue(term, { emitEvent: false });
    this.periodForm.setValue(period, { emitEvent: false });
    this.includesWeekendControl.setValue(includesWeekend, { emitEvent: false });

    if (this.data.schedule) {
      if (
        this.data.schedule.includesWeekend &&
        this.data.schedule.days.some((d) => (d.day === 6 || d.day === 7) && d.hasReadOnlyHours)
      ) {
        this.includesWeekendControl.disable({ emitEvent: false });
      }

      const isAdhoc = this.data.shift?.isAdhoc;
      if (isAdhoc) {
        this.shiftIdControl.disable({ emitEvent: false });
      } else {
        this.shiftIdControl.setValue(this.data.schedule.shiftId, { emitEvent: false });
      }

      this.hasAdhocShiftControl.setValue(isAdhoc, { emitEvent: false });
      this.hasAdhocShiftControl.disable({ emitEvent: false });
    }
  }

  expandToCoverReadOnlyDates(period: Period): Period {
    if (!this.readOnlyDates.length) {
      return { ...period };
    }
    return {
      startDate: min([period.startDate, this.readOnlyDates[0]]),
      endDate: max([period.endDate, this.readOnlyDates[this.readOnlyDates.length - 1]])
    };
  }

  getTermStart(term: SchoolTerm | 0) {
    switch (term) {
      case 0:
      case SchoolTerm.TermOne:
        return this.data.schoolYearSettings.schoolYearStartDate;
      case SchoolTerm.TermTwo:
        return this.data.schoolYearSettings.secondTermStartDate;
      default:
        throwError('Unknown term');
    }
  }

  getTermEnd(term: SchoolTerm | 0) {
    switch (term) {
      case SchoolTerm.TermOne:
        return this.data.schoolYearSettings.firstTermEndDate;
      case 0:
      case SchoolTerm.TermTwo:
        return this.data.schoolYearSettings.schoolYearEndDate;
      default:
        throwError('Unknown term');
    }
  }

  getDatesInPeriod({ startDate, endDate }: Period): Date[] {
    if (startDate > endDate) {
      return [];
    }

    return eachDayOfInterval({
      start: startDate,
      end: endDate
    }).filter((d) => this.scheduleDaysSet.has(getISODay(d)));
  }

  shrinkToFirstUnusedSubPeriod({ startDate, endDate }: Period): Period | null {
    const usedDatesSet = new Set<Date>(this.data.usedDatesWeeks.dates);
    const datesInPeriod = this.getDatesInPeriod({ startDate, endDate });
    const firstUsedDate = datesInPeriod.findIndex((d) => usedDatesSet.has(d));
    if (firstUsedDate === -1) {
      return { startDate, endDate };
    }

    if (firstUsedDate > 0) {
      return { startDate, endDate: datesInPeriod[firstUsedDate - 1] };
    }

    const firstUnusedDate = datesInPeriod.findIndex((d) => !usedDatesSet.has(d));
    if (firstUnusedDate === -1) {
      return null;
    }

    const nextUsedDate = datesInPeriod.findIndex((d, i) => i > firstUnusedDate && usedDatesSet.has(d));
    if (nextUsedDate === -1) {
      return { startDate: datesInPeriod[firstUnusedDate], endDate };
    }

    return { startDate: datesInPeriod[firstUnusedDate], endDate: datesInPeriod[nextUsedDate - 1] };
  }

  getPeriodFromTerm(term: SchoolTerm | 0): Period | null {
    const startDate = this.getTermStart(term);
    const endDate = this.getTermEnd(term);

    const period = this.expandToCoverReadOnlyDates({ startDate, endDate });
    return this.shrinkToFirstUnusedSubPeriod(period);
  }

  weekSelectionChanged(week: WeekModel) {
    week.selected = !week.selected;

    const term = this.term;
    if (term != null) {
      // selecting a week should expand the period if necessary

      let { startDate, endDate } = this.period;

      if (this.weeks.find((w) => w.selected) === week) {
        // the first week in the range was selected

        if (startDate) {
          startDate = min([startDate, week.startDate]);
        } else {
          startDate = week.startDate;
        }

        // if the term start is in the selected week expand down to the term start
        const termStartDate = this.getTermStart(term);
        if (week.startDate <= termStartDate && termStartDate <= week.endDate) {
          startDate = max([termStartDate, startDate]);
        }
      }

      if (
        this.weeks
          .slice()
          .reverse()
          .find((w) => w.selected) === week
      ) {
        // the last week in the range was selected

        if (endDate) {
          endDate = max([endDate, week.endDate]);
        } else {
          endDate = week.endDate;
        }

        // if the term end is in the selected week expand up to the term end
        const termEndDate = this.getTermEnd(term);
        if (week.startDate <= termEndDate && termEndDate <= week.endDate) {
          endDate = min([termEndDate, endDate]);
        }
      }

      this.periodForm.setValue({ startDate, endDate }, { emitEvent: false });
    }
  }

  selectAll(filterPredicate = (week: { readOnly: boolean; weekNumber: number }) => true) {
    this.selectAllInternal(this.period, filterPredicate);
  }

  selectAllInternal(
    { startDate, endDate }: Nullable<Period>,
    filterPredicate = (week: { readOnly: boolean; weekNumber: number }) => true
  ) {
    if (!startDate || !endDate) {
      return;
    }

    const start = startOfWeek(startDate, { weekStartsOn: 1 });
    const end = endOfWeek(endDate, { weekStartsOn: 1 });

    const weekIntersectsPeriod = (week: { startDate: Date; endDate: Date }) =>
      start <= week.endDate && end >= week.startDate;

    this.weeks.forEach((w) => {
      if (!w.disabled) {
        w.selected = weekIntersectsPeriod(w) && filterPredicate(w);
      }
    });
  }

  deselectAll() {
    this.weeks.forEach((w) => (w.selected = w.readOnly));
  }

  selectEven() {
    this.selectAll((w) => w.readOnly || w.weekNumber % 2 === 0);
  }

  selectOdd() {
    this.selectAll((w) => w.readOnly || w.weekNumber % 2 !== 0);
  }

  hasAdhocShiftChanged() {
    if (this.hasAdhocShiftControl.getRawValue()) {
      this.shiftIdControl.setValue(null, { emitEvent: false });
      this.shiftIdControl.disable({ emitEvent: false });
    } else {
      this.shiftIdControl.enable({ emitEvent: false });
    }
  }

  get lastStep(): number {
    return this.includesWeekendControl.getRawValue() ? 9 : 7;
  }

  get readOnlyHours() {
    return (
      this.data.schedule?.days
        ?.map((d) =>
          d.hasReadOnlyHours
            ? d.hours.filter((h) => h.isReadOnly).map((h) => ({ day: d.day, hourNumber: h.hourNumber }))
            : []
        )
        .flat(1) ?? []
    );
  }

  previous() {
    if (this.currentStep === 3 && !this.hasAdhocShiftControl.getRawValue()) {
      this.currentStep = 1;
      return;
    }

    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  next() {
    switch (this.currentStep) {
      case 1:
        this.finishPeriod();
        return;
      case 2:
        this.finishShift();
        return;
      case 3:
      case 4:
      case 5:
      case 6:
      case 7:
      case 8:
      case 9:
        this.finishDay();
        return;
      default:
        throw new Error('Invalid step.');
    }
  }

  finishPeriod() {
    // hack to show term/dates/shift is required as there is no form that is submitted
    this.termControl.markAsTouched();
    this.periodForm.controls.startDate.markAsTouched();
    this.periodForm.controls.endDate.markAsTouched();
    this.shiftIdControl.markAsTouched();

    if (this.periodForm.invalid || this.termControl.invalid || this.shiftIdControl.invalid || this.periodFinishing) {
      return;
    }

    if (this.weeks.filter((w) => w.selected).length === 0) {
      this.errors = ['Трябва да е избрана поне една седмица'];
      return;
    }

    this.errors = [];

    if (this.hasAdhocShiftControl.getRawValue()) {
      this.currentStep++;
    } else {
      const shiftId = this.shiftIdControl.value as number;
      let canChangeShiftPromise: Promise<string | null>;
      if (
        shiftId == null ||
        this.data.scheduleId == null ||
        this.data.schedule == null ||
        this.data.schedule.shiftId === shiftId
      ) {
        canChangeShiftPromise = Promise.resolve(null);
      } else {
        canChangeShiftPromise = this.schedulesService
          .canChangeShiftWith({
            schoolYear: this.data.schoolYear,
            instId: this.data.instId,
            classBookId: this.data.classBookId,
            scheduleId: this.data.scheduleId,
            shiftId
          })
          .toPromise();
      }

      this.periodFinishing = true;
      canChangeShiftPromise
        .then((canChangeShiftError) => {
          if (canChangeShiftError) {
            this.errors = [canChangeShiftError];
            return Promise.resolve();
          }

          let updateShiftPromise: Promise<void>;
          if (this.shift && this.shiftId === shiftId) {
            updateShiftPromise = Promise.resolve();
          } else {
            updateShiftPromise = this.shiftsService
              .get({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                shiftId
              })
              .toPromise()
              .then((shift) => {
                this.shift = shift;
                this.shiftId = shiftId;
              });
          }

          return updateShiftPromise.then(() => {
            this.currentStep += 2;
            this.createDaysForm(this.fb, this.shift!, this.data.schedule?.days);
          });
        })
        .finally(() => {
          this.periodFinishing = false;
        });
    }
  }

  finishShift() {
    this.shiftFormSubmitted = true;

    if (this.shiftForm.invalid) {
      return;
    }

    const shiftFormValue = this.shiftForm.getRawValue();
    this.shift = {
      isMultiday: shiftFormValue.isMultiday,
      days: shiftFormValue.days.map(({ day, hours }) => ({
        day,
        hours: hours.map((h) => ({
          hourNumber: h.hourNumber,
          startTime: h.startTime ?? throwError('startTime should not be null'),
          endTime: h.endTime ?? throwError('endTime should not be null')
        }))
      }))
    };

    this.currentStep++;
    this.createDaysForm(this.fb, this.shift!, this.data.schedule?.days);
  }

  createDaysForm(
    fb: UntypedFormBuilder,
    shift: {
      isMultiday: boolean;
      days: {
        day: number;
        hours: {
          hourNumber: number;
          startTime: string;
          endTime: string;
        }[];
      }[];
    },
    existingScheduleDays: Schedules_Get['days'] | null | undefined
  ) {
    const daysOfWeek = Array.from({ length: 7 }, (_, i) => i + 1);

    let dayHours: {
      day: number;
      hours: {
        hourNumber: number;
        startTime: string;
        endTime: string;
      }[];
    }[];

    if (shift.isMultiday) {
      if (shift.days.length !== 7) {
        throw new Error(`Unexpected number of days ${shift.days.length} for a isMultiday:true shift`);
      }

      dayHours = shift.days;
    } else {
      if (shift.days.length !== 1) {
        throw new Error(`Unexpected number of days ${shift.days.length} for a isMultiday:false shift`);
      }

      dayHours = [];
      for (const day of daysOfWeek) {
        dayHours.push({
          day,
          hours: shift.days[0].hours
        });
      }
    }

    this.days = (existingScheduleDays || daysOfWeek.map((d) => ({ day: d, hours: [], hasReadOnlyHours: false }))).map(
      (d) => ({
        ...d,
        hours: Array.from(
          dayHours.find(({ day }) => d.day === day)?.hours ?? throwError('could not find day'),
          (h) => ({
            hourNumber: h.hourNumber,
            startTime: h.startTime,
            endTime: h.endTime,
            groups: defaultIfNullOrEmpty(
              d.hours
                .filter((oldHour) => oldHour.hourNumber === h.hourNumber)
                .map((oldHour) => ({
                  curriculumId: oldHour.curriculumId,
                  isReadOnly: oldHour.isReadOnly,
                  location: oldHour.location
                })),
              [{ curriculumId: null, isReadOnly: false, location: null }]
            )
          })
        )
      })
    );

    this.daysForm = fb.group({
      days: fb.array(
        this.days.map((d) =>
          fb.array(
            d.hours.map((h) =>
              fb.array(
                h.groups.map((g) => {
                  return fb.group({
                    curriculumId: [{ value: g.curriculumId, disabled: g.isReadOnly }],
                    location: [{ value: g.location, disabled: g.isReadOnly }]
                  });
                })
              )
            ),
            {
              validators: [
                (control: AbstractControl): ValidationErrors | null => {
                  const hours = control.value as ({
                    curriculumId: number | null;
                    location: string | null;
                  } | null)[][];

                  let error: { hourErrors: string[] } | null = null;

                  const hasDuplicatedGroups = hours.some((h) => {
                    const seenCurriculumIds = new Set<number>();

                    return h.some((g) => {
                      if (g?.curriculumId != null) {
                        if (seenCurriculumIds.has(g.curriculumId)) {
                          return true; // Duplicate found
                        }
                        seenCurriculumIds.add(g.curriculumId);
                      }
                      return false;
                    });
                  });
                  if (hasDuplicatedGroups) {
                    error = error || { hourErrors: [] };
                    error.hourErrors.push(
                      'Не може да изберете един предмет от учебния план повече от веднъж за конкретен час.'
                    );
                  }

                  return error;
                }
              ]
            }
          )
        )
      )
    });

    this.checkIndividualCurriculums();

    this.daysForm.valueChanges
      .pipe(
        distinctUntilChanged(),
        tap(() => {
          this.checkIndividualCurriculums();
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
  }

  checkIndividualCurriculums(): void {
    const daysFormData = this.daysForm.getRawValue().days;
    const curriculumIds = [
      ...new Set(
        daysFormData
          .flatMap((day: any) => day.flatMap((hour: any) => hour.flatMap((curriculums: any) => curriculums)))
          .filter((id: any) => id != null)
      )
    ] as number[];

    if (this.data.isIndividualSchedule && curriculumIds.length > 0) {
      this.schedulesService
        .getScheduleCurriculumsInfo({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          classBookId: this.data.classBookId,
          curriculumIds: curriculumIds
        })
        .toPromise()
        .then((curriculumsInfo) => {
          const notIndividualCurriculums = curriculumsInfo.filter((c) => !c.isIndividualCurriculum);
          if (notIndividualCurriculums.length > 0) {
            let warningMessage = 'Избрали сте предмети, които не са от индивидуален учебен план:';
            daysFormData.map((day: any, dayIndex: number) => {
              const currentDayCurriculums = daysFormData[dayIndex]
                .flatMap((hour: any) => hour.flatMap((curriculums: any) => curriculums))
                .filter((id: any) => id != null);
              const notIndividualCurriculumsNames = notIndividualCurriculums
                .filter((c) => currentDayCurriculums.includes(c.curriculumId))
                .map((item) => item.curriculumName);

              if (notIndividualCurriculumsNames.length > 0) {
                warningMessage =
                  warningMessage + ` ${dayNames[dayIndex + 1]} - ${notIndividualCurriculumsNames.join(', ')};`;
              }
            });

            this.individualCurriculumsWarningMessage = warningMessage;
            this.showIndividualCurriculumsWarning = true;
          } else {
            this.showIndividualCurriculumsWarning = false;
          }
        });
    }
  }

  addGroup(hourIndex: number) {
    this.days[this.currentStep - 3].hours[hourIndex].groups.push({
      curriculumId: null,
      isReadOnly: false,
      location: null
    });

    const daysArray = this.daysForm.get('days') as UntypedFormArray;
    const hoursArray = daysArray.controls[this.currentStep - 3] as UntypedFormArray;
    const groupsArray = hoursArray.controls[hourIndex] as UntypedFormArray;
    groupsArray.push(
      this.fb.group({
        curriculumId: [{ value: null, disabled: false }],
        location: [{ value: null, disabled: false }]
      })
    );
  }

  removeGroup(hourIndex: number, groupIndex: number) {
    this.days[this.currentStep - 3].hours[hourIndex].groups.splice(groupIndex, 1);

    const daysArray = this.daysForm.get('days') as UntypedFormArray;
    const hoursArray = daysArray.controls[this.currentStep - 3] as UntypedFormArray;
    const groupsArray = hoursArray.controls[hourIndex] as UntypedFormArray;
    groupsArray.removeAt(groupIndex);
  }

  finishDay() {
    if (this.daysForm.invalid) {
      return;
    }

    if (this.currentStep < this.lastStep) {
      this.currentStep++;
    } else {
      const { days: daysValue } = this.daysForm.getRawValue() as {
        days: ({ curriculumId: number | null; location: string | null } | null)[][][];
      };

      if (
        daysValue.filter((d) => d.filter((h) => h.filter((curriculumId) => curriculumId != null).length > 0).length > 0)
          .length === 0
      ) {
        this.errors = ['Трябва да има поне един ден с часове.'];
        return;
      }

      this.saving = true;

      const term = this.termControl.value as SchoolTerm | 0;
      const { startDate, endDate } = this.periodForm.value as Period;
      const weeks = this.weeks.filter((w) => w.selected).map((w) => ({ year: w.year, weekNumber: w.weekNumber }));
      const days = this.days.map((d, di) => ({
        day: d.day,
        hours: d.hours.map((h, hi) => ({
          hourNumber: h.hourNumber,
          groups: h.groups.map((g, gi) => ({
            curriculumId: daysValue[di][hi][gi]?.curriculumId,
            location: daysValue[di][hi][gi]?.location
          }))
        }))
      }));

      let shiftData: Pick<
        CreateScheduleCommand,
        'hasAdhocShift' | 'shiftId' | 'adhocShiftIsMultiday' | 'adhocShiftDays'
      >;

      if (this.hasAdhocShiftControl.value) {
        shiftData = {
          hasAdhocShift: true,
          adhocShiftIsMultiday: this.shift?.isMultiday,
          adhocShiftDays: this.shift?.days
        };
      } else {
        shiftData = {
          hasAdhocShift: false,
          shiftId: this.shiftIdControl.value as number
        };
      }

      let createOrUpdatePromise;
      if (this.data.scheduleId) {
        createOrUpdatePromise = this.schedulesService
          .update({
            schoolYear: this.data.schoolYear,
            instId: this.data.instId,
            classBookId: this.data.classBookId,
            scheduleId: this.data.scheduleId,
            updateScheduleCommand: {
              isIndividualSchedule: this.data.isIndividualSchedule,
              personId: this.data.personId,
              term: term === 0 ? null : term,
              startDate,
              endDate,
              includesWeekend: !!this.includesWeekendControl.getRawValue(),
              ...shiftData,
              weeks,
              days
            }
          })
          .toPromise();
      } else {
        createOrUpdatePromise = this.schedulesService
          .create({
            schoolYear: this.data.schoolYear,
            instId: this.data.instId,
            classBookId: this.data.classBookId,
            createScheduleCommand: {
              isIndividualSchedule: this.data.isIndividualSchedule,
              personId: this.data.personId,
              term: term === 0 ? null : term,
              startDate,
              endDate,
              includesWeekend: !!this.includesWeekendControl.getRawValue(),
              ...shiftData,
              weeks,
              days
            }
          })
          .toPromise();
      }

      createOrUpdatePromise
        .then(() => this.dialogRef.close(true))
        .catch((err) => {
          if (err.status === 400 && err.error?.errorMessages?.length > 0) {
            this.errors = err.error.errorMessages;
          } else {
            const requestId = err.headers.get('X-Sb-Request-Id');

            this.errors = requestId
              ? [`Възникна грешка! Моля, опитайте отново. Номер на грешка: ${requestId}`]
              : ['Възникна грешка! Моля, опитайте отново.'];
            GlobalErrorHandler.instance.handleError(err, true);
          }
        })
        .finally(() => {
          this.saving = false;
        });
    }
  }
}
