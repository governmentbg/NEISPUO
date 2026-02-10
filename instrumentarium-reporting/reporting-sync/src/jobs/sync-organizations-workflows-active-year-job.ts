import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncOrganizationsWorkflowsActiveYearJob: CronJob = {
  name: 'sync-organizations-workflows-active-year',
  schedule: SCHEDULE_CONFIG.syncOrganizationsWorkflowsActiveYear,
  enabled: true,
  description: 'Sync azure_temp.AzureOrganizationsView view to ClickHouse table R_Organizations_Workflows_Active_Year',
  task: async () => {
    const config = SYNC_CONFIGS.organizationsWorkflowsActiveYear;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncOrganizationsWorkflowsActiveYearJob.name, err),
    );
  },
};
