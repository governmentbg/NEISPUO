import { Component, OnDestroy, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { Subscription } from "rxjs";
import { Menu } from "src/app/enums/menu.enum";
import { HelperService } from "../../../services/helpers.service";
import { DynamicComponent } from "../../../shared/dynamic-component";

@Component({
  selector: "app-select",
  templateUrl: "./select.component.html",
  styleUrls: ["./select.component.scss"]
})
export class SelectComponent extends DynamicComponent implements OnInit, OnDestroy {
  optionLabel: string;
  appendClass: string;

  private regixValueChangedSubscription: Subscription;

  constructor(private router: Router, private helperService: HelperService) {
    super();
  }

  ngOnInit() {
    const url = this.router.url;
    this.appendClass = url.includes(Menu.Edit) || url.includes(Menu.Create) ? ".l-container-card" : ".l-container-card";
    this.optionLabel = this.field.value;

    if (this.field.options) {
      const option = this.field.options.find(option => option.code === this.field.value);
      option && (this.optionLabel = option.label);
      this.mode === this.modes.Edit && (this.field.options = this.field.options.filter(field => field.isValid !== false));
    }

    this.regixValueChangedSubscription = this.helperService.regixValueSet.subscribe(data => {
      if (data.fldName === this.field.name) {
        this.onSelectionChange({ value: data.value });
      }
    });
  }

  ngOnDestroy(): void {
    this.regixValueChangedSubscription && this.regixValueChangedSubscription.unsubscribe();
  }
}
