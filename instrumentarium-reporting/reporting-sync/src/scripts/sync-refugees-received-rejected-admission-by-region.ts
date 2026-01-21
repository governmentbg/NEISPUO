import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncRefugeesReceivedRejectedAdmissionByRegion: CliCommand = {
  name: 'Sync Refugees Received Rejected Admission By Region',
  description: 'Sync refugee.RefugeesReceivedRejectedAdmissionByRegion view to ClickHouse table RefugeesReceivedRejectedAdmissionByRegion',
  action: async () => {
    const config = SYNC_CONFIGS.refugeesReceivedRejectedAdmissionByRegion;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncRefugeesReceivedRejectedAdmissionByRegion.name, err),
    );
  },
};
