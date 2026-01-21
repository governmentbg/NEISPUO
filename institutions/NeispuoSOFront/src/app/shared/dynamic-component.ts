import { EventEmitter } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { InfluenceType } from "../enums/influenceType.enum";
import { Mode } from "../enums/mode.enum";
import { ValidatorType } from "../enums/validatorType.enum";
import { FieldConfig, RegixData } from "../models/field.interface";
import { Influence } from "../models/influence.interface";

export class DynamicComponent {
  valueChange: EventEmitter<{
    value: string;
    influence: Influence;
    filterValue: string;
    label?: string;
    fieldName?: string;
  }> = new EventEmitter();

  filterTable: EventEmitter<string[]> = new EventEmitter();

  performProcedure: EventEmitter<{
    procName: string;
    procParams: Object;
    canSign?: boolean;
    searchByEik?: boolean;
    requiredFields?: string[];
    groupValues: Object;
    generateCertificate?: boolean;
    button?: FieldConfig;
  }> = new EventEmitter<{
    procName: string;
    procParams: Object;
    canSign?: boolean;
    searchByEik?: boolean;
    requiredFields?: string[];
    groupValues: Object;
    generateCertificate?: boolean;
    button?: FieldConfig;
  }>();

  performRegixProc: EventEmitter<{ regixData: RegixData; groupValues: any }> = new EventEmitter<{
    regixData: RegixData;
    groupValues: any;
  }>();

  field: FieldConfig;
  group: FormGroup;
  mode: Mode;
  showLabel: boolean;

  get modes() {
    return Mode;
  }

  get required() {
    return (
      !!this.field.validations &&
      !!this.field.validations.find(
        validation => validation.name === ValidatorType.Required || validation.name === ValidatorType.RequiredControl
      )
    );
  }

  onSelectionChange(event) {
    let influence: Influence = null;

    if (this.field.influence) {
      for (const influenceRecord of this.field.influence) {
        if (influenceRecord.type === InfluenceType.Options || influenceRecord.type === InfluenceType.SetValue) {
          let url: string = event.value ? influenceRecord.url : null;
          influence = { fieldName: influenceRecord.fieldName, url, type: influenceRecord.type };
          this.valueChange.emit({
            value: event.value,
            influence,
            filterValue: event.value,
            fieldName: this.field.name
          });
        } else {
          influence = { ...influenceRecord };
          this.valueChange.emit({
            value: event.value,
            influence,
            filterValue: null,
            label: event.label,
            fieldName: this.field.name
          });
        }
      }
    }

    if (this.field.dependingTables?.length) {
      this.filterTable.emit(this.field.dependingTables);
    }
  }
}
