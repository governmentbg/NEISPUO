import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncRefugeesReceivedRejectedAdmissionByRegionJob: CronJob = {
  name: 'sync-refugees-received-rejected-admission-by-region',
  schedule: SCHEDULE_CONFIG.syncRefugeesReceivedRejectedAdmissionByRegion,
  enabled: true,
  description: 'Sync refugee.RefugeesReceivedRejectedAdmissionByRegion view to ClickHouse table RefugeesReceivedRejectedAdmissionByRegion',
  task: async () => {
    const config = SYNC_CONFIGS.refugeesReceivedRejectedAdmissionByRegion;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncRefugeesReceivedRejectedAdmissionByRegionJob.name, err),
    );
  },
};
