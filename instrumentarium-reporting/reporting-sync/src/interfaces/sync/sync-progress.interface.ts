export interface SyncProgress {
  totalRecords: number;
  processedRecords: number;
  currentBatch: number;
  totalBatches: number;
  percentage: number;
} 