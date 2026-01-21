import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncRegularAverageAbsencePerClassJob: CronJob = {
  name: 'sync-regular-average-absence-per-class',
  schedule: SCHEDULE_CONFIG.syncRegularAverageAbsencePerClass,
  enabled: true,
  description: 'Sync reporting.R_Regular_Average_Absence_Per_Class view to ClickHouse table R_Regular_Average_Absence_Per_Class',
  task: async () => {
    const config = SYNC_CONFIGS.regularAverageAbsencePerClass;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncRegularAverageAbsencePerClassJob.name, err),
    );
  },
};


