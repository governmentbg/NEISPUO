import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

import { SYNC_CONFIGS } from '../constants/sync-config-registry';

export const syncPgAverageAbsencePerStudent: CliCommand = {
  name: 'Sync PG Average Absence Per Student',
  description: 'Sync reporting.R_PG_Average_Absence_Per_Student view to ClickHouse table R_PG_Average_Absence_Per_Student',
  action: async () => {
    const config = SYNC_CONFIGS.pgAverageAbsencePerStudent;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncPgAverageAbsencePerStudent.name, err),
    );
  },
};


