import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

import { SYNC_CONFIGS } from '../constants/sync-config-registry';

export const syncPgFamilyReasonAbsences: CliCommand = {
  name: 'Sync PG Family Reason Absences',
  description: 'Sync reporting.R_PG_Family_Reason_Absences view to ClickHouse table R_PG_Family_Reason_Absences',
  action: async () => {
    const config = SYNC_CONFIGS.pgFamilyReasonAbsences;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncPgFamilyReasonAbsences.name, err),
    );
  },
};


