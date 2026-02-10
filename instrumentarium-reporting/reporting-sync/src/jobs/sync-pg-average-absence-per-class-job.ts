import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncPgAverageAbsencePerClassJob: CronJob = {
  name: 'sync-pg-average-absence-per-class',
  schedule: SCHEDULE_CONFIG.syncPgAverageAbsencePerClass,
  enabled: true,
  description: 'Sync reporting.R_PG_Average_Absence_Per_Class view to ClickHouse table R_PG_Average_Absence_Per_Class',
  task: async () => {
    const config = SYNC_CONFIGS.pgAverageAbsencePerClass;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncPgAverageAbsencePerClassJob.name, err),
    );
  },
};


