import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncHostels: CliCommand = {
  name: 'Sync Hostels',
  description: 'Sync inst_basic.R_hostels view to ClickHouse table R_hostels',
  action: async () => {
    const config = SYNC_CONFIGS.hostels;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncHostels.name, err),
    );
  },
};
