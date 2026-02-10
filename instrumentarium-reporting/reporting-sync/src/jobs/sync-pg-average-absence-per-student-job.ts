import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncPgAverageAbsencePerStudentJob: CronJob = {
  name: 'sync-pg-average-absence-per-student',
  schedule: SCHEDULE_CONFIG.syncPgAverageAbsencePerStudent,
  enabled: true,
  description: 'Sync reporting.R_PG_Average_Absence_Per_Student view to ClickHouse table R_PG_Average_Absence_Per_Student',
  task: async () => {
    const config = SYNC_CONFIGS.pgAverageAbsencePerStudent;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncPgAverageAbsencePerStudentJob.name, err),
    );
  },
};


