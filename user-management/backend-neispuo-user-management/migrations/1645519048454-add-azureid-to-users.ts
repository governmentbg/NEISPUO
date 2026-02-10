import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddAzureIdToUsers1645519048454 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`ALTER TABLE core.Person ADD AzureID VARCHAR(100) NULL`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.Users ADD AzureID VARCHAR(100) NULL`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.UsersArchived ADD AzureID VARCHAR(100) NULL`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.Organizations ADD AzureID VARCHAR(100) NULL`, undefined);
        await queryRunner.query(
            `ALTER TABLE azure_temp.OrganizationsArchived ADD AzureID VARCHAR(100) NULL`,
            undefined,
        );
    }

    /* AFTER GARGOV GETS THINGS DONE SEE IF ENROLLMENTS SHOULD BE REFACTORED ASWELL*/
    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`ALTER TABLE core.Person DROP COLUMN AzureID;`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.Users DROP COLUMN AzureID;`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.UsersArchived DROP COLUMN AzureID;`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.Organizations DROP COLUMN AzureID;`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.OrganizationsArchived DROP COLUMN AzureID;`, undefined);
    }
}
