import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

import { SYNC_CONFIGS } from '../constants/sync-config-registry';

export const syncTwentyFivePercentageAbsences: CliCommand = {
  name: 'Sync Twenty Five Percentage Absences',
  description: 'Sync reporting.R_Twenty_Five_Percentage_Absences view to ClickHouse table R_Twenty_Five_Percentage_Absences',
  action: async () => {
    const config = SYNC_CONFIGS.twentyFivePercentageAbsences;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncTwentyFivePercentageAbsences.name, err),
    );
  },
};
