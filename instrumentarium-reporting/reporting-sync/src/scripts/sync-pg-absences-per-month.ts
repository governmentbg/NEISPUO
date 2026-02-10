import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

import { SYNC_CONFIGS } from '../constants/sync-config-registry';

export const syncPgAbsencesPerMonth: CliCommand = {
  name: 'Sync PG Absences Per Month',
  description: 'Sync reporting.R_PG_Absences_Per_Month view to ClickHouse table R_PG_Absences_Per_Month',
  action: async () => {
    const config = SYNC_CONFIGS.pgAbsencesPerMonth;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncPgAbsencesPerMonth.name, err),
    );
  },
};


