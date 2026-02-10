import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddNewArchiveCron1672925019463 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'archive-failed-workflows', N'0 * * * * *', 0, 1, 0);
            `,
        );
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'archive-not-started-workflows', N'0 * * * * *', 0, 1, 0);
                `,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            DELETE 
                FROM 
                    azure_temp.CronJobConfig 
                WHERE 
                    Name IN (
                        'archive-failed-workflows',
                        'archive-not-started-workflows'
                    )
            `,
            undefined,
        );
    }
}
