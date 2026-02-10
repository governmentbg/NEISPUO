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
  action?: "edit" | "preview" | "fastPreview" | "fakePreview";
  canDeleteRow?: boolean;
  canDeleteLastRow?: boolean;
  paramName?: string; // param to be passed on
  procedureName?: string;
  switchControl?: { listColumns: string[]; label: string };
  multiAdd?: MultiAdd;
  createParams?: Object;
  help?: string;
  reorder?: boolean;
  orderProcedure?: string;
  firstRowDisabled?: boolean;
  disabledViewMode?: boolean;
  additionalParams?: string[];
  forceOperation?: number;
  editableByMonRuo?: boolean;

  values?: Row[];
  dataLoaded?: boolean;
  createNewHiddenBy?: { fieldName: string; condition: (string | number)[]; hide: boolean, notNull: boolean }[];
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

  canDeleteRow?: boolean;
  hasEditButton?: boolean;
  fakePreview?: boolean;
}

interface MultiAdd {
  addFrom: string;
  listColumns: string[];
  dependsOn: string[];
  title: string;
  comparisonColumn: string;
  tooltip: string;
  tableColumns: string[];
  procedure?: string;
  checkDeleteProcedure?: string;
}
