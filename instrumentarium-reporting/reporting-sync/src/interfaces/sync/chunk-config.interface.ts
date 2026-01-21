export interface ChunkConfig {
  columnName: string;
  operator: string;
  values: (() => string[]) | (() => Promise<string[]>);
}
