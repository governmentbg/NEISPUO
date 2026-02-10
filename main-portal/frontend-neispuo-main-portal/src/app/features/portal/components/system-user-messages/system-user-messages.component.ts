import { Component, OnInit } from '@angular/core';
import { NeispuoModuleService } from '@portal/neispuo-modules/neispuo-module.service';
import { SystemUserMessagesService } from '@portal/services/system-user-messages.service';
import { of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { DialogService } from 'primeng/dynamicdialog';
import { SystemMessageFormComponent } from '../system-message-form/system-message-form.component';
import { ToastService } from '@shared/services/toast.service';
import { SystemMessagePayload } from '../system-message-form/system-message-form.interface';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { ConfirmationService } from 'primeng/api';
import { SystemUserMessage } from './system-user-message.interface';

@Component({
  selector: 'app-system-user-messages',
  templateUrl: './system-user-messages.component.html',
  styleUrls: ['./system-user-messages.component.scss']
})
export class SystemUserMessagesComponent implements OnInit {
  messages: SystemUserMessage[] = [];

  expandedMessages: Record<number, boolean> = {};

  loading = false;

  error = false;

  errorMessage = '';

  constructor(
    private readonly nmService: NeispuoModuleService,
    private readonly sumService: SystemUserMessagesService,
    private readonly dialogService: DialogService,
    private readonly toastService: ToastService,
    private readonly confirmationService: ConfirmationService,
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit(): void {
    this.loadMessages();
    this.nmService
      .getCategories()
      .pipe(
        catchError((_) => {
          return of([]);
        })
      )
      .subscribe();
  }

  loadMessages(): void {
    this.loading = true;
    this.error = false;
    this.errorMessage = '';

    this.sumService
      .getSystemUserMessages()
      .pipe(
        finalize(() => {
          this.loading = false;
        }),
        catchError(() => {
          this.error = true;
          this.errorMessage = 'Грешка при зареждане на съобщенията';
          return of([]);
        })
      )
      .subscribe((messages) => {
        this.messages = messages;
      });
  }

  openAddModal(): void {
    const ref = this.dialogService.open(SystemMessageFormComponent, {
      header: 'Ново съобщение',
      width: '80%',
      contentStyle: { overflow: 'auto' },
      baseZIndex: 10000
    });

    ref.onClose.subscribe((formData: SystemMessagePayload) => {
      if (formData) {
        this.sumService.createSystemMessage(formData).subscribe({
          next: () => {
            this.refreshMessages();
            this.toastService.initiate({
              content: 'Съобщението е създадено успешно.',
              style: 'success',
              sticky: false,
              position: 'bottom-right'
            });
          },
          error: (_) => {
            this.toastService.initiate({
              content: 'Грешка при създаване на съобщение.',
              style: 'error',
              sticky: false,
              position: 'bottom-right'
            });
          }
        });
      }
    });
  }

  editMessage(message: SystemMessagePayload): void {
    const ref = this.dialogService.open(SystemMessageFormComponent, {
      header: 'Редакция на съобщение',
      width: '80%',
      data: message,
      contentStyle: { overflow: 'auto' },
      baseZIndex: 10000
    });

    ref.onClose.subscribe((updatedData: SystemMessagePayload) => {
      if (updatedData) {
        this.sumService.updateSystemMessage(message.id, updatedData).subscribe({
          next: () => {
            this.refreshMessages();
            this.toastService.initiate({
              content: 'Съобщението е обновено успешно.',
              style: 'success',
              sticky: false,
              position: 'bottom-right'
            });
          },
          error: (_) => {
            this.toastService.initiate({
              content: 'Грешка при обновяване на съобщението.',
              style: 'error',
              sticky: false,
              position: 'bottom-right'
            });
          }
        });
      }
    });
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
        this.sumService.deleteSystemMessage(id).subscribe({
          next: () => {
            this.refreshMessages();
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

  toggleMessage(id: number): void {
    this.expandedMessages[id] = !this.expandedMessages[id];
  }

  refreshMessages(): void {
    this.loadMessages();
  }

  getTrustedHtml(html: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(html);
  }
}
