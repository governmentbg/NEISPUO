import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NeispuoModuleService } from '@portal/neispuo-modules/neispuo-module.service';
import { EmailTemplateService } from '@portal/services/email-template.service';
import { ToastService } from '@shared/services/toast.service';
import { EditorComponent } from '@tinymce/tinymce-angular';
import DOMPurify from 'dompurify';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Editor } from 'tinymce';
import { EmailTemplate } from '../email-template-list/interfaces/email-template.interface';
import { EMAIL_TEMPLATE_EDITOR_CONFIG } from './editor.config';
import { EmailTemplateForm } from './interfaces/email-template-form.interface';
import { EmailTemplateTypeResponse } from './interfaces/email-template-type-response.interface';
import { VariableMapping } from './interfaces/variable-mapping.interface';
import {
  buildPlaceholderSpan,
  transformPlaceholdersToEditor,
  transformPlaceholdersToStorage
} from './utils/placeholder.util';

@Component({
  selector: 'app-email-template',
  templateUrl: './email-template.component.html',
  styleUrls: ['./email-template.component.scss']
})
export class EmailTemplateComponent implements OnInit, OnDestroy {
  @ViewChild('tinymceEditorComponent') tinymceEditorComponent!: EditorComponent;

  variableMappings: VariableMapping[] = [];
  scalarVariableMappings: VariableMapping[] = [];
  tabularVariableMappings: VariableMapping[] = [];
  tabularVariableGroups: { name: string; variables: VariableMapping[] }[] = [];

  emailTemplateTypes: EmailTemplateTypeResponse[] = [];
  private selectedTemplateType: EmailTemplateTypeResponse | null = null;

  form: EmailTemplateForm = {
    emailTemplateTypeId: null,
    title: '',
    content: '',
    isActive: false,
    recipients: []
  };

  recipientInput: string = '';
  showDropdown: boolean = false;
  filteredSuggestions: string[] = [];
  placeholders: string[] = [];
  formSubmitted = false;

  templateTypesError = false;
  placeholdersError = false;

  isEditMode = false;
  templateId: number | null = null;
  isLoading = false;

  private rawTemplateContent: string = '';
  isContentLoading = false;
  private contentTransformed = false;
  private editorReady = false;
  private dataReady = false;
  private editorChangeTimeout: any;
  isTemplateTypeDisabled = true;

  EDITOR_CONFIG = {
    ...EMAIL_TEMPLATE_EDITOR_CONFIG,
    setup: (editor: Editor) => {
      editor.on('change', () => {
        this.form.content = editor.getContent();
      });
    }
  };

  constructor(
    private readonly nmService: NeispuoModuleService,
    private readonly emailTemplateService: EmailTemplateService,
    private readonly toastService: ToastService,
    private readonly route: ActivatedRoute,
    private readonly router: Router
  ) {}

  ngOnInit() {
    this.loadCategories();
    this.loadTemplateTypes();
    this.checkRouteMode();

    if (!this.isEditMode) {
      this.isTemplateTypeDisabled = false;
    }
  }

  ngOnDestroy() {
    if (this.editorChangeTimeout) {
      clearTimeout(this.editorChangeTimeout);
    }
  }

  onTemplateTypeChange() {
    const previousTemplateTypeId = this.selectedTemplateType?.id || null;

    if (previousTemplateTypeId && this.hasUsedPlaceholders()) {
      /**
       * PrimeNG's p-dropdown mutates its internal state immediately after the user action.
       * Re-assigning the previous value in the same change-detection cycle does **not** always
       * refresh the control visually – we end up with the placeholder text.
       * Wrapping the reassignment in `setTimeout` ensures it runs in the next tick, forcing
       * the component to re-render with the correct option selected.
       */
      setTimeout(() => {
        this.form.emailTemplateTypeId = previousTemplateTypeId;
        this.selectedTemplateType = this.emailTemplateTypes.find((t) => t.id === previousTemplateTypeId) || null;
        this.updateTemplateTypeState();
      });

      this.toastService.initiate({
        content: 'Премахнете всички плейсхолдъри преди да смените типа съобщение.',
        style: 'error',
        sticky: false,
        position: 'bottom-right'
      });
      return;
    }

    if (this.form.emailTemplateTypeId) {
      this.selectedTemplateType = this.emailTemplateTypes.find((t) => t.id === this.form.emailTemplateTypeId) || null;
      this.variableMappings = this.selectedTemplateType?.variableMappings || [];
      this.splitVariableMappings();
      this.placeholdersError = false;
    } else {
      this.selectedTemplateType = null;
      this.variableMappings = [];
      this.scalarVariableMappings = [];
      this.tabularVariableMappings = [];
      this.tabularVariableGroups = [];
    }

    this.updateTemplateTypeState();
    this.tryRenderContent();
  }

