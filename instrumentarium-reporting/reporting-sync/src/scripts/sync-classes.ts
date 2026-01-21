import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncClasses: CliCommand = {
  name: 'Sync Classes',
  description: 'Sync inst_basic.R_Classes view to ClickHouse table R_Classes',
  action: async () => {
    const config = SYNC_CONFIGS.classes;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncClasses.name, err),
    );
  },
}; 