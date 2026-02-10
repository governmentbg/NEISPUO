import { Component, Input, OnDestroy } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormArray,
  FormBuilder,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  NonNullableFormBuilder,
  Validator,
  ɵRawValue
} from '@angular/forms';
import { trackByIndex } from 'projects/shared/utils/directive';
import { dayNames } from 'projects/shared/utils/schedule';
import { throwError } from 'projects/shared/utils/various';
import { Subject, Subscription } from 'rxjs';
import { ShiftHoursFormValue } from './shift-hours-form/shift-hours-form.component';

export type ShiftFormValue = Shift;

function createShiftForm(fb: NonNullableFormBuilder) {
  return fb.group({
    isMultiday: [false],
    days: new FormArray<DayForm>([])
  });
}
type ShiftForm = ReturnType<typeof createShiftForm>;
type Shift = ɵRawValue<ShiftForm>;

function createDayForm(fb: NonNullableFormBuilder, day: number, hours?: ShiftHoursFormValue) {
  return fb.group({
    day: [day],
    hours: [hours ?? []]
  });
}
type DayForm = ReturnType<typeof createDayForm>;

function addPreposition(str: string) {
  const p = str[0] === 'в' || str[0] === 'В' ? 'във' : 'в';
  return `${p} ${str}`;
}

@Component({
  selector: 'sb-shift-form',
  templateUrl: './shift-form.component.html',
  styleUrls: ['./shift-form.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      multi: true,
      useExisting: ShiftFormComponent
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: ShiftFormComponent
    }
  ]
})
export class ShiftFormComponent implements ControlValueAccessor, Validator, OnDestroy {
  // an angular form wrapped as a control
  // see https://blog.angular-university.io/angular-custom-form-controls/

  @Input() submitted = false;
  @Input() usedHours: { day: number; hourNumber: number }[] = [];
  @Input() usedHoursMessage = '';

  private readonly destroyed$ = new Subject<void>();

  readonly dayNames = dayNames;
  readonly trackByIndex = trackByIndex;

  readonly form = createShiftForm(this.fb.nonNullable);

  postponeValidationErrors = false;

  // eslint-disable-next-line @typescript-eslint/no-empty-function
  onTouched = () => {};

  onChangeSubs: Subscription[] = [];

  constructor(private fb: FormBuilder) {}

  ngOnDestroy() {
    for (const sub of this.onChangeSubs) {
      sub.unsubscribe();
    }

    this.destroyed$.next();
    this.destroyed$.complete();
  }

  registerOnChange(onChange: any) {
    const sub = this.form.valueChanges.subscribe(onChange);
    this.onChangeSubs.push(sub);
  }

  registerOnTouched(onTouched: () => void) {
    this.onTouched = onTouched;
  }

  setDisabledState(disabled: boolean) {
    if (disabled) {
      this.form.disable({ emitEvent: false });
    } else {
      this.form.enable({ emitEvent: false });
    }
  }

  writeValue(v: any) {
    if (v) {
      const value = v as Shift;

      this.days.clear();
      this.form.setValue({ isMultiday: value.isMultiday, days: [] });

      for (const { day, hours } of value.days) {
        this.days.push(createDayForm(this.fb.nonNullable, day, hours));
      }
    }
  }

  validate(control: AbstractControl) {
    if (this.form.valid) {
      return null;
    }

    return { errors: true };
  }

  get days() {
    return this.form.get('days') as unknown as FormArray<DayForm>;
  }

  get hasRequiredErrorsDays() {
    return this.days.controls
      .map((dayControl) => ({
        day: dayControl.value.day ?? throwError('day should not be null'),
        hasRequiredErrors: !!dayControl.get('hours')?.errors?.hasRequiredErrors
      }))
      .filter(({ hasRequiredErrors }) => hasRequiredErrors)
      .map(({ day }) => day);
  }

  get hasRequiredErrorsDaysNames() {
    return addPreposition(this.hasRequiredErrorsDays.map((d) => dayNames[d]).join(', '));
  }

