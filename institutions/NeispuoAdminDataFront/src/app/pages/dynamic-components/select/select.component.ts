import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { Menu } from "src/app/enums/menu.enum";
import { DynamicComponent } from "../../../shared/dynamic-component";

@Component({
  selector: "app-select",
  templateUrl: "./select.component.html",
  styleUrls: ["./select.component.scss"]
})
export class SelectComponent extends DynamicComponent implements OnInit {
  optionLabel: string;
  appendClass: string;

  constructor(private router: Router) {
    super();
  }

  ngOnInit() {
    const url = this.router.url;
    this.appendClass =
      url.includes(Menu.Edit) || url.includes(Menu.Create) ? ".l-container-card" : ".section-class-vscroll";
    this.optionLabel = this.field.value;

    if (this.field.options) {
      const option = this.field.options.find(option => option.code === this.field.value);
      option && (this.optionLabel = option.label);
      this.mode === this.modes.Edit &&
        (this.field.options = this.field.options.filter(field => field.isValid !== false));
    }
  }
}
