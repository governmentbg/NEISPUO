import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

import { SYNC_CONFIGS } from '../constants/sync-config-registry';

export const syncStudents: CliCommand = {
  name: 'Sync Students',
  description: 'Sync reporting.R_Students view to ClickHouse table R_Students',
  action: async () => {
    const config = SYNC_CONFIGS.students;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncStudents.name, err),
    );
  },
};
