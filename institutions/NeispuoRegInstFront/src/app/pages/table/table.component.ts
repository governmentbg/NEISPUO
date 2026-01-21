import { Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { FieldType } from '../../enums/fieldType.enum';
import { FormTypeInt } from '../../enums/formType.enum';
import { Menu } from '../../enums/menu.enum';
import { Mode, ModeInt } from '../../enums/mode.enum';
import { FieldConfig } from '../../models/field.interface';
import { Option } from '../../models/option.interface';
import { Row, Table } from '../../models/table.interface';
import { FormDataService } from '../../services/form-data.service';
import { MatMultiSortTableDataSource } from '../../shared/multisort/mat-multi-sort-data-source';
import { MatMultiSort } from '../../shared/multisort/mat-multi-sort.directive';
import { TableData } from '../../shared/multisort/table-data';
import { environment } from '../../../environments/environment';
import { AuthService } from 'src/app/auth/auth.service';
import { MessagesService } from '../../services/messages.service';
import { MatDialog } from '@angular/material/dialog';
import { ModalComponent } from '../../shared/modal/modal.component';

@Component({
  selector: 'app-table',
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.scss']
})
export class TableComponent implements OnInit, OnDestroy {
  @Input() table: Table;
  @Input() parentGroup: FormGroup;
  @Input() mode: Mode;

  @Output() addRecordActive: EventEmitter<boolean> = new EventEmitter<boolean>();

  displayedColumns: string[] = [];
  tableData: TableData<FormGroup>;

  columns: { id: string; name: string }[] = [];

  @ViewChild('input', { static: false }) input: ElementRef;
  @ViewChild('newRecord', { static: false }) newRecord: ElementRef;
  @ViewChild(MatMultiSort) sort: MatMultiSort;

  private routeSubscription: Subscription;
  array: FormArray;

  length: number = 0;
  row: Row;
  addNewRecord: boolean = false;

  private counter: number = 0;

  constructor(
    private fb: FormBuilder,
    private formDataService: FormDataService,
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthService,
    private messageService: MessagesService,
    private dialog: MatDialog
  ) {}

  get modes() {
    return Mode;
  }

  get type() {
    return FieldType;
  }

  ngOnInit() {
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
      this.tableData.pageSize = 10;
      this.getData();
    }, 0);
  }

  ngOnDestroy() {
    this.routeSubscription && this.routeSubscription.unsubscribe();
  }

  private createFormArray() {
    this.displayedColumns = [];
    this.table.fields.forEach(field => {
      this.columns.push({ id: field.name, name: field.label });
      this.displayedColumns.push(field.name);
    });

    const hasActionsColumn = this.tableWithButtons();
    hasActionsColumn && this.columns.push({ id: 'actions', name: 'Действия' });
    hasActionsColumn && this.displayedColumns.push('actions');

    this.array = this.fb.array([]);
    if (this.table && this.table.values) {
      for (let i = 0; i < this.table.values.length; i++) {
        this.addTableRow(this.table.values[i], i);
      }
    }
  }

  trackByCellId(index, row: FormGroup) {
    return row ? Object.keys(row.controls)[0] : index;
  }

  getField(row: FormGroup, fieldName: string) {
    const key = Object.keys(row.controls)[0];
    const valuesRow = key ? this.table.values.find(value => value.id + '' === key) : { fields: [] };
    return valuesRow.fields.find(field => field.name === fieldName);
  }

  getFormGroup(row: FormGroup) {
    const key = Object.keys(row.controls)[0];
    return row ? row.get(key) : null;
  }

  private applyFilter() {
    const filterValue = this.input ? this.input.nativeElement.value : null;

    if (!filterValue) {
      return this.array.controls.slice();
    } else {
      const newControls = [];

      this.array.controls.forEach((formGroup: FormGroup) => {
        let allControlValues = '';
        const innerFormGroupKey = Object.keys(formGroup.controls)[0];
        const innerGroup = <FormGroup>formGroup.controls[innerFormGroupKey];

        for (let innerKey in innerGroup.controls) {
          const field = this.getField(formGroup, innerKey);
          if (field) {
            const fieldValue = field.type === FieldType.Select ? this.getOptionLabel(field.value, field.options) : field.value;
            fieldValue && (allControlValues += fieldValue);
          }
        }

        if (allControlValues.toLowerCase().indexOf(filterValue.toLowerCase()) !== -1) {
          newControls.push(formGroup);
        }
      });

      return newControls;
    }
  }

  private addTableRow(row: { id: string; fields: FieldConfig[] }, position: number) {
    const idGroup = this.fb.group({});
    const rowGroup = this.fb.group({});
    row.fields.forEach(field => this.formDataService.addControl(field, rowGroup, false, false));
    idGroup.addControl(row.id + '', rowGroup); // just in case id is not a string
    this.array.insert(position, idGroup);
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

    const firstField = this.table.values.find(value => value.id + '' === key1).fields.find(field => field.name === sorting[0]);
    const secondField = this.table.values.find(value => value.id + '' === key2).fields.find(field => field.name === sorting[0]);

    let first = firstField.type === FieldType.Select ? this.getOptionLabel(firstField.value, firstField.options) : firstControl.value;

    let second = secondField.type === FieldType.Select ? this.getOptionLabel(secondField.value, secondField.options) : secondConrol.value;

    first && typeof first === 'string' && (first = first.toLowerCase());
    second && typeof second === 'string' && (second = second.toLowerCase());

    if (first > second) {
      return dirs[0] === 'asc' ? 1 : -1;
    } else if (first < second || first === null) {
      return dirs[0] === 'asc' ? -1 : 1;
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
    const row = this.getRow(dtl);
    if (this.table.createNew) {
      this.row = row;
      this.addNewRecord = true;
      this.addRecordActive.emit(this.addNewRecord);

      setTimeout(() => {
        this.newRecord.nativeElement.scrollIntoView({
          behavior: 'smooth',
          block: 'start'
        });
      }, 0);
    } else if (!row.navigateActiveProc) {
      let queryParams: any = {
        procID: row.procID,
        instKind: row.instKind,
        instType: row.instType,
        instid: row.instid,
        sysuserid: this.authService.getSysUserId(),
        region: this.authService.getRegion()
      };
      environment.production && (queryParams = this.formDataService.encodeParams(queryParams));

      this.router.navigate([`/${Menu.EditProcedure}/${row.formName}`], { queryParams });
    } else {
      const active = row.isActive ? Menu.Active : Menu.Inactive;
      const formType = FormTypeInt[row.instType - 1];

      let queryParams: any = {
        instKind: row.instKind,
        instid: row.instid,
        procID: row.procID,
        sysuserid: this.authService.getSysUserId(),
        region: this.authService.getRegion()
      };
      environment.production && (queryParams = this.formDataService.encodeParams(queryParams));

      this.router.navigate([`/${active}/${formType}/${row.formName}`], { queryParams });
    }
  }

  preview(dtl: FormGroup) {
    const row = this.getRow(dtl);
    return row.navigateActiveProc && row.instType;
  }

  onSort() {
    if (this.table.sortable !== false) {
      this.tableData.onSortEvent();
    }
  }

  async onAddRecord(data: { fields: FieldConfig[]; procedureName: string; formValues: any }) {
    if (!data.fields || !data.fields.length) {
      this.addNewRecord = false;
      this.addRecordActive.emit(this.addNewRecord);
    } else {
      const queryParams = environment.production
        ? this.formDataService.decodeParams(this.route.snapshot.queryParams['q'])
        : this.route.snapshot.queryParams;

      let formValues = data.formValues;
      for (const key in queryParams) {
        if (key !== 't') {
          formValues[key] = queryParams[key];
        }
      }

      let operationType;

      if (this.row) {
        operationType = ModeInt.update;
        formValues.id = this.row.id;
      } else {
        operationType = ModeInt.create;
        formValues.id = null;
      }

      if (operationType === ModeInt.create) {
        await this.createHelper(data.fields);
      } else {
        this.updateHelper(formValues, data.fields);
      }

      this.addNewRecord = false;
      this.addRecordActive.emit(this.addNewRecord);
    }
  }

  private async createHelper(dataFields: FieldConfig[]) {
    const firstRecord = this.table.values.length === 0;

    const id = 'new' + this.counter;
    this.counter++;

    let newRecordValues = {};

    const fields = this.getFields(dataFields, newRecordValues);
    const index = this.mode === this.modes.View ? (this.tableData.pageIndex || 0) * this.tableData.pageSize : null;

    index !== null ? this.table.values.splice(index, 0, { id, fields }) : this.table.values.push({ id, fields });
    this.addTableRow({ ...this.table.values[index !== null ? index : this.table.values.length - 1] }, index);

    setTimeout(() => {
      firstRecord && (this.tableData.dataSource = new MatMultiSortTableDataSource(this.sort, true));
      this.getData();
    }, 0);
  }

  private updateHelper(formValues, dataFields: FieldConfig[]) {
    const dtl: FormGroup = <FormGroup>this.array.controls.find((fg: FormGroup) => Object.keys(fg.controls)[0] === this.row.id + '');

    const row: FormGroup = <FormGroup>dtl.controls[this.row.id + ''];
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

  private getFields(tempFields: FieldConfig[], additionalValues = {}) {
    const fields: FieldConfig[] = [];

    for (const field of this.table.fields) {
      const tmpFld = tempFields.find(fld => fld.name === field.name);

      if (tmpFld && (tmpFld.type == FieldType.Select || tmpFld.type == FieldType.SearchSelect) && tmpFld.value) {
        const option = tmpFld.options.find(option => option.code === tmpFld.value);
        field.value = option.label;
        field.code = option.code;
      } else if (tmpFld && tmpFld.type == FieldType.Checkbox) {
        field.value = tmpFld.value ? 'да' : 'не';
      } else if (tmpFld) {
        field.value = tmpFld.value;
      } else {
        field.value = additionalValues[field.name];
      }

      fields.push({ ...field });
    }

    return fields;
  }

  createNew() {
    this.row = null;
    this.addNewRecord = true;
    this.addRecordActive.emit(this.addNewRecord);

    setTimeout(() => {
      this.newRecord.nativeElement.scrollIntoView({
        behavior: 'smooth',
        block: 'start'
      });
    }, 0);
  }

  tableWithButtons() {
    return (
      (this.table.hasEditButton !== false || this.table.canDeleteRow) && !(this.table.disabledViewMode && this.mode === this.modes.View)
    );
  }

  tableWithOneButton() {
    return (
      ((this.table.hasEditButton !== false && !this.table.canDeleteRow) ||
        (this.table.hasEditButton === false && this.table.canDeleteRow)) &&
      !(this.table.disabledViewMode && this.mode === this.modes.View)
    );
  }

  deleteRow(dtl: FormGroup) {
    const dialogRef = this.dialog.open(ModalComponent, {
      width: '45%',
      data: {
        message: this.messageService.modalQuestions.deleteRecord,
        confirmBtnLbl: 'Да',
        cancelBtnLbl: 'Не'
      }
    });

    dialogRef.afterClosed().subscribe((result: boolean) => {
      if (result) {
        const row = this.getRow(dtl);
        this.table.values = this.table.values.filter(val => val.id != row.id);
        this.array.controls = this.array.controls.filter((control: FormGroup) => Object.keys(control.controls)[0] != row.id);
    
        this.getData();
      }
    });

  }

  getFieldVal(dtl: FormGroup, fieldName: string) {
    const row = this.getRow(dtl);
    const field = row.fields.find(fld => fld.name === fieldName);
    return field.value ? field.value + '' : field.value;
  }

  private getRow(dtl: FormGroup) {
    const key = Object.keys(dtl.controls)[0];
    return this.table.values.find(val => val.id == key);
  }
}
