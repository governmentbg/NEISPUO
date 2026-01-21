import { FieldConfig } from "./field.interface";

export interface Table {
  label: number;
  name: string;
  fields: FieldConfig[];
  hasEditButton?: boolean;
  paramName?: string;
  rendered?: boolean;
  sortable?: boolean;
  searchable?: boolean;

  values?: {
    id: string;
    fields: FieldConfig[];
    formName?: string;
    procID?: number | string;
    instKind: string;
    instid: number;
  }[];
}
