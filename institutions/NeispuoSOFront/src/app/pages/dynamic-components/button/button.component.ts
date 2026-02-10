import { Component, OnInit } from "@angular/core";
import { DynamicComponent } from "src/app/shared/dynamic-component";

@Component({
  selector: "app-button",
  templateUrl: "./button.component.html",
  styleUrls: ["./button.component.scss"]
})
export class ButtonComponent extends DynamicComponent implements OnInit {
  constructor() {
    super();
  }

  ngOnInit() {}

  performProc() {
    if (this.field.canSign) {
      this.performProcedure.emit({
        procName: this.field.procName,
        procParams: this.field.procParams,
        canSign: this.field.canSign,
        searchByEik: this.field.searchByEik,
        requiredFields: this.field.requiredFields,
        groupValues: this.group.getRawValue()
      });
    } else if (this.field.procName) {
      this.performProcedure.emit({
        procName: this.field.procName,
        procParams: this.field.procParams,
        requiredFields: this.field.requiredFields,
        groupValues: this.group.getRawValue(),
        generateCertificate: this.field.generateCertificate,
        button: this.field
      });
    }
  }

  getLabel() {
    if (this.field.generateCertificate) {
      return this.field.value ? "Изтегляне" : "Генериране";
    }
    return this.field.label;
  }
}
