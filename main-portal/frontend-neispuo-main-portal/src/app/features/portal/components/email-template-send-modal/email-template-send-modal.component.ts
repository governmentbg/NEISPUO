import { Component } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { EmailTemplateService } from '@portal/services/email-template.service';
import { ToastService } from '@shared/services/toast.service';

@Component({
  selector: 'app-email-template-send-modal',
  templateUrl: './email-template-send-modal.component.html',
  styleUrls: ['./email-template-send-modal.component.scss']
})
export class EmailTemplateSendModalComponent {
  fromDate: Date | null = null;
  toDate: Date | null = null;
  
  private fromDateTouched = false;
  private toDateTouched = false;

  readonly maxSelectableDate: Date = new Date();

  get userInteracted(): boolean {
    return this.fromDateTouched || this.toDateTouched;
  }

  get isDateSelectionInvalid(): boolean {
    const onlyOneProvided = (!!this.fromDate) !== (!!this.toDate);
    const rangeInvalid = this.fromDate && this.toDate ? this.toDate <= this.fromDate : false;
    return onlyOneProvided || rangeInvalid;
  }

  get disabledReason(): string | null {
    if (!this.userInteracted) {
      return null;
    }

    if ((!!this.fromDate) !== (!!this.toDate)) {
      return 'Моля въведете и двете дати или оставете и двете празни.';
    }

    if (this.fromDate && this.toDate && this.toDate <= this.fromDate) {
      return 'Крайната дата трябва да бъде след началната дата.';
    }

    return null;
  }

  get isSendDisabled(): boolean {
    return this.userInteracted && this.isDateSelectionInvalid;
  }

  isSending = false;
  response: { success?: boolean; skipped?: boolean; messageBG?: string } | null = null;

  private readonly templateId: number;

  constructor(
    private readonly emailTemplateService: EmailTemplateService,
    private readonly toastService: ToastService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  ) {
    this.templateId = this.config.data.templateId;
  }

  onDateChange(value: Date | null, field: 'from' | 'to'): void {
    const touched = value !== null;
    
    if (field === 'from') {
      this.fromDateTouched = touched;
      this.fromDate = value;
    } else {
      this.toDateTouched = touched;
      this.toDate = value;
    }
  }

  send() {
    this.response = null;

    if (this.isSendDisabled) {
      return;
    }

    this.isSending = true;
    const payload: any = {};
    if (this.fromDate) payload.fromDate = this.fromDate;
    if (this.toDate) payload.toDate = this.toDate;

    this.emailTemplateService.sendTemplate(this.templateId, payload).subscribe({
      next: (res) => {
        this.response = res;
      },
      error: (_) => {
        this.response = { success: false, messageBG: 'Грешка при изпращане на имейл.' };
      },
      complete: () => {
        this.isSending = false;
      }
    });
  }

  close() {
    this.ref.close();
  }
} 