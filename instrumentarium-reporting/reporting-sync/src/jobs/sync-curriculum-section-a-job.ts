import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncCurriculumSectionAJob: CronJob = {
  name: 'sync-curriculum-section-a',
  schedule: SCHEDULE_CONFIG.syncCurriculumSectionA,
  enabled: true,
  description: 'Sync reporting.R_Curriculum_Section_A view to ClickHouse table R_Curriculum_Section_A',
  task: async () => {
    const config = SYNC_CONFIGS.curriculumSectionA;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncCurriculumSectionAJob.name, err),
    );
  },
};
