import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncStudentsJob: CronJob = {
  name: 'sync-students',
  schedule: SCHEDULE_CONFIG.syncStudents,
  enabled: true,
  description: 'Sync reporting.R_Students view to ClickHouse table R_Students',
  task: async () => {
    const config = SYNC_CONFIGS.students;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncStudentsJob.name, err),
    );
  },
}; 