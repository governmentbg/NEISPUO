import { CronJob } from '../interfaces';
import { checkForRuntimeChanges } from '../services/cron.service';
import { error } from '../services/logger.service';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const jobDatabaseSync: CronJob = {
  name: 'job-database-sync',
  schedule: SCHEDULE_CONFIG.jobDatabaseSync,
  enabled: true,
  description: 'Runtime job schedule and enabled state manager',
  task: async () => {
    try {
      await checkForRuntimeChanges();
    } catch (err) {
      error('❌ Runtime job manager failed:', err);
    }
  },
};
