import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

import { SYNC_CONFIGS } from '../constants/sync-config-registry';

export const syncRegularAverageAbsencePerStudent: CliCommand = {
  name: 'Sync Regular Average Absence Per Student',
  description: 'Sync reporting.R_Regular_Average_Absence_Per_Student view to ClickHouse table R_Regular_Average_Absence_Per_Student',
  action: async () => {
    const config = SYNC_CONFIGS.regularAverageAbsencePerStudent;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncRegularAverageAbsencePerStudent.name, err),
    );
  },
};


