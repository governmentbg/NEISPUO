import { Component, OnInit } from "@angular/core";
import { DynamicComponent } from "src/app/shared/dynamic-component";

@Component({
  selector: "app-breakpoint",
  templateUrl: "./breakpoint.component.html",
  styleUrls: ["./breakpoint.component.scss"]
})
export class BreakpointComponent extends DynamicComponent implements OnInit {
  constructor() {
    super();
  }

  ngOnInit(): void {}
}
