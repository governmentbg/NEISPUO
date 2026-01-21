import { EmailTemplateType } from "./email-template-type.interface";

export interface EmailTemplate {
  id: number;
  title: string;
  content: string;
  recipients: string[];
  isActive: boolean;
  createdBy: string;
  updatedBy: string;
  createdAt: Date;
  updatedAt: Date;
  emailTemplateTypeId: number;
  emailTemplateType: Partial<EmailTemplateType>;
}
