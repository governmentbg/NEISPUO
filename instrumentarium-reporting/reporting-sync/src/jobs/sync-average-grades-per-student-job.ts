import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncAverageGradesPerStudentJob: CronJob = {
  name: 'sync-average-grades-per-student',
  schedule: SCHEDULE_CONFIG.syncAverageGradesPerStudent,
  enabled: true,
  description: 'Sync reporting.R_Average_Grades_Per_Student view to ClickHouse table R_Average_Grades_Per_Student',
  task: async () => {
    const config = SYNC_CONFIGS.averageGradesPerStudent;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncAverageGradesPerStudentJob.name, err),
    );
  },
};


