import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { FieldType } from "../../enums/fieldType.enum";
import { InfluenceType } from "../../enums/influenceType.enum";
import { Menu } from "../../enums/menu.enum";
import { Mode } from "../../enums/mode.enum";
import { FieldConfig } from "../../models/field.interface";
import { Influence } from "../../models/influence.interface";
import { Table } from "../../models/table.interface";
import { FormDataService } from "../../services/form-data.service";
import { KeepFiltersService } from "../../services/keep-filters.service";

@Component({
  selector: "app-body",
  templateUrl: "./body.component.html",
  styleUrls: ["./body.component.scss"]
})
export class BodyComponent implements OnInit {
  active: boolean;
  filters: FieldConfig[];
  isLoading: boolean = false;
  filtersFormGroup: FormGroup;
  table: Table;
  tableFormGroup: FormGroup;
  keyWord: string = "";

  get modes() {
    return Mode;
  }

  constructor(
    private route: ActivatedRoute,
    private formDataService: FormDataService,
    private fb: FormBuilder,
    private keepFiltersService: KeepFiltersService
  ) {}

  ngOnInit() {
    const url = this.route.parent.snapshot.url[0].path;
    this.active = url === Menu.Active;

    this.isLoading = true;

    this.filtersFormGroup = this.fb.group({}, { validators: [this.atLeastOneChosen] });

    if (!this.isNull(this.keepFiltersService.filters) || this.keepFiltersService.keyWord) {
      this.initFilters(this.keepFiltersService.filters);
      this.tableFormGroup = this.fb.group({});
      this.table = this.keepFiltersService.table;
      this.onInput(this.keepFiltersService.keyWord);
      this.isLoading = false;
    } else {
      this.formDataService.getFilters().subscribe(res => {
        this.initFilters(res);
        this.isLoading = false;
      });
    }
  }

  private initFilters(filters: FieldConfig[]) {
    this.filters = this.formDataService.deepCopyArr(filters);

    this.filters.forEach(field => {
      const control = this.fb.control(field.value || null);
      this.filtersFormGroup.addControl(field.name, control);
    });
  }

  onSearch() {
    this.isLoading = true;

    const tableData = this.table ? this.formDataService.deepCopyObj(this.table) : null;
    const filterValues = {};
    for (const key in this.filtersFormGroup.value) {
      if (this.filtersFormGroup.value[key]) {
        const val = this.getFilterValue(this.filtersFormGroup.value[key]);
        val && (filterValues[key] = val);
      }
    }

    this.formDataService.getFilteredData(this.active ? 1 : 0, filterValues, this.keyWord, tableData).subscribe(res => {
      this.isLoading = false;
      this.table = res.sections[0].table;
      this.tableFormGroup = this.fb.group({});
    });

    this.keepFiltersService.keyWord = this.keyWord;
    this.keepFiltersService.filters = this.filters ? this.formDataService.deepCopyArr(this.filters) : null;

    this.keepFiltersService.filters.forEach(filter => {
      const control = this.filtersFormGroup.get(filter.name);
      filter.value = control.value;
    });
  }

  onValueChange(event: { value: string; influence: Influence; filterValue: string }) {
    if (event.influence && event.influence.type === InfluenceType.Options) {
      const influencedField = this.filters.find(field => field.name === event.influence.fieldName);
      const formControl = this.filtersFormGroup.get(event.influence.fieldName);

      if (!influencedField || !formControl) {
        return;
      }

      formControl.setValue(null);
      influencedField.value = null;

      if (!event.influence.url || !event.filterValue) {
        influencedField.options = [];
        this.formDataService.optionsChanged.next(influencedField.name);
        this.loopInfluences(influencedField);
      } else {
        this.formDataService
          .getDynamicNomenclature(event.influence.url, { filterValue: event.filterValue })
          .subscribe(res => {
            influencedField.options = res;
            this.formDataService.optionsChanged.next(influencedField.name);

            if (influencedField.type !== FieldType.SearchMultiSelect) {
              influencedField.options.unshift({ label: "", code: null });
            }

            this.loopInfluences(influencedField);
          });
      }
    }
  }

  private loopInfluences(influencedField) {
    if (influencedField.influence) {
      for (const influenceRecord of influencedField.influence) {
        this.onValueChange({ value: null, influence: { ...influenceRecord }, filterValue: null });
      }
    }
  }

  private getFilterValue(eventValue) {
    if (eventValue && typeof eventValue === "object" && eventValue.length === undefined) {
      eventValue = eventValue.code;
    } else if (eventValue && typeof eventValue === "object" && eventValue.length >= 0) {
      const finalVal = [];
      for (const val of eventValue) {
        finalVal.push(val.code);
      }
      eventValue = finalVal.length > 0 ? finalVal : null;
    }

    return eventValue;
  }

  atLeastOneChosen(form: FormGroup): { [s: string]: boolean } {
    let noneChosen = true;
    for (const key in form.controls) {
      const val = form.controls[key].value;

      if ((val && typeof val !== "object") || (val && (val.code || val.length > 0))) {
        noneChosen = false;
        break;
      }
    }

    return noneChosen ? { noneChosen: true } : null;
  }

  onInput(value) {
    this.keyWord = value;
    for (const key in this.filtersFormGroup.controls) {
      !!this.keyWord ? this.filtersFormGroup.controls[key].disable() : this.filtersFormGroup.controls[key].enable();
    }
  }

  clear() {
    this.keyWord = "";
    for (const key in this.filtersFormGroup.controls) {
      this.filtersFormGroup.controls[key].enable();
      this.filtersFormGroup.controls[key].setValue(null);
    }
  }

  private isNull(filters: FieldConfig[]) {
    if (!filters || filters.length === 0) {
      return true;
    }

    let isNull = true;

    for (const filter of filters) {
      if (filter.value) {
        isNull = false;
        break;
      }
    }

    return isNull;
  }
}