  get hasPatternErrorsDays() {
    return this.days.controls
      .map((dayControl) => ({
        day: dayControl.value.day ?? throwError('day should not be null'),
        hasPatternErrors: !!dayControl.get('hours')?.errors?.hasPatternErrors
      }))
      .filter(({ hasPatternErrors }) => hasPatternErrors)
      .map(({ day }) => day);
  }

  get hasPatternErrorsDaysNames() {
    return addPreposition(this.hasPatternErrorsDays.map((d) => dayNames[d]).join(', '));
  }

  get hasNegativeHourErrorsDays() {
    return this.days.controls
      .map((dayControl) => ({
        day: dayControl.value.day ?? throwError('day should not be null'),
        hasNegativeHourErrors: !!dayControl.get('hours')?.errors?.hasNegativeHourErrors
      }))
      .filter(({ hasNegativeHourErrors }) => hasNegativeHourErrors)
      .map(({ day }) => day);
  }

  get hasNegativeHourErrorsDaysNames() {
    return addPreposition(this.hasNegativeHourErrorsDays.map((d) => dayNames[d]).join(', '));
  }

  get hasDuplicatedHourNumberErrorsDays() {
    return this.days.controls
      .map((dayControl) => ({
        day: dayControl.value.day ?? throwError('day should not be null'),
        hasDuplicatedHourNumberErrors: !!dayControl.get('hours')?.errors?.hasDuplicatedHourNumberErrors
      }))
      .filter(({ hasDuplicatedHourNumberErrors }) => hasDuplicatedHourNumberErrors)
      .map(({ day }) => day);
  }

  get hasDuplicatedHourNumberErrorsDaysNames() {
    return addPreposition(this.hasDuplicatedHourNumberErrorsDays.map((d) => dayNames[d]).join(', '));
  }

  dayHasHours(day: number) {
    return this.form.getRawValue().days[day - 1].hours.length > 0;
  }

  get shiftHasMoreThanOneHour() {
    const value = this.form.getRawValue();
    let hoursCount = 0;

    if (!value.isMultiday) {
      hoursCount = value.days[0].hours.length;
      return hoursCount > 1;
    }

    for (const day of value.days) {
      hoursCount += day.hours.length;
      if (hoursCount > 1) {
        return true;
      }
    }

    return false;
  }

  getUsedHours(day: number) {
    if (this.form.value.isMultiday) {
      return this.usedHours.filter((h) => h.day === day).map((h) => h.hourNumber);
    } else {
      return Array.from(
        this.usedHours
          .reduce((set, h) => {
            set.add(h.hourNumber);
            return set;
          }, new Set<number>())
          .values()
      );
    }
  }

  onIsMultidayChanged() {
    // make the validation error text stable until the next cycle
    // to prevent ExpressionChangedAfterItHasBeenCheckedError
    this.postponeValidationErrors = true;

    try {
      const days = this.days;
      const value = this.form.value;
      if (value.isMultiday) {
        const d = value.days ?? throwError('days should not be null');
        const hours = d[0]?.hours ?? throwError('hours should not be null');
        for (let d = 2; d <= 7; d++) {
          this.days.push(createDayForm(this.fb.nonNullable, d, hours));
        }
      } else {
        for (let i = 6; i >= 1; i--) {
          days.removeAt(i);
        }

        const mondayControl = days.at(0);
        if (this.usedHours.length) {
          // add the used hours missing from monday

          const monday = mondayControl.getRawValue();
          const missingUsedHours = this.getUsedHours(1).filter(
            (usedHourNumber) => !monday.hours.find((h) => h.hourNumber === usedHourNumber)
          );
          mondayControl.setValue({
            day: 1,
            hours: [
              ...monday.hours,
              ...missingUsedHours.map((hourNumber) => ({ hourNumber, startTime: null, endTime: null }))
            ].sort((h1, h2) => h1.hourNumber - h2.hourNumber)
          });
        } else {
          // do not allow an empty day as the only day

          if (!mondayControl.getRawValue().hours.length) {
            mondayControl.setValue({
              day: 1,
              hours: [{ hourNumber: 1, startTime: '07:30', endTime: '08:10' }]
            });
          }
        }
      }
    } finally {
      setTimeout(() => {
        this.postponeValidationErrors = false;
      });
    }
  }
}
