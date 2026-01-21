import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncPhonesJob: CronJob = {
  name: 'sync-phones',
  schedule: SCHEDULE_CONFIG.syncPhones,
  enabled: true,
  description: 'Sync inst_basic.Phones view to ClickHouse table Phones',
  task: async () => {
    const config = SYNC_CONFIGS.phones;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncPhonesJob.name, err),
    );
  },
}; 