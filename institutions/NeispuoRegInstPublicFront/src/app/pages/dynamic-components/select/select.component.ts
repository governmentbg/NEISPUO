import { Component, OnInit } from "@angular/core";
import { InfluenceType } from "../../../enums/influenceType.enum";
import { DynamicComponent } from "../../../shared/dynamic-component";

@Component({
  selector: "app-select",
  templateUrl: "./select.component.html",
  styleUrls: ["./select.component.scss"]
})
export class SelectComponent extends DynamicComponent implements OnInit {
  optionLabel: string;

  constructor() {
    super();
  }

  ngOnInit() {
    if (this.field.options) {
      const option = this.field.options.find(option => option.code === this.field.value);
      option && (this.optionLabel = option.label);
    }
  }
}
