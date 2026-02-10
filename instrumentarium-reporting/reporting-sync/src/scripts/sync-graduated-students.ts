import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

import { SYNC_CONFIGS } from '../constants/sync-config-registry';

export const syncGraduatedStudents: CliCommand = {
  name: 'Sync Graduated Students',
  description: 'Sync reporting.R_Graduated_Students view to ClickHouse table R_Graduated_Students',
  action: async () => {
    const config = SYNC_CONFIGS.graduatedStudents;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncGraduatedStudents.name, err),
    );
  },
};
