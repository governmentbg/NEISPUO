import { Component, OnInit, ViewChild } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { AppToastMessageService } from '@core/services/app-toast-message.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-saved-reports',
  templateUrl: './saved-reports.page.html',
  styleUrls: ['./saved-reports.page.scss']
})
export class SavedReportsPage implements OnInit {
  @ViewChild('savedReportsDataView') savedReportsDataView;
  public isLoading: boolean = false;

  constructor(
    private apiService: ApiService, 
    private toastService: AppToastMessageService
  ) {}
  public savedReports: any[];

  ngOnInit(): void {
    this.isLoading = true;
    this.loadSavedReports();
  }

  loadSavedReports() {
    this.apiService
      .get('/v1/saved-reports')
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (reports) => {
          this.savedReports = reports.data;
        },
        error: (err) => {
          this.toastService.displayError('Възникна грешка при зареждане на данните. Моля опитайте отново.');
        }
      });
  }

  applyFilter(event: any, matchMode: string) {
    this.savedReportsDataView.filter((event.target as HTMLInputElement).value, matchMode);
  }
}
