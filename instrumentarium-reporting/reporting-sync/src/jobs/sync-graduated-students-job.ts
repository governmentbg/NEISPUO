import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncGraduatedStudentsJob: CronJob = {
  name: 'sync-graduated-students',
  schedule: SCHEDULE_CONFIG.syncGraduatedStudents,
  enabled: true,
  description: 'Sync reporting.R_Graduated_Students view to ClickHouse table R_Graduated_Students',
  task: async () => {
    const config = SYNC_CONFIGS.graduatedStudents;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncGraduatedStudentsJob.name, err),
    );
  },
}; 