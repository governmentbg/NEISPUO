import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddEnrollments2Crons1673819184247 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'azure-enrollments-archive2', N'0 */5 * * * *', 0, 1, 0);
                `,
        );
        await queryRunner.query(
            `  
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'azure-enrollments-revert2', N'*/10 * * * * *', 0, 1, 0);
            `,
        );
        await queryRunner.query(
            `  
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'azure-enrollments-student-to-class-create2', N'*/5 * * * * *', 0, 1, 0);
            `,
        );
        await queryRunner.query(
            `  
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'azure-enrollments-teacher-to-class-create2', N'*/20 * * * * *', 0, 1, 0);
            `,
        );
        await queryRunner.query(
            `  
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'azure-enrollments-to-school-create2', N'*/1 * * * * *', 0, 1, 0);
            `,
        );
        await queryRunner.query(
            `  
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'azure-enrollments-to-school-check2', N'*/10 * * * * *', 0, 1, 0);
            `,
        );
        await queryRunner.query(
            `  
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'azure-enrollments-to-class-check2', N'*/10 * * * * *', 0, 1, 0);
            `,
        );
        await queryRunner.query(
            `  
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'restart-failed-workflows2', N'*/30 * * * * *', 0, 1, 0);
            `,
        );
        await queryRunner.query(
            `  
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'archive-failed-workflows2', N'0 * * * * *', 0, 1, 0);
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
                        'azure-enrollments-archive2',
                        'azure-enrollments-revert2',
                        'azure-enrollments-student-to-class-create2',
                        'azure-enrollments-teacher-to-class-create2',
                        'azure-enrollments-to-school-create2',
                        'azure-enrollments-to-school-check2',
                        'azure-enrollments-to-class-check2',
                        'restart-failed-workflows2',
                        'archive-failed-workflows2',
                    )
            `,
            undefined,
        );
    }
}
