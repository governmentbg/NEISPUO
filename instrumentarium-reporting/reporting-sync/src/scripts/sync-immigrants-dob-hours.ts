import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncImmigrantsDobHours: CliCommand = {
  name: 'Sync Immigrants DOB Hours',
  description: 'Sync inst_basic.RImmigrantsDOBHours view to ClickHouse table RImmigrantsDOBHours',
  action: async () => {
    const config = SYNC_CONFIGS.immigrantsDobHours;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncImmigrantsDobHours.name, err),
    );
  },
};
