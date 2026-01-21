import { Column, Entity, Index, PrimaryGeneratedColumn } from 'typeorm';
import { SyncStatusEnum } from 'src/shared/enums/sync-status.enum';

@Entity({ schema: 'reporting', name: 'SynchronizationLog' })
@Index(['sourceName', 'targetName'], { unique: true })
export class SynchronizationLog {
  @PrimaryGeneratedColumn({ name: 'SynchronizationLogID' })
  SynchronizationLogID: number;

  @Column({
    name: 'SourceName',
    type: 'nvarchar',
    length: 255,
  })
  sourceName: string;

  @Column({
    name: 'TargetName',
    type: 'nvarchar',
    length: 255,
  })
  targetName: string;

  @Column({
    name: 'Status',
    type: 'nvarchar',
    length: 50,
    default: SyncStatusEnum.IN_PROGRESS,
  })
  status: SyncStatusEnum;

  @Column({
    name: 'LastSyncStartedAt',
    type: 'datetime2',
    nullable: true,
  })
  lastSyncStartedAt: Date;

  @Column({
    name: 'LastSyncSucceededAt',
    type: 'datetime2',
    nullable: true,
  })
  lastSyncSucceededAt: Date;

  @Column({
    name: 'Error',
    type: 'nvarchar',
    length: 'max',
    nullable: true,
  })
  error: string;
}
