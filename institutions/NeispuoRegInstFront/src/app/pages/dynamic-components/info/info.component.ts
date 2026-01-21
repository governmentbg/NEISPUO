import { Component, OnInit } from "@angular/core";
import { DynamicComponent } from "src/app/shared/dynamic-component";

@Component({
  selector: "app-info",
  templateUrl: "./info.component.html",
  styleUrls: ["./info.component.scss"]
})
export class InfoComponent extends DynamicComponent implements OnInit {
  defaultValue: any;

  constructor() {
    super();
  }

  ngOnInit() {}
}
