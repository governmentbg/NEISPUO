import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncEnrollmentsWorkflowsActiveYearJob: CronJob = {
  name: 'sync-enrollments-workflows-active-year',
  schedule: SCHEDULE_CONFIG.syncEnrollmentsWorkflowsActiveYear,
  enabled: true,
  description: 'Sync azure_temp.AzureEnrollmentsView view to ClickHouse table R_Enrollments_Workflows_Active_Year',
  task: async () => {
    const config = SYNC_CONFIGS.enrollmentsWorkflowsActiveYear;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncEnrollmentsWorkflowsActiveYearJob.name, err),
    );
  },
};
