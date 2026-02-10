export interface ViewConfig {
  from: string;
  alias: string;
  extraSelect?: string[];
  extraWhere?: string[];
  extraGroupBy?: string[];
  workflowTypes: number[];
  errorCategoryCase: string;
}
