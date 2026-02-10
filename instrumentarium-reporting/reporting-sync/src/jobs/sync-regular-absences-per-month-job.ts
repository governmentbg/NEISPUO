import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncRegularAbsencesPerMonthJob: CronJob = {
  name: 'sync-regular-absences-per-month',
  schedule: SCHEDULE_CONFIG.syncRegularAbsencesPerMonth,
  enabled: true,
  description: 'Sync reporting.R_Regular_Absences_Per_Month view to ClickHouse table R_Regular_Absences_Per_Month',
  task: async () => {
    const config = SYNC_CONFIGS.regularAbsencesPerMonth;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncRegularAbsencesPerMonthJob.name, err),
    );
  },
};


