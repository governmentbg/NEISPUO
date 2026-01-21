import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncPersonalNJob: CronJob = {
  name: 'sync-personal-n',
  schedule: SCHEDULE_CONFIG.syncPersonalN,
  enabled: true,
  description: 'Sync inst_basic.R_Personal_N view to ClickHouse table R_Personal_N',
  task: async () => {
    const config = SYNC_CONFIGS.personalN;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncPersonalNJob.name, err),
    );
  },
};
