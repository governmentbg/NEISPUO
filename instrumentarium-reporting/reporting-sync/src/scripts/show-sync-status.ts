import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import { logSyncError, showAllSyncLogs } from '../services/sync.service';

export const showSyncStatus: CliCommand = {
  name: 'Show Sync Status',
  description: 'Shows the status of all syncs',
  action: async () => {
    await executeCommandWithContinue(
      async () => {
        await showAllSyncLogs();
      },
      (err) => logSyncError(showSyncStatus.name, err),
    );
  },
};
