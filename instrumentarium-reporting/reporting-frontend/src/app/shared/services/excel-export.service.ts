import { Injectable } from '@angular/core';
import * as xlsx from 'xlsx-js-style';
import * as fileSaver from 'file-saver';
import * as moment from 'moment';
@Injectable({
  providedIn: 'root'
})
export class ExcelExportService {
  private EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
  private EXCEL_EXTENSION = '.xlsx';

  constructor() {}

  private saveAsExcelFile(buffer: any, fileName: string): void {
    const data: Blob = new Blob([buffer], {
      type: this.EXCEL_TYPE
    });
    fileSaver.saveAs(data, fileName + this.EXCEL_EXTENSION);
  }

  exportExcel(rows: any[], fileName: string, skipHeader: boolean, headers?: any, merge?: any) {
    let worksheet = xlsx.utils.json_to_sheet(rows, { skipHeader: skipHeader });

    if (headers) {
      worksheet = { ...worksheet, ...headers };
    }

    for (const [key, value] of Object.entries(worksheet)) {
      if (typeof value !== 'string') {
        value.s = {
          alignment: {
            vertical: 'top',
            horizontal: 'left',
            wrapText: true
          }
        };
      }
      if (typeof value === 'object' && value?.t === 's' && value?.v !== undefined) {
        const stringValue = value.v;
        const date = new Date(stringValue);

        if (!isNaN(date.getTime())) {
          const formattedDate = moment().format('L');
          worksheet[key].v = formattedDate;
        }
      }
    }

    if (merge) {
      worksheet['!merges'] = merge;
    }

    const workbook = { Sheets: { data: worksheet }, SheetNames: ['data'] };
    const excelBuffer: any = xlsx.write(workbook, {
      bookType: 'xlsx',
      type: 'array',
      cellStyles: true
    });
    this.saveAsExcelFile(excelBuffer, fileName);
  }
}
