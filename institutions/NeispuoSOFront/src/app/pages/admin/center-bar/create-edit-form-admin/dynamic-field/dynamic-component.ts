import { EventEmitter } from "@angular/core";
import { FocusedElementType } from "src/app/enums/focusedElementType";
import { ValidatorType } from 'src/app/enums/validatorType.enum';
import { FieldConfig } from 'src/app/models/field.interface';
import { UpdateJsonService } from 'src/app/services/update-json.service';

export class DynamicComponentAdmin {
  fieldRemoved: EventEmitter<number> = new EventEmitter();
  field: FieldConfig;

  get required() {
    return (
      !!this.field.validations &&
      !!this.field.validations.find(validation => validation.name === ValidatorType.Required)
    );
  }

  constructor(public updateJsonService: UpdateJsonService) {}

  focus() {
    this.updateJsonService.focusedElements.push({
      index: this.field.idntfr,
      type: FocusedElementType.Field,
      element: this.field
    });
  }
}
