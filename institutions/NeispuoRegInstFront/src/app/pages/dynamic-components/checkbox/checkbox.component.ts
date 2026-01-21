import { Component, OnInit } from "@angular/core";
import { DynamicComponent } from "../../../shared/dynamic-component";

@Component({
  selector: "app-checkbox",
  templateUrl: "./checkbox.component.html",
  styleUrls: ["./checkbox.component.scss"]
})
export class CheckboxComponent extends DynamicComponent implements OnInit {
  constructor() {
    super();
  }

  ngOnInit() {
    // for some reason attr.disabled doesn't work
    if (this.mode === this.modes.View || this.field.disabled) {
      const control = this.group.get(this.field.name);
      control.disable();
    }
  }
}
