import { Directive, HostListener, Inject, Optional, Self } from '@angular/core';
import { NgControl } from '@angular/forms';
import { isValid } from 'date-fns';
import { getFormControl } from 'projects/shared/utils/directive';

@Directive({
  selector: '[sbClearInvalidDate]'
})
export class ClearInvalidDateDirective {
  constructor(@Optional() @Self() @Inject(NgControl) private ngControl: NgControl) {}

  @HostListener('blur')
  onBlur() {
    const formControl = getFormControl(this.ngControl);

    if (!formControl) return;

    if (!isValid(formControl.value)) {
      formControl.setValue(null);
    }
  }
}
