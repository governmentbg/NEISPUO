import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncRefugeesSearchingOrReceivedAdmission: CliCommand = {
  name: 'Sync Refugees Searching Or Received Admission',
  description: 'Sync refugee.RefugeesSearchingOrReceivedAdmission view to ClickHouse table RefugeesSearchingOrReceivedAdmission',
  action: async () => {
    const config = SYNC_CONFIGS.refugeesSearchingOrReceivedAdmission;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncRefugeesSearchingOrReceivedAdmission.name, err),
    );
  },
};
