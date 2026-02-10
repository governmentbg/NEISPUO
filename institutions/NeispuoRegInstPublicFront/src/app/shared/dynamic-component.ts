import { EventEmitter } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { InfluenceType } from "../enums/influenceType.enum";
import { Mode } from "../enums/mode.enum";
import { FieldConfig } from "../models/field.interface";
import { Influence } from "../models/influence.interface";

export class DynamicComponent {
  valueChange: EventEmitter<{ value: string; influence: Influence; filterValue: string }> = new EventEmitter();
  field: FieldConfig;
  group: FormGroup;
  mode: Mode;
  showLabel: boolean;

  get modes() {
    return Mode;
  }

  get required() {
    return this.field.validations && !!this.field.validations.find(validation => validation.name === "required");
  }

  onSelectionChange(event) {
    let influence: Influence = null;
    let filterValue: string = this.getFilterValue(event.value);

    if (this.field.influence) {
      for (const influenceRecord of this.field.influence) {
        if (influenceRecord.type === InfluenceType.Options) {
          let url: string = filterValue ? influenceRecord.url : null;
          influence = { fieldName: influenceRecord.fieldName, url, type: InfluenceType.Options };
          this.valueChange.emit({ value: event.value, influence, filterValue });
        } else {
          influence = { ...influenceRecord };
          this.valueChange.emit({ value: event.value, influence, filterValue: null });
        }
      }
    }
  }

  private getFilterValue(eventValue) {
    if (eventValue && typeof eventValue === "object" && eventValue.length >= 0) {
      const finalVal = [];
      for (const val of eventValue) {
        finalVal.push(val.code);
      }
      eventValue = finalVal.length > 0 ? finalVal : null;
    }

    return eventValue;
  }
}
