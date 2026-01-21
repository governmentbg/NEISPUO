import { Component, ElementRef, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges, ViewChild } from "@angular/core";
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
import { MultipleAddComponent } from "./multiple-add/multiple-add.component";
import { HelperService } from "src/app/services/helpers.service";
import { environment } from "../../../environments/environment";

import * as XLSX from "xlsx";
import pdfMake from "pdfmake";
import pdfFonts from "pdfmake/build/vfs_fonts";
import { CustomFormControl, CustomFormGroup } from "src/app/models/custom-form-control.interface";
import { Tabs } from "src/app/enums/tabs.enum";
import { AzureService } from "src/app/services/azure.service";
pdfMake.vfs = pdfFonts.pdfMake.vfs;

@Component({
  selector: "app-table",
  templateUrl: "./table.component.html",
  styleUrls: ["./table.component.scss"]
})
export class TableComponent implements OnInit, OnChanges, OnDestroy {
  @Input() table: Table;
  @Input() mode: Mode;
  @Input() parentGroup: FormGroup;
  @Input() multiAddMode: boolean = false;
  @Input() checkedList: string[] = [];
  @Input() submissionPeriod: { isOpenPeriod: boolean; period: number; schoolYear: number };
  @Input() isLocked: boolean;
  @Input() filterTable: boolean = false;

  @Output() check: EventEmitter<{
    checked: boolean;
    id: string;
    label: string;
  }> = new EventEmitter<{
    checked: boolean;
    id: string;
    label: string;
  }>();

  @Output() checkAll: EventEmitter<{ idList: string[]; labelList: string[] }> = new EventEmitter<{
    idList: string[];
    labelList: string[];
  }>();

  @Output() addRecordActive: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() refresh: EventEmitter<void> = new EventEmitter<void>();
  @Output() filteredTable: EventEmitter<void> = new EventEmitter<void>();

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
  isMonRuo: boolean = false;
  isHistory: boolean = false;

  private sysRoleID: string;
  private tab: string;
  private isOpenCampaign: boolean;

