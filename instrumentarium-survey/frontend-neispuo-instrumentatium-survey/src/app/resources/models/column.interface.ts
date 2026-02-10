import { CondOperator } from '@nestjsx/crud-request';
import { SelectItem } from 'primeng/api';

export interface IColumn<T> {
  field: keyof (T | any);
  header: string;
  filter?: {
    type: 'text' | 'int' | 'select' | 'date';
    operator: CondOperator;
    selectionOptions?: SelectItem[];
  };
}
