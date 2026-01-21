import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncPhones: CliCommand = {
  name: 'Sync Phones',
  description: 'Sync inst_basic.Phones view to ClickHouse table Phones',
  action: async () => {
    const config = SYNC_CONFIGS.phones;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncPhones.name, err),
    );
  },
};
