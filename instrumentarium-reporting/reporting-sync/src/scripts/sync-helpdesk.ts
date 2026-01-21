import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncHelpdesk: CliCommand = {
  name: 'Sync Helpdesk',
  description: 'Sync reporting.R_Helpdesk view to ClickHouse table R_Helpdesk',
  action: async () => {
    const config = SYNC_CONFIGS.helpdesk;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncHelpdesk.name, err),
    );
  },
};
