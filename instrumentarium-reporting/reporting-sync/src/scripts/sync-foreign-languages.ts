import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncForeignLanguages: CliCommand = {
  name: 'Sync Foreign Languages',
  description: 'Sync inst_basic.R_foreign_languages view to ClickHouse table R_foreign_languages',
  action: async () => {
    const config = SYNC_CONFIGS.foreignLanguages;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncForeignLanguages.name, err),
    );
  },
};
