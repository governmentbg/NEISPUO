import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncClassesWorkflowsActiveYear: CliCommand = {
  name: 'Sync Classes Workflows Active Year',
  description: 'Sync azure_temp.AzureClassesView view to ClickHouse table R_Classes_Workflows_Active_Year',
  action: async () => {
    const config = SYNC_CONFIGS.classesWorkflowsActiveYear;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncClassesWorkflowsActiveYear.name, err),
    );
  },
};
