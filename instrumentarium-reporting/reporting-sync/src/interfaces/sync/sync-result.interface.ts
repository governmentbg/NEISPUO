export interface SyncResult {
  success: boolean;
  recordsProcessed: number;
  recordsInserted: number;
  executionTimeMs: number;
  error?: string;
  startTime: Date;
  endTime: Date;
} 