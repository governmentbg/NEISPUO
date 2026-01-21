import { Component, OnInit } from "@angular/core";
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
}
