import { Component, OnInit } from "@angular/core";
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
}
