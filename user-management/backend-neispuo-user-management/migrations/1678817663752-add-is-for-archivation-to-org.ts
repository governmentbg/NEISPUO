import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddIsForArchivationToOrg1678817663752 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> { 
        await queryRunner.query(
            `
            DELETE 
                FROM 
                    azure_temp.CronJobConfig 
                WHERE 
                    Name IN ( 
                        'restart-failed-workflows',
                        'restart-failed-workflows2'
                    )
            `,
            undefined,
        );
        
    }

    public async down(queryRunner: QueryRunner): Promise<void> { 
        await queryRunner.query(
            `
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'restart-failed-workflows', N'*/30 * * * * *', 0, 1, 0);
            `,
            undefined,
        );
        await queryRunner.query(
            `
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'restart-failed-workflows2', N'*/30 * * * * *', 0, 1, 0);
            `,
            undefined,
        );
    }
}
