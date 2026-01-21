import { Component, Input, OnInit } from "@angular/core";
import { FieldType } from "src/app/enums/fieldType.enum";
import { FocusedElementType } from "src/app/enums/focusedElementType";
import { FieldConfig } from "src/app/models/field.interface";
import { Table } from "src/app/models/table.interface";
import { UpdateJsonService } from "src/app/services/update-json.service";

@Component({
  selector: "app-table-admin",
  templateUrl: "./table-admin.component.html",
  styleUrls: ["./table-admin.component.scss"]
})
export class TableAdminComponent implements OnInit {
  @Input() table: Table;

  length: number = 0;
  currentDate = new Date();

  constructor(public updateJsonService: UpdateJsonService) {}

  get type() {
    return FieldType;
  }

  ngOnInit() {}

  focus() {
    this.updateJsonService.focusedElements.push({
      index: this.table.idntfr,
      type: FocusedElementType.Table,
      element: this.table
    });
  }

  focusField(field: FieldConfig) {
    this.updateJsonService.focusedElements.push({
      index: field.idntfr,
      type: FocusedElementType.Column,
      element: field
    });
  }

  calcColumnWidth(fieldWidth: number) {
    let count = 0;
    let sum = 0;

    for (const field of this.table.fields) {
      if (field.width > 0) {
        sum += field.width;
        count++;
      }
    }

    const coef = 100 / sum;

    if (count === this.table.fields.length) {
      return fieldWidth * coef;
    }

    if (fieldWidth > 0) {
      return fieldWidth;
    }

    return (100 - sum) / (this.table.fields.length - count);
  }
}
