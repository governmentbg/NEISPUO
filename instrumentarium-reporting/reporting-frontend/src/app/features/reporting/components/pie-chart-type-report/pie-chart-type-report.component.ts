import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { AppToastMessageService } from '@core/services/app-toast-message.service';
import { ReportBuilderQuery } from '@reporting/pages/report-builder/state/report-builder.query';
import { DrillDownService } from '@reporting/services/drill-down.service';
import { PieChartTypeReportService } from '@reporting/services/pie-chart-type-report.service';
import { ReportSummaryService } from '@reporting/services/report-summary.service';
import { ReportService } from '@reporting/services/report.service';
import { CONSTANTS } from '@shared/constants';
import { BaseChartDirective } from 'ng2-charts';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { DrillDownTableComponent } from '../drill-down-table/drill-down-table.component';
import { ReportingSidebarService } from '../reporting-sidebar/reporting-sidebar.service';
import { Subscription } from 'rxjs';
@Component({
  selector: 'app-pie-chart-type-report',
  templateUrl: './pie-chart-type-report.component.html',
  styleUrls: ['./pie-chart-type-report.component.scss']
})
export class PieChartTypeReportComponent implements OnInit, OnDestroy {
  @ViewChild(BaseChartDirective) private chart: BaseChartDirective;
  ref: DynamicDialogRef;
  private dialogRefSubscription: Subscription;
  constructor(
    public reportService: ReportService,
    public reportSummaryService: ReportSummaryService,
    public pieChartService: PieChartTypeReportService,
    public sidebarService: ReportingSidebarService,
    public dialogService: DialogService,
    private drillDownService: DrillDownService,
    private rbQuery: ReportBuilderQuery,
    private toastService: AppToastMessageService
  ) {}

  ngOnInit(): void {
    if (!this.reportSummaryService.sumarizedBy.name) {
      return;
    }

    if (this.pieChartService.loadOnInit) {
      this.pieChartService.loadPie(this.reportService.lastLazyLoadEvent, this.pieChartService.pieChartPivotConfig);
    }
  }

  openDrillDownDialog(event: any) {
    this.drillDownService.isPieChartData = true;
    const dataIndex = this.chart.datasets[0].label;
    const rowIndex = event.active[0].index;
    if (rowIndex === CONSTANTS.PIE_CHART_LAST_SLICE_INDEX || dataIndex.split(',').length > 1) {
      // dataIndex.split(',').length > 1 temporarily disable drill-down functionality of the pivot pie chart
      return;
    }

    this.ref = this.dialogService.open(DrillDownTableComponent, {
      header: this.rbQuery.getValue().report.title,
      width: '90vw',
      contentStyle: { 'max-height': '90vh', overflow: 'hidden' },
      baseZIndex: 10000,
      data: {
        dataIndex,
        rowIndex
      }
    });

    this.dialogRefSubscription = this.ref.onClose.subscribe({
      next: () => {
        this.drillDownService.cleanUpData();
      },
      error: (err) => {
        this.toastService.displayError();
      }
    });
  }

  ngOnDestroy(): void {
    this.pieChartService.loadOnInit = true;
    this.pieChartService.pieChartPivotConfig = null;

    if (this.dialogRefSubscription) {
      this.dialogRefSubscription.unsubscribe();
    }
  }
}
