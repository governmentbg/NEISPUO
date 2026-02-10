import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncAverageGradesPerClassJob: CronJob = {
  name: 'sync-average-grades-per-class',
  schedule: SCHEDULE_CONFIG.syncAverageGradesPerClass,
  enabled: true,
  description: 'Sync reporting.R_Average_Grades_Per_Class view to ClickHouse table R_Average_Grades_Per_Class',
  task: async () => {
    const config = SYNC_CONFIGS.averageGradesPerClass;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncAverageGradesPerClassJob.name, err),
    );
  },
};


