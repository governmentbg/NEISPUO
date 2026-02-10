import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddNewCronConfig1668175127661 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'azure-organizations-revert', N'*/5 * * * * *', 0, 1, 0);
            `,
        );
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'daily-run-script-sync-schools', N'0 30 * * * *', 0, 1, 0);
                `,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {}
}
