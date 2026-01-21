import {
  Component,
  ElementRef,
  forwardRef,
  Input,
  OnChanges,
  Renderer2,
  SimpleChanges,
  ViewChild
} from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { roundDecimalGrade } from 'projects/shared/utils/book';

export const DECIMAL_GRADE_PICKER_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => DecimalGradePickerComponent),
  multi: true
};

@Component({
  selector: 'sb-decimal-grade-picker',
  templateUrl: './decimal-grade-picker.component.html',
  styleUrls: ['./decimal-grade-picker.component.scss'],
  providers: [DECIMAL_GRADE_PICKER_VALUE_ACCESSOR]
})
export class DecimalGradePickerComponent implements ControlValueAccessor, OnChanges {
  @Input() allowEmpty = true;
  @Input() allowFractional = true;
  @Input() showNumberInput = true;
  @Input() showGradeButtons = true;

  @ViewChild('input', { static: true }) inputElementRef!: ElementRef;

  disabled = false;
  value: number | null = null;
  pickerValue: number | null = null;
  inputValue: string | null = null;

  onChange = (_: any) => {
    // do nothing
  };
  onTouched = () => {
    // do nothing
  };

  constructor(private _renderer: Renderer2) {
    this.onChange = (_: any) => {
      // do nothing
    };
    this.onTouched = () => {
      // do nothing
    };
    this.disabled = false;
  }

  ngOnChanges(changes: SimpleChanges) {
    // refresh input value when 'allowFractional' has changed
    if (changes.allowFractional && !changes.allowFractional.isFirstChange()) {
      this.onInputChange();
    }
  }

  setDecimalGrade(value: number | null) {
    if (this.disabled) {
      return;
    }

    this.writeValue(value);

    this.onChange(this.value);
    this.onTouched();
  }

  onInputChange() {
    const value = (this.inputElementRef.nativeElement as HTMLInputElement).value;
    const roundedNumberValue = parseFloat(parseFloat(value).toFixed(this.allowFractional ? 2 : 0));

    this.writeValue(
      !isNaN(roundedNumberValue) ? (roundedNumberValue < 3 ? 2 : roundedNumberValue > 6 ? 6 : roundedNumberValue) : null
    );

    this.onChange(this.value);
  }

  writeValue(value: number | null) {
    this.value = value;

    if (value == null) {
      this.pickerValue = null;
      this.inputValue = null;
    } else {
      this.pickerValue = roundDecimalGrade(value);
      this.inputValue = value.toFixed(2);
    }

    const normalizedInputValue = this.inputValue == null ? '' : this.inputValue;
    this._renderer.setProperty(this.inputElementRef.nativeElement, 'value', normalizedInputValue);
  }

  registerOnChange(fn: (_: any) => void) {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void) {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean) {
    this.disabled = isDisabled;
  }
}
