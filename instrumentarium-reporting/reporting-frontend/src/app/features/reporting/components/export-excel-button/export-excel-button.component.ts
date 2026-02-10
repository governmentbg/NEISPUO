import { Component, Input, OnInit } from '@angular/core';
import { DownloadManagerService } from '@core/services/download-manager.service';
import { ApiService } from '@core/services/api.service';
import { PivotTableTypeReportService } from '@reporting/services/pivot-table-type-report.service';
import { ReportService } from '@reporting/services/report.service';
import { TableTypeReportService } from '@reporting/services/table-type-report.service';
import { VisualizationTypeEnum } from '@shared/enums/visualization-type.enum';

@Component({
  selector: 'app-export-excel-button',
  templateUrl: './export-excel-button.component.html',
  styleUrls: ['./export-excel-button.component.scss']
})
export class ExportExcelButtonComponent implements OnInit {

  get exportTooltip(): string {
    return this.isDisabled
      ? 'Подготвяме вашия файл за изтегляне'
      : 'Изтегли';
  }

  get exportIcon(): string {
    return this.isDisabled ? 'pi pi-spin pi-spinner' : 'pi pi-file-excel';
  }

  @Input()
  visualizationType: VisualizationTypeEnum;

  get isExportExcelButtonDisabled() {
    return this.tableTypeReportService.isLoading || this.pivotTableTypeReportService.isLoading;
  }

  constructor(
    private tableTypeReportService: TableTypeReportService,
    private pivotTableTypeReportService: PivotTableTypeReportService,
    private apiService: ApiService,
    private reportService: ReportService,
    private downloadManager: DownloadManagerService
  ) { }

  isDisabled: boolean = false;

  ngOnInit(): void { }

  exportExcel() {
    debugger
    this.isDisabled = true;
    const id = Date.now().toString();
    const fileName = `${this.reportService.queryObject?.dimensions[0]?.split('.')[0]}_${id}.xlsx` || `report_${id}.xlsx`;
    this.downloadManager.addDownload({
      id,
      fileName,
      status: 'downloading',
      startedAt: new Date()
    });
    this.apiService
      .downloadFile(`/v1/cubejs/download-excel`, { query: this.reportService.queryObject })
      .subscribe(
        (res) => {
          this.downloadManager.updateDownload(id, {
            status: 'completed',
            blob: res,
            finishedAt: new Date()
          });
          // Optionally auto-download
          const url = window.URL.createObjectURL(res);
          const a = document.createElement('a');
          a.href = url;
          a.download = fileName;
          a.click();
          window.URL.revokeObjectURL(url);
        },
        (err) => {
          this.downloadManager.updateDownload(id, {
            status: 'error',
            error: err,
            finishedAt: new Date()
          });
          console.error(err);
        }
      )
      .add(() => {
        this.isDisabled = false;
      });
  }
}
