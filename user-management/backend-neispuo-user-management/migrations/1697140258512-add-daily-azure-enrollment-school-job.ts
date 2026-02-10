import {MigrationInterface, QueryRunner} from "typeorm";

export class AddDailySyncAzureSchoolEnrollments1697140258512 implements MigrationInterface {

    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'daily-run-script-sync-school-enrollments', N'0 30 0-1 * * *', 0, 1, 0);
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
                        'daily-run-script-sync-school-enrollments'
                    )
            `,
            undefined,
        );
    }

}
