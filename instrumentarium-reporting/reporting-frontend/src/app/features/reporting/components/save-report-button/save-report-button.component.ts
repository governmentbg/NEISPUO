import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { Report } from '@core/models/report.model';
import { PieChartTypeReportService } from '@reporting/services/pie-chart-type-report.service';
import { PivotTableTypeReportService } from '@reporting/services/pivot-table-type-report.service';
import { ReportService } from '@reporting/services/report.service';
import { SavedReportsService } from '@reporting/services/saved-reports.service';
import { TableTypeReportService } from '@reporting/services/table-type-report.service';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { combineLatest, take } from 'rxjs';
import { SaveReportFormComponent } from '../save-report-form/save-report-form.component';

@Component({
  selector: 'app-save-report-button',
  templateUrl: './save-report-button.component.html',
  styleUrls: ['./save-report-button.component.scss']
})
export class SaveReportButtonComponent implements OnInit {
  public report: Report;
  public currentUserId: number;
  public selectedRole$ = this.authQuery.select('selected_role');
  private reportOwnerId: number;
  ref: DynamicDialogRef;
  routeParams: any = {};

  get componentShouldInit() {
    return this.reportOwnerId === this.currentUserId || this.reportService.isCubeData;
  }

  get isSaveButtonDisabled() {
    return (
      this.tableTypeReportService.isLoading || this.pivotTableTypeReportService.isLoading || this.pieChartService.isLoading
    );
  }

  constructor(
    private dialogService: DialogService,
    private route: ActivatedRoute,
    public savedReportsService: SavedReportsService,
    public reportService: ReportService,
    private authQuery: AuthQuery,
    private tableTypeReportService: TableTypeReportService,
    private pivotTableTypeReportService: PivotTableTypeReportService,
    private pieChartService: PieChartTypeReportService
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe({
      next: (params) => {
        this.routeParams = params;
      }
    });

    if (!this.reportService.isCubeData) {
      combineLatest([this.savedReportsService.savedReport$, this.selectedRole$])
        .pipe(take(1))
        .subscribe({
          next: ([report, selectedRole]) => {
            this.reportOwnerId = report.CreatedBy.SysUserID;
            this.currentUserId = selectedRole.SysUserID;
          }
        });
    }
  }

  openSaveReportDialog() {
    const { databaseView, reportId } = this.routeParams;
    this.ref = this.dialogService.open(SaveReportFormComponent, {
      header: this.getHeader(reportId),
      width: '40%',
      data: {
        databaseView,
        reportId
      }
    });
  }

  getHeader(reportId: number): string {
    return reportId ? 'Редактирай справка' : 'Нова справка';
  }
}
