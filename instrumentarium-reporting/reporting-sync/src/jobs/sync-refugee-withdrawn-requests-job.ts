import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncRefugeeWithdrawnRequestsJob: CronJob = {
  name: 'sync-refugee-withdrawn-requests',
  schedule: SCHEDULE_CONFIG.syncRefugeeWithdrawnRequests,
  enabled: true,
  description: 'Sync refugee.RefugeeWithdrawnRequests view to ClickHouse table RefugeeWithdrawnRequests',
  task: async () => {
    const config = SYNC_CONFIGS.refugeeWithdrawnRequests;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncRefugeeWithdrawnRequestsJob.name, err),
    );
  },
};
