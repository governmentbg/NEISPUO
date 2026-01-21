import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import {
  logSyncError,
  logSyncResult,
  syncViewToTable,
} from '../services/sync.service';

export const syncCurriculumSectionBProfessional: CliCommand = {
  name: 'Sync Curriculum Section B Professional',
  description: 'Sync reporting.R_Curriculum_Section_B_Professional view to ClickHouse table R_Curriculum_Section_B_Professional',
  action: async () => {
    const config = SYNC_CONFIGS.curriculumSectionBProfessional;

    await executeCommandWithContinue(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncCurriculumSectionBProfessional.name, err),
    );
  },
};
