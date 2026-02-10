import { MigrationInterface, QueryRunner } from 'typeorm';

export class RenameEnrollmentArchiveIDColumns1649419256189 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `EXEC sp_rename 'azure_temp.EnrollmentsArchived.UserID', 'UserAzureID', 'COLUMN';`,
            undefined,
        );
        await queryRunner.query(
            `EXEC sp_rename 'azure_temp.EnrollmentsArchived.OrganizationID', 'OrganizationAzureID', 'COLUMN';`,
            undefined,
        );
        await queryRunner.query(
            `EXEC sp_rename 'azure_temp.EnrollmentsArchived.ClassID', 'ClassAzureID', 'COLUMN';`,
            undefined,
        );
        await queryRunner.query('ALTER TABLE azure_temp.EnrollmentsArchived ADD OrganizationRowID INT NULL', undefined);
        await queryRunner.query('ALTER TABLE azure_temp.EnrollmentsArchived ADD UserRowID INT NULL', undefined);
        await queryRunner.query('ALTER TABLE azure_temp.EnrollmentsArchived ADD ClassRowID INT NULL', undefined);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `EXEC sp_rename 'azure_temp.EnrollmentsArchived.UserAzureID', 'UserID', 'COLUMN';`,
            undefined,
        );
        await queryRunner.query(
            `EXEC sp_rename 'azure_temp.EnrollmentsArchived.OrganizationAzureID', 'OrganizationID', 'COLUMN';`,
            undefined,
        );
        await queryRunner.query(
            `EXEC sp_rename 'azure_temp.EnrollmentsArchived.ClassAzureID', 'ClassID', 'COLUMN';`,
            undefined,
        );
        await queryRunner.query('ALTER TABLE azure_temp.EnrollmentsArchived DROP COLUMN OrganizationRowID', undefined);
        await queryRunner.query('ALTER TABLE azure_temp.EnrollmentsArchived DROP COLUMN UserRowID', undefined);
        await queryRunner.query('ALTER TABLE azure_temp.EnrollmentsArchived DROP COLUMN ClassRowID', undefined);
    }
}
