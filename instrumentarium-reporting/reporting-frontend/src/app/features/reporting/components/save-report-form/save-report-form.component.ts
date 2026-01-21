import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SavedReportsService } from '@reporting/services/saved-reports.service';
import { ReportService } from '@reporting/services/report.service';
import { SelectItem } from 'primeng/api';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ReportingSidebarService } from '../reporting-sidebar/reporting-sidebar.service';
import { RoleEnum, RoleEnumTranslation } from '@shared/enums/role.enum';
import { ReportSummaryService } from '@reporting/services/report-summary.service';
import { PivotTableTypeReportService } from '@reporting/services/pivot-table-type-report.service';
import { Router } from '@angular/router';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { Report } from '@core/models/report.model';
import { VisualizationTypeEnum } from '@shared/enums/visualization-type.enum';
import { PieChartTypeReportService } from '@reporting/services/pie-chart-type-report.service';
import { AppToastMessageService } from '@core/services/app-toast-message.service';
import { Subscription, take } from 'rxjs';

export type PageType = 'create' | 'update';
enum ShareReportErrorEnum {
  ROLE_NOT_ALLOWED_TO_SHARE = 'ROLE_NOT_ALLOWED_TO_SHARE',
  ROLES_NOT_ALLOWED_TO_SHARE_WITH = 'ROLES_NOT_ALLOWED_TO_SHARE_WITH'
}

@Component({
  selector: 'app-save-report-form',
  templateUrl: './save-report-form.component.html',
  styleUrls: ['./save-report-form.component.scss']
})
export class SaveReportFormComponent implements OnInit, OnDestroy {
  public rolesToShareWith: SelectItem[];
  public form: FormGroup;
  private report: Report;
  public pageType: PageType;
  public selectedRole$ = this.authQuery.select('selected_role');
  private currentUserRole: RoleEnum;
  private subscription: Subscription;

  constructor(
    public ref: DynamicDialogRef,
    private sidebarService: ReportingSidebarService,
    public reportService: ReportService,
    private savedReportsService: SavedReportsService,
    private reportSummaryService: ReportSummaryService,
    private pivotTableTypeReportService: PivotTableTypeReportService,
    public config: DynamicDialogConfig,
    private fb: FormBuilder,
    private router: Router,
    private authQuery: AuthQuery,
    private pieChartService: PieChartTypeReportService,
    private toastService: AppToastMessageService
  ) {}

  ngOnInit(): void {
    const reportId = this.config.data.reportId;
    this.subscription = this.selectedRole$.subscribe({
      next: (role) => {
        this.currentUserRole = role.SysRoleID;
        this.rolesToShareWith = this.getRolesToShareWithSelectionOptions(role.SysRoleID);
      },
      error: (err) => {
        this.toastService.displayError();
      }
    });

    if (reportId) {
      this.pageType = 'update';
      this.savedReportsService
        .getReport('saved-reports', reportId)
        .pipe(take(1))
        .subscribe({
          next: (report) => {
            this.report = report;
            this.initForm(report);
          },
          error: (err) => {
            this.toastService.displayError('Възникна грешка при зареждане на данните. Моля опитайте отново.');
          }
        });
    } else {
      this.pageType = 'create';
      this.initForm();
    }
  }

  getRolesToShareWithSelectionOptions(currentUserRole: RoleEnum): SelectItem[] {
    return this.savedReportsService
      .scopeRolesToShareWith(currentUserRole)
      .map((r) => ({ label: RoleEnumTranslation[r], value: r }));
  }

  private initForm(report?: Report) {
    this.form = this.fb.group({
      Name: [report?.Name || null, Validators.required],
      Description: [report?.Description || null],
      SharedWith: [report?.SharedWith.length ? [...report?.SharedWith] : []]
    });
  }

  onSubmit() {
    if (this.form.invalid) {
      return;
    }

    const report = {
      ...this.form.getRawValue(),
      DatabaseView: this.config.data.databaseView,
      Visualization: this.sidebarService.visualization,
      Query: {
        ...this.reportService.lastLazyLoadEvent,
        measures: this.reportService.selectedMeasures,
        dimensions: this.reportService.selectedDimensions,
        columns: this.reportService.selectedColumns,
        sumarizedBy: this.reportSummaryService.sumarizedBy,
        summaryGroupedBy: this.reportSummaryService.summaryGroupedBy,
        pivotConfig:
          this.sidebarService.visualization === VisualizationTypeEnum.pivot_table
            ? this.pivotTableTypeReportService.tablePivotConfig
            : this.pieChartService.pieChartPivotConfig
      }
    };

    if (this.pageType === 'create') {
      this.savedReportsService
        .createReport(report)
        .pipe(take(1))
        .subscribe({
          next: (success) => {
            this.toastService.displaySuccess('Справката беше успешно създадена.');
            this.router.navigateByUrl('/', { skipLocationChange: true }).then(() =>
              this.router.navigate([`/saved-reports/report-builder/${success.DatabaseView}/${success.ReportID}`], {
                queryParams: { visualization: success.Visualization }
              })
            );
          },
          error: (err) => this.toastService.displayError(this.getErrorMessage(err, 'create'))
        });
    } else if (this.pageType === 'update') {
      this.savedReportsService
        .updateReport(this.config.data.reportId, report)
        .pipe(take(1))
        .subscribe({
          next: (success) => {
            this.toastService.displaySuccess('Промените по справката бяха успешно запазени.');
            this.router.navigateByUrl('/', { skipLocationChange: true }).then(() =>
              this.router.navigate([`/saved-reports/report-builder/${success.DatabaseView}/${success.ReportID}`], {
                queryParams: { visualization: success.Visualization }
              })
            );
          },
          error: (err) => this.toastService.displayError(this.getErrorMessage(err, 'update'))
        });
    }
    this.ref.close();
  }

  isNameInvalid() {
    return this.form.get('Name').invalid && this.form.get('Name').touched;
  }

  getErrorMessage(error: any, pageType: PageType): string {
    console.error(error);
    let userFriendlyErrorMessage =
      pageType === 'create'
        ? 'Възникна грешка при запазване на справката.'
        : 'Възникна грешка при запазване на промените по справката.';
    if (error && error.status === 403 && error.error.error === ShareReportErrorEnum.ROLE_NOT_ALLOWED_TO_SHARE) {
      userFriendlyErrorMessage = `Споделянето на справки не е разрешено за потребители с роля ${
        RoleEnumTranslation[this.currentUserRole]
      }`;
    }
    if (error && error.status === 403 && error.error.error === ShareReportErrorEnum.ROLES_NOT_ALLOWED_TO_SHARE_WITH) {
      userFriendlyErrorMessage = `Потребители с роля ${
        RoleEnumTranslation[this.currentUserRole]
      } могат да споделят справки само със следните роли: ${this.rolesToShareWith.map((r) => r.label).join(', ')}`;
    }

    return userFriendlyErrorMessage;
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
