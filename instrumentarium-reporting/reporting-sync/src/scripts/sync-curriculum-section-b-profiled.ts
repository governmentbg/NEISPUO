import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncCurriculumSectionBProfiled: CliCommand = {
  name: 'Sync Curriculum Section B Profiled',
  description: 'Sync reporting.R_Curriculum_Section_B_Profiled view to ClickHouse table R_Curriculum_Section_B_Profiled',
  action: async () => {
    const config = SYNC_CONFIGS.curriculumSectionBProfiled;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncCurriculumSectionBProfiled.name, err),
    );
  },
};
