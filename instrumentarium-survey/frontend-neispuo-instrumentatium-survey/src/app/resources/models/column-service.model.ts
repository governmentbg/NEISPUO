import { IColumn } from './column.interface';

export class ColumnServiceConfig<T> {
  localStorageKey: string;
  allColumns: IColumn<T>[];
  fixedVisibleColumnFields: ((keyof T) | any)[] = [];
  hiddenColumnsFields?: ((keyof T) | any)[] = [];
}
