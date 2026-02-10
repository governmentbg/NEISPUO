import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncStudentsDetails: CliCommand = {
  name: 'Sync Students Details',
  description: 'Sync reporting.R_Students_Details view to ClickHouse table R_Students_Details',
  action: async () => {
    const config = SYNC_CONFIGS.studentsDetails;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncStudentsDetails.name, err),
    );
  },
};
