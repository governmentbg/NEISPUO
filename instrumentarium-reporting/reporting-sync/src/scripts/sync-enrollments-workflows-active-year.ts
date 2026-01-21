import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncEnrollmentsWorkflowsActiveYear: CliCommand = {
  name: 'Sync Enrollments Workflows Active Year',
  description: 'Sync azure_temp.AzureEnrollmentsView view to ClickHouse table R_Enrollments_Workflows_Active_Year',
  action: async () => {
    const config = SYNC_CONFIGS.enrollmentsWorkflowsActiveYear;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncEnrollmentsWorkflowsActiveYear.name, err),
    );
  },
};