  private routeSubscription: Subscription;
  private tableValuesChangedSubscription: Subscription;
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
    private helperService: HelperService,
    private azureService: AzureService
  ) {}

  get modes() {
    return Mode;
  }

  get type() {
    return FieldType;
  }

  ngOnInit() {
    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : { ...this.route.snapshot.queryParams };

    this.isMonRuo = this.authService.isMon() || this.authService.isRuo();
    this.formName = this.route.snapshot.params["formName"];
    this.isHistory = this.route.snapshot.params.tab === Tabs.history || queryParams.year !== undefined;
    this.tab = this.router.url.split("/").length > 3 ? this.router.url.split("/")[3] : null;
    this.sysRoleID = queryParams.sysroleid;
    this.isOpenCampaign = queryParams.isOpenCampaign == "true" || queryParams.isOpenCampaign == 1;

    if (!this.table.action && this.table.hasEditButton !== false) {
      this.table.action = "edit";
    }

    if (
      this.table.action &&
      this.table.action === "edit" &&
      ((this.isMonRuo && !this.table.editableByMonRuo) || this.isHistory || (this.isLocked && this.table.name !== "changeYearDataList"))
    ) {
      this.table.action = "fakePreview";
      this.table.values.forEach(row => (row.fakePreview = !!row.hasEditButton));
    }

    this.createFormArray();
    this.initMultiAdd();

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
    this.tableData.pageSize = 25;

    setTimeout(() => {
      this.tableData.dataSource = new MatMultiSortTableDataSource(this.sort, true);
      this.getData();
    }, 0);

    this.tableValuesChangedSubscription = this.helperService.tableValuesChanged.subscribe(
      (event: { fieldVal: string; multiselectField: string }) => {
        this.filterTableValues(event);
      }
    );

    this.fieldHiddenSubscription = this.helperService.fieldHidden.subscribe(fieldName => {
      const field = this.table.fields.find(fld => fld.name === fieldName);

      field && this.initColumns();
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.filterTable && this.filterTable) {
      this.isLoading = true;

      const dependsOn = this.dependsOn();

      const queryParams = environment.production
        ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
        : { ...this.route.snapshot.queryParams };

      this.formDataService.getMultiAddTableValuesOnly(this.table.multiAdd.addFrom, { ...queryParams, ...dependsOn }).subscribe(
        (res: any[]) => {
          const fldName = this.table.multiAdd.comparisonColumn || "id";
          const ids = res.map(elem => elem.id + "");
          const comparisonIds = res.map(elem => elem[fldName] + "");

          this.table.values = this.table.values.filter(row => ids.includes(row.id + ""));
          this.array.controls = this.array.controls.filter((fg: FormGroup) => ids.includes(Object.keys(fg.controls)[0]));
          this.checkedList = this.checkedList.filter(elem => comparisonIds.includes(elem));
          this.getData();

          this.filterTable = false;
          this.filteredTable.emit();
          this.isLoading = false;
        },
        err => (this.isLoading = false)
      );
    }
  }

  ngOnDestroy() {
    this.routeSubscription && this.routeSubscription.unsubscribe();
    this.tableValuesChangedSubscription && this.tableValuesChangedSubscription.unsubscribe();
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

    if (this.multiAddMode) {
      this.columns.unshift({ id: "check", name: "" });
      this.displayedColumns.unshift("check");
      this.displayedFilterColumns.unshift("check-filter");
    } else if (this.table.reorder) {
      this.columns.unshift({ id: "reorder", name: "" });
      this.displayedColumns.unshift("reorder");
      this.displayedFilterColumns.unshift("reorder-filter");
    }

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

  private addTableRow(row: Row, index = null) {
    const idGroup = this.fb.group({});
    const rowGroup = this.fb.group({});
    row.fields.forEach(field => this.helperService.addControl(field, rowGroup, false, false, false, false));
    idGroup.addControl(row.id + "", rowGroup); // just in case id is not a string
    index !== null ? this.array.insert(index, idGroup) : this.array.push(idGroup);

    if (this.tableWithButtons() && !this.columns.find(column => column.id === "actions")) {
      this.columns.push({ id: "actions", name: "Действия" });
      this.displayedColumns.push("actions");
      this.displayedFilterColumns.push("actions-filter");
    }
  }

  private initMultiAdd() {
    if (this.table && this.table.values && this.table.multiAdd && !this.multiAddMode) {
      const fldName = this.table.multiAdd.comparisonColumn;

      for (const parentControl of this.array.controls) {
        const key = Object.keys((<FormGroup>parentControl).controls)[0];
        const innerControl = (<FormGroup>parentControl).controls[key];

        const fldVal = fldName ? (<FormGroup>innerControl).controls[fldName].value : key;
        this.checkedList.push(fldVal + "");
      }
    }
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
        const val = fieldVal && typeof fieldVal === "object" && fieldVal._d ? fieldVal._d.toLocaleDateString() : fieldVal;

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
      const pagesize = this.tableData.pageSize || 25;
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

    if (
      (this.table.action === "edit" && this.table.createNew !== "samePage") ||
      (this.table.action === "fakePreview" && this.table.createNew !== "samePage")
    ) {
      const url = this.router.url.split("?")[0];

      if (url !== "/" + Menu.NewStaffPosition && url !== "/" + Menu.NewSubjectInstitution) {
        this.authService.setPrevUrlData(this.router.url.split("?")[0], this.route.snapshot.queryParams);
      }

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
        ...row.additionalParams,
        editableByMonRuo: this.table.editableByMonRuo ? true : null
      };

      // if (this.submissionPeriod) {
      //   queryParams.period = this.submissionPeriod.period;
      //   queryParams.schoolYear = this.submissionPeriod.schoolYear;
      // }

      environment.production && (queryParams = this.helperService.encodeParams(queryParams));

      this.authService.setTableName(this.table.name);
      this.router.navigate([path], {
        relativeTo: this.route,
        queryParams
      });
    } else if (this.table.action === "edit" || this.table.action === "fakePreview") {
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
      if (this.table.switchControl) {
        sessionStorage.setItem(
          "list",
          this.helperService.encodeParams({
            tableName: this.table.name,
            columns: this.table.switchControl.listColumns,
            switchLabel: this.table.switchControl.label,
            paramName: this.table.paramName,
            additionalParams: this.table.additionalParams
          }).q
        );
      }

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

      if (this.submissionPeriod) {
        queryParams.period = this.submissionPeriod.period;
        queryParams.schoolYear = this.submissionPeriod.schoolYear;
      }

      environment.production && (queryParams = this.helperService.encodeParams(queryParams));

      this.authService.setTableName(this.table.name);
      this.router.navigate(["/", Menu.Create, this.table.formName], { queryParams });
    } else {
      this.row = null;
      this.addNewRecord = true;
      this.addRecordActive.emit(this.addNewRecord);

      setTimeout(() => {
        this.newRecord.nativeElement.scrollIntoView({ behavior: "smooth", block: "start" });
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
          .subscribe(
            async (res: any) => {
              try {
                res && (res = JSON.parse(res));
              } catch (err) {}

              if ((res && res.length && res[0].OperationResultType === 1) || (res && res.OperationResultType === 1)) {
                await this.onSuccess(operationType, res, data, formValues);
              } else if ((res && res.length && res[0].OperationResultType === 0) || (res && res.OperationResultType === 0)) {
                this.formDataService.getMessages().subscribe(messages => {
                  const index = res.MessageCode || res[0].MessageCode;
                  this.confirmUpdate(data, this.table.procedureName, operationType, formValues, messages[index]);
                });
              } else if ((res && res.length && res[0].OperationResultType === 2) || (res && res.OperationResultType === 2)) {
                this.formDataService.getMessages().subscribe(messages => {
                  const index = res.MessageCode || res[0].MessageCode;
                  this.isLoading = false;
                  this.snackbarService.openErrorSnackbar(messages[index]);
                });
              } else {
                await this.onSuccess(operationType, res, data, formValues);
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
        if (operationType === ModeInt.create) {
          await this.createHelper({}, data.fields);
        } else {
          this.updateHelper({}, formValues, data.fields);
        }

        this.addNewRecord = false;
        this.addRecordActive.emit(this.addNewRecord);
      }
    }
  }

  private confirmUpdate(data, procedureName: string, operationType: number, formValues, message: string) {
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
        this.formDataService.submitForm({ data, procedureName, operationType }).subscribe(
          async (res: any) => {
            res && (res = JSON.parse(res));
            if ((res && res.length && res[0].OperationResultType === 1) || (res && res.OperationResultType === 1)) {
              await this.onSuccess(operationType, res, data, formValues);
            }

            this.isLoading = false;
          },
          errorCode => {
            this.formDataService.getMessages().subscribe(messages => {
              this.snackbarService.openErrorSnackbar(messages[errorCode]);
              this.isLoading = false;
            });
          }
        );
      }
    });
  }

  private async onSuccess(operationType, res, data, formValues) {
    if (operationType === ModeInt.create) {
      await this.createHelper(res, data.fields, formValues);
    } else {
      this.updateHelper(res, formValues, data.fields);
    }

    this.addNewRecord = false;
    this.addRecordActive.emit(this.addNewRecord);

    this.snackbarService.openSuccessSnackbar(this.messageService.successMessages.saveSuccess);
  }

  private async createHelper(result, dataFields: FieldConfig[], formValues = null) {
    const firstRecord = this.table.values.length === 0;

    const id =
      result && typeof result === "object" && result.length > 0
        ? result[0][this.table.paramName]
        : result && typeof result === "object" && result.length === undefined && result[this.table.paramName] !== undefined
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

    const body = environment.production
      ? { ...this.helperService.decodeParams(this.route.snapshot.queryParams["q"]), ...result }
      : { ...this.route.snapshot.queryParams, ...result };

    let [canDeleteRow, hasEditButton] = this.evalHelper(this.table, body);

    if (index !== null) {
      this.table.values.splice(index, 0, { id, fields, formDataId: id, canDeleteRow, hasEditButton });
    } else {
      this.table.values.push({ id, fields, formDataId: id, canDeleteRow, hasEditButton });
    }

    this.addTableRow({ ...this.table.values[index !== null ? index : this.table.values.length - 1] }, index);

    setTimeout(() => {
      firstRecord && (this.tableData.dataSource = new MatMultiSortTableDataSource(this.sort, true));
      this.getData();
    }, 0);
  }

  //pass table for eval
  private evalHelper(table: Table, body) {
    const canDeleteRowCondition = table.canDeleteRow;
    const hasEditButtonCondition = table.hasEditButton !== undefined ? table.hasEditButton : true;

    let canDeleteRow = canDeleteRowCondition;
    if (table && canDeleteRowCondition && (canDeleteRowCondition + "").includes("if")) {
      canDeleteRow = this.formDataService.substituteParams(canDeleteRowCondition + "", body);
      eval(canDeleteRow + "");
      canDeleteRow = table.canDeleteRow;
    }

    let hasEditButton = hasEditButtonCondition;
    if (table && hasEditButtonCondition && (hasEditButtonCondition + "").includes("if")) {
      hasEditButton = this.formDataService.substituteParams(hasEditButtonCondition + "", body);
      eval(hasEditButton + "");
      hasEditButton = table.hasEditButton;
    }

    return [canDeleteRow, hasEditButton];
  }

  private updateHelper(result, formValues, dataFields: FieldConfig[]) {
    const removeRow = result && typeof result === "object" && result.length > 0 ? result[0]?.removeRow : result?.removeRow;

    if (!removeRow) {
      const dtl: FormGroup = <FormGroup>this.array.controls.find((fg: FormGroup) => Object.keys(fg.controls)[0] === this.row.id + "");

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
    } else {
      this.array.controls = this.array.controls.filter((fg: FormGroup) => Object.keys(fg.controls)[0] !== this.row.id + "");

      this.getData();
    }
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

    if (
      ![
        "curriculumListA",
        "curriculumListB",
        "curriculumListV",
        "curriculumListG",
        "curriculumListD",
        "curriculumListE",
        "curriculumListF",
        "staff"
      ].includes(this.table.name)
    ) {
      const dialogRef = this.dialog.open(ModalComponent, {
        width: "45%",
        data: {
          message: this.messageService.modalQuestions.deleteRecord,
          confirmBtnLbl: "Да",
          cancelBtnLbl: "Не"
        }
      });

      dialogRef.afterClosed().subscribe((result: boolean) => {
        if (result) {
          this.onDelete(row);
        }
      });
    } else {
      this.onDelete(row);
    }
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
      const extdata = queryParams.extdata;
      const extAlldata = queryParams.extAlldata;

      formValues[this.table.paramName] = row.formDataId || id;
      formValues.instid = instid;
      formValues.sysuserid = sysuserid;
      formValues.sysroleid = sysroleid;
      formValues.extdata = extdata;
      formValues.extAlldata = extAlldata;

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
            } else if ((res && res.length && res[0].OperationResultType === 0) || (res && res.OperationResultType === 0)) {
              this.formDataService.getMessages().subscribe(messages => {
                const index = res.MessageCode || res[0].MessageCode;
                this.confirmDelete(id, row.formDataId, formValues, this.table.procedureName, operationType, messages[index], instid);
              });
            } else if ((res && res.length && res[0].OperationResultType === 2) || (res && res.OperationResultType === 2)) {
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
      if (this.table.multiAdd) {
        const fldName = this.table.multiAdd.comparisonColumn;

        const fldVal = fldName ? row.fields.find(fld => fld.name === fldName).value : id;
        this.checkedList = this.checkedList.filter(val => val != fldVal);
      }

      this.successfulDeleteHelper(id, row.formDataId, {}, false);
    }
  }

  private successfulDelete(id, formDataId, instid, data, procedureName, operationType) {
    if (
      [
        "curriculumListA",
        "curriculumListB",
        "curriculumListV",
        "curriculumListG",
        "curriculumListD",
        "curriculumListE",
        "curriculumListF",
        "staff"
      ].includes(this.table.name) &&
      environment.production
    ) {
      if (this.table.name === "staff") {
        const body = { personID: formDataId ? +formDataId : +id, institutionID: +instid };
        this.azureService.deleteTeacher(body).subscribe(
          res => {
            this.onAzureCall(
              id,
              formDataId,
              { status: res.status },
              "/v1/azure-integrations/teacher/enrollment-school-delete",
              body,
              data,
              procedureName,
              operationType,
              0
            );

            res.status === 200 || res.status === 201
              ? this.deleteFromDb(id, formDataId, data, procedureName, operationType)
              : this.onAzureCall(
                  id,
                  formDataId,
                  res,
                  "/v1/azure-integrations/teacher/enrollment-school-delete",
                  body,
                  data,
                  procedureName,
                  operationType,
                  1
                );
          },
          err => {
            this.onAzureCall(
              id,
              formDataId,
              { status: err.status },
              "/v1/azure-integrations/teacher/enrollment-school-delete",
              body,
              data,
              procedureName,
              operationType,
              0
            );

            this.onAzureCall(
              id,
              formDataId,
              err,
              "/v1/azure-integrations/teacher/enrollment-school-delete",
              body,
              data,
              procedureName,
              operationType,
              1
            );
          }
        );
      } else {
        const body = { curriculumID: formDataId ? +formDataId : +id };
        this.azureService.deleteClass(body).subscribe(
          res => {
            this.onAzureCall(
              id,
              formDataId,
              { status: res.status },
              "/v1/azure-integrations/class/delete",
              body,
              data,
              procedureName,
              operationType,
              0
            );

            res.status === 200 || res.status === 201
              ? this.deleteFromDb(id, formDataId, data, procedureName, operationType)
              : this.onAzureCall(id, formDataId, res, "/v1/azure-integrations/class/delete", body, data, procedureName, operationType, 1);
          },
          err => {
            this.onAzureCall(
              id,
              formDataId,
              { status: err.status },
              "/v1/azure-integrations/class/delete",
              body,
              data,
              procedureName,
              operationType,
              0
            );
            this.onAzureCall(id, formDataId, err, "/v1/azure-integrations/class/delete", body, data, procedureName, operationType, 1);
          }
        );
      }
    } else {
      this.deleteFromDb(id, formDataId, data, procedureName, operationType);
    }
  }

  private onAzureCall(id, formDataId, error, apiCall: string, payload, data, procedureName, operationType, isError) {
    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : this.route.snapshot.queryParams;

    const body = {
      data: {
        sysuserid: queryParams.sysuserid,
        instid: queryParams.instid,
        schoolYear: queryParams.schoolYear,
        error_number: error?.error?.status || error?.status,
        error_message: error?.error?.message || error?.message,
        error_procedure: environment.azureUrl + apiCall,
        operationType: ModeInt.delete,
        curriculumID: payload.curriculumID,
        personID: payload.personID,
        payload: JSON.stringify(payload),
        isError
      },
      procedureName: "logs.azureErrorLog",
      operationType: ModeInt.create
    };

    this.formDataService.performProcedure(body).subscribe(
      res => this.deleteFromDb(id, formDataId, data, procedureName, operationType),
      err => this.deleteFromDb(id, formDataId, data, procedureName, operationType)
    );
  }

  private onAzureErrorNoDelete(error, apiCall, payload, isError) {
    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : this.route.snapshot.queryParams;

    const body = {
      data: {
        sysuserid: queryParams.sysuserid,
        instid: queryParams.instid,
        schoolYear: queryParams.schoolYear,
        error_number: error?.error?.status || error?.status,
        error_message: error?.error?.message || error?.message,
        error_procedure: environment.azureUrl + apiCall,
        curriculumID: payload.curriculumID,
        personID: payload.personID,
        operationType: ModeInt.delete,
        payload: JSON.stringify(payload),
        isError
      },
      procedureName: "logs.azureErrorLog",
      operationType: ModeInt.create
    };

    this.formDataService.performProcedure(body).subscribe();
  }

  private successfulDeleteHelper(id, formDataId, res, showSuccessMessage = true) {
    this.isLoading = false;
    showSuccessMessage && this.snackbarService.openSuccessSnackbar(this.messageService.successMessages.saveSuccess);
    this.table.values = this.table.values.filter(val => val.id != id);
    this.array.controls = this.array.controls.filter((control: FormGroup) => Object.keys(control.controls)[0] != id);

    res = res && res.length ? res[0] : res.length === undefined ? res : {};
    if (
      showSuccessMessage &&
      ["studentMainClassGroups", "studentOtherClassGroups", "staffPositions", "staff"].includes(this.table.name) &&
      res.curriculumDeletedString &&
      environment.production
    ) {
      const allCurriculums = res.curriculumDeletedString.split(",");

      const queryParams = environment.production
        ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
        : this.route.snapshot.queryParams;

      const personId = queryParams.personid ? +queryParams.personid : +formDataId;

      for (let curriculumID of allCurriculums) {
        const body = {
          curriculumID: +curriculumID,
          personIDsToCreate: [],
          personIDsToDelete: [personId],
          institutionID: +queryParams.instid
        };
        this.azureService.updateClass(body).subscribe(
          res => {
            this.onAzureErrorNoDelete({ status: res.status }, "/v1/azure-integrations/class/update", body, 0);
            res.status === 200 || res.status === 201 ? {} : this.onAzureErrorNoDelete(res, "/v1/azure-integrations/class/update", body, 1);
          },
          err => {
            this.onAzureErrorNoDelete({ status: err.status }, "/v1/azure-integrations/class/update", body, 0);
            this.onAzureErrorNoDelete(err, "/v1/azure-integrations/class/update", body, 1);
          }
        );
      }
    }

    showSuccessMessage && this.refresh.next();
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

        this.successfulDelete(id, formDataId, instid, data, procedureName, operationType);
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
      (((this.canEditRow() || this.canDeleteRow()) &&
        (!this.isMonRuo || this.table.editableByMonRuo) &&
        !this.isHistory &&
        (!this.isLocked || this.table.name === "changeYearDataList")) ||
        this.table.action === "preview" ||
        this.canFakePreview() ||
        this.table.action === "fastPreview") &&
      !(this.table.disabledViewMode && this.mode === this.modes.View)
    );
  }

  tableWithOneButton() {
    return (
      ((((this.canEditRow() && !this.canDeleteRow() && !this.isLocked) ||
        (!this.canEditRow() &&
          this.canDeleteRow() &&
          (!this.isLocked || this.table.name === "changeYearDataList") &&
          !(this.table.values.length === 1 && this.table.canDeleteLastRow === false)) ||
        (this.canEditRow() &&
          !this.isLocked &&
          this.canDeleteRow() &&
          this.table.canDeleteLastRow === false &&
          this.table.values.length === 1)) &&
        (!this.isMonRuo || this.table.editableByMonRuo) &&
        !this.isHistory) ||
        ((this.isMonRuo || this.isHistory || this.isLocked) &&
          (this.table.action === "fastPreview" || this.table.action === "preview" || this.canFakePreview()))) &&
      !(this.table.disabledViewMode && this.mode === this.modes.View)
    );
  }

  private canDeleteRow() {
    let canDeleteRow = false;

    for (let row of this.table.values) {
      if (row.canDeleteRow) {
        canDeleteRow = true;
        break;
      }
    }

    return canDeleteRow;
  }

  private canEditRow() {
    let hasEditButton = false;

    for (let row of this.table.values) {
      if (row.hasEditButton && !row.fakePreview) {
        hasEditButton = true;
        break;
      }
    }

    return hasEditButton || this.table.action;
  }

  private canFakePreview() {
    let fakePreview = false;

    for (let row of this.table.values) {
      if (row.fakePreview) {
        fakePreview = true;
        break;
      }
    }

    return fakePreview;
  }

  hasDeleteButton(dtl: FormGroup, index: number) {
    const row = this.getRow(dtl);

    return (
      row.canDeleteRow &&
      !this.isMonRuo &&
      !this.isHistory &&
      !this.isLocked &&
      !(this.array.controls.length === 1 && this.table.canDeleteLastRow === false) &&
      !(this.table.firstRowDisabled && index === 0)
    );
  }

  hasEditButton(dtl: FormGroup, index: number) {
    const row = this.getRow(dtl);

    return row.hasEditButton && !(this.table.firstRowDisabled && index === 0) && !row.fakePreview;
  }

  hasFakePreview(dtl: FormGroup) {
    const row = this.getRow(dtl);

    return row.fakePreview;
  }

  actionAvailable(dtl: FormGroup, index: number) {
    return (
      (((!this.isMonRuo || this.table.editableByMonRuo) &&
        this.hasEditButton(dtl, index) &&
        !this.isHistory &&
        !(this.table.firstRowDisabled && index === 0)) ||
        this.table.action === "fastPreview" ||
        this.table.action === "preview" ||
        this.hasFakePreview(dtl)) &&
      !(this.table.disabledViewMode && this.mode === this.modes.View)
    );
  }

  createNewStaffMember() {
    this.authService.setPrevUrlData(this.router.url.split("?")[0], this.route.snapshot.queryParams);

    this.router.navigate(["/", Menu.NewStaffMember], { queryParamsHandling: "preserve" });
  }

  createNewSubjectInstitution() {
    this.authService.setPrevUrlData(this.router.url.split("?")[0], this.route.snapshot.queryParams);

    this.router.navigate(["/", Menu.NewSubjectInstitution], { queryParamsHandling: "preserve" });
  }

  createNewStaffPosition() {
    this.authService.setPrevUrlData(this.router.url.split("?")[0], this.route.snapshot.queryParams);

    this.router.navigate(["/", Menu.NewStaffPosition], { queryParamsHandling: "preserve" });
  }

  isChecked(dtl: FormGroup) {
    const row = this.getRow(dtl);
    const fldName = this.table.multiAdd.comparisonColumn;
    const fldVal = fldName ? row.fields.find(fld => fld.name === fldName).value : row.id;
    return this.checkedList.includes(fldVal + "");
  }

  onCheck(event, dtl: FormGroup) {
    event.preventDefault();
    const row = this.getRow(dtl);

    let label = "";

    if (this.table.multiAdd.listColumns) {
      for (const column of this.table.multiAdd.listColumns) {
        const fld = row.fields.find(fld => fld.name === column);
        label += fld.value + " ";
      }
    }

    const fldName = this.table.multiAdd.comparisonColumn;
    const fldVal = fldName ? row.fields.find(fld => fld.name === fldName).value : row.id;

    const checked = !this.isChecked(dtl);
    if (checked || !this.table.multiAdd.checkDeleteProcedure) {
      this.check.next({ checked, id: fldVal + "", label });
    } else {
      this.isLoading = true;
      const operationType = 13;

      const queryParams = environment.production
        ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
        : this.route.snapshot.queryParams;

      let formValues: any = { ...queryParams };
      formValues.id = fldVal;
      formValues.forceOperation = 0;

      this.formDataService
        .submitForm({
          data: formValues,
          procedureName: this.table.multiAdd.checkDeleteProcedure,
          operationType
        })
        .subscribe((res: any) => {
          res && (res = JSON.parse(res));
          if ((res && res.length && res[0].OperationResultType === 2) || (res && res.OperationResultType === 2)) {
            this.formDataService.getMessages().subscribe(messages => {
              const index = res.MessageCode || res[0].MessageCode;
              this.isLoading = false;

              this.snackbarService.openErrorSnackbar(messages[index]);
            });
          } else {
            this.isLoading = false;
            this.check.next({ checked, id: fldVal + "", label });
          }
        });
    }
  }

  allChecked() {
    return this.checkedList.length === this.table.values.length;
  }

  onCheckAll(event) {
    event.preventDefault();
    const checked = !this.allChecked();
    if (checked) {
      const lists = this.getLists();

      this.checkAll.emit({ idList: lists.idList, labelList: lists.labelList });
    } else {
      this.checkAll.emit({ idList: [], labelList: [] });
    }
  }

  private getLists() {
    const idList = [];
    const labelList = [];

    this.table.values.forEach(val => {
      let label = "";

      if (this.table.multiAdd.listColumns) {
        for (const column of this.table.multiAdd.listColumns) {
          const fld = val.fields.find(fld => fld.name === column);
          label += fld.value + " ";
        }
      }

      const fldName = this.table.multiAdd.comparisonColumn;
      const fldVal = fldName ? val.fields.find(fld => fld.name === fldName).value : val.id;
      idList.push(fldVal + "");
      labelList.push(label);
    });

    return { idList, labelList };
  }

  addRows() {
    const fields = JSON.parse(JSON.stringify(this.table.fields));

    fields.forEach(fld => {
      if (!this.table.multiAdd.tableColumns.includes(fld.name)) {
        fld.rendered = false;
      }
    });

    const table = {
      ...this.table,
      searchable: true,
      values: [],
      fields
    };

    const dependsOn = this.dependsOn();

    const dRef = this.dialog.open(MultipleAddComponent, {
      width: "75%",
      panelClass: "l-modal-custom-add-row",
      data: {
        table,
        mode: Mode.Edit,
        checkedList: [...this.checkedList],
        dependsOn
      }
    });

    dRef.afterClosed().subscribe(
      (data: {
        values: {
          id: string;
          fields: FieldConfig[];
          formName?: string;
          formDataId?: number | string;
          checked?: boolean;
        }[];
        checkedList: string[];
      }) => {
        if (data) {
          const firstRecord = this.table.values.length === 0;
          const fldName = this.table.multiAdd.comparisonColumn;

          for (const value of data.values) {
            const rowValues = this.getRowValues(value);
            const fldVal = fldName ? rowValues.fields.find(fld => fld.name === fldName).value : rowValues.id;
            if (data.checkedList.includes(fldVal + "") && !this.checkedList.includes(fldVal + "")) {
              this.checkedList.push(fldVal + "");
              this.table.values.push({ ...rowValues });
              this.addTableRow({ ...rowValues });
            }
          }

          setTimeout(() => {
            this.table.values = this.table.values.filter(val => {
              const fldVal = fldName ? val.fields.find(fld => fld.name === fldName).value : val.id;
              return data.checkedList.includes(fldVal + "");
            });

            this.array.controls = this.array.controls.filter(row => {
              const key = Object.keys((<FormGroup>row).controls)[0];
              const innerControl = (<FormGroup>row).controls[key];

              const controlValue = fldName ? (<FormGroup>innerControl).controls[fldName].value : key;
              return data.checkedList.includes(controlValue + "");
            });

            this.checkedList = [...data.checkedList];
            firstRecord && (this.tableData.dataSource = new MatMultiSortTableDataSource(this.sort, true));
            this.getData();
          }, 0);

          if (this.table.multiAdd.procedure) {
            const transformedData = this.transformData(data.values);
            this.isLoading = true;
            const body = {
              data: transformedData,
              procedureName: this.table.multiAdd.procedure,
              operationType: 21
            };

            this.formDataService.performProcedure(body).subscribe(
              res => {
                this.isLoading = false;
                this.transformForm(res);
              },
              err => (this.isLoading = false)
            );
          }
        }
      }
    );
  }

  private transformData(
    data: {
      id: string;
      fields: FieldConfig[];
      formName?: string;
      formDataId?: number | string;
      checked?: boolean;
    }[]
  ) {
    const transformedData = [];
    data.forEach(row => {
      const transformedRow: any = { id: row.id };
      row.fields.forEach(fld => (transformedRow[fld.name] = fld.value));
      transformedData.push(transformedRow);
    });

    return { [this.table.name]: transformedData };
  }

  private transformForm(formValues) {
    for (const key in formValues) {
      if (formValues[key] && typeof formValues[key] === "object" && formValues[key].length && typeof formValues[key][0] === "object") {
        this.setArrayValues(key, formValues[key]);
      } else {
        this.parentGroup.controls[key] !== undefined && this.parentGroup.controls[key].setValue(formValues[key]);
      }
    }
  }

  private setArrayValues(arrayName: string, arrayValues: any[]) {
    const array: FormArray = <FormArray>this.parentGroup.controls[arrayName];
    for (const value of arrayValues) {
      const row: FormGroup = <FormGroup>array.controls.find((group: FormGroup) => Object.keys(group.controls)[0] === value.id);
      if (row) {
        const innerGroup: FormGroup = <FormGroup>row.controls[value.id + ""];
        for (const key in value) {
          innerGroup.controls[key] !== undefined && innerGroup.controls[key].setValue(value[key]);
        }
      }
    }
  }

  private getRowValues(rowValues: {
    id: string;
    fields: FieldConfig[];
    formName?: string;
    formDataId?: number | string;
    checked?: boolean;
  }) {
    for (const field of this.table.fields) {
      if (!rowValues.fields.find(fld => fld.name === field.name)) {
        rowValues.fields.push({ ...field, value: null });
      }
    }
    return rowValues;
  }

  private dependsOn() {
    if (!this.table.multiAdd.dependsOn) {
      return {};
    }

    const dependsOn = {};
    this.table.multiAdd.dependsOn.forEach(fieldName => {
      const fld = this.parentGroup.get(fieldName);

      if (!fld) {
        for (const key in this.parentGroup.controls) {
          const controlValue = this.parentGroup.controls[key].value;

          if (controlValue && typeof controlValue === "object" && controlValue.length) {
            for (let row of (<FormArray>this.parentGroup.controls[key]).controls) {
              const key = Object.keys((<FormGroup>row).controls)[0];
              const formControl = (<FormGroup>(<FormGroup>row).controls[key]).controls[fieldName];
              if (formControl) {
                const fieldVal = (formControl as CustomFormControl).code || (formControl as CustomFormControl).value;
                dependsOn[fieldName] ? dependsOn[fieldName].push(fieldVal) : (dependsOn[fieldName] = [fieldVal]);
              }
            }
          }
        }
      } else if (fld.value !== undefined && fld.value !== null) {
        dependsOn[fieldName] = fld.value;
      }
    });

    return dependsOn;
  }

  multiAddDisabled() {
    if (this.table.multiAdd.dependsOn && this.table.multiAdd.dependsOn.length) {
      return Object.keys(this.dependsOn()).length !== this.table.multiAdd.dependsOn.length;
    } else {
      return false;
    }
  }

  exportExcel() {
    const fileName = this.table.name + ".xlsx";
    const tableData = this.getTableDataArr();

    const ws: XLSX.WorkSheet = XLSX.utils.aoa_to_sheet(tableData);
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    ws["!cols"] = tableData[0].map((col, i) => ({
      wch: Math.max(...tableData.map(row => (row && row[i] ? row[i].toString().length : 0)))
    }));

    XLSX.utils.book_append_sheet(wb, ws, "Sheet1");
    XLSX.writeFile(wb, fileName);
  }

  exportPdf() {
    const fileName = this.table.name + ".pdf";
    const body = this.getTableDataArr(true);
    const len = body && body.length > 0 ? body[0].length : 0;
    const widths = Array(len).fill("auto");

    const docDefinition = {
      content: [{ table: { headerRows: 1, widths, body } }]
    };

    pdfMake.createPdf(docDefinition).download(fileName);
  }

  private getTableDataArr(isPdf = false) {
    const data = [];

    let row = [];

    this.table.fields.forEach(fld => {
      const val = isPdf ? { text: fld.label, bold: true } : fld.label;
      fld.rendered !== false && row.push(val);
    });
    data.push(row);

    let filteredValues = this.applyFilter();

    if (this.tableData.sortParams.length > 0) {
      filteredValues.sort((u1, u2) => this.sortData(u1, u2, this.tableData.sortParams, this.tableData.sortDirs));
    }

    for (const parentRow of filteredValues) {
      const key = Object.keys(parentRow.controls)[0];
      const innerRow = parentRow.controls[key];

      row = [];

      for (const fld of this.table.fields) {
        fld.rendered !== false && row.push(innerRow.controls[fld.name].value);
      }

      data.push(row);
    }

    return data;
  }

  fastPreview(row: FormGroup) {
    if (this.table.action === "fastPreview") {
      (<CustomFormGroup>row).opened = !(<CustomFormGroup>row).opened;
    }
  }

  performAction(dtl: FormGroup, index: number) {
    if (this.actionAvailable(dtl, index)) {
      this.table.action === "fastPreview" ? this.fastPreview(dtl) : this.navigate(dtl);
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

  swapUp(index: number) {
    const position = index;
    index += (this.tableData.pageIndex || 0) * this.tableData.pageSize;
    const prev = this.array.controls[index - 1];

    this.array.controls[index - 1] = this.array.controls[index];
    this.array.controls[index] = prev;

    const previousPage = (this.tableData.pageIndex || 0) - 1;
    position === 0 && (this.tableData.pageIndex = previousPage);
    this.getData();
  }

  swapDown(index: number) {
    const position = index;
    index += (this.tableData.pageIndex || 0) * this.tableData.pageSize;
    const next = this.array.controls[index + 1];

    this.array.controls[index + 1] = this.array.controls[index];
    this.array.controls[index] = next;

    const nextPage = (this.tableData.pageIndex || 0) + 1;
    position === this.tableData.pageSize - 1 && (this.tableData.pageIndex = nextPage);
    this.getData();
  }

  reorderDisabled() {
    if ((this.input && this.input.nativeElement.value) || this.tableData.sortParams.length) {
      return true;
    }

    if (this.table.searchType === "column") {
      let allNull = true;
      Object.keys(this.filterGroup.value).forEach(key => (allNull = allNull && !this.filterGroup.value[key]));
      return !allNull;
    }

    return false;
  }

  columnNumber() {
    return this.table.fields.filter(fld => fld.rendered !== false).length;
  }

  filterTableValues(event: { fieldVal: string; multiselectField: string }) {
    if (this.table.fields.find(fld => event.multiselectField === fld.name) && this.table.multiAdd) {
      this.table.values = this.table.values.filter(row => {
        const rowFld = row.fields.find(fld => fld.name === event.multiselectField);
        return rowFld.value != event.fieldVal;
      });

      this.array.controls = this.array.controls.filter(row => {
        const key = Object.keys((<FormGroup>row).controls)[0];
        const innerGroup = <FormGroup>(<FormGroup>row).controls[key];
        return innerGroup.controls[event.multiselectField].value != event.fieldVal;
      });

      const fldName = this.table.multiAdd.comparisonColumn;

      this.checkedList = this.checkedList.filter(val => {
        let found = false;

        for (const parentControl of this.array.controls) {
          const key = Object.keys((<FormGroup>parentControl).controls)[0];
          const innerControl = (<FormGroup>parentControl).controls[key];

          const fldVal = fldName ? (<FormGroup>innerControl).controls[fldName].value : key;
          found = found || fldVal == val;
        }

        return found;
      });

      this.getData();
    }
  }

  saveReorder() {
    this.isLoading = true;

    const rowValues = this.array.getRawValue();
    const orderValues: any[] = [];

    for (let i = 0; i < rowValues.length; i++) {
      const id = Object.keys(rowValues[i])[0];
      orderValues.push({ [this.table.paramName]: id, ordernum: i + 1 });
    }

    this.formDataService
      .submitForm({
        data: orderValues,
        procedureName: this.table.orderProcedure,
        operationType: 15
      })
      .subscribe(res => {
        this.snackbarService.openSuccessSnackbar(this.messageService.successMessages.saveSuccess);
        this.isLoading = false;
      });
  }

  hasNewRecordButton() {
    return (
      this.table.createNew &&
      !this.table.createNewHidden &&
      !this.isMonRuo &&
      !this.isHistory &&
      !this.isLocked &&
      !(this.table.disabledViewMode && this.mode === this.modes.View) &&
      (!this.submissionPeriod || (this.submissionPeriod.isOpenPeriod && this.table.values.length === 0)) &&
      ((this.sysRoleID == "0" && this.isOpenCampaign) || this.tab !== Tabs.list)
    );
  }
}
