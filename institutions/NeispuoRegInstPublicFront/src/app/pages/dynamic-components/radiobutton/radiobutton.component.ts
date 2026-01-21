import { AfterViewChecked, Component, OnInit, ViewChildren } from "@angular/core";
import { MatCheckbox } from "@angular/material/checkbox";
import { DynamicComponent } from "../../../shared/dynamic-component";

@Component({
  selector: "app-radiobutton",
  templateUrl: "./radiobutton.component.html",
  styleUrls: ["./radiobutton.component.scss"]
})
export class RadiobuttonComponent extends DynamicComponent implements OnInit, AfterViewChecked {
  checkedCode = "";

  @ViewChildren(MatCheckbox) items: MatCheckbox[];

  constructor() {
    super();
  }

  ngOnInit() {
    this.checkedCode = this.field.value;
    if (this.field.value && this.field.influence) super.onSelectionChange({ value: this.field.value });
  }

  ngAfterViewChecked() {
    // for some reason attr.disabled doesn't work
    if(this.mode === this.modes.View) {
      this.items && this.items.forEach(item => item.disabled = true)
    }
  }

  onSelectionChange(code) {
    let current_item: MatCheckbox = null;
    this.items.forEach((item: MatCheckbox) => {
      if (item.value !== code) {
        item.checked = false;
      } else {
        current_item = item;
      }
    });

    if (!current_item) {
      return;
    }

    let patch: any = {};
    let value = null;
    if (current_item.checked) {
      value = current_item.value;
      patch[this.field.name] = value;
    } else {
      patch[this.field.name] = null;
    }

    this.group.patchValue(patch);
    super.onSelectionChange({ value });
  }
}
