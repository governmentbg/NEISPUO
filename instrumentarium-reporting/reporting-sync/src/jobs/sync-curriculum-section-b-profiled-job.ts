import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncCurriculumSectionBProfiledJob: CronJob = {
  name: 'sync-curriculum-section-b-profiled',
  schedule: SCHEDULE_CONFIG.syncCurriculumSectionBProfiled,
  enabled: true,
  description: 'Sync reporting.R_Curriculum_Section_B_Profiled view to ClickHouse table R_Curriculum_Section_B_Profiled',
  task: async () => {
    const config = SYNC_CONFIGS.curriculumSectionBProfiled;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncCurriculumSectionBProfiledJob.name, err),
    );
  },
};
