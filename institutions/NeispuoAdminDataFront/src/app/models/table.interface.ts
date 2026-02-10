import { FieldConfig } from "./field.interface";

export interface Table {
  label?: string;
  name: string;
  fields: FieldConfig[];
  hasEditButton?: boolean;
  rendered?: boolean;
  sortable?: boolean;
  searchable?: boolean;
  searchType?: "all" | "column";
  createNew?: "redirect" | "samePage";
  formName?: string;
  action?: "edit" | "preview";
  canDeleteRow?: boolean;
  canDeleteLastRow?: boolean;
  paramName?: string; // param to be passed on
  procedureName?: string;
  createParams?: Object;
  help?: string;
  firstRowDisabled?: boolean;
  disabledViewMode?: boolean;
  additionalParams?: string[];
  forceOperation?: number;

  values?: Row[];
  dataLoaded?: boolean;
  createNewHiddenBy?: { fieldName: string; condition: (string | number)[]; hide: boolean; notNull: boolean }[];
  createNewHidden?: boolean;

  idntfr?: number;
  pseudoLabel?: string;
}

export interface Row {
  id: string;
  fields: FieldConfig[];
  formName?: string;
  formDataId?: number | string;
  additionalParams?: Object;
}
