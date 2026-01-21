import { Component, Injector, Input } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { BaseField } from '../base-field';

@Component({
  selector: 'sb-number-field',
  templateUrl: './number-field.component.html',
  // eslint-disable-next-line @angular-eslint/no-inputs-metadata-property
  inputs: ['label', 'placeholder', 'validations', 'readonly', 'hint'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: NumberFieldComponent,
      multi: true
    }
  ]
})
export class NumberFieldComponent extends BaseField {
  @Input() precision = 0;

  constructor(injector: Injector) {
    super(injector);
  }
}
