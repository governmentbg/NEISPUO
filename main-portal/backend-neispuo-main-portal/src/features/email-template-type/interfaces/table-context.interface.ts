import { VariableMapping } from 'src/shared/interfaces/variable-mapping.interface';

export interface TableContext {
  columns: VariableMapping[];
  rows: Record<string, unknown>[];
}
