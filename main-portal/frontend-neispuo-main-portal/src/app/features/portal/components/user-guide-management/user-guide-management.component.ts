import { Component, OnInit } from '@angular/core';
import { UserGuideAdditionModalComponent } from '@portal/components/user-guide-addition-modal/user-guide-addition-modal.component';
import { NeispuoModuleService } from '@portal/neispuo-modules/neispuo-module.service';
import { NeispuoUserGuide } from '@portal/neispuo-modules/neispuo-user-guide.interface';
import { UserGuideManagementService } from '@portal/services/user-guide-management.service';
import { ConfirmationService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ToastService } from '@shared/services/toast.service';
import { SubSink } from 'subsink';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Component({
  selector: 'app-user-guide-management',
  templateUrl: './user-guide-management.component.html',
  styleUrls: ['./user-guide-management.component.scss']
})
export class UserGuideManagementComponent implements OnInit {
  userGuides: any[] = [];

  loading = false;

  error = false;

  errorMessage = '';

  subs = new SubSink();

  ref: DynamicDialogRef;

  constructor(
    public userGuideManagementService: UserGuideManagementService,
    private dialogService: DialogService,
    private nmService: NeispuoModuleService,
    private confirmationService: ConfirmationService,
    private toastService: ToastService
  ) {}

  ngOnInit() {
    this.loadCategoriesAndUserGuides();
  }

  openUserGuideModal(userGuide?: NeispuoUserGuide) {
    let title = 'Добавяне на ръководство';
    if (userGuide) {
      title = 'Редактиране на ръководство';
    }
    this.ref = this.dialogService.open(UserGuideAdditionModalComponent, {
      header: title,
      baseZIndex: 1000,
      width: '50%',
      data: {
        userGuide: userGuide
      }
    });

    this.ref.onClose.subscribe((result) => {
      // refresh only if the modal was submitted
      if (result) {
        this.refreshUserGuides();
      }
    });
  }

  deleteUserGuide(id: number) {
    this.confirmationService.confirm({
      header: 'Потвърждение',
      message: 'Сигурни ли сте, че искате да изтриете това ръководство?',
      accept: async () => {
        this.userGuideManagementService.deleteUserGuideByID(id).subscribe({
          next: () => {
            this.toastService.initiate({
              content: 'Успешно изтриване на ръководство.',
              style: 'success',
              sticky: false,
              position: 'bottom-right'
            });
            this.refreshUserGuides();
          },
          error: (_) => {
            this.toastService.initiate({
              content: 'Грешка при изтриване на ръководство.',
              style: 'error',
              sticky: false,
              position: 'bottom-right'
            });
          }
        });
      },
      reject: () => {}
    });
  }

  refreshUserGuides(): void {
    this.loadCategoriesAndUserGuides();
  }

  loadCategoriesAndUserGuides() {
    this.loading = true;
    this.error = false;
    this.errorMessage = '';

    this.nmService
      .getCategories()
      .pipe(
        catchError((error) => {
          this.loading = false;
          this.error = true;
          this.errorMessage = 'Грешка при зареждане на ръководствата';
          return of([]);
        })
      )
      .subscribe((res) => {
        this.loading = false;
        this.userGuideManagementService.loadUserGuides(res);
        this.userGuides = this.userGuideManagementService.userGuides || [];
      });
  }

  shouldDisableDownload(userGuide: NeispuoUserGuide): boolean {
    return !(userGuide && userGuide.filename);
  }

  shouldDisableCopyLink(userGuide: NeispuoUserGuide): boolean {
    if (!userGuide || !userGuide.URLOverride) {
      return true;
    }
    return userGuide.URLOverride.trim() === '';
  }
}
