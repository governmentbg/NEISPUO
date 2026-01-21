import { Component, Injector, Input } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { BaseField } from '../base-field';
import { INomVO } from '../nom-select/nom-service';

@Component({
  selector: 'sb-select-field',
  templateUrl: './select-field.component.html',
  // eslint-disable-next-line @angular-eslint/no-inputs-metadata-property
  inputs: ['label', 'placeholder', 'validations', 'hint'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: SelectFieldComponent,
      multi: true
    }
  ]
})
export class SelectFieldComponent extends BaseField {
  @Input() items: INomVO<any>[] = [];
  @Input() noDataMessage = '';

  constructor(injector: Injector) {
    super(injector);
  }
}
