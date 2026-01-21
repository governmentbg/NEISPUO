import { ChunkConfig } from './chunk-config.interface';
import { ColumnMapping } from './column-mapping.interface';

export interface SyncConfig {
  source: string;
  target: string;
  columnMappings?: ColumnMapping[];
  chunkConfigs?: [] | [ChunkConfig] | [ChunkConfig, ChunkConfig];
}
