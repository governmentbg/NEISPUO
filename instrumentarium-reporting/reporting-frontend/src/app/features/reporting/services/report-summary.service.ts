import { Injectable } from '@angular/core';
import { ReportingSidebarService } from '@reporting/components/reporting-sidebar/reporting-sidebar.service';
import { VisualizationTypeEnum } from '@shared/enums/visualization-type.enum';
import { BehaviorSubject } from 'rxjs';
import { PieChartTypeReportService } from './pie-chart-type-report.service';
import { PivotTableTypeReportService } from './pivot-table-type-report.service';
import { ReportService } from './report.service';

@Injectable()
export class ReportSummaryService {
  private _sumarizedBy = new BehaviorSubject<any>({});
  private _summaryGroupedBy = new BehaviorSubject<any>([]);

  set sumarizedBy(response) {
    this._sumarizedBy.next(response);
  }

  get sumarizedBy() {
    return this._sumarizedBy.getValue();
  }

  set summaryGroupedBy(response) {
    this._summaryGroupedBy.next(response);
  }

  get summaryGroupedBy() {
    return this._summaryGroupedBy.getValue();
  }

  constructor(
    private reportService: ReportService,
    private pivotTableTypeReportService: PivotTableTypeReportService,
    private pieChartTypeReportService: PieChartTypeReportService,
    private sidebarService: ReportingSidebarService
  ) {}

  async updateSummary(event: any) {
    if (event.value?.aggType) {
      this.sumarizedBy = event.value;
      this.reportService.selectedMeasures = [event.value];
      if (!this.summaryGroupedBy.length && this.sidebarService.visualization === VisualizationTypeEnum.pivot_table) {
        this.pivotTableTypeReportService.lazyLoadOnInit = false;
      }
      if (!this.summaryGroupedBy.length && this.sidebarService.visualization === VisualizationTypeEnum.pie_chart) {
        this.pieChartTypeReportService.loadOnInit = false;
      }
    } else {
      this.summaryGroupedBy = event.value;
      this.reportService.selectedDimensions = event.value;
    }
  }

  cleanUpData() {
    this.sumarizedBy = {};
    this.summaryGroupedBy = [];
  }
}
