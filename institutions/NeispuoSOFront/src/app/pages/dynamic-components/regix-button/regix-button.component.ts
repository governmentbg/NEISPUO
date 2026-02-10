import { Component, OnInit } from "@angular/core";
import { DynamicComponent } from "../../../shared/dynamic-component";

@Component({
  selector: "app-regix-button",
  templateUrl: "./regix-button.component.html",
  styleUrls: ["./regix-button.component.scss"]
})
export class RegixButtonComponent extends DynamicComponent implements OnInit {
  constructor() {
    super();
  }

  ngOnInit() {}

  onPerformRegixProc() {
    this.performRegixProc.emit({
      regixData: this.field.regixData,
      groupValues: this.group.getRawValue()
    });
  }
}
