import { Component, Injector, Input } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { BaseField } from '../base-field';

@Component({
  selector: 'sb-textarea-field',
  templateUrl: './textarea-field.component.html',
  // eslint-disable-next-line @angular-eslint/no-inputs-metadata-property
  inputs: ['label', 'placeholder', 'validations', 'readonly', 'hint'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: TextareaFieldComponent,
      multi: true
    }
  ]
})
export class TextareaFieldComponent extends BaseField {
  @Input() textareaMaxSize = 1000;

  @Input() minRows = 1;
  @Input() maxRows = this.textareaMaxSize;

  constructor(injector: Injector) {
    super(injector);
  }
}
