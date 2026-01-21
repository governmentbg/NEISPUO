import { Component, EventEmitter, forwardRef, Input, Output } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';

@Component({
  selector: 'sb-slide-toggle',
  templateUrl: './slide-toggle.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SlideToggleComponent),
      multi: true
    }
  ]
})
export class SlideToggleComponent implements ControlValueAccessor {
  @Input() disabled = false;
  @Input() color: 'primary' | 'accent' | 'warn' = 'primary';
  @Input() labelPosition: 'before' | 'after' = 'after';
  @Input() label = '';

  private _value = false;

  @Output() toggleChange = new EventEmitter<boolean>();

  onChange = (_: any) => {
    // do nothing
  };
  onTouched = () => {
    // do nothing
  };

  constructor() {
    this.onChange = (_: any) => {
      // do nothing
    };
    this.onTouched = () => {
      // do nothing
    };
    this.disabled = false;
  }

  get value(): boolean {
    return this._value;
  }

  set value(val: boolean) {
    this._value = val;
    this.onChange(val);
    this.toggleChange.emit(val);
  }

  writeValue(value: boolean): void {
    this._value = value;
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onSlideToggleChange(event: MatSlideToggleChange): void {
    this.value = event.checked;
  }
}
