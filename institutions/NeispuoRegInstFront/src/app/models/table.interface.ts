import { FieldConfig } from './field.interface';

export interface Table {
  label: string;
  name: string;
  fields: FieldConfig[];
  hasEditButton?: boolean;
  rendered?: boolean;
  sortable?: boolean;
  searchable?: boolean;
  disabledViewMode?: boolean;
  formName: string;
  canDeleteRow?: boolean;
  createNew?: boolean;
  paramName?: string;

  values?: Row[];

  idntfr?: number;
  pseudoLabel?: string;
}

export interface Row {
  id: string;
  fields: FieldConfig[];
  formName?: string;
  procID?: number | string;
  instKind?: string;
  instid?: number;
  navigateActiveProc?: boolean;
  instType?: number;
  isActive?: boolean;
}
