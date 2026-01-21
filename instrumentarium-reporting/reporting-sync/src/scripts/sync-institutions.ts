import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncInstitutions: CliCommand = {
  name: 'Sync Institutions',
  description: 'Sync inst_basic.R_institutions view to ClickHouse table R_Institutions',
  action: async () => {
    const config = SYNC_CONFIGS.institutions;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncInstitutions.name, err),
    );
  },
};
