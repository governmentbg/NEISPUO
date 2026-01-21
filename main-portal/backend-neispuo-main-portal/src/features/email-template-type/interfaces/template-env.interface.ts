import { VariableMapping } from 'src/shared/interfaces/variable-mapping.interface';
import { DataFetchResult } from './data-fetch-result.interface';

export interface TemplateEnv {
  dataFetchResult: DataFetchResult;
  mappings: VariableMapping[];
}
