import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddRowIDToClasses1649166798934 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('ALTER TABLE azure_temp.Enrollments ADD OrganizationRowID INT NULL', undefined);
        await queryRunner.query('ALTER TABLE azure_temp.Enrollments ADD UserRowID INT NULL', undefined);
        await queryRunner.query('ALTER TABLE azure_temp.Enrollments ADD ClassRowID INT NULL', undefined);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('ALTER TABLE azure_temp.Enrollments DROP COLUMN OrganizationRowID', undefined);
        await queryRunner.query('ALTER TABLE azure_temp.Enrollments DROP COLUMN UserRowID', undefined);
        await queryRunner.query('ALTER TABLE azure_temp.Enrollments DROP COLUMN ClassRowID', undefined);
    }
}
