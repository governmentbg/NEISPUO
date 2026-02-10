import { CronJob } from '../interfaces';
import { executeSyncJob } from '../services/cron.service';
import { logSyncError, logSyncResult, syncViewToTable } from '../services/sync.service';
import { SYNC_CONFIGS } from '../constants/sync-config-registry';
import { SCHEDULE_CONFIG } from '../constants/schedule.config';

export const syncRegularFamilyReasonAbsencesJob: CronJob = {
  name: 'sync-regular-family-reason-absences',
  schedule: SCHEDULE_CONFIG.syncRegularFamilyReasonAbsences,
  enabled: true,
  description: 'Sync reporting.R_Regular_Family_Reason_Absences view to ClickHouse table R_Regular_Family_Reason_Absences',
  task: async () => {
    const config = SYNC_CONFIGS.regularFamilyReasonAbsences;
    await executeSyncJob(
      async () => {
        const result = await syncViewToTable(config);
        logSyncResult(result);
      },
      (err) => logSyncError(syncRegularFamilyReasonAbsencesJob.name, err),
    );
  },
};


