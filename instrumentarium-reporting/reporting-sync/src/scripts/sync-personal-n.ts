import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncPersonalN: CliCommand = {
  name: 'Sync Personal N',
  description: 'Sync inst_basic.R_Personal_N view to ClickHouse table R_Personal_N',
  action: async () => {
    const config = SYNC_CONFIGS.personalN;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncPersonalN.name, err),
    );
  },
};
