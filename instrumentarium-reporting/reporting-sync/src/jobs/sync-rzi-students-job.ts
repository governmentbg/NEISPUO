import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncRziStudentsJob: CronJob = {
  name: 'sync-rzi-students',
  schedule: SCHEDULE_CONFIG.syncRziStudents,
  enabled: true,
  description: 'Sync reporting.R_RZI_Students view to ClickHouse table R_RZI_Students',
  task: async () => {
    const config = SYNC_CONFIGS.rziStudents;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncRziStudentsJob.name, err),
    );
  },
};
