import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncPersonalStaffJob: CronJob = {
  name: 'sync-personal-staff',
  schedule: SCHEDULE_CONFIG.syncPersonalStaff,
  enabled: true,
  description: 'Sync reporting.R_Personal_Staff view to ClickHouse table R_Personal_Staff',
  task: async () => {
    const config = SYNC_CONFIGS.personalStaff;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncPersonalStaffJob.name, err),
    );
  },
};
