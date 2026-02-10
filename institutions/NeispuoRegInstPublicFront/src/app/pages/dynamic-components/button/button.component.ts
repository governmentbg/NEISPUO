import { Component, OnInit } from "@angular/core";
import { DynamicComponent } from "../../../shared/dynamic-component";

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
}
