import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncTwentyFivePercentageAbsencesJob: CronJob = {
  name: 'sync-twenty-five-percentage-absences',
  schedule: SCHEDULE_CONFIG.syncTwentyFivePercentageAbsences,
  enabled: true,
  description: 'Sync reporting.R_Twenty_Five_Percentage_Absences view to ClickHouse table R_Twenty_Five_Percentage_Absences',
  task: async () => {
    const config = SYNC_CONFIGS.twentyFivePercentageAbsences;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncTwentyFivePercentageAbsencesJob.name, err),
    );
  },
};
