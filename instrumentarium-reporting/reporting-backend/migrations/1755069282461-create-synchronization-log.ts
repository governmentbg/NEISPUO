import { MigrationInterface, QueryRunner } from 'typeorm';

export class CreateSynchronizationLog1755069282461
  implements MigrationInterface
{
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
      CREATE TABLE reporting.SynchronizationLog (
        SynchronizationLogID INT NOT NULL IDENTITY(1,1),
        SourceName NVARCHAR(255) NOT NULL,
        TargetName NVARCHAR(255) NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'IN_PROGRESS',
        LastSyncStartedAt DATETIME2 NULL,
        LastSyncSucceededAt DATETIME2 NULL,
        Error NVARCHAR(MAX) NULL,
        CONSTRAINT PK_SynchronizationLog_SynchronizationLogID PRIMARY KEY (SynchronizationLogID),
        CONSTRAINT UQ_SynchronizationLog_SourceName_TargetName UNIQUE (SourceName, TargetName),
        CONSTRAINT CK_SynchronizationLog_Status CHECK (Status IN ('SYNCHRONIZED', 'IN_PROGRESS', 'FAILED'))
      )
    `);
  }

  public async down(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`DROP TABLE reporting.SynchronizationLog`);
  }
}
