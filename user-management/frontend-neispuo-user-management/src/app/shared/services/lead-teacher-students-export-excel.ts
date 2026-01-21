import { Injectable } from '@angular/core';
import { IColumn } from '@shared/models/column.interface';
import { ApiService } from '@core/services/api.service';
import { catchError, tap } from 'rxjs/operators';
import { of } from 'rxjs';
import { MessageService } from 'primeng/api';
import { TeacherClassesResponseDTO } from '@shared/business-object-model/responses/teacher-classes-response.dto';
import { DynamicColumnService } from './dynamic-column.service';
import { ExcelExportService } from './excel-export.service';

interface ExportColumn {
    dottedPropName: string;
    propTranslation: string;
}

@Injectable({
    providedIn: 'root',
})
export class LeadTeacherStudentsExportExcelService {
    tableVisibleColumns: IColumn<TeacherClassesResponseDTO>[] = [];

    constructor(
        private excelExportService: ExcelExportService,
        private columnService: DynamicColumnService<TeacherClassesResponseDTO>,
        private apiService: ApiService,
        private messageService: MessageService,
    ) {
        this.columnService.visibleColumns.subscribe((vc) => (this.tableVisibleColumns = vc));
    }

    private getColumnsToExport() {
        const columnsToExport: ExportColumn[] = this.tableVisibleColumns.map((c) => ({
            dottedPropName: c.field as string,
            propTranslation: c.header,
        }));

        return [...columnsToExport];
    }

    private createExportRows(
        teacherClassesResponseDTO: TeacherClassesResponseDTO[],
        columnsToExport: ExportColumn[],
        getDataMethod: (value: any, dotNotatedProperty: string) => string,
    ) {
        const exportRows: any[] = [];
        for (const student of teacherClassesResponseDTO) {
            const exportObject: { [k: string]: string } = {};
            for (const exportColumn of columnsToExport) {
                const translatedHeader = exportColumn.propTranslation;
                const fieldData = getDataMethod(student, exportColumn.dottedPropName);
                exportObject[translatedHeader] = fieldData;
            }
            exportRows.push(exportObject);
        }
        return exportRows;
    }

    createExcel(queryParams: any, getDataMethod: (value: any, dotNotatedProperty: string) => string) {
        return this.apiService.get('/v1/lead-teacher-students', queryParams).pipe(
            tap((success: { data: any }) => {
                const columnsToExport = this.getColumnsToExport();
                const rowsToExport = this.createExportRows(success.data, columnsToExport, getDataMethod);
                this.excelExportService.exportExcel(rowsToExport, 'export');
            }),
            catchError((error) => {
                console.log(error);

                this.messageService.add({
                    severity: 'error',
                    summary: 'Възникна грешка.',
                    detail: error.message,
                });

                return of(false);
            }),
        );
    }
}
