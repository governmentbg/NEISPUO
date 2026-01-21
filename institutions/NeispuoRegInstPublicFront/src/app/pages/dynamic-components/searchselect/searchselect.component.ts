import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { FormControl } from "@angular/forms";
import { MatSelect } from "@angular/material/select";
import { ReplaySubject, Subject, Subscription } from "rxjs";
import { take, takeUntil } from "rxjs/operators";
import { InfluenceType } from "../../../enums/influenceType.enum";
import { Option } from "../../../models/option.interface";
import { FormDataService } from "../../../services/form-data.service";
import { DynamicComponent } from "../../../shared/dynamic-component";

@Component({
  selector: "app-searchselect",
  templateUrl: "./searchselect.component.html",
  styleUrls: ["./searchselect.component.scss"]
})
export class SearchSelectComponent extends DynamicComponent implements OnInit, OnDestroy {
  optionLabel: string;

  filterCtrl: FormControl = new FormControl();
  filteredOptions: ReplaySubject<Option[]> = new ReplaySubject<Option[]>(1);

  protected _onDestroy = new Subject<void>();

  private optionsChangedSubscription: Subscription;

  @ViewChild("s", { static: false }) searchSelect: MatSelect;

  constructor(private formDataService: FormDataService) {
    super();
  }

  ngOnInit() {
    if (this.field.options) {
      const option = this.field.options.find(option => option.code === this.field.value);
      option && (this.optionLabel = option.label);
    } else {
      this.field.options = [];
    }

    this.filteredOptions.next(this.field.options.slice());

    // listen for search field value changes
    this.filterCtrl.valueChanges.pipe(takeUntil(this._onDestroy)).subscribe(() => {
      this.filterOptions();
    });

    this.optionsChangedSubscription = this.formDataService.optionsChanged.subscribe(fieldName => {
      if (fieldName === this.field.name) {
        this.filterOptions();
      }
    });
  }

  ngAfterViewInit() {
    this.setInitialValue();
  }

  ngOnDestroy() {
    this._onDestroy.next();
    this._onDestroy.complete();
    this.optionsChangedSubscription && this.optionsChangedSubscription.unsubscribe();
  }

  protected setInitialValue() {
    this.filteredOptions.pipe(take(1), takeUntil(this._onDestroy)).subscribe(() => {
      // setting the compareWith property to a comparison function
      // triggers initializing the selection according to the initial value of
      // the form control (i.e. _initializeSelection())
      // this needs to be done after the filteredOptions are loaded initially
      // and after the mat-option elements are available
      this.searchSelect && (this.searchSelect.compareWith = (a: Option, b: Option) => a && b && a.code === b.code);
    });
  }

  protected filterOptions() {
    if (!this.field.options) {
      return;
    }

    let search = this.filterCtrl.value;
    if (!search) {
      this.filteredOptions.next(this.field.options.slice());
      return;
    } else {
      search = search.toLowerCase();
    }

    this.filteredOptions.next(this.field.options.filter(option => option.label.toLowerCase().indexOf(search) > -1));
  }
}
