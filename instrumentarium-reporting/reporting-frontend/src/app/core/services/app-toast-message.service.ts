import { Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';

@Injectable()
export class AppToastMessageService {
  constructor(private messageService: MessageService) {}

  displaySuccess(summary?: string, detail?: string, outletKey: string = 'globalAppToast') {
    this.messageService.clear();
    this.messageService.add({
      severity: 'success',
      summary: summary,
      detail: detail,
      closable: false,
      life: 2000,
      key: outletKey
    });
  }

  displayError(detail?: string, outletKey: string = 'globalAppToast') {
    this.messageService.clear();
    this.messageService.add({
      severity: 'error',
      summary: 'Възникна грешка.',
      detail: detail || 'Възникна грешка. Моля опитайте отново.',
      closable: true,
      sticky: true,
      key: outletKey
    });
  }

  displayWarn(summary?: string, detail?: string, outletKey: string = 'globalAppToast') {
    this.messageService.clear();
    this.messageService.add({
      severity: 'warn',
      summary: summary,
      detail: detail || 'Възникна грешка. Моля опитайте отново.',
      closable: true,
      sticky: true,
      key: outletKey
    });
  }
}
