import { VariableMapping } from './variable-mapping.interface';

export interface EmailTemplateTypeResponse {
  id: number;
  displayName: string;
  variableMappings: VariableMapping[];
}
