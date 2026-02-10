import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

import { SYNC_CONFIGS } from '../constants/sync-config-registry';

export const syncAverageGradesPerClass: CliCommand = {
  name: 'Sync Average Grades Per Class',
  description: 'Sync reporting.R_Average_Grades_Per_Class view to ClickHouse table R_Average_Grades_Per_Class',
  action: async () => {
    const config = SYNC_CONFIGS.averageGradesPerClass;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncAverageGradesPerClass.name, err),
    );
  },
};