  saveTemplate() {
    this.formSubmitted = true;

    if (!this.isFormValid()) {
      return;
    }

    this.isLoading = true;

    const rawHtml = this.tinymceEditorComponent?.editor?.getContent() || this.form.content;
    const transformedHtml = transformPlaceholdersToStorage(rawHtml);
    const sanitizedHtml = DOMPurify.sanitize(transformedHtml);

    const payload: EmailTemplateForm = {
      emailTemplateTypeId: this.form.emailTemplateTypeId,
      title: this.form.title,
      content: sanitizedHtml,
      recipients: this.form.recipients,
      isActive: this.form.isActive
    };

    const operation$ =
      this.isEditMode && this.templateId
        ? this.emailTemplateService.updateTemplate(this.templateId, payload)
        : this.emailTemplateService.saveTemplate(payload);

    const successMessage = this.isEditMode ? 'Съобщението е обновено успешно.' : 'Съобщението е създадено успешно.';
    const errorMessage = this.isEditMode ? 'Грешка при обновяване на съобщението.' : 'Грешка при създаване на съобщение.';

    operation$.subscribe({
      next: () => {
        this.toastService.initiate({
          content: successMessage,
          style: 'success',
          sticky: false,
          position: 'bottom-right'
        });
        this.navigateToTemplateListPage();
      },
      error: (_) => {
        this.toastService.initiate({
          content: errorMessage,
          style: 'error',
          sticky: false,
          position: 'bottom-right'
        });
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  cancel() {
    this.navigateToTemplateListPage();
  }

  confirmEmail(): void {
    const email = this.recipientInput.trim();
    if (this.isValidEmail(email)) {
      this.form.recipients.push(email);
      this.recipientInput = '';
      this.showDropdown = false;
    }
  }

  removeRecipient(email: string): void {
    this.form.recipients = this.form.recipients.filter((e) => e !== email);
  }

  onEmailInput(event: any): void {
    const query = event.query.trim();
    this.filteredSuggestions = this.isValidEmail(query) ? [query] : [];
  }

  onEmailSelect(event: any): void {
    const email = event;
    if (this.isValidEmail(email) && !this.form.recipients.includes(email)) {
      this.form.recipients.push(email);
    }
    this.recipientInput = '';
    this.filteredSuggestions = [];
  }

  insertPlaceholder(variable: VariableMapping) {
    if (!this.tinymceEditorComponent?.editor) return;
    const editor: Editor = this.tinymceEditorComponent.editor;
    const placeholderHtml = buildPlaceholderSpan(variable);
    editor.insertContent(placeholderHtml);
    editor.focus();
  }

  onEditorInit() {
    this.editorReady = true;
    if (this.tinymceEditorComponent?.editor) {
      this.tinymceEditorComponent.editor.mode.set('readonly');
      this.setupEditorChangeHandler();
    }
    this.tryRenderContent();
  }

  isValidEmail(email: string) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
  }

  hasValidRecipients() {
    return this.form.recipients.every((email) => this.isValidEmail(email));
  }

  private checkRouteMode() {
    const url = this.router.url;
    if (url.includes('/edit/')) {
      this.isEditMode = true;
      this.templateId = Number(this.route.snapshot.paramMap.get('id'));
      if (this.templateId) {
        this.loadTemplateForEdit();
      }
    } else {
      this.isEditMode = false;
    }
  }

  private loadTemplateTypes() {
    this.emailTemplateService
      .getTemplateTypes()
      .pipe(
        catchError(() => {
          this.templateTypesError = true;
          return of([]);
        })
      )
      .subscribe((response) => {
        this.emailTemplateTypes = response;
        if (this.isEditMode && this.form.emailTemplateTypeId) {
          this.onTemplateTypeChange();
        }
        if (!this.isEditMode) {
          this.dataReady = true;
          this.isTemplateTypeDisabled = false;
        }
        this.tryRenderContent();
      });
  }

  private loadCategories() {
    this.nmService
      .getCategories()
      .pipe(
        catchError((_) => {
          return of([]);
        })
      )
      .subscribe();
  }

  private loadTemplateForEdit() {
    if (!this.templateId) return;

    this.emailTemplateService.getOneTemplate(this.templateId).subscribe({
      next: (template: EmailTemplate) => {
        this.rawTemplateContent = template.content;
        this.isContentLoading = true;

        this.form = {
          emailTemplateTypeId: template.emailTemplateTypeId,
          title: template.title,
          content: 'Loading content...',
          isActive: template.isActive,
          recipients: template.recipients || []
        };

        this.onTemplateTypeChange();
        this.dataReady = true;
        this.tryRenderContent();
      },
      error: (_) => {
        this.toastService.initiate({
          content: 'Грешка при зареждане на съобщението.',
          style: 'error',
          sticky: false,
          position: 'bottom-right'
        });
        this.navigateToTemplateListPage();
      }
    });
  }

  private splitVariableMappings() {
    this.scalarVariableMappings = (this.variableMappings || []).filter((v) => !v.key.includes('.'));
    this.tabularVariableMappings = (this.variableMappings || []).filter((v) => v.key.includes('.'));

    const groupMap: { [key: string]: VariableMapping[] } = {};
    for (const v of this.tabularVariableMappings) {
      const prefix = v.key.split('.')[0];
      if (!groupMap[prefix]) groupMap[prefix] = [];
      groupMap[prefix].push(v);
    }
    this.tabularVariableGroups = Object.entries(groupMap).map(([name, variables]) => ({
      name: name.charAt(0).toUpperCase() + name.slice(1),
      variables
    }));
  }

  private tryRenderContent() {
    if (this.contentTransformed || !this.editorReady || !this.dataReady) return;
    if (this.isEditMode && !this.variableMappings.length) return;

    const baseHtml = this.isEditMode ? this.rawTemplateContent : this.form.content || '';
    const transformedContent = transformPlaceholdersToEditor(baseHtml, this.variableMappings);

    if (this.tinymceEditorComponent?.editor) {
      this.tinymceEditorComponent.editor.setContent(transformedContent, { format: 'raw', no_events: true });
      this.tinymceEditorComponent.editor.undoManager?.clear();
      this.tinymceEditorComponent.editor.mode.set('design');
    }

    this.form.content = transformedContent;
    this.contentTransformed = true;
    this.isContentLoading = false;

    this.updateTemplateTypeState();
  }

  private setupEditorChangeHandler() {
    if (!this.tinymceEditorComponent?.editor) return;

    this.tinymceEditorComponent.editor.on('input change keyup', () => {
      clearTimeout(this.editorChangeTimeout);
      this.editorChangeTimeout = setTimeout(() => {
        this.updateTemplateTypeState();
      }, 300);
    });
  }

  private updateTemplateTypeState() {
    if (!this.form.emailTemplateTypeId) {
      this.isTemplateTypeDisabled = false;
      return;
    }

    if (this.isEditMode && !this.variableMappings.length) {
      this.isTemplateTypeDisabled = true;
      return;
    }

    this.isTemplateTypeDisabled = this.hasUsedPlaceholders();
  }

  private isFormValid() {
    return Boolean(
      this.form.emailTemplateTypeId && this.form.title.trim() && this.form.content.trim() && this.hasValidRecipients()
    );
  }

  private hasUsedPlaceholders(): boolean {
    if (!this.variableMappings.length) return false;

    const currentContent = this.tinymceEditorComponent?.editor?.getContent() || this.form.content || '';

    const tempDiv = document.createElement('div');
    tempDiv.innerHTML = currentContent;
    const placeholderSpans = tempDiv.querySelectorAll('span.placeholder-inserted');

    for (const span of Array.from(placeholderSpans)) {
      const key = span.getAttribute('data-placeholder-key');
      if (key && this.variableMappings.some((v) => v.key === key)) {
        return true;
      }
    }

    for (const variable of this.variableMappings) {
      if (currentContent.includes(`{{${variable.key}}}`)) {
        return true;
      }
    }

    return false;
  }

  private navigateToTemplateListPage(): void {
    this.router.navigate(['/portal/sync-messages']);
  }
}
