export interface EmailTemplateForm {
  emailTemplateTypeId?: number;
  title: string;
  content: string;
  isActive: boolean;
  recipients: string[];
}
