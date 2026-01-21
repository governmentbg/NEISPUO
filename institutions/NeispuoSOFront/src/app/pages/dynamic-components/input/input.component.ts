import { Component, OnInit } from "@angular/core";
import { InfluenceType } from "../../../enums/influenceType.enum";
import { DynamicComponent } from "../../../shared/dynamic-component";

@Component({
  selector: "app-input",
  templateUrl: "./input.component.html",
  styleUrls: ["input.component.scss"]
})
export class InputComponent extends DynamicComponent implements OnInit {
  constructor() {
    super();
  }

  ngOnInit() {}

  // for require influence
  onInputChanged(event) {
    if (
      (event.target.value.length === 0 || event.target.value.length === 1) &&
      this.field.influence &&
      this.field.influence.length
    ) {
      this.onSelectionChange({ value: event.target.value });
    }
  }
}
