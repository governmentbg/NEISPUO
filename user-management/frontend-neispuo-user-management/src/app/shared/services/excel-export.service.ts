import { Injectable } from '@angular/core';
import * as xlsx from 'xlsx';
import * as fileSaver from 'file-saver';

@Injectable({
    providedIn: 'root',
})
export class ExcelExportService {
    private EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';

    private EXCEL_EXTENSION = '.xlsx';

    private saveAsExcelFile(buffer: any, fileName: string): void {
        const data: Blob = new Blob([buffer], {
            type: this.EXCEL_TYPE,
        });
        fileSaver.saveAs(data, fileName + this.EXCEL_EXTENSION);
    }

    exportExcel(rows: any[], fileName: string) {
        const worksheet = xlsx.utils.json_to_sheet(rows);
        const workbook = { Sheets: { data: worksheet }, SheetNames: ['data'] };
        const excelBuffer: any = xlsx.write(workbook, { bookType: 'xlsx', type: 'array' });
        this.saveAsExcelFile(excelBuffer, fileName);
    }
}
