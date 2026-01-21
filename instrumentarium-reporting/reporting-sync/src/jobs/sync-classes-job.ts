import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncClassesJob: CronJob = {
  name: 'sync-classes',
  schedule: SCHEDULE_CONFIG.syncClasses,
  enabled: true,
  description: 'Sync inst_basic.R_Classes view to ClickHouse table R_Classes',
  task: async () => {
    const config = SYNC_CONFIGS.classes;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncClassesJob.name, err),
    );
  },
}; 