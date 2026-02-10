import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncClassesWorkflowsActiveYearJob: CronJob = {
  name: 'sync-classes-workflows-active-year',
  schedule: SCHEDULE_CONFIG.syncClassesWorkflowsActiveYear,
  enabled: true,
  description: 'Sync azure_temp.AzureClassesView view to ClickHouse table R_Classes_Workflows_Active_Year',
  task: async () => {
    const config = SYNC_CONFIGS.classesWorkflowsActiveYear;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncClassesWorkflowsActiveYearJob.name, err),
    );
  },
};
