import { Component, OnInit } from "@angular/core";
import { DynamicComponent } from "src/app/shared/dynamic-component";

@Component({
  selector: "app-label",
  templateUrl: "./label.component.html",
  styleUrls: ["./label.component.scss"]
})
export class LabelComponent extends DynamicComponent implements OnInit {
  style: any = {};

  constructor() {
    super();
  }

  ngOnInit(): void {
    this.style = {
      "font-weight": this.field.bold ? "bold" : "normal",
      "font-size": this.field.size ? this.field.size + "px" : "inherit"
    };

    this.field.color && (this.style.color = this.field.color);
  }
}
