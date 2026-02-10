import { Component, OnInit } from "@angular/core";
import { DynamicComponent } from "src/app/shared/dynamic-component";

@Component({
  selector: "app-date-time",
  templateUrl: "./date-time.component.html",
  styleUrls: ["./date-time.component.scss"]
})
export class DateTimeComponent extends DynamicComponent implements OnInit {
  constructor() {
    super();
  }

  ngOnInit() {
    const currentValue = this.field.value;
    if (currentValue) {
      const actualValue = currentValue.replace("T", " ") + "ч.";
      this.field.value = actualValue;
      this.group.get(this.field.name).setValue(actualValue);
    }
  }
}
