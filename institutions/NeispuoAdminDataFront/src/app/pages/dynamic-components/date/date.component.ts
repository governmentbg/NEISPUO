import { Component, OnInit } from "@angular/core";
import { InfluenceType } from "../../../enums/influenceType.enum";
import { DynamicComponent } from "../../../shared/dynamic-component";

@Component({
  selector: "app-date",
  templateUrl: "./date.component.html",
  styleUrls: ["./date.component.scss"]
})
export class DateComponent extends DynamicComponent implements OnInit {
  private prev = false;
  constructor() {
    super();
  }

  ngOnInit() {}

  onDateChanged(event) {
    if (
      !!event.target.value !== this.prev &&
      this.field.influence &&
      this.field.influence.length
    ) {
      this.prev = !!event.target.value;
      this.onSelectionChange({ value: event.target.value });
    }
  }
}
