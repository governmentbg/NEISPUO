import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddIndexesCronJobConfig1667826446595 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`
        CREATE UNIQUE INDEX UQ_IX_Name ON azure_temp.CronJobConfig (Name);`);

        await queryRunner.query(`
        CREATE UNIQUE INDEX UQ_IX_JobID ON azure_temp.CronJobConfig (JobID);`);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {}
}
