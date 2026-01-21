import { EventEmitter } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { InfluenceType } from "../enums/influenceType.enum";
import { Mode } from "../enums/mode.enum";
import { ValidatorType } from "../enums/validatorType.enum";
import { FieldConfig } from "../models/field.interface";
import { Influence } from "../models/influence.interface";

export class DynamicComponent {
  valueChange: EventEmitter<{ value: string; influence: Influence; filterValue: string }> = new EventEmitter();
  selectionChange: EventEmitter<{ value: string; fieldName: string }> = new EventEmitter();
  field: FieldConfig;
  group: FormGroup;
  mode: Mode;
  showLabel: boolean;

  get modes() {
    return Mode;
  }

  get required() {
    return !!this.field.validations.find(
      validation => validation.name === ValidatorType.Required || validation.name === ValidatorType.RequiredControl
    );
  }

  onSelectionChange(event) {
    let influence: Influence = null;

    if (this.field.influence) {
      for (const influenceRecord of this.field.influence) {
        if (influenceRecord.type === InfluenceType.Options) {
          let url: string = event.value ? influenceRecord.url : null;
          influence = { fieldName: influenceRecord.fieldName, url, type: InfluenceType.Options };
          this.valueChange.emit({ value: event.value, influence, filterValue: event.value });
        } else {
          influence = { ...influenceRecord };
          this.valueChange.emit({ value: event.value, influence, filterValue: null });
        }
      }
    }

    this.selectionChange.emit({ value: event.value, fieldName: event.fieldName });
  }
}
