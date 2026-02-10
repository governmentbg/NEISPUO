import { Component, OnInit } from '@angular/core';
import { ReportBuilderQuery } from '@reporting/pages/report-builder/state/report-builder.query';
import { PieChartTypeReportService } from '@reporting/services/pie-chart-type-report.service';
import { PivotTableTypeReportService } from '@reporting/services/pivot-table-type-report.service';
import { ReportSummaryService } from '@reporting/services/report-summary.service';
import { ReportService } from '@reporting/services/report.service';
import { TableTypeReportService } from '@reporting/services/table-type-report.service';
import { VisualizationTypeEnum } from '@shared/enums/visualization-type.enum';
import { ReportingSidebarService } from '../reporting-sidebar/reporting-sidebar.service';

@Component({
  selector: 'app-summary',
  templateUrl: './summary.component.html',
  styleUrls: ['./summary.component.scss']
})
export class SummaryComponent implements OnInit {
  constructor(
    private sidebarService: ReportingSidebarService,
    private rbQuery: ReportBuilderQuery,
    public reportSummaryService: ReportSummaryService,
    private tableReportService: TableTypeReportService,
    private reportService: ReportService,
    private pivotTableReportService: PivotTableTypeReportService,
    private pieChartTypeReportService: PieChartTypeReportService
  ) {}
  public availableDimensions$ = this.rbQuery.availableDimensions$;
  public availableMeasures$ = this.rbQuery.availableMeasures$;

  ngOnInit(): void {
    this.sidebarService.change.subscribe((v) => {
      this.sidebarService.isSidebarOpen = v.isSidebarOpen;
      this.sidebarService.visualization = v.visualization;
      this.sidebarService.options = v.options;
    });
  }

  async updateSummaryColumns() {
    this.reportService.selectedColumns = [...this.reportService.selectedDimensions, ...this.reportService.selectedMeasures];
    if (!this.reportService.selectedColumns.length) {
      return;
    }
    await this.pivotTableReportService.updatePivotConfig(
      this.reportService.selectedDimensions.map((d) => d.name),
      this.reportService.selectedMeasures.map((m) => m.name)
    );

    await this.pieChartTypeReportService.updatePivotConfig(
      this.reportService.selectedDimensions.map((d) => d.name),
      this.reportService.selectedMeasures.map((m) => m.name)
    );

    this.tableReportService.first = 0;
    this.pivotTableReportService.first = 0;

    if (this.sidebarService.visualization === VisualizationTypeEnum.table) {
      await this.tableReportService.load(this.reportService.lastLazyLoadEvent);
    }

    if (
      this.sidebarService.visualization === VisualizationTypeEnum.pivot_table &&
      this.reportSummaryService.sumarizedBy.name
    ) {
      this.pivotTableReportService.lazyLoadOnInit = false;
      await this.pivotTableReportService.loadPivot(
        this.reportService.lastLazyLoadEvent || { first: 0 },
        this.pivotTableReportService.tablePivotConfig
      );
    }

    if (
      this.sidebarService.visualization === VisualizationTypeEnum.pie_chart &&
      this.reportSummaryService.sumarizedBy.name
    ) {
      this.pieChartTypeReportService.loadOnInit = false;
      await this.pieChartTypeReportService.loadPie(
        this.reportService.lastLazyLoadEvent || { first: 0 },
        this.pieChartTypeReportService.pieChartPivotConfig
      );
    }
  }
}
