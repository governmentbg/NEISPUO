import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { NgOption } from "@ng-select/ng-select";
import { Menu } from "src/app/enums/menu.enum";
import { DynamicComponent } from "../../../shared/dynamic-component";

@Component({
  selector: "app-multiselect",
  templateUrl: "./multiselect.component.html",
  styleUrls: ["./multiselect.component.scss"]
})
export class MultiselectComponent extends DynamicComponent implements OnInit {
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
      this.field.options = this.field.options.filter(option => option.code);
      const option = this.field.options.find(option => option.code === this.field.value);
      option && (this.optionLabel = option.label);
      this.mode === this.modes.Edit &&
        (this.field.options = this.field.options.filter(field => field.isValid !== false));
    }
  }

  onSelectionChange(event: Array<any>) {
    let label = "";
    if (event) {
      for (const value of event) {
        value.label && (label += value.label + "; ");
      }
    }

    super.onSelectionChange({ value: this.group.get(this.field.name).value, label });
  }

  selectedItems(selectedItems: NgOption[]) {
    let tooltip = "";

    for (let i = 0; i < selectedItems.length; i++) {
      tooltip += i === selectedItems.length - 1 ? selectedItems[i].label : selectedItems[i].label + ", ";
    }

    return tooltip;
  }
}
