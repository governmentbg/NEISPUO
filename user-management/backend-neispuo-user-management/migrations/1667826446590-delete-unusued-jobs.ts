import { MigrationInterface, QueryRunner } from 'typeorm';

export class DeleteUnusedJobs1667826446590 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`
        DELETE FROM azure_temp.CronJobConfig
        WHERE name = 'azure-enrollments-create' OR name = 'azure-users-delete-graduated-students';
        `);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {}
}
