import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncRziStudents: CliCommand = {
  name: 'Sync RZI Students',
  description: 'Sync reporting.R_RZI_Students view to ClickHouse table R_RZI_Students',
  action: async () => {
    const config = SYNC_CONFIGS.rziStudents;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncRziStudents.name, err),
    );
  },
};
