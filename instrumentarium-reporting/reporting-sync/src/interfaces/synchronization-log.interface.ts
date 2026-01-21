import { SyncStatusEnum } from 'src/enums/sync-status.enum';

export interface SynchronizationLog {
  SynchronizationLogID: number;
  sourceName: string;
  targetName: string;
  status: SyncStatusEnum;
  lastSyncStartedAt: Date;
  lastSyncSucceededAt?: Date;
  error?: string;
}
