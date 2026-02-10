import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

import { SYNC_CONFIGS } from '../constants/sync-config-registry';

export const syncRegularAbsencesPerMonth: CliCommand = {
  name: 'Sync Regular Absences Per Month',
  description: 'Sync reporting.R_Regular_Absences_Per_Month view to ClickHouse table R_Regular_Absences_Per_Month',
  action: async () => {
    const config = SYNC_CONFIGS.regularAbsencesPerMonth;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncRegularAbsencesPerMonth.name, err),
    );
  },
};


