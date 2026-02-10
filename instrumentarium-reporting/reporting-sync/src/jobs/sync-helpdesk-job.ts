import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncHelpdeskJob: CronJob = {
  name: 'sync-helpdesk',
  schedule: SCHEDULE_CONFIG.syncHelpdesk,
  enabled: true,
  description: 'Sync reporting.R_Helpdesk view to ClickHouse table R_Helpdesk',
  task: async () => {
    const config = SYNC_CONFIGS.helpdesk;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncHelpdeskJob.name, err),
    );
  },
};
