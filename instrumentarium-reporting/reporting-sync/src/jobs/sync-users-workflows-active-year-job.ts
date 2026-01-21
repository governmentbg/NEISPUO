import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncUsersWorkflowsActiveYearJob: CronJob = {
  name: 'sync-users-workflows-active-year',
  schedule: SCHEDULE_CONFIG.syncUsersWorkflowsActiveYear,
  enabled: true,
  description: 'Sync azure_temp.AzureUsersView view to ClickHouse table R_Users_Workflows_Active_Year',
  task: async () => {
    const config = SYNC_CONFIGS.usersWorkflowsActiveYear;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncUsersWorkflowsActiveYearJob.name, err),
    );
  },
};
