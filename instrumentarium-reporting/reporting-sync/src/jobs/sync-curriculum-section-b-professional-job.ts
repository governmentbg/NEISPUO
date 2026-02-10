import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncCurriculumSectionBProfessionalJob: CronJob = {
  name: 'sync-curriculum-section-b-professional',
  schedule: SCHEDULE_CONFIG.syncCurriculumSectionBProfessional,
  enabled: true,
  description: 'Sync reporting.R_Curriculum_Section_B_Professional view to ClickHouse table R_Curriculum_Section_B_Professional',
  task: async () => {
    const config = SYNC_CONFIGS.curriculumSectionBProfessional;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncCurriculumSectionBProfessionalJob.name, err),
    );
  },
};
