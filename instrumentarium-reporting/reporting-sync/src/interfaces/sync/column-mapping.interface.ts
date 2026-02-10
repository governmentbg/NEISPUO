export interface ColumnMapping {
  columnName: string;
  transformFunction: (value: string) => any;
} 