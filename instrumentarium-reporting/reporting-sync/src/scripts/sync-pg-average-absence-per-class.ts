import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

import { SYNC_CONFIGS } from '../constants/sync-config-registry';

export const syncPgAverageAbsencePerClass: CliCommand = {
  name: 'Sync PG Average Absence Per Class',
  description: 'Sync reporting.R_PG_Average_Absence_Per_Class view to ClickHouse table R_PG_Average_Absence_Per_Class',
  action: async () => {
    const config = SYNC_CONFIGS.pgAverageAbsencePerClass;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncPgAverageAbsencePerClass.name, err),
    );
  },
};


