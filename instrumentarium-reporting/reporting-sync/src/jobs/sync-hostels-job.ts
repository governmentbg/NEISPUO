import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncHostelsJob: CronJob = {
  name: 'sync-hostels',
  schedule: SCHEDULE_CONFIG.syncHostels,
  enabled: true,
  description: 'Sync inst_basic.R_hostels view to ClickHouse table R_hostels',
  task: async () => {
    const config = SYNC_CONFIGS.hostels;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncHostelsJob.name, err),
    );
  },
};
