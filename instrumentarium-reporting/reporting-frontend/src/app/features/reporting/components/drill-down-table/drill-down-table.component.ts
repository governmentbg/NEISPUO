import { Component, OnInit, ViewChild } from '@angular/core';
import { DrillDownService } from '@reporting/services/drill-down.service';
import { ReportService } from '@reporting/services/report.service';
import { PrimengConfigService } from '@shared/services/primeng-config.service';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';

@Component({
  selector: 'app-drill-down-table',
  templateUrl: './drill-down-table.component.html',
  styleUrls: ['./drill-down-table.component.scss']
})
export class DrillDownTableComponent implements OnInit {
  @ViewChild('ddTable') ddTable: Table;
  public dataIndex: string;
  public rowIndex: number;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    public reportService: ReportService,
    public drillDownService: DrillDownService,
    public primengConfig: PrimengConfigService,
  ) {}

  ngOnInit(): void {
    this.dataIndex = this.config.data.dataIndex;
    this.rowIndex = this.config.data.rowIndex;
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

  isRequiredDrillDownQueryFilter(columnName: string) {
    // Checks if column name is part of the drill-down required query filters
    const initialFilters = this.drillDownService.requiredDrillDownQueryFilters.map((f) => f.member);
    return initialFilters.includes(columnName);
  }
}
