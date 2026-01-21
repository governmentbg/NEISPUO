import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncOccupationsByQualifications: CliCommand = {
  name: 'Sync Occupations By Qualifications',
  description: 'Sync reporting.R_Occupations_By_Qualifications view to ClickHouse table R_Occupations_By_Qualifications',
  action: async () => {
    const config = SYNC_CONFIGS.occupationsByQualifications;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncOccupationsByQualifications.name, err),
    );
  },
};
