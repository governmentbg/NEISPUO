import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncCurriculumSectionA: CliCommand = {
  name: 'Sync Curriculum Section A',
  description: 'Sync reporting.R_Curriculum_Section_A view to ClickHouse table R_Curriculum_Section_A',
  action: async () => {
    const config = SYNC_CONFIGS.curriculumSectionA;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncCurriculumSectionA.name, err),
    );
  },
};
