import { Component, OnInit } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { EmailTemplate } from '@portal/components/email-template-list/interfaces/email-template.interface';
import { EmailTemplateTypeResponse } from '@portal/components/email-template/interfaces/email-template-type-response.interface';
import { VariableMapping } from '@portal/components/email-template/interfaces/variable-mapping.interface';
import { transformPlaceholdersToEditor } from '@portal/components/email-template/utils/placeholder.util';
import { EmailTemplateService } from '@portal/services/email-template.service';
import DOMPurify from 'dompurify';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'app-email-template-preview-modal',
  templateUrl: './email-template-preview-modal.component.html',
  styleUrls: ['./email-template-preview-modal.component.scss']
})
export class EmailTemplatePreviewModalComponent implements OnInit {
  template: EmailTemplate;
  sanitizedContent: SafeHtml;
  isLoading = true;
  hasError = false;
  errorMessage = '';

  constructor(
    private emailTemplateService: EmailTemplateService,
    private sanitizer: DomSanitizer,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  ) {}

  ngOnInit(): void {
    this.template = this.config.data.template as EmailTemplate;
    if (!this.template) {
      this.hasError = true;
      this.errorMessage = 'Няма данни за шаблон.';
      this.isLoading = false;
      return;
    }

    this.emailTemplateService.getTemplateTypes().subscribe({
      next: (types: EmailTemplateTypeResponse[]) => {
        const tplType = types.find((t) => t.id === this.template.emailTemplateTypeId);
        const variableMappings: VariableMapping[] = tplType?.variableMappings || [];

        const transformed = transformPlaceholdersToEditor(this.template.content, variableMappings);
        const purified = DOMPurify.sanitize(transformed);
        this.sanitizedContent = this.sanitizer.bypassSecurityTrustHtml(purified);
        this.isLoading = false;
      },
      error: (_) => {
        this.hasError = true;
        this.errorMessage = 'Грешка при зареждане на типовете съобщения.';
        this.isLoading = false;
      }
    });
  }
}
