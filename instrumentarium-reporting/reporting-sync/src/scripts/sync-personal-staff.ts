import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncPersonalStaff: CliCommand = {
  name: 'Sync Personal Staff',
  description: 'Sync reporting.R_Personal_Staff view to ClickHouse table R_Personal_Staff',
  action: async () => {
    const config = SYNC_CONFIGS.personalStaff;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncPersonalStaff.name, err),
    );
  },
};
