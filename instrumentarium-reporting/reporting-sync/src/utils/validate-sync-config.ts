export function validateSyncConfig(config: any): { valid: boolean; errors: string[] } {
  const errors: string[] = [];

  if (!config.source || typeof config.source !== 'string') {
    errors.push('source is required and must be a string');
  }

  if (!config.target || typeof config.target !== 'string') {
    errors.push('target is required and must be a string');
  }

  if (
    config.batchSize &&
    (typeof config.batchSize !== 'number' || config.batchSize <= 0)
  ) {
    errors.push('batchSize must be a positive number');
  }

  return {
    valid: errors.length === 0,
    errors,
  };
} 