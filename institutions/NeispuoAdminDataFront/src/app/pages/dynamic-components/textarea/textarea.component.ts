import { Component, OnInit } from "@angular/core";
import { InfluenceType } from "../../../enums/influenceType.enum";
import { DynamicComponent } from "../../../shared/dynamic-component";

@Component({
  selector: "app-textarea",
  templateUrl: "./textarea.component.html",
  styleUrls: ["./textarea.component.scss"]
})
export class TextareaComponent extends DynamicComponent implements OnInit {
  constructor() {
    super();
  }

  ngOnInit(): void {}

  // for require influence
  onTextChanged(event) {
    if (
      (event.target.value.length === 0 || event.target.value.length === 1) &&
      this.field.influence &&
      this.field.influence.find(infl => infl.type === InfluenceType.Require)
    ) {
      this.onSelectionChange({ value: event.target.value });
    }
  }
}
