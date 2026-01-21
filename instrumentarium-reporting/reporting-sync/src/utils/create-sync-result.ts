export function createSyncResult(
  success: boolean,
  recordsProcessed: number,
  recordsInserted: number,
  startTime: Date,
  error?: string,
): any {
  const endTime = new Date();
  return {
    success,
    recordsProcessed,
    recordsInserted,
    executionTimeMs: endTime.getTime() - startTime.getTime(),
    error,
    startTime,
    endTime,
  };
} 