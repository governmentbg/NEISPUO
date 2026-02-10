import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncImmigrantsDobHoursJob: CronJob = {
  name: 'sync-immigrants-dob-hours',
  schedule: SCHEDULE_CONFIG.syncImmigrantsDobHours,
  enabled: true,
  description: 'Sync inst_basic.RImmigrantsDOBHours view to ClickHouse table RImmigrantsDOBHours',
  task: async () => {
    const config = SYNC_CONFIGS.immigrantsDobHours;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncImmigrantsDobHoursJob.name, err),
    );
  },
};
