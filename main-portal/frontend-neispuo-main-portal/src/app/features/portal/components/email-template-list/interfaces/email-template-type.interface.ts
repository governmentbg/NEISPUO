import { EmailTemplate } from './email-template.interface';
import { VariableMapping } from './variable-mapping.interface';

export interface EmailTemplateType {
  id: number;
  displayName: string;
  contentProvider: string;
  description?: string;
  variableMappings: VariableMapping[];
  emailTemplates?: Partial<EmailTemplate>[];
}
