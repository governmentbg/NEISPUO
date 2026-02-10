import { Component, Injector, Input } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { BaseField } from '../base-field';

@Component({
  selector: 'sb-text-field',
  templateUrl: './text-field.component.html',
  // eslint-disable-next-line @angular-eslint/no-inputs-metadata-property
  inputs: ['label', 'placeholder', 'validations', 'readonly', 'hint'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: TextFieldComponent,
      multi: true
    }
  ]
})
export class TextFieldComponent extends BaseField {
  @Input() textMaxSize = 100;

  constructor(injector: Injector) {
    super(injector);
  }
}
