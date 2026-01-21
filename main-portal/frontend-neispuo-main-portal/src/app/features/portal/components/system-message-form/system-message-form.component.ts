import { Component, OnDestroy, OnInit } from '@angular/core';
import { SystemUserMessagesService } from '@portal/services/system-user-messages.service';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { SYSTEM_MESSAGE_EDITOR_CONFIG } from './editor-config';
import DOMPurify from 'dompurify';
import { HTML_SANITIZER_CONFIG } from './html-sanitizer-config';
import { catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import { SystemMessageForm, SystemMessagePayload } from './system-message-form.interface';
import { SysRole } from '@shared/interfaces/sys-role.interface';
import { RawEditorSettings } from 'tinymce';

const DATE_FORMAT = 'dd/mm/yy';

@Component({
  selector: 'app-system-message-form',
  templateUrl: './system-message-form.component.html',
  styleUrls: ['./system-message-form.component.scss']
})
export class SystemMessageFormComponent implements OnInit, OnDestroy {
  formSubmitted = false;

  rolesError = false;

  private isEditMode = false;

  readonly dateFormat = DATE_FORMAT;

  rolesList: SysRole[] = [];

  // used to track changes to the form
  private originalForm: SystemMessageForm = {
    title: '',
    content: '',
    roles: [],
    startDate: null,
    endDate: null
  };

  form: SystemMessageForm = {
    title: '',
    content: '',
    roles: [],
    startDate: null,
    endDate: null
  };

  editorConfig: RawEditorSettings = SYSTEM_MESSAGE_EDITOR_CONFIG;

  today = new Date();
  minEndDate: Date | null = null;

  constructor(
    private sumService: SystemUserMessagesService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  ) {}

  ngOnInit() {
    // fixes color-picker not appearing in Rich Text Editor when used in a Modal
    document.body.classList.add('editor-in-modal');

    const message = this.config.data;

    this.sumService
      .getAllSysRoles()
      .pipe(
        catchError(() => {
          this.rolesError = true;
          return of([]);
        })
      )
      .subscribe((roles) => {
        this.rolesList = roles;

        if (message) {
          this.isEditMode = true;
          this.form = this.buildFormFromMessage(message);
          this.originalForm = { ...this.form };

          if (this.form.startDate) {
            this.onStartDateChange();
          }
        }
      });
  }

  ngOnDestroy() {
    document.body.classList.remove('editor-in-modal');
  }

  close(): void {
    this.ref.close();
  }

  save(): void {
    this.formSubmitted = true;
    if (!this.isFormValid()) return;

    if (this.isEditMode && this.isFormUnchanged()) {
      this.ref.close(); // silently close modal
      return;
    }

    const payload: SystemMessagePayload = {
      title: this.form.title.trim(),
      content: this.sanitizeHtml(this.form.content),
      roles: this.form.roles.join(','), // backend expects string of comma separated numbers e.g - "1,2,3"
      startDate: this.formatDateForDb(this.form.startDate),
      endDate: this.formatDateForDb(this.form.endDate)
    };

    this.ref.close(payload);
  }

  hasSelectedRoles(): boolean {
    return Array.isArray(this.form.roles) && this.form.roles.length > 0;
  }

  onStartDateChange(): void {
    if (!this.form.startDate) {
      this.minEndDate = null;
      return;
    }

    const copy = new Date(this.form.startDate);
    copy.setDate(copy.getDate() + 1);
    this.minEndDate = copy;
  }

  private isFormValid(): boolean {
    return Boolean(
      this.form.title.trim() &&
        this.form.content.trim() &&
        this.hasSelectedRoles() &&
        this.form.startDate &&
        this.form.endDate
    );
  }

  private sanitizeHtml(content: string): string {
    return DOMPurify.sanitize(content, HTML_SANITIZER_CONFIG);
  }

  private formatDateForDb(date: Date): string | null {
    if (!date) return null;

    const localDate = new Date(date);
    localDate.setHours(0, 0, 0, 0);

    const pad = (n: number) => n.toString().padStart(2, '0');

    const yyyy = localDate.getFullYear();
    const MM = pad(localDate.getMonth() + 1);
    const dd = pad(localDate.getDate());
    const HH = pad(localDate.getHours());
    const mm = pad(localDate.getMinutes());
    const ss = pad(localDate.getSeconds());

    return `${yyyy}-${MM}-${dd} ${HH}:${mm}:${ss}.000`;
  }

  private buildFormFromMessage(message: any): SystemMessageForm {
    return {
      title: message.title || '',
      content: message.content || '',
      roles: Array.isArray(message.roles) ? message.roles.map((r: any) => r.id) : [],
      startDate: message.startDate ? new Date(message.startDate) : null,
      endDate: message.endDate ? new Date(message.endDate) : null
    };
  }

  private isFormUnchanged() {
    return (
      this.form.title === this.originalForm.title &&
      this.sanitizeHtml(this.form.content) === this.sanitizeHtml(this.originalForm.content) &&
      this.form.startDate?.getTime?.() === this.originalForm.startDate?.getTime?.() &&
      this.form.endDate?.getTime?.() === this.originalForm.endDate?.getTime?.() &&
      this.arraysEqual(this.form.roles, this.originalForm.roles)
    );
  }

  private arraysEqual(a: number[], b: number[]) {
    if (a.length !== b.length) return false;
    return a.every((val, i) => val === b[i]);
  }
}
