import { Component, Input, OnDestroy } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormArray,
  FormBuilder,
  FormGroup,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  NonNullableFormBuilder,
  ValidationErrors,
  Validator,
  Validators,
  ɵElement,
  ɵRawValue
} from '@angular/forms';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { hourRegex } from 'projects/shared/utils/date';
import { throwError } from 'projects/shared/utils/various';
import { merge, Subject, Subscription } from 'rxjs';
import { map, takeUntil, tap } from 'rxjs/operators';

export type ShiftHoursFormValue = Array<Omit<Hour, 'duration'>>;

type FormGroupType<T> = FormGroup<{
  [K in keyof T]: ɵElement<T[K], never>;
}>;

function createShiftHoursForm(fb: NonNullableFormBuilder) {
  const hoursGroup = {
    hours: new FormArray<HourForm>([])
  };
  return fb.group(hoursGroup, {
    validators: [
      (control: AbstractControl): ValidationErrors | null => {
        const typedControl = control as FormGroupType<typeof hoursGroup>;
        const hours = typedControl.value.hours;
        const duplicatedHourNumbers = [];
        const allHours = new Set<number>();
        if (hours) {
          for (const { hourNumber } of hours) {
            if (hourNumber == null) {
              continue;
            }

            if (allHours.has(hourNumber)) {
              duplicatedHourNumbers.push(hourNumber);
            } else {
              allHours.add(hourNumber);
            }
          }
        }

        return duplicatedHourNumbers.length ? { duplicatedHourNumbers } : null;
      }
    ]
  });
}

function createHourForm(
  fb: NonNullableFormBuilder,
  hourNumber: number,
  startTime: string | null,
  duration: string | number | null,
  endTime: string | null
) {
  return fb.group({
    hourNumber: [hourNumber, [Validators.required, Validators.min(0)]],
    startTime: [startTime, [Validators.required, Validators.pattern(hourRegex)]],
    duration: [duration, Validators.required],
    endTime: [endTime, [Validators.required, Validators.pattern(hourRegex)]]
  });
}
type HourForm = ReturnType<typeof createHourForm>;
type Hour = ɵRawValue<HourForm>;

@Component({
  selector: 'sb-shift-hours-form',
  templateUrl: './shift-hours-form.component.html',
  styleUrls: ['./shift-hours-form.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      multi: true,
      useExisting: ShiftHoursFormComponent
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: ShiftHoursFormComponent
    }
  ]
})
export class ShiftHoursFormComponent implements ControlValueAccessor, Validator, OnDestroy {
  // an angular form wrapped as a control
  // see https://blog.angular-university.io/angular-custom-form-controls/

  @Input() allowZeroHours = true;
  @Input() usedHours: number[] = [];

  private readonly destroyed$ = new Subject<void>();
  private readonly clearEventHandlers$ = new Subject<void>();

  readonly fasPlus = fasPlus;
  readonly fasTrashAlt = fasTrashAlt;

  readonly predefinedDurations = [
    { id: 0, text: 'Въведи край' },
    { id: 30, text: '30' },
    { id: 35, text: '35' },
    { id: 40, text: '40' },
    { id: 45, text: '45' },
    { id: 50, text: '50' },
    { id: 55, text: '55' },
    { id: 60, text: '60' },
    { id: 90, text: '90' }
  ];

  readonly form = createShiftHoursForm(this.fb.nonNullable);

  // eslint-disable-next-line @typescript-eslint/no-empty-function
  onTouched = () => {};

  onChangeSubs: Subscription[] = [];

  constructor(private fb: FormBuilder) {}

