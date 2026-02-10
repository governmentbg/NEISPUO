import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  OnInit,
  ViewChild,
  OnDestroy,
  ChangeDetectionStrategy
} from '@angular/core';
import { AppToastMessageService } from '@core/services/app-toast-message.service';
import { ReportBuilderQuery } from '@reporting/pages/report-builder/state/report-builder.query';
import { DrillDownService } from '@reporting/services/drill-down.service';
import { ReportService } from '@reporting/services/report.service';
import { TableTypeReportService } from '@reporting/services/table-type-report.service';
import { CONSTANTS } from '@shared/constants';
import { PrimengConfigService } from '@shared/services/primeng-config.service';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';
import { DrillDownTableComponent } from '../drill-down-table/drill-down-table.component';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-table-type-report',
  templateUrl: './table-type-report.component.html',
  styleUrls: ['./table-type-report.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TableTypeReportComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('rbTable', { static: false }) rbTable: Table;
  public hasQueryParams: boolean = false;
  public report$ = this.rbQuery.report$;
  private dialogRefSubscription: Subscription;

  CONSTANTS = CONSTANTS;
  ref: DynamicDialogRef;

  constructor(
    public reportService: ReportService,
    public primengConfig: PrimengConfigService,
    public tableTypeReportService: TableTypeReportService,
    public dialogService: DialogService,
    private drillDownService: DrillDownService,
    private rbQuery: ReportBuilderQuery,
    private toastService: AppToastMessageService,
    private changeDetector: ChangeDetectorRef
  ) {}

  ngOnInit(): void {}
  trackByRowId(index: number, item: any): any {
    return item.id || index;
  }

  ngAfterViewInit(): void {
    if (this.reportService.lastLazyLoadEvent) {
      // This is required in order to fill data in the primeng table filter inputs
      this.rbTable.filters = this.reportService.lastLazyLoadEvent.filters;
      this.rbTable.sortOrder = this.reportService.lastLazyLoadEvent.sortOrder;
      this.rbTable.sortField = this.reportService.lastLazyLoadEvent.sortField;
    }
    this.changeDetector.detectChanges();
  }

  shouldShowTooltip(value: string): boolean {
    return value?.length > 30;
  }

  getColumnFilterType(colType: string) {
    if (colType === 'number') {
      return 'numeric';
    } else if (colType === 'time') {
      return 'date';
    } else {
      return 'text';
    }
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

  ngOnDestroy() {
    if (this.dialogRefSubscription) {
      this.dialogRefSubscription.unsubscribe();
    }
  }
}
