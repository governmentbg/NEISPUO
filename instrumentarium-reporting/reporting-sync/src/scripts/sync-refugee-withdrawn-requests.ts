import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncRefugeeWithdrawnRequests: CliCommand = {
  name: 'Sync Refugee Withdrawn Requests',
  description: 'Sync refugee.RefugeeWithdrawnRequests view to ClickHouse table RefugeeWithdrawnRequests',
  action: async () => {
    const config = SYNC_CONFIGS.refugeeWithdrawnRequests;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncRefugeeWithdrawnRequests.name, err),
    );
  },
};
