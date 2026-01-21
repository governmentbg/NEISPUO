import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

import { SYNC_CONFIGS } from '../constants/sync-config-registry';

export const syncAverageGradesPerStudent: CliCommand = {
  name: 'Sync Average Grades Per Student',
  description: 'Sync reporting.R_Average_Grades_Per_Student view to ClickHouse table R_Average_Grades_Per_Student',
  action: async () => {
    const config = SYNC_CONFIGS.averageGradesPerStudent;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncAverageGradesPerStudent.name, err),
    );
  },
};


