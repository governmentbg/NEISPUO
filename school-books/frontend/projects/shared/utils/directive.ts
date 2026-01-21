import {
  FormControlDirective,
  FormControlName,
  FormGroupDirective,
  NgControl,
  UntypedFormControl
} from '@angular/forms';

export function getFormControl(ngControl: NgControl): UntypedFormControl | null {
  if (ngControl instanceof FormControlName && ngControl.formDirective) {
    return (ngControl.formDirective as FormGroupDirective).form.get(ngControl.path) as UntypedFormControl;
  } else if (ngControl instanceof FormControlDirective) {
    return ngControl.control;
  }
  return null;
}

export function trackByIndex(index: number, _item: unknown) {
  return index;
}
