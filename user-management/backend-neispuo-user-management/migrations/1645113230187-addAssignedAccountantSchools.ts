import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddAssignedAccountantSchools1645113230187 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `ALTER TABLE azure_temp.Users ADD AssignedAccountantSchools nvarchar(255) CONSTRAINT AzureUsersAssignedAccountantSchoolsDefault DEFAULT NULL`,
            undefined,
        );

        await queryRunner.query(
            `ALTER TABLE azure_temp.UsersArchived ADD AssignedAccountantSchools nvarchar(255) CONSTRAINT AzureUsersHistoryAssignedAccountantSchoolsDefault  DEFAULT NULL`,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `ALTER TABLE azure_temp.Users DROP CONSTRAINT AzureUsersAssignedAccountantSchoolsDefault`,
            undefined,
        );
        await queryRunner.query(`ALTER TABLE azure_temp.Users DROP COLUMN AssignedAccountantSchools`, undefined);
        await queryRunner.query(
            `ALTER TABLE azure_temp.UsersArchived DROP CONSTRAINT AzureUsersHistoryAssignedAccountantSchoolsDefault`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE azure_temp.UsersArchived DROP COLUMN AssignedAccountantSchools`,
            undefined,
        );
    }
}
