import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncInstitutionsJob: CronJob = {
  name: 'sync-institutions',
  schedule: SCHEDULE_CONFIG.syncInstitutions,
  enabled: true,
  description: 'Sync inst_basic.R_institutions view to ClickHouse table R_Institutions',
  task: async () => {
    const config = SYNC_CONFIGS.institutions;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncInstitutionsJob.name, err),
    );
  },
}; 