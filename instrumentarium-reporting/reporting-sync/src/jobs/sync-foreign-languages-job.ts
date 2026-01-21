import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncForeignLanguagesJob: CronJob = {
  name: 'sync-foreign-languages',
  schedule: SCHEDULE_CONFIG.syncForeignLanguages,
  enabled: true,
  description: 'Sync inst_basic.R_foreign_languages view to ClickHouse table R_foreign_languages',
  task: async () => {
    const config = SYNC_CONFIGS.foreignLanguages;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncForeignLanguagesJob.name, err),
    );
  },
};
