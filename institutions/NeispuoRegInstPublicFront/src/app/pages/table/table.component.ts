import { Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges, ViewChild } from "@angular/core";
import { FormArray, FormBuilder, FormGroup } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { FieldType } from "../../enums/fieldType.enum";
import { Menu } from "../../enums/menu.enum";
import { Mode } from "../../enums/mode.enum";
import { FieldConfig } from "../../models/field.interface";
import { Option } from "../../models/option.interface";
import { Table } from "../../models/table.interface";
import { FormDataService } from "../../services/form-data.service";
import { KeepFiltersService } from "../../services/keep-filters.service";
import { MatMultiSortTableDataSource } from "../../shared/multisort/mat-multi-sort-data-source";
import { MatMultiSort } from "../../shared/multisort/mat-multi-sort.directive";
import { TableData } from "../../shared/multisort/table-data";

@Component({
  selector: "app-table",
  templateUrl: "./table.component.html",
  styleUrls: ["./table.component.scss"],
})
export class TableComponent implements OnInit, OnChanges {
  @Input() table: Table;
  @Input() parentGroup: FormGroup;

  displayedColumns: string[] = [];
  displayedFilterColumns: string[] = [];
  tableData: TableData<FormGroup>;
  columns: { id: string; name: string }[] = [];

  filterGroup: FormGroup;
  isLoading = false;
  length: number = 0;
  path: string;

  @ViewChild(MatMultiSort) sort: MatMultiSort;

  private init = false;

  private array: FormArray;

  constructor(
    private fb: FormBuilder,
    private formDataService: FormDataService,
    private router: Router,
    private keepFiltersService: KeepFiltersService,
    private route: ActivatedRoute
  ) {}

  get modes() {
    return Mode;
  }

  get menu() {
    return Menu;
  }

