import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncOccupationsByQualificationsJob: CronJob = {
  name: 'sync-occupations-by-qualifications',
  schedule: SCHEDULE_CONFIG.syncOccupationsByQualifications,
  enabled: true,
  description: 'Sync reporting.R_Occupations_By_Qualifications view to ClickHouse table R_Occupations_By_Qualifications',
  task: async () => {
    const config = SYNC_CONFIGS.occupationsByQualifications;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncOccupationsByQualificationsJob.name, err),
    );
  },
};
