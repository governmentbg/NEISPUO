import { Injectable } from '@angular/core';
import { ExcelExportService } from '@shared/services/excel-export.service';
import { ReportService } from './report.service';
import { TableTypeReportService } from './table-type-report.service';
import { PivotTableTypeReportService } from './pivot-table-type-report.service';
import { VisualizationTypeEnum } from '@shared/enums/visualization-type.enum';

@Injectable()
export class ReportExcelExportService {
  constructor(
    private excelExportService: ExcelExportService,
    private reportService: ReportService,
    private tableReportService: TableTypeReportService,
    private pivotTableTypeReportService: PivotTableTypeReportService
  ) {}

  private getColumnsToExport(data) {
    return this.reportService.selectedColumns;
  }

  private createExportRows(data, columnsToExport: any[]) {
    const exportRows: any[] = [];

    for (const d of data) {
      const exportObject = {};
      for (const exportColumn of columnsToExport) {
        let fieldData = d[exportColumn.name];
        exportObject[exportColumn.shortTitle] = fieldData;
      }
      exportRows.push(exportObject);
    }

    return exportRows;
  }

  private createPivotExportRows(data, columnsToExport: any[]) {
    const exportRows: any[] = [];

    for (const d of data) {
      let exportObject = {};

      for (const exportColumn of columnsToExport) {
        const fieldData = this.getFieldData(exportColumn, d);
        exportObject = { ...exportObject, ...fieldData };
      }

      exportRows.push(exportObject);
    }

    return exportRows;
  }

  private getFieldData(column, data) {
    let fieldData = null;

    if (column?.children) {
      const childColumns = column.children;

      fieldData = this.getFieldData(childColumns[0], data);

      for (let childColumn of childColumns) {
        const childData = this.getFieldData(childColumn, data);
        fieldData = { ...fieldData, ...childData };
      }

      const newFieldData = {};

      for (let childTitle in fieldData) {
        const childFieldData = fieldData[childTitle];
        newFieldData[`${column.shortTitle}\n${childTitle}`] = childFieldData;
      }

      return (fieldData = newFieldData);
    }
    return (fieldData = { [column.shortTitle]: data[column.dataIndex] });
  }

  async createExcel(visualizatonType?: VisualizationTypeEnum) {
    if (visualizatonType === VisualizationTypeEnum.pivot_table) {
      const dataPivot = await this.pivotTableTypeReportService.loadPivot(
        this.reportService.lastLazyLoadEvent,
        this.pivotTableTypeReportService.tablePivotConfig
      );

      let pivotTableColsToExport = this.reportService.resultSet.tableColumns(
        this.pivotTableTypeReportService.tablePivotConfig
      );
      this.pivotTableTypeReportService.isLoading = false;
      const rowsToExport = this.createPivotExportRows(dataPivot, pivotTableColsToExport);
      return this.excelExportService.exportExcel(rowsToExport, 'export', false);
    }
    const data = await this.tableReportService.loadTableDataForExcelExport();
    const columnsToExport = this.getColumnsToExport(data);
    const rowsToExport = this.createExportRows(data, columnsToExport);
    this.tableReportService.isLoading = false;
    return this.excelExportService.exportExcel(rowsToExport, 'export', false);
  }
}