  ngOnInit() {
    this.createColumns();
    this.array = this.createFormArray();
    this.length = this.array.controls.length;

    const parent = this.route.parent;
    const url = parent.snapshot.url.length > 0 ? parent.snapshot.url : parent.parent.snapshot.url;
    this.path = url[0].path;

    this.parentGroup.addControl(this.table.name, this.array);
    this.tableData = new TableData<FormGroup>(this.columns, {
      defaultSortParams: this.keepFiltersService.sortParams,
      defaultSortDirs: this.keepFiltersService.sortDirs,
      totalElements: this.array.controls.length,
    });

    this.tableData.nextObservable.subscribe(() => this.getData());
    this.tableData.sortObservable.subscribe(() => this.getData());
    this.tableData.previousObservable.subscribe(() => this.getData());
    this.tableData.sizeObservable.subscribe(() => this.getData());

    for (const key in this.filterGroup.controls) {
      this.filterGroup.controls[key].setValue(this.keepFiltersService.tableFilter[key]);
    }

    setTimeout(() => {
      this.tableData.dataSource = new MatMultiSortTableDataSource(this.sort, true);
      this.tableData.pageSize = 10;
      this.getData();
    }, 0);
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.table && this.init) {
      this.array = this.createFormArray();
      this.parentGroup.setControl(this.table.name, this.array);
      this.getData();
    } else if (changes.table && !this.init) {
      this.init = true;
    }
  }

  private createColumns() {
    this.displayedColumns = [];
    this.table.fields.forEach(field => {
      this.columns.push({ id: field.name, name: field.label });
      this.displayedColumns.push(field.name);
      this.displayedFilterColumns.push(field.name + "filter");
    });

    this.table.hasEditButton !== false && this.columns.push({ id: "actions", name: "Действия" });
    this.table.hasEditButton !== false && this.displayedColumns.push("actions");
    this.table.hasEditButton !== false && this.displayedFilterColumns.push("actions-filter");
  }

  createFormArray() {
    this.filterGroup = this.fb.group({});
    this.table.fields.forEach(field => {
      const control = this.fb.control(null);
      this.filterGroup.addControl(field.name, control);
    });

    const array = this.fb.array([]);
    if (this.table && this.table.values) {
      for (let i = 0; i < this.table.values.length; i++) {
        this.addTableRow(this.table.values[i], array, i);
      }
    }
    return array;
  }

  trackByCellId(index, row: FormGroup) {
    return row ? Object.keys(row.controls)[0] : index;
  }

  getField(row: FormGroup, fieldName: string) {
    const key = Object.keys(row.controls)[0];
    const valuesRow = key ? this.table.values.find(value => value.id + "" === key) : { fields: [] };
    return valuesRow.fields.find(field => field.name === fieldName);
  }

  getFormGroup(row: FormGroup) {
    const key = Object.keys(row.controls)[0];
    return row ? row.get(key) : null;
  }

  onSortEvent(fieldName: string) {
    this.sort.sort({ id: fieldName, start: "asc", disableClear: false });
    this.tableData.onSortEvent();
  }

  private applyFilter() {
    let allNull = true;
    Object.keys(this.filterGroup.value).forEach(key => (allNull = allNull && !this.filterGroup.value[key]));

    if (allNull) {
      return this.array.controls.slice();
    } else {
      const newControls = [];

      this.array.controls.forEach((formGroup: FormGroup) => {
        let fulfilsAllFilters = true;
        const innerFormGroupKey = Object.keys(formGroup.controls)[0];
        const innerGroup = <FormGroup>formGroup.controls[innerFormGroupKey];

        for (let innerKey in innerGroup.controls) {
          const field = this.getField(formGroup, innerKey);
          if (field) {
            const fieldValue =
              field.type === FieldType.Select ? this.getOptionLabel(field.value, field.options) : field.value + "";

            const text = this.filterGroup.controls[field.name].value;
            const textArr = text ? text.split(" ") : [];
            let includes = false;
            textArr.forEach(text => {
              text && (includes = includes || fieldValue.toLowerCase().indexOf(text.toLowerCase()) >= 0);
            });

            text && (fulfilsAllFilters = fulfilsAllFilters && includes);
          }
        }

        if (fulfilsAllFilters) {
          newControls.push(formGroup);
        }
      });

      return newControls;
    }
  }

  private addTableRow(row: { id: string; fields: FieldConfig[] }, array: FormArray, position: number) {
    const idGroup = this.fb.group({});
    const rowGroup = this.fb.group({});
    row.fields.forEach(field => this.formDataService.addControl(field, rowGroup));
    idGroup.addControl(row.id + "", rowGroup); // just in case id is not a string
    array.insert(position, idGroup);
  }

  getData(goToBeginning = false) {
    this.isLoading = true;
    goToBeginning && (this.tableData.pageIndex = 0);

    const page = this.tableData.pageIndex || 0;
    const sorting = this.tableData.sortParams;
    const pagesize = this.tableData.pageSize;
    const dirs = this.tableData.sortDirs;

    const tempData = this.applyFilter();
    this.length = tempData.length;

    if (sorting.length === 0) {
      this.tableData.data = tempData.slice(page * pagesize, (page + 1) * pagesize);
    } else if (sorting.length > 0) {
      const sortedData = tempData.sort((u1, u2) => {
        return this.sortData(u1, u2, sorting, dirs);
      });
      this.tableData.data = sortedData.slice(page * pagesize, (page + 1) * pagesize);
    }

    this.isLoading = false;
  }

  private sortData(d1: FormGroup, d2: FormGroup, sorting: string[], dirs: string[]): number {
    const key1 = Object.keys(d1.controls)[0];
    const key2 = Object.keys(d2.controls)[0];

    const firstControl = (<FormGroup>d1.controls[key1]).controls[sorting[0]];
    const secondConrol = (<FormGroup>d2.controls[key2]).controls[sorting[0]];

    const firstField = this.table.values
      .find(value => value.id + "" === key1)
      .fields.find(field => field.name === sorting[0]);
    const secondField = this.table.values
      .find(value => value.id + "" === key2)
      .fields.find(field => field.name === sorting[0]);

    let first =
      firstField.type === FieldType.Select
        ? this.getOptionLabel(firstField.value, firstField.options)
        : firstControl.value;

    let second =
      secondField.type === FieldType.Select
        ? this.getOptionLabel(secondField.value, secondField.options)
        : secondConrol.value;

    first && typeof first === "string" && (first = first.toLowerCase());
    second && typeof second === "string" && (second = second.toLowerCase());

    if (first > second) {
      return dirs[0] === "asc" ? 1 : -1;
    } else if (first < second || first === null) {
      return dirs[0] === "asc" ? -1 : 1;
    } else {
      if (sorting.length > 1) {
        sorting = sorting.slice(1, sorting.length);
        dirs = dirs.slice(1, dirs.length);
        return this.sortData(d1, d2, sorting, dirs);
      } else {
        return 0;
      }
    }
  }

  private getOptionLabel(code: number, options: Option[]) {
    const option = options.find(option => option.code === code);
    return option ? option.label : null;
  }

  navigate(dtl: FormGroup) {
    const key = Object.keys(dtl.controls)[0];
    const row = this.table.values.find(val => val.id == key);

    this.keepFiltersService.tableFilter = this.filterGroup.value;
    this.keepFiltersService.sortDirs = this.tableData.sortDirs;
    this.keepFiltersService.sortParams = this.tableData.sortParams;

    this.router.navigate([`/${Menu.Preview}/${row.formName}/${row.instid}`], {
      queryParams: { procID: row.procID, instKind: row.instKind },
    });
  }

  onSort(fieldName: string) {
    if (this.table.sortable !== false) {
      this.onSortEvent(fieldName);
    }
  }
}
