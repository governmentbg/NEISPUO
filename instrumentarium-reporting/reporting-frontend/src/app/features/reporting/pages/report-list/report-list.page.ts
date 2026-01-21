import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { AppToastMessageService } from '@core/services/app-toast-message.service';
import { CubejsClient } from '@cubejs-client/ngx';
import { CubeJsClientService } from '@shared/services/cubejs-client.service';
import { Subscription, finalize, firstValueFrom } from 'rxjs';
import { AvailableReportsQuery } from 'src/app/features/reporting/avalailable-reports/available-report.query';
import { AvailableReportsService } from 'src/app/features/reporting/avalailable-reports/available-report.service';
import { MessagesModule } from 'primeng/messages';

@Component({
  selector: 'app-report-list',
  templateUrl: './report-list.page.html',
  styleUrls: ['./report-list.page.scss']
})
export class ReportListPage implements OnInit, OnDestroy {
  @ViewChild('reportListDataView') reportListDataView;
  private cubejs: CubejsClient;
  private cubejsSubscription: Subscription;
  public isLoading: boolean = false;

  constructor(
    private cubeJsClientService: CubeJsClientService,
    private arService: AvailableReportsService,
    private arQuery: AvailableReportsQuery,
    private toastService: AppToastMessageService
  ) {}

  public availableReports$ = this.arQuery.select('availableReports');

  async ngOnInit() {
    this.isLoading = true;
    this.cubejs = await firstValueFrom(this.cubeJsClientService.cubeJs$);
    this.cubejsSubscription = this.cubejs
      .meta({})
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: async (v) => {
          this.arService.updateAvailableReports(v.meta.cubes);
        },
        error: (err) => {
          this.toastService.displayError('Възникна грешка при зареждане на данните. Моля опитайте отново.');
        }
      });
  }

  applyFilter(event: any, matchMode: string) {
    this.reportListDataView.filter((event.target as HTMLInputElement).value, matchMode);
  }

  ngOnDestroy(): void {
    if (this.cubejsSubscription) {
      this.cubejsSubscription.unsubscribe();
    }
  }
}
