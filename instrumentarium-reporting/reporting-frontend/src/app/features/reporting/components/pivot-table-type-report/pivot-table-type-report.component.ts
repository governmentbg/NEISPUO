import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { AppToastMessageService } from '@core/services/app-toast-message.service';
import { ReportBuilderQuery } from '@reporting/pages/report-builder/state/report-builder.query';
import { DrillDownService } from '@reporting/services/drill-down.service';
import { PivotTableTypeReportService } from '@reporting/services/pivot-table-type-report.service';
import { ReportSummaryService } from '@reporting/services/report-summary.service';
import { ReportService } from '@reporting/services/report.service';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { DrillDownTableComponent } from '../drill-down-table/drill-down-table.component';
import { ReportingSidebarService } from '../reporting-sidebar/reporting-sidebar.service';
import { VisualizationTypeEnum } from '@shared/enums/visualization-type.enum';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-pivot-table-type-report',
  templateUrl: './pivot-table-type-report.component.html',
  styleUrls: ['./pivot-table-type-report.component.scss']
})
export class PivotTableTypeReportComponent implements OnInit, OnDestroy {
  @ViewChild('rbPivotTable') rbPivotTable: any = null;
  public report$ = this.rbQuery.report$;
  ref: DynamicDialogRef;
  public visualizatonType: VisualizationTypeEnum = VisualizationTypeEnum.pivot_table;
  private dialogRefSubscription: Subscription;

  constructor(
    public reportService: ReportService,
    public pivotTableReportService: PivotTableTypeReportService,
    public reportSummaryService: ReportSummaryService,
    private rbQuery: ReportBuilderQuery,
    public dialogService: DialogService,
    private drillDownService: DrillDownService,
    private toastService: AppToastMessageService,
    public sidebarService: ReportingSidebarService
  ) {}

  ngOnInit(): void {
    if (!this.reportSummaryService.sumarizedBy.name) {
      return;
    }
    this.pivotTableReportService.lazyLoadOnInit = true;
  }

  openDrillDownDialog(dataIndex: string, rowIndex: number) {
    if (!this.reportService.columnIsMeasure(dataIndex)) {
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

  isISODate(value: string): boolean {
    const date = value['RPersonal.BirthDate'];

    try {
      const regex = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})\.(\d{3})Z$/;
      return regex.test(date);
    } catch (error) {
      return false;
    }
  }

  ngOnDestroy(): void {
    this.pivotTableReportService.lazyLoadOnInit = true;
    if (this.dialogRefSubscription) {
      this.dialogRefSubscription.unsubscribe();
    }
  }
}
