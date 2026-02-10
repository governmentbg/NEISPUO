import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { AppToastMessageService } from '@core/services/app-toast-message.service';
import { RoleEnumTranslation } from '@shared/enums/role.enum';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-saved-report-card',
  templateUrl: './saved-report-card.component.html',
  styleUrls: ['./saved-report-card.component.scss']
})
export class SavedReportCardComponent implements OnInit, OnDestroy {
  @Input() report;
  public sharedWith: [];
  public selectedRole$ = this.authQuery.select('selected_role');
  public currentUserId: number;
  private subscription: Subscription;

  get isCreatedByVisible() {
    return this.report.CreatedBy.SysUserID !== this.currentUserId;
  }

  get sharedWithRoles() {
    return this.report.SharedWith.map((r) => RoleEnumTranslation[r]).join(', ');
  }

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private authQuery: AuthQuery,
    private toastService: AppToastMessageService
  ) {}

  ngOnInit(): void {
    this.subscription = this.selectedRole$.subscribe({
      next: (r) => {
        this.currentUserId = r.SysUserID;
      },
      error: (err) => {
        this.toastService.displayError();
      }
    });
  }

  goToReport() {
    this.router.navigate([`report-builder/${this.report.DatabaseView}/${this.report.ReportID}`], {
      relativeTo: this.route,
      queryParams: {
        visualization: this.report.visualization
      }
    });
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
