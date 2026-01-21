import { Component, Input, OnInit, OnChanges, SimpleChanges, ViewChild, AfterViewInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { FieldType } from 'src/app/enums/fieldType.enum';
import { Table } from 'src/app/models/table.interface';

import * as XLSX from "xlsx";
import pdfMake from "pdfmake";

@Component({
  selector: 'app-custom-table',
  templateUrl: './custom-table.component.html',
  styleUrls: ['./custom-table.component.scss']
})
export class CustomTableComponent implements OnInit, OnChanges, AfterViewInit {
  @Input() tableMeta!: Table;
  @Input() tableLabel!: string;
  @Input() tableData: any[] = [];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  displayedColumns: string[] = [];
  displayedFilterColumns: string[] = [];
  columnLabels: { [key: string]: string } = {};

  dataSource = new MatTableDataSource<any>([]);
  filterGroup: FormGroup;

  globalFilter = '';

  get type() {
    return FieldType;
  }

  constructor(private fb: FormBuilder) {
    this.filterGroup = this.fb.group({});
  }

  ngOnInit(): void {
    if (this.tableMeta && this.tableMeta.fields) {
      this.initFilterControls();
    }

    this.filterGroup.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(() => this.applyFilters());
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['tableMeta'] && this.tableMeta) {
      this.setupTable();
      this.initFilterControls();
    }

    if (changes['tableData'] && this.tableData) {
      this.dataSource.data = this.tableData;
      this.applyFilters();
    }
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  private setupTable(): void {
    const fields = this.tableMeta.fields;
    this.displayedColumns = fields.map(f => f.name);
    this.displayedFilterColumns = fields.map(f => f.name + 'filter');

    this.columnLabels = {};
    fields.forEach(f => {
      this.columnLabels[f.name] = f.label;
    });
  }

  private initFilterControls(): void {
    Object.keys(this.filterGroup.controls).forEach(controlName => {
      this.filterGroup.removeControl(controlName);
    });

    this.tableMeta.fields.forEach(field => {
      this.filterGroup.addControl(field.name, new FormControl(''));
    });
  }

  applyFilters(): void {
    const filterValues = this.filterGroup.value;

    this.dataSource.filterPredicate = (data, filter) => {
      const globalMatch = this.globalFilter
        ? Object.values(data).some(value =>
          value?.toString().toLowerCase().includes(this.globalFilter.toLowerCase())
        )
        : true;

      const columnMatch = Object.keys(filterValues).every(key => {
        const filterVal = filterValues[key]?.toLowerCase();
        return !filterVal || data[key]?.toString().toLowerCase().includes(filterVal);
      });

      return globalMatch && columnMatch;
    };

    this.dataSource.filter = Math.random().toString();
  }

  applyGlobalFilter(event: KeyboardEvent): void {
    const inputValue = (event.target as HTMLInputElement).value;
    this.globalFilter = inputValue.trim().toLowerCase();
    this.applyFilters();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  formatCellValue(value: any, type: FieldType): string {
    if (type === FieldType.Date && value) {
      const date = new Date(value);
      if (!isNaN(date.getTime())) {
        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const year = date.getFullYear();

        return `${day}.${month}.${year}`;
      }
      return value;
    }
    return value?.toString() || '';
  }

  getFieldType(columnName: string): FieldType {
    const field = this.tableMeta.fields.find(f => f.name === columnName);
    return field ? field.type : FieldType.Label;
  }

  exportExcel() {
    const fileName = this.tableMeta.name + ".xlsx";
    const aoa = this.getTableDataArr();

    const ws: XLSX.WorkSheet = XLSX.utils.aoa_to_sheet(aoa);
    const wb: XLSX.WorkBook = XLSX.utils.book_new();

    ws["!cols"] = aoa[0].map((_, colIndex) => ({
      wch: Math.max(
        ...aoa.map(row =>
          row[colIndex] ? row[colIndex].toString().length : 10
        )
      )
    }));

    XLSX.utils.book_append_sheet(wb, ws, "Sheet1");
    XLSX.writeFile(wb, fileName);
  }

  exportPdf() {
    const fileName = this.tableMeta.name + ".pdf";
    const body = this.getTableDataArr(true);
    const len = body && body.length > 0 ? body[0].length : 0;
    const widths = Array(len).fill('*');

    const docDefinition = {
      pageOrientation: "landscape",
      pageSize: "A4",
      content: [{ table: { headerRows: 1, widths, body } }]
    };

    pdfMake.createPdf(docDefinition).download(fileName);
  }

  private getTableDataArr(isPdf = false): any[][] {
    const data: any[][] = [];

    const headerRow: any[] = [];
    this.tableMeta.fields.forEach(fld => {
      if (fld.rendered !== false) {
        const label = isPdf ? { text: fld.label, bold: true } : fld.label;
        headerRow.push(label);
      }
    });
    data.push(headerRow);

    for (const rowObj of this.tableData) {
      const row: any[] = [];

      for (const fld of this.tableMeta.fields) {
        if (fld.rendered !== false) {
          let val = rowObj[fld.name];

          if (fld.type === 'date' && val) {
            const date = new Date(val);
            val = !isNaN(date.getTime())
              ? `${String(date.getDate()).padStart(2, '0')}.${String(date.getMonth() + 1).padStart(2, '0')}.${date.getFullYear()}`
              : val;
          }

          row.push(val !== undefined ? val : '');
        }
      }

      data.push(row);
    }

    return data;
  }

}
