import { MigrationInterface, QueryRunner } from 'typeorm';

export class AzureOrganizationInitialPass1637249120756 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            'ALTER TABLE azure_temp.Organizations ADD Password nvarchar(255) CONSTRAINT PasswordOrganizationDefault DEFAULT 0',
            undefined,
        );

        await queryRunner.query(
            'ALTER TABLE azure_temp.Organizations_History ADD Password nvarchar(255) CONSTRAINT PasswordOrganizationHistoryDefault  DEFAULT 0',
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            'ALTER TABLE azure_temp.Organizations DROP CONSTRAINT PasswordOrganizationDefault',
            undefined,
        );
        await queryRunner.query('ALTER TABLE azure_temp.Organizations DROP COLUMN Password', undefined);
        await queryRunner.query(
            'ALTER TABLE azure_temp.Organizations_History DROP CONSTRAINT PasswordOrganizationHistoryDefault',
            undefined,
        );
        await queryRunner.query('ALTER TABLE azure_temp.Organizations_History DROP COLUMN Password', undefined);
    }
}
