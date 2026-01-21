import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncPgAbsencesPerMonthJob: CronJob = {
  name: 'sync-pg-absences-per-month',
  schedule: SCHEDULE_CONFIG.syncPgAbsencesPerMonth,
  enabled: true,
  description: 'Sync reporting.R_PG_Absences_Per_Month view to ClickHouse table R_PG_Absences_Per_Month',
  task: async () => {
    const config = SYNC_CONFIGS.pgAbsencesPerMonth;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncPgAbsencesPerMonthJob.name, err),
    );
  },
};


