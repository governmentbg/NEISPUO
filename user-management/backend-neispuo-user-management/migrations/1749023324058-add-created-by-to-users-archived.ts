import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddCreatedByToUsersArchived1749023369835 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('ALTER TABLE azure_temp.UsersArchived ADD CreatedBy varchar(255) DEFAULT NULL');
        await queryRunner.query(
            'ALTER TABLE azure_temp.UsersArchivedPreviousYears ADD CreatedBy varchar(255) DEFAULT NULL',
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('ALTER TABLE azure_temp.UsersArchived DROP COLUMN CreatedBy');
        await queryRunner.query('ALTER TABLE azure_temp.UsersArchivedPreviousYears DROP COLUMN CreatedBy');
    }
}
