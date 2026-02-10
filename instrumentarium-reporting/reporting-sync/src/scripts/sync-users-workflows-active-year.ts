import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncUsersWorkflowsActiveYear: CliCommand = {
  name: 'Sync Users Workflows Active Year',
  description: 'Sync azure_temp.AzureUsersView view to ClickHouse table R_Users_Workflows_Active_Year',
  action: async () => {
    const config = SYNC_CONFIGS.usersWorkflowsActiveYear;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncUsersWorkflowsActiveYear.name, err),
    );
  },
};
