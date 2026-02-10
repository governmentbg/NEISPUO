import { Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from "@angular/core";
import { FormArray, FormBuilder, FormGroup } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { ActivatedRoute, Router } from "@angular/router";
import { Subscription } from "rxjs";
import { AuthService } from "../../auth/auth.service";
import { MessagesService } from "../../services/messages.service";
import { SnackbarService } from "../../services/snackbar.service";
import { ModalComponent } from "../../shared/modal/modal.component";
import { FieldType } from "../../enums/fieldType.enum";
import { Menu } from "../../enums/menu.enum";
import { Mode, ModeInt } from "../../enums/mode.enum";
import { FieldConfig } from "../../models/field.interface";
import { Row, Table } from "../../models/table.interface";
import { FormDataService } from "../../services/form-data.service";
import { MatMultiSortTableDataSource } from "../../shared/multisort/mat-multi-sort-data-source";
import { MatMultiSort } from "../../shared/multisort/mat-multi-sort.directive";
import { TableData } from "../../shared/multisort/table-data";
import { HelperService } from "src/app/services/helpers.service";
import { environment } from "../../../environments/environment";

@Component({
  selector: "app-table",
  templateUrl: "./table.component.html",
  styleUrls: ["./table.component.scss"]
})
export class TableComponent implements OnInit, OnDestroy {
  @Input() table: Table;
  @Input() mode: Mode;
  @Input() parentGroup: FormGroup;

  @Output() addRecordActive: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() refresh: EventEmitter<void> = new EventEmitter<void>();

  row: Row;
  displayedColumns: string[] = [];
  displayedFilterColumns: string[] = [];
  tableData: TableData<FormGroup>;
  addNewRecord: boolean = false;
  formName: string;
  filterGroup: FormGroup;
  isLoading: boolean = false;

  columns: { id: string; name: string }[] = [];
  counter: number = 0; //to remove

  @ViewChild("input", { static: false }) input: ElementRef;
  @ViewChild("newRecord", { static: false }) newRecord: ElementRef;
  @ViewChild(MatMultiSort, { static: false }) sort: MatMultiSort;

  array: FormArray;
  length: number = 0;

  private routeSubscription: Subscription;
  private fieldHiddenSubscription: Subscription;

  constructor(
    private fb: FormBuilder,
    private formDataService: FormDataService,
    private router: Router,
    private route: ActivatedRoute,
    private snackbarService: SnackbarService,
    private messageService: MessagesService,
    private dialog: MatDialog,
    private authService: AuthService,
    private helperService: HelperService
  ) {}

  get modes() {
    return Mode;
  }

  get type() {
    return FieldType;
  }

  ngOnInit() {
    this.formName = this.route.snapshot.params["formName"];

    !this.table.action && this.table.hasEditButton !== false && (this.table.action = "edit");

    this.createFormArray();

    this.length = this.array.controls.length;
    this.parentGroup.addControl(this.table.name, this.array);

    this.tableData = new TableData<FormGroup>(this.columns, {
      defaultSortParams: [],
      defaultSortDirs: [],
      totalElements: this.array.controls.length
    });

    this.tableData.nextObservable.subscribe(() => this.getData());
    this.tableData.sortObservable.subscribe(() => this.getData());
    this.tableData.previousObservable.subscribe(() => this.getData());
    this.tableData.sizeObservable.subscribe(() => this.getData());

    setTimeout(() => {
      this.tableData.dataSource = new MatMultiSortTableDataSource(this.sort, true);
      this.getData();
    }, 0);

    this.fieldHiddenSubscription = this.helperService.fieldHidden.subscribe(fieldName => {
      const field = this.table.fields.find(fld => fld.name === fieldName);

      field && this.initColumns();
    });
  }

  ngOnDestroy() {
    this.routeSubscription && this.routeSubscription.unsubscribe();
    this.fieldHiddenSubscription && this.fieldHiddenSubscription.unsubscribe();
  }

  private createFormArray() {
    if (this.table.searchable !== false && this.table.searchType === "column") {
      this.filterGroup = this.fb.group({});
      this.table.fields.forEach(field => {
        const control = this.fb.control(null);
        this.filterGroup.addControl(field.name, control);
      });
    }

    this.initColumns();

    this.initArray();
  }

  private initColumns() {
    this.columns = [];
    this.displayedColumns = [];
    this.displayedFilterColumns = [];

    this.table.fields.forEach(field => {
      if (field.rendered !== false && !field.hidden) {
        this.columns.push({ id: field.name, name: field.label });
        this.displayedColumns.push(field.name);
        this.displayedFilterColumns.push(field.name + "filter");
      }
    });

    const hasActionColumn = this.tableWithButtons();

    hasActionColumn && this.columns.push({ id: "actions", name: "Действия" });
    hasActionColumn && this.displayedColumns.push("actions");
    hasActionColumn && this.displayedFilterColumns.push("actions-filter");
  }

  private initArray() {
    this.array = this.fb.array([]);

    if (this.table && this.table.values) {
      for (const value of this.table.values) {
        this.addTableRow(value);
      }
    }
  }

  private addTableRow(row: { id: string; fields: FieldConfig[] }, index = null) {
    const idGroup = this.fb.group({});
    const rowGroup = this.fb.group({});
    row.fields.forEach(field => this.helperService.addControl(field, rowGroup, false, false, false, false));
    idGroup.addControl(row.id + "", rowGroup); // just in case id is not a string
    index !== null ? this.array.insert(index, idGroup) : this.array.push(idGroup);
  }

  trackById(index, row: FormGroup) {
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

  private applyFilter() {
    let hasFilter = false;
    let filterValue;

    if (this.table.searchable !== false && this.table.searchType !== "column") {
      filterValue = this.input ? this.input.nativeElement.value : null;
      filterValue && (hasFilter = true);
    } else if (this.table.searchable !== false) {
      let allNull = true;
      Object.keys(this.filterGroup.value).forEach(key => (allNull = allNull && !this.filterGroup.value[key]));
      !allNull && (hasFilter = true);
    }

    if (!hasFilter) {
      return this.array.controls.slice();
    } else if (filterValue) {
      return this.allFilterHelper(filterValue);
    } else {
      return this.columnFilterHelper();
    }
  }

  private allFilterHelper(filterValue: string) {
    const newControls = [];

    this.array.controls.forEach((formGroup: FormGroup) => {
      let contains = false;
      const innerFormGroupKey = Object.keys(formGroup.controls)[0];
      const innerGroup = <FormGroup>formGroup.controls[innerFormGroupKey];

      for (let innerKey in innerGroup.controls) {
        const fieldVal = innerGroup.controls[innerKey].value;
        const val =
          fieldVal && typeof fieldVal === "object" && fieldVal._d ? fieldVal._d.toLocaleDateString() : fieldVal;

        if (val && (val + "").toLowerCase().indexOf(filterValue.toLowerCase()) !== -1) {
          contains = true;
          break;
        }
      }

      if (contains) {
        newControls.push(formGroup);
      }
    });

    return newControls;
  }

  private columnFilterHelper() {
    const newControls = [];

    this.array.controls.forEach((formGroup: FormGroup) => {
      let fulfilsAllFilters = true;
      const innerFormGroupKey = Object.keys(formGroup.controls)[0];
      const innerGroup = <FormGroup>formGroup.controls[innerFormGroupKey];

      for (let innerKey in innerGroup.controls) {
        const field = this.getField(formGroup, innerKey);
        if (field) {
          const fieldVal = innerGroup.controls[innerKey].value;
          const val =
            fieldVal && typeof fieldVal === "object" && fieldVal._d
              ? fieldVal._d.toLocaleDateString()
              : fieldVal || fieldVal === 0
              ? fieldVal + ""
              : fieldVal;

          const text = this.filterGroup.controls[field.name].value;
          text && (fulfilsAllFilters = fulfilsAllFilters && val && val.toLowerCase().indexOf(text.toLowerCase()) >= 0);
        }
      }

      if (fulfilsAllFilters) {
        newControls.push(formGroup);
      }
    });

    return newControls;
  }

  getData(goToBeginning = false) {
    goToBeginning && (this.tableData.pageIndex = 0);

    if (this.array.controls.length > 0) {
      let page = this.tableData.pageIndex || 0;
      const sorting = this.tableData.sortParams;
      const pagesize = this.tableData.pageSize || 10;
      const dirs = this.tableData.sortDirs;

      const tempData = this.applyFilter();
      this.length = tempData.length;

      if (tempData.length === page * pagesize && this.tableData.pageIndex > 0) {
        this.tableData.pageIndex--;
        page--;
      }

      if (sorting.length === 0) {
        this.tableData.data = tempData.slice(page * pagesize, (page + 1) * pagesize);
      } else if (sorting.length > 0) {
        const sortedData = tempData.sort((u1, u2) => {
          return this.sortData(u1, u2, sorting, dirs);
        });
        this.tableData.data = sortedData.slice(page * pagesize, (page + 1) * pagesize);
      }
    }
  }

  private sortData(d1: FormGroup, d2: FormGroup, sorting: string[], dirs: string[]): number {
    const key1 = Object.keys(d1.controls)[0];
    const key2 = Object.keys(d2.controls)[0];

    const firstControl = (<FormGroup>d1.controls[key1]).controls[sorting[0]];
    const secondConrol = (<FormGroup>d2.controls[key2]).controls[sorting[0]];

    let first = firstControl.value;
    let second = secondConrol.value;

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

  onSort() {
    if (this.table.sortable !== false) {
      this.tableData.onSortEvent();
    }
  }

  navigate(dtl: FormGroup) {
    const row = this.getRow(dtl);

    if (this.table.action === "edit" && this.table.createNew !== "samePage") {
      this.authService.setPrevUrlData(this.router.url.split("?")[0], this.route.snapshot.queryParams);
      let queryParams = environment.production
        ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
        : { ...this.route.snapshot.queryParams };
      const path =
        this.table.action === "edit"
          ? `/${Menu.Edit}/${this.table.formName || row.formName}`
          : `/${Menu.Preview}/${this.route.snapshot.params.type}/${this.table.formName || row.formName}`;
      queryParams[this.table.paramName] = row.formDataId || row.id;
      queryParams = {
        ...queryParams,
        ...row.additionalParams
      };

      environment.production && (queryParams = this.helperService.encodeParams(queryParams));

      this.authService.setTableName(this.table.name);
      this.router.navigate([path], {
        relativeTo: this.route,
        queryParams
      });
    } else if (this.table.action === "edit") {
      this.row = row;
      this.addNewRecord = true;
      this.addRecordActive.emit(this.addNewRecord);

      setTimeout(() => {
        this.newRecord.nativeElement.scrollIntoView({
          behavior: "smooth",
          block: "start"
        });
      }, 0);
    } else {
      this.helperService.tableItemSelected.next({
        paramName: this.table.paramName,
        formDataId: row.formDataId || row.id,
        additionalParams: row.additionalParams
      });
    }
  }

  createNew() {
    if (this.table.createNew === "redirect") {
      this.authService.setPrevUrlData(this.router.url.split("?")[0], this.route.snapshot.queryParams);

      let queryParams = environment.production
        ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
        : { ...this.route.snapshot.queryParams };

      if (this.table.createParams) {
        for (const key in this.table.createParams) {
          queryParams[key] = this.table.createParams[key];
        }
      }

      queryParams[this.table.paramName] = "null";

      environment.production && (queryParams = this.helperService.encodeParams(queryParams));

      this.authService.setTableName(this.table.name);
      this.router.navigate(["/", Menu.Create, this.table.formName], {
        queryParams
      });
    } else {
      this.row = null;
      this.addNewRecord = true;
      this.addRecordActive.emit(this.addNewRecord);

      setTimeout(() => {
        this.newRecord.nativeElement.scrollIntoView({
          behavior: "smooth",
          block: "start"
        });
      }, 0);
    }
  }

  async onAddRecord(data: { fields: FieldConfig[]; procedureName: string; formValues: any }) {
    if (!data.fields || !data.fields.length) {
      this.addNewRecord = false;
      this.addRecordActive.emit(this.addNewRecord);
    } else {
      const queryParams = environment.production
        ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
        : this.route.snapshot.queryParams;

      let formValues = data.formValues;
      for (const key in queryParams) {
        if (key !== "t") {
          formValues[key] = queryParams[key];
        }
      }

      let operationType;

      if (this.row) {
        operationType = ModeInt.update;
        formValues[this.table.paramName] = this.row.formDataId || this.row.id;
        formValues = { ...formValues, ...this.row.additionalParams };
      } else {
        operationType = ModeInt.create;
        formValues[this.table.paramName] = null;
      }

      if (this.mode === this.modes.View) {
        this.formDataService
          .submitForm({
            data: formValues,
            procedureName: data.procedureName,
            operationType
          })
          .subscribe(async (res: any) => {
            if (operationType === ModeInt.create) {
              await this.createHelper(res, data.fields, formValues);
            } else {
              this.updateHelper(formValues, data.fields);
            }

            this.addNewRecord = false;
            this.addRecordActive.emit(this.addNewRecord);

            this.snackbarService.openSuccessSnackbar(this.messageService.successMessages.saveSuccess);
          });
      } else {
        if (operationType === ModeInt.create) {
          await this.createHelper({}, data.fields);
        } else {
          this.updateHelper(formValues, data.fields);
        }

        this.addNewRecord = false;
        this.addRecordActive.emit(this.addNewRecord);
      }
    }
  }

  private async createHelper(result, dataFields: FieldConfig[], formValues = null) {
    const firstRecord = this.table.values.length === 0;
    try {
      result && (result = JSON.parse(result));
    } catch (err) {}

    const id =
      result && typeof result === "object" && result.length > 0
        ? result[0][this.table.paramName]
        : result &&
          typeof result === "object" &&
          result.length === undefined &&
          result[this.table.paramName] !== undefined
        ? result[this.table.paramName]
        : "new" + this.counter;
    this.counter++;

    let newRecordValues = {};

    if (formValues) {
      formValues[this.table.paramName] = id;
      let dbData: any = await this.formDataService.getData(this.route.snapshot.params.formName, formValues).toPromise();
      dbData = dbData && dbData.length ? dbData[0] : dbData;

      if (dbData && dbData[this.table.name]) {
        newRecordValues = dbData[this.table.name].find(record => record.id == id);
      }
    }

    const fields = this.getFields(dataFields, newRecordValues);
    const index = this.mode === this.modes.View ? (this.tableData.pageIndex || 0) * this.tableData.pageSize : null;

    index !== null
      ? this.table.values.splice(index, 0, { id, fields, formDataId: id })
      : this.table.values.push({ id, fields, formDataId: id });
    this.addTableRow({ ...this.table.values[index !== null ? index : this.table.values.length - 1] }, index);

    setTimeout(() => {
      firstRecord && (this.tableData.dataSource = new MatMultiSortTableDataSource(this.sort, true));
      this.getData();
    }, 0);
  }

  private updateHelper(formValues, dataFields: FieldConfig[]) {
    const dtl: FormGroup = <FormGroup>(
      this.array.controls.find((fg: FormGroup) => Object.keys(fg.controls)[0] === this.row.id + "")
    );

    const row: FormGroup = <FormGroup>dtl.controls[this.row.id + ""];
    for (const key in row.controls) {
      row.controls[key].setValue(formValues[key]);
    }

    const fields = this.getFields(dataFields);
    const tableRow = this.table.values.find(value => value.id == this.row.id);
    tableRow.fields.forEach(fld => {
      const field = fields.find(f => f.name === fld.name);
      fld.value = field.value;
    });

    this.getData();
  }

  getFieldVal(dtl: FormGroup, fieldName: string) {
    const row = this.getRow(dtl);
    const field = row.fields.find(fld => fld.name === fieldName);
    return field.value ? field.value + "" : field.value;
  }

  private getFields(tempFields: FieldConfig[], additionalValues = {}) {
    const fields: FieldConfig[] = [];

    for (const field of this.table.fields) {
      const tmpFld = tempFields.find(fld => fld.name === field.name);

      if (tmpFld && (tmpFld.type == FieldType.Select || tmpFld.type == FieldType.Searchselect)) {
        const option = tmpFld.options.find(option => option.code === tmpFld.value);
        field.value = option.label;
        field.code = option.code;
      } else if (tmpFld && tmpFld.type == FieldType.Checkbox) {
        field.value = tmpFld.value ? "да" : "не";
      } else if (tmpFld) {
        field.value = tmpFld.value;
      } else {
        field.value = additionalValues[field.name];
      }

      fields.push({ ...field });
    }

    return fields;
  }

  delete(dtl: FormGroup) {
    const row = this.getRow(dtl);

    this.onDelete(row);
  }

  private onDelete(row: Row) {
    this.isLoading = true;
    const id = row.id + "";
    const operationType = ModeInt.delete;
    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : this.route.snapshot.queryParams;
    const instid = queryParams.instid;

    if (this.mode === this.modes.View) {
      let formValues: any = {};
      const sysuserid = queryParams.sysuserid;
      const sysroleid = queryParams.sysroleid;

      formValues[this.table.paramName] = row.formDataId || id;
      formValues.instid = instid;
      formValues.sysuserid = sysuserid;
      formValues.sysroleid = sysroleid;

      formValues.forceOperation = this.table.forceOperation || 0;
      formValues = { ...formValues, ...row.additionalParams };

      this.formDataService
        .submitForm({
          data: formValues,
          procedureName: this.table.procedureName,
          operationType
        })
        .subscribe(
          (res: any) => {
            res && (res = JSON.parse(res));
            if ((res && res.length && res[0].OperationResultType === 1) || (res && res.OperationResultType === 1)) {
              this.successfulDeleteHelper(id, row.formDataId, res);
            } else if (
              (res && res.length && res[0].OperationResultType === 0) ||
              (res && res.OperationResultType === 0)
            ) {
              this.formDataService.getMessages().subscribe(messages => {
                const index = res.MessageCode || res[0].MessageCode;
                this.confirmDelete(
                  id,
                  row.formDataId,
                  formValues,
                  this.table.procedureName,
                  operationType,
                  messages[index],
                  instid
                );
              });
            } else if (
              (res && res.length && res[0].OperationResultType === 2) ||
              (res && res.OperationResultType === 2)
            ) {
              this.formDataService.getMessages().subscribe(messages => {
                const index = res.MessageCode || res[0].MessageCode;
                this.isLoading = false;
                this.snackbarService.openErrorSnackbar(messages[index]);
              });
            } else {
              this.successfulDeleteHelper(id, row.formDataId, res);
            }
          },
          errorCode => {
            if (errorCode !== -1) {
              this.formDataService.getMessages().subscribe(messages => {
                this.snackbarService.openErrorSnackbar(messages[errorCode]);
                this.isLoading = false;
              });
            }
          }
        );
    } else {
      this.successfulDeleteHelper(id, {}, false);
    }
  }

  private successfulDelete(id, formDataId, data, procedureName, operationType) {
    this.deleteFromDb(id, formDataId, data, procedureName, operationType);
  }

  private successfulDeleteHelper(id, res, showSuccessMessage = true) {
    this.isLoading = false;
    showSuccessMessage && this.snackbarService.openSuccessSnackbar(this.messageService.successMessages.saveSuccess);
    this.table.values = this.table.values.filter(val => val.id != id);
    this.array.controls = this.array.controls.filter((control: FormGroup) => Object.keys(control.controls)[0] != id);

    res = res && res.length ? res[0] : res.length === undefined ? res : {};

    this.refresh.next();
    this.getData();
  }

  private confirmDelete(id, formDataId, data, procedureName: string, operationType: number, message: string, instid) {
    this.isLoading = false;
    const dialogRef = this.dialog.open(ModalComponent, {
      width: "45%",
      data: {
        message,
        confirmBtnLbl: "Да",
        cancelBtnLbl: "Не"
      }
    });

    dialogRef.afterClosed().subscribe((result: boolean) => {
      if (result) {
        this.isLoading = true;
        data.forceOperation = 1;

        this.successfulDelete(id, formDataId, data, procedureName, operationType);
      }
    });
  }

  private deleteFromDb(id, formDataId, data, procedureName, operationType) {
    this.formDataService.submitForm({ data, procedureName, operationType }).subscribe(
      (res: any) => {
        res && (res = JSON.parse(res));
        if ((res && res.length && res[0].OperationResultType === 1) || (res && res.OperationResultType === 1)) {
          this.successfulDeleteHelper(id, formDataId, res);
        } else {
          this.isLoading = false;
        }
      },
      errorCode => {
        this.formDataService.getMessages().subscribe(messages => {
          this.snackbarService.openErrorSnackbar(messages[errorCode]);
          this.isLoading = false;
        });
      }
    );
  }

  tableWithButtons() {
    return (
      (this.table.hasEditButton !== false || this.table.canDeleteRow || this.table.action === "preview") &&
      !(this.table.disabledViewMode && this.mode === this.modes.View)
    );
  }

  tableWithOneButton() {
    return (
      ((this.table.hasEditButton !== false && !this.table.canDeleteRow) ||
        (this.table.hasEditButton === false &&
          this.table.canDeleteRow &&
          !(this.table.values.length === 1 && this.table.canDeleteLastRow === false)) ||
        (this.table.hasEditButton !== false &&
          this.table.canDeleteRow &&
          this.table.canDeleteLastRow === false &&
          this.table.values.length === 1)) &&
      !(this.table.disabledViewMode && this.mode === this.modes.View)
    );
  }

  hasDeleteButton(index: number) {
    return (
      this.table.canDeleteRow &&
      !(this.array.controls.length === 1 && this.table.canDeleteLastRow === false) &&
      !(this.table.firstRowDisabled && index === 0)
    );
  }

  actionAvailable(index: number) {
    return (
      ((this.table.hasEditButton !== false && !(this.table.firstRowDisabled && index === 0)) ||
        this.table.action === "preview") &&
      !(this.table.disabledViewMode && this.mode === this.modes.View)
    );
  }

  performAction(dtl: FormGroup, index: number) {
    if (this.actionAvailable(index)) {
      this.navigate(dtl);
    }
  }

  getRow(dtl: FormGroup) {
    const key = Object.keys(dtl.controls)[0];
    return this.table.values.find(val => val.id == key);
  }

  isLastRecord(index: number) {
    const pages = Math.ceil(this.table.values.length / this.tableData.pageSize);
    const lastPageCount = this.table.values.length % this.tableData.pageSize;

    return (this.tableData.pageIndex || 0) === pages - 1 && index === lastPageCount - 1;
  }

  columnNumber() {
    return this.table.fields.filter(fld => fld.rendered !== false).length;
  }

  hasNewRecordButton() {
    return (
      this.table.createNew &&
      !this.table.createNewHidden &&
      !(this.table.disabledViewMode && this.mode === this.modes.View)
    );
  }
}
