import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncStudentsDetailsJob: CronJob = {
  name: 'sync-students-details',
  schedule: SCHEDULE_CONFIG.syncStudentsDetails,
  enabled: true,
  description: 'Sync reporting.R_Students_Details view to ClickHouse table R_Students_Details',
  task: async () => {
    const config = SYNC_CONFIGS.studentsDetails;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncStudentsDetailsJob.name, err),
    );
  },
}; 