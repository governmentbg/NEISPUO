import { Component, OnInit } from '@angular/core';
import { ReportBuilderQuery } from '@reporting/pages/report-builder/state/report-builder.query';
import { ReportService } from '@reporting/services/report.service';
import { TableTypeReportService } from '@reporting/services/table-type-report.service';

@Component({
  selector: 'app-table-type-report-settings',
  templateUrl: './table-type-report-settings.component.html',
  styleUrls: ['./table-type-report-settings.component.scss']
})
export class TableTypeReportSettingsComponent implements OnInit {
  constructor(
    private rbQuery: ReportBuilderQuery,
    public reportService: ReportService,
    public tableReportService: TableTypeReportService
  ) {}
  public availableDimensions$ = this.rbQuery.availableDimensions$;

  ngOnInit(): void {}

  async updateTableColumns() {
    this.reportService.selectedColumns = [...this.reportService.selectedDimensions, ...this.reportService.selectedMeasures];
    if (!this.reportService.selectedColumns.length) {
      return;
    }
    await this.tableReportService.load({ ...this.reportService.lastLazyLoadEvent, first: 0 });
  }
}
