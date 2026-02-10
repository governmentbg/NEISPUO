import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncRefugeesSearchingOrReceivedAdmissionJob: CronJob = {
  name: 'sync-refugees-searching-or-received-admission',
  schedule: SCHEDULE_CONFIG.syncRefugeesSearchingOrReceivedAdmission,
  enabled: true,
  description: 'Sync refugee.RefugeesSearchingOrReceivedAdmission view to ClickHouse table RefugeesSearchingOrReceivedAdmission',
  task: async () => {
    const config = SYNC_CONFIGS.refugeesSearchingOrReceivedAdmission;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncRefugeesSearchingOrReceivedAdmissionJob.name, err),
    );
  },
};
