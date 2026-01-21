import { Directive, ElementRef, forwardRef, Renderer2 } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

export const NUMBER_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => NumberDirective),
  multi: true
};

// This directive is a direct copy of angular's NumberValueAccessor directive
// https://github.com/angular/angular/blob/master/packages/forms/src/directives/number_value_accessor.ts
//
// The point of it is to allow applying it to <input type="text"> without changing the type to "number"

@Directive({
  selector: '[sbNumber]',
  // eslint-disable-next-line @angular-eslint/no-host-metadata-property
  host: { '(input)': 'onChange($event.target.value)', '(blur)': 'onTouched()' },
  providers: [NUMBER_VALUE_ACCESSOR]
})
export class NumberDirective implements ControlValueAccessor {
  onChange = (_: any) => {
    // do nothing
  };

  onTouched = () => {
    // do nothing
  };

  constructor(private _renderer: Renderer2, private _elementRef: ElementRef) {}

  writeValue(value: number): void {
    this._renderer.setProperty(this._elementRef.nativeElement, 'value', value);
  }

  registerOnChange(fn: (_: number | null) => void): void {
    this.onChange = (value) => {
      fn(value === '' ? null : parseFloat(value));
    };
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this._renderer.setProperty(this._elementRef.nativeElement, 'disabled', isDisabled);
  }
}
