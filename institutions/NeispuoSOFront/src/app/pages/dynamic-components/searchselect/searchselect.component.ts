import { AfterViewInit } from "@angular/core";
import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { NgSelectComponent } from "@ng-select/ng-select";
import { Subscription } from "rxjs";
import { Menu } from "src/app/enums/menu.enum";
import { HelperService } from "src/app/services/helpers.service";
import { DynamicComponent } from "../../../shared/dynamic-component";

@Component({
  selector: "app-searchselect",
  templateUrl: "./searchselect.component.html",
  styleUrls: ["./searchselect.component.scss"]
})
export class SearchSelectComponent extends DynamicComponent implements OnInit, OnDestroy, AfterViewInit {
  optionLabel: string;
  appendClass: string;

  @ViewChild("s", { static: false }) select: NgSelectComponent;

  private optionsChangedSubscription: Subscription;
  private regixValueChangedSubscription: Subscription;

  constructor(private helperService: HelperService, private router: Router) {
    super();
  }

  ngOnInit() {
    const url = this.router.url;
    this.appendClass =
      url.includes(Menu.Edit) || url.includes(Menu.Create) ? ".l-container-card" : ".l-container-card";
    this.optionLabel = this.field.value;

    if (this.field.options) {
      const option = this.field.options.find(option => option.code === this.field.value);
      option && (this.optionLabel = option.label);
      this.mode === this.modes.Edit &&
        (this.field.options = this.field.options.filter(field => field.isValid !== false));
    }

    this.optionsChangedSubscription = this.helperService.optionsChanged.subscribe(fieldName => {
      if (fieldName === this.field.name && this.select) {
        this.select.setDisabledState(false);
      }
    });

    this.regixValueChangedSubscription = this.helperService.regixValueSet.subscribe(data => {
      if (data.fldName === this.field.name) {
        this.onSelectionChange({ value: data.value });
      }
    });
  }

  ngAfterViewInit() {
    if (
      (!this.field.options || !this.field.options.length) &&
      this.select &&
      this.field.influencedBy &&
      this.field.influencedBy.length > 0
    ) {
      this.select.setDisabledState(true);
    }
  }

  ngOnDestroy() {
    this.optionsChangedSubscription && this.optionsChangedSubscription.unsubscribe();
    this.regixValueChangedSubscription && this.regixValueChangedSubscription.unsubscribe();
  }
}
