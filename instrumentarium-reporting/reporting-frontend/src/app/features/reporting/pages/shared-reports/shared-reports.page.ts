import { Component, OnInit, ViewChild } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { AppToastMessageService } from '@core/services/app-toast-message.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-shared-reports',
  templateUrl: './shared-reports.page.html',
  styleUrls: ['./shared-reports.page.scss']
})
export class SharedReportsPage implements OnInit {
  @ViewChild('sharedReportsDataView') sharedReportsDataView;
  public isLoading: boolean = false;

  constructor(
    private apiService: ApiService, 
    private toastService: AppToastMessageService
  ) {}
  public sharedReports: any[];

  ngOnInit(): void {
    this.isLoading = true;
    this.loadSharedReports();
  }

  loadSharedReports() {
    this.apiService
      .get('/v1/shared-reports')
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (reports) => {
          this.sharedReports = reports.data;
        },
        error: (err) => {
          this.toastService.displayError('Възникна грешка при зареждане на данните. Моля опитайте отново.');
        }
      });
  }

  applyFilter(event: any, matchMode: string) {
    this.sharedReportsDataView.filter((event.target as HTMLInputElement).value, matchMode);
  }
}
