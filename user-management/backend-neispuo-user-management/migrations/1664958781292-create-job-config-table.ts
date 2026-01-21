import { MigrationInterface, QueryRunner } from 'typeorm';

export class CreateJobConfigTable1664958781292 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            CREATE TABLE azure_temp.CronJobConfig (
                JobID int IDENTITY(1,1) NOT NULL,
                Name varchar(255) NULL,
                Cron varchar(255) NULL,
                IsRunning int NULL,
                IsActive int NULL,
                MarkedForRestart int NULL
            );
            `,
            undefined,
        );
        await queryRunner.query(
            `
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'azure-classes-check', N'*/10 * * * * *', 0, 1, 0);
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'azure-classes-create', N'*/10 * * * * *', 0, 1, 0);
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'azure-classes-revert', N'*/5 * * * * *', 0, 1, 0);
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'azure-enrollments-check', N'*/10 * * * * *', 0, 1, 0);
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'azure-enrollments-create', N'*/10 * * * * *', 0, 1, 0);
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'azure-enrollments-revert', N'*/5 * * * * *', 0, 1, 0);
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'azure-organizations-check', N'0 */1 * * * *', 0, 1, 0);
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'azure-organizations-create', N'0 */1 * * * *', 0, 1, 0);
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'azure-organizations-revert', N'*/5 * * * * *', 0, 1, 0);
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'azure-users-check', N'*/10 * * * * *', 0, 1, 0);
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'azure-users-create', N'*/10 * * * * *', 0, 1, 0);
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'azure-users-delete-graduated-students', N'0 */30 0-6 * * 6-7', 0, 0, 0);
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'azure-users-revert', N'*/5 * * * * *', 0, 1, 0);
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'daily-run-script-sync-users', N'0 15 * * * *', 0, 1, 0);
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'error-email-notification', N'0 0 0 * * 5', 0, 1, 0);
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'sync-classes', N'*/10 * * * * *', 0, 1, 0);
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'sync-institutions', N'0 */1 * * * *', 0, 1, 0);
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'sync-users', N'*/10 * * * * *', 0, 1, 0);
            `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            DROP TABLE azure_temp.CronJobConfig;
            `,
            undefined,
        );
    }
}