  ngOnDestroy() {
    for (const sub of this.onChangeSubs) {
      sub.unsubscribe();
    }

    this.clearEventHandlers$.complete();
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  registerOnChange(onChange: any) {
    const sub = this.form.valueChanges.pipe(map((v) => v.hours)).subscribe(onChange);
    this.onChangeSubs.push(sub);
  }

  registerOnTouched(onTouched: () => void) {
    this.onTouched = onTouched;
  }

  setDisabledState(disabled: boolean) {
    if (disabled) {
      this.removeEventHandlers();
      this.form.disable({ emitEvent: false });
    } else {
      this.form.enable({ emitEvent: false });
      this.attachEventHandlers();
    }
  }

  writeValue(v: any) {
    if (v) {
      const value = v as Hour[];

      this.removeEventHandlers();
      this.hours.clear();

      for (let i = 0; i < value.length; i++) {
        const hour = value[i];

        let duration: number | null = null;

        if (hour.startTime != null && hour.endTime != null) {
          const startTimeMinutes = this.timespanToMinutes(hour.startTime);
          let endTimeMinutes = this.timespanToMinutes(hour.endTime);

          if (startTimeMinutes > endTimeMinutes) {
            // wrap around 24h
            endTimeMinutes = endTimeMinutes + 24 * 60;
          }

          duration = endTimeMinutes - startTimeMinutes;
          if (!this.predefinedDurations.find((d) => d.id === duration)) {
            duration = 0;
          }
        }

        this.hours.push(createHourForm(this.fb.nonNullable, hour.hourNumber, hour.startTime, duration, hour.endTime));
      }

      this.attachEventHandlers();
    }
  }

  validate(control: AbstractControl) {
    if (this.form.valid || this.form.disabled) {
      return null;
    } else {
      return {
        hasRequiredErrors: this.hours.controls.some(
          (h) =>
            h.controls.hourNumber.errors?.required ||
            h.controls.startTime.errors?.required ||
            h.controls.duration.errors?.required ||
            h.controls.endTime.errors?.required
        ),
        hasPatternErrors: this.hours.controls.some(
          (h) => h.controls.startTime.errors?.pattern || h.controls.endTime.errors?.pattern
        ),
        hasNegativeHourErrors: this.hours.controls.some((h) => h.controls.hourNumber.errors?.min),
        hasDuplicatedHourNumberErrors: !!this.form.errors?.duplicatedHourNumbers
      };
    }
  }

  addHour() {
    const lastHourNumber = this.form.value.hours?.reverse().find((h: any) => h.hourNumber)?.hourNumber ?? 0;
    this.hours.push(createHourForm(this.fb.nonNullable, lastHourNumber + 1, null, null, null));
    this.removeEventHandlers();
    this.attachEventHandlers();
  }

  removeHour(i: number) {
    this.hours.removeAt(i);
    this.removeEventHandlers();
    this.attachEventHandlers();
  }

  get hours() {
    return this.form.get('hours') as unknown as FormArray<HourForm>;
  }

  private startTimeControlAt(i: number) {
    return this.hours.at(i).get('startTime') ?? throwError("'startTime' control missing");
  }

  private durationControlAt(i: number) {
    return this.hours.at(i).get('duration') ?? throwError("'duration' control missing");
  }

  private endTimeControlAt(i: number) {
    return this.hours.at(i).get('endTime') ?? throwError("'endTime' control missing");
  }

  private removeEventHandlers() {
    this.clearEventHandlers$.next();
  }

  private attachEventHandlers() {
    for (let i = 0; i < this.hours.length; i++) {
      const startTimeControl = this.startTimeControlAt(i);
      const durationControl = this.durationControlAt(i);

      startTimeControl.valueChanges
        .pipe(
          tap((startTime) => {
            this.syncEndTime(startTime, null, i);
          }),
          takeUntil(merge(this.destroyed$, this.clearEventHandlers$))
        )
        .subscribe();

      durationControl.valueChanges
        .pipe(
          tap((durationStr) => {
            // parseInt will correctly handle string | number | null
            const duration = parseInt(durationStr as any, 10);
            this.syncEndTime(null, duration, i);
          }),
          takeUntil(merge(this.destroyed$, this.clearEventHandlers$))
        )
        .subscribe();
    }
  }

  private syncEndTime(startTime: string | null, duration: number | null, i: number) {
    if (startTime == null) {
      startTime = this.startTimeControlAt(i).value;
    }

    if (duration == null) {
      // parseInt will correctly handle string | number | null
      duration = parseInt(this.durationControlAt(i).value as any, 10);
    }

    if (duration === 0) {
      return;
    }

    const endTimeControl = this.endTimeControlAt(i);

    if (startTime) {
      const endTimeTotalMinutes = this.timespanToMinutes(startTime) + duration;
      if (isNaN(endTimeTotalMinutes)) {
        return;
      }

      const endTimeHours = Math.floor(endTimeTotalMinutes / 60) % 24;
      const endTimeMinutes = endTimeTotalMinutes % 60;
      endTimeControl.setValue(
        `${endTimeHours.toString().padStart(2, '0')}:${endTimeMinutes.toString().padStart(2, '0')}`
      );
    }
  }

  private timespanToMinutes(timespan: string): number {
    const match = hourRegex.exec(timespan);
    if (!match) {
      return NaN;
    }

    const hours = parseInt(match[1], 10);
    const minutes = parseInt(match[2], 10);
    return hours * 60 + minutes;
  }
}
