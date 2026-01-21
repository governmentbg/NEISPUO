import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { EmailTemplateService } from '@portal/services/email-template.service';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ToastService } from '@shared/services/toast.service';
import { ConfirmationService } from 'primeng/api';
import { EmailTemplate } from './interfaces/email-template.interface';
import { NeispuoModuleService } from '@portal/neispuo-modules/neispuo-module.service';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { EmailTemplatePreviewModalComponent } from '@portal/components/email-template-preview-modal/email-template-preview-modal.component';
import { EmailTemplateSendModalComponent } from '@portal/components/email-template-send-modal/email-template-send-modal.component';

@Component({
  selector: 'app-email-template-list',
  templateUrl: './email-template-list.component.html',
  styleUrls: ['./email-template-list.component.scss']
})
export class EmailTemplateListComponent implements OnInit {
  templates$ = this.emailTemplateService.getAllTemplates().pipe(
    catchError((error) => {
      return of([]);
    })
  );

  ref: DynamicDialogRef;

  constructor(
    private readonly nmService: NeispuoModuleService,
    private readonly emailTemplateService: EmailTemplateService,
    private readonly router: Router,
    private readonly toastService: ToastService,
    private readonly confirmationService: ConfirmationService,
    private readonly dialogService: DialogService
  ) {}

  ngOnInit(): void {
    this.nmService
      .getCategories()
      .pipe(
        catchError((_) => {
          return of([]);
        })
      )
      .subscribe();
  }

  openCreatePage(): void {
    this.router.navigate(['/portal/sync-messages/create']);
  }

  editTemplate(template: EmailTemplate): void {
    this.router.navigate(['/portal/sync-messages/edit', template.id]);
  }

  confirmDelete(id: number): void {
    this.confirmationService.confirm({
      header: 'Потвърждение',
      message: 'Сигурни ли сте, че искате да изтриете това съобщение?',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Да',
      rejectLabel: 'Не',
      acceptButtonStyleClass: 'p-button-danger',
      rejectButtonStyleClass: 'p-button-outlined p-button-secondary',
      accept: () => {
        this.emailTemplateService.deleteTemplate(id).subscribe({
          next: () => {
            this.refreshTemplates();
            this.toastService.initiate({
              content: 'Съобщението е изтрито успешно.',
              style: 'success',
              sticky: false,
              position: 'bottom-right'
            });
          },
          error: () => {
            this.toastService.initiate({
              content: 'Грешка при изтриване на съобщението.',
              style: 'error',
              sticky: false,
              position: 'bottom-right'
            });
          }
        });
      }
    });
  }

  refreshTemplates(): void {
    this.templates$ = this.emailTemplateService.getAllTemplates().pipe(
      catchError((_) => {
        return of([]);
      })
    );
  }

  openPreview(template: EmailTemplate): void {
    this.ref = this.dialogService.open(EmailTemplatePreviewModalComponent, {
      header: `Преглед на съобщение - ${template.title}`,
      width: '70%',
      data: { template }
    });
  }

  openSend(template: EmailTemplate): void {
    this.ref = this.dialogService.open(EmailTemplateSendModalComponent, {
      header: `Изпращане на имейл - ${template.title}`,
      width: '30%',
      data: { templateId: template.id }
    });
  }
}
