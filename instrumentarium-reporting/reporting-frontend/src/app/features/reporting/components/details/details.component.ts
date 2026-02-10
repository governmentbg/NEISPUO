import { Component, OnInit, OnDestroy } from '@angular/core';
import { Report } from '@core/models/report.model';
import { ReportBuilderQuery } from '@reporting/pages/report-builder/state/report-builder.query';
import { ReportService } from '@reporting/services/report.service';
import { SavedReportsService } from '@reporting/services/saved-reports.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.scss']
})
export class DetailsComponent implements OnInit, OnDestroy {
  public cube: any;
  public report: Report;
  private subscriptions: Subscription = new Subscription();

  constructor(
    private rbQuery: ReportBuilderQuery,
    private savedReportService: SavedReportsService,
    public reportService: ReportService
  ) {}

  ngOnInit(): void {
    this.subscriptions.add(
      this.rbQuery.report$.subscribe({
        next: (r) => {
          this.cube = r;
        },
        error: (err) => {
          console.error(err);
        }
      })
    );

    this.subscriptions.add(
      this.savedReportService.savedReport$.subscribe({
        next: (r) => {
          this.report = r;
        },
        error: (err) => {
          console.error(err);
        }
      })
    );
  }

  ngOnDestroy(): void {
    if (this.subscriptions) this.subscriptions.unsubscribe();
  }
}
