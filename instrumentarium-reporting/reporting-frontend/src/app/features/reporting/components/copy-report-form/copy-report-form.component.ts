import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { Report } from '@core/models/report.model';
import { AppToastMessageService } from '@core/services/app-toast-message.service';
import { SavedReportsService } from '@reporting/services/saved-reports.service';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'app-copy-report-form',
  templateUrl: './copy-report-form.component.html',
  styleUrls: ['./copy-report-form.component.scss']
})
export class CopyReportFormComponent implements OnInit {
  public form: FormGroup;
  private report: Report;

  constructor(
    public ref: DynamicDialogRef,
    private savedReportsService: SavedReportsService,
    private fb: FormBuilder,
    private router: Router,
    public config: DynamicDialogConfig,
    private toastService: AppToastMessageService
  ) {}

  ngOnInit(): void {
    this.report = this.config.data.report;
    this.initForm(this.report);
  }

  initForm(report: any) {
    this.form = this.fb.group({
      Name: `Копие на ${report.Name}`,
      Description: report.Description
    });
    this.form.get('Name').disable();
    this.form.get('Description').disable();
  }

  onSubmit() {
    delete this.report.ReportID;
    const report = { ...this.report, ...this.form.getRawValue(), SharedWith: [] };
    this.savedReportsService.createReport(report).subscribe({
      next: (success) => {
        this.toastService.displaySuccess('Справката беше успешно копирана.');

        // skipLocationChange is necessary to reload the page to the same url with different param
        this.router.navigateByUrl('/', { skipLocationChange: true }).then(() =>
          this.router.navigate([`/saved-reports/report-builder/${success.DatabaseView}/${success.ReportID}`], {
            queryParams: { visualization: success.Visualization }
          })
        );
      },
      error: (err) => this.toastService.displayError('Възникна грешка при копирането на справката. Моля, опитайте отново.')
    });
    this.ref.close();
  }

  onReject() {
    this.ref.close();
  }
}
