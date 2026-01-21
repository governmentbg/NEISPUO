import { Component, OnInit } from "@angular/core";
import { Mode } from "src/app/enums/mode.enum";
import { DynamicComponent } from "src/app/shared/dynamic-component";

@Component({
  selector: "app-file",
  templateUrl: "./file.component.html",
  styleUrls: ["./file.component.scss"]
})
export class FileComponent extends DynamicComponent implements OnInit {
  constructor() {
    super();
  }

  get modes() {
    return Mode;
  }

  ngOnInit() {}
}
