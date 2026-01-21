import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncOrganizationsWorkflowsActiveYear: CliCommand = {
  name: 'Sync Organizations Workflows Active Year',
  description: 'Sync azure_temp.AzureOrganizationsView view to ClickHouse table R_Organizations_Workflows_Active_Year',
  action: async () => {
    const config = SYNC_CONFIGS.organizationsWorkflowsActiveYear;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncOrganizationsWorkflowsActiveYear.name, err),
    );
  },
};
