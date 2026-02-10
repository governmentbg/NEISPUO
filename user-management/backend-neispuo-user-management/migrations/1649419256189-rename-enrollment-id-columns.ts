import { MigrationInterface, QueryRunner } from 'typeorm';

export class RenameEnrollmentIDColumns1649419256189 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`EXEC sp_rename 'azure_temp.Enrollments.UserID', 'UserAzureID', 'COLUMN';`, undefined);
        await queryRunner.query(
            `EXEC sp_rename 'azure_temp.Enrollments.OrganizationID', 'OrganizationAzureID', 'COLUMN';`,
            undefined,
        );
        await queryRunner.query(
            `EXEC sp_rename 'azure_temp.Enrollments.ClassID', 'ClassAzureID', 'COLUMN';`,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`EXEC sp_rename 'azure_temp.Enrollments.UserAzureID', 'UserID', 'COLUMN';`, undefined);
        await queryRunner.query(
            `EXEC sp_rename 'azure_temp.Enrollments.OrganizationAzureID', 'OrganizationID', 'COLUMN';`,
            undefined,
        );
        await queryRunner.query(
            `EXEC sp_rename 'azure_temp.Enrollments.ClassAzureID', 'ClassID', 'COLUMN';`,
            undefined,
        );
    }
}
