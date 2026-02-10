import { MigrationInterface, QueryRunner } from 'typeorm';

export class TurnOnOrganizationsArchive1673264630052 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
                UPDATE azure_temp.CronJobConfig
                SET IsActive=1
                WHERE Name=N'azure-organizations-archive'
            `,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
                UPDATE azure_temp.CronJobConfig
                SET IsActive=0
                WHERE Name=N'azure-organizations-archive'
            `,
        );
    }
}
