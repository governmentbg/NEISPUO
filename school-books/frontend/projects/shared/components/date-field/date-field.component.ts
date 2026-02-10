import { Component, Injector } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { faCalendarDay as fadCalendarDay } from '@fortawesome/pro-duotone-svg-icons/faCalendarDay';
import { BaseField } from '../base-field';

@Component({
  selector: 'sb-date-field',
  templateUrl: './date-field.component.html',
  styleUrls: ['./date-field.component.scss'],
  // eslint-disable-next-line @angular-eslint/no-inputs-metadata-property
  inputs: ['label', 'placeholder', 'validations', 'readonly', 'hint'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: DateFieldComponent,
      multi: true
    }
  ]
})
export class DateFieldComponent extends BaseField {
  readonly fadCalendarDay = fadCalendarDay;

  constructor(injector: Injector) {
    super(injector);
  }
}
