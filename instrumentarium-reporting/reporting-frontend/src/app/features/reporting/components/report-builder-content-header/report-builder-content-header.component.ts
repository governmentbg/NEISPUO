import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { Report } from '@core/models/report.model';
import { AppToastMessageService } from '@core/services/app-toast-message.service';
import { PieChartTypeReportService } from '@reporting/services/pie-chart-type-report.service';
import { PivotTableTypeReportService } from '@reporting/services/pivot-table-type-report.service';
import { ReportService } from '@reporting/services/report.service';
import { SavedReportsService } from '@reporting/services/saved-reports.service';
import { TableTypeReportService } from '@reporting/services/table-type-report.service';
import { MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { CopyReportFormComponent } from '../copy-report-form/copy-report-form.component';
import { Subscription, take } from 'rxjs';

@Component({
  selector: 'app-report-builder-content-header',
  templateUrl: './report-builder-content-header.component.html',
  styleUrls: ['./report-builder-content-header.component.scss']
})
export class ReportBuilderContentHeaderComponent implements OnInit, OnDestroy {
  public report: Report;
  private ref: DynamicDialogRef;
  public selectedRole$ = this.authQuery.select('selected_role');
  private subscription: Subscription;

  get isActionButtonDisabled() {
    return (
      this.tableTypeReportService.isLoading || this.pivotTableTypeReportService.isLoading || this.pieChartService.isLoading
    );
  }

  get componentShouldInit() {
    return !this.reportService.isCubeData && !!this.report;
  }

  constructor(
    public savedReportsService: SavedReportsService,
    private dialogService: DialogService,
    public reportService: ReportService,
    private tableTypeReportService: TableTypeReportService,
    private pivotTableTypeReportService: PivotTableTypeReportService,
    private toastService: AppToastMessageService,
    private pieChartService: PieChartTypeReportService,
    private authQuery: AuthQuery,
    private messageService: MessageService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.subscription = this.savedReportsService.savedReport$.subscribe({
      next: (r) => {
        this.report = r;
      },
      error: (err) => {
        this.toastService.displayError();
      }
    });
  }

  openCopyReportDialog() {
    this.ref = this.dialogService.open(CopyReportFormComponent, {
      header: 'Създаване на копие',
      width: '40%',
      data: {
        report: this.report
      }
    });
  }

  deleteReport() {
    this.savedReportsService.deleteReport(this.report.ReportID).pipe(take(1)).subscribe({
      next: (res) => {
        const url = this.router.url.split('/').slice(0, -3).join('/');
        this.router.navigateByUrl(url);
        this.messageService.clear();
        this.messageService.add({
          summary: 'Справката беше изтрита',
          severity: 'success'
        });
      },
      error: (err) => {
        console.error(err);
      }
    });
  }

  ngOnDestroy(): void {
    this.savedReportsService.cleanUpData();
    if(this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
