import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { QualitativeGrade } from 'projects/sb-api-client/src/model/qualitativeGrade';

export const QUALITATIVE_GRADE_PICKER_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => QualitativeGradePickerComponent),
  multi: true
};

@Component({
  selector: 'sb-qualitative-grade-picker',
  templateUrl: './qualitative-grade-picker.component.html',
  providers: [QUALITATIVE_GRADE_PICKER_VALUE_ACCESSOR]
})
export class QualitativeGradePickerComponent implements ControlValueAccessor {
  @Input() allowEmpty = true;
  @Input() showGradeName = false;

  disabled = false;
  qualitativeGrade: QualitativeGrade | null = null;

  QUALITATIVE_GRADE_POOR = QualitativeGrade.Poor;
  QUALITATIVE_GRADE_FAIR = QualitativeGrade.Fair;
  QUALITATIVE_GRADE_GOOD = QualitativeGrade.Good;
  QUALITATIVE_GRADE_VERY_GOOD = QualitativeGrade.VeryGood;
  QUALITATIVE_GRADE_EXCELLENT = QualitativeGrade.Excellent;

  private onChange = (_: any) => {
    // do nothing
  };
  private onTouched = () => {
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

  setQualitativeGrade(value: QualitativeGrade | null) {
    if (this.disabled) {
      return;
    }

    this.qualitativeGrade = value;

    this.onChange(this.qualitativeGrade);
    this.onTouched();
  }

  writeValue(value: any) {
    this.qualitativeGrade = value;
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
