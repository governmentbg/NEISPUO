import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddJobsEnrollments1667826446582 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`
        INSERT INTO azure_temp.CronJobConfig
        (Name, Cron, IsRunning, IsActive, MarkedForRestart)
        VALUES( N'azure-enrollments-to-school-check', N'*/10 * * * * *', 0, 1, 0),
        ( N'azure-enrollments-to-class-check', N'*/10 * * * * *', 0, 1, 0),
        ( N'azure-enrollments-to-class-create', N'*/10 * * * * *', 0, 1, 0),
        ( N'azure-enrollments-to-school-create', N'*/10 * * * * *', 0, 1, 0);
        `);

        await queryRunner.query(`
        DELETE FROM azure_temp.CronJobConfig
        WHERE name = 'azure-organizations-revert';
        `);

        await queryRunner.query(`
        DELETE FROM azure_temp.CronJobConfig
        WHERE name = 'azure-enrollments-check';
        `);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {}
}
