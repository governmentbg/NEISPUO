import { AfterViewInit } from "@angular/core";
import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { NgSelectComponent } from "@ng-select/ng-select";
import { Subscription } from "rxjs";
import { FormDataService } from "../../../services/form-data.service";
import { DynamicComponent } from "../../../shared/dynamic-component";

@Component({
  selector: "app-searchselect",
  templateUrl: "./searchselect.component.html",
  styleUrls: ["./searchselect.component.scss"],
})
export class SearchSelectComponent extends DynamicComponent implements OnInit, OnDestroy, AfterViewInit {
  optionLabel: string;

  @ViewChild("s", { static: false }) select: NgSelectComponent;

  private optionsChangedSubscription: Subscription;

  constructor(private formDataService: FormDataService) {
    super();
  }

  ngOnInit() {
    if (this.field.options) {
      const option = this.field.options.find(option => option.code === this.field.value);
      option && (this.optionLabel = option.label);
      this.mode === this.modes.Edit &&
        (this.field.options = this.field.options.filter(field => field.isValid !== false));
    }

    this.optionsChangedSubscription = this.formDataService.optionsChanged.subscribe(fieldName => {
      if (fieldName === this.field.name && (!this.field.options || !this.field.options.length) && this.select) {
        this.select.setDisabledState(true);
      } else if (fieldName === this.field.name && this.select) {
        this.select.setDisabledState(false);
      }
    });
  }

  ngAfterViewInit() {
    if ((!this.field.options || !this.field.options.length) && this.select) {
      this.select.setDisabledState(true);
    }
  }

  ngOnDestroy() {
    this.optionsChangedSubscription && this.optionsChangedSubscription.unsubscribe();
  }
}
