import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddNeispuoAccess1642758500258 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `ALTER TABLE azure_temp.Users ADD HasNeispuoAccess INT CONSTRAINT AzureUsersHasNeispuoAccessDefault DEFAULT NULL`,
            undefined,
        );

        await queryRunner.query(
            `ALTER TABLE azure_temp.UsersArchived ADD HasNeispuoAccess INT CONSTRAINT AzureUsersHistoryHasNeispuoAccessDefault  DEFAULT NULL`,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `ALTER TABLE azure_temp.Users DROP CONSTRAINT AzureUsersHasNeispuoAccessDefault`,
            undefined,
        );
        await queryRunner.query(`ALTER TABLE azure_temp.Users DROP COLUMN HasNeispuoAccess`, undefined);
        await queryRunner.query(
            `ALTER TABLE azure_temp.UsersArchived DROP CONSTRAINT AzureUsersHistoryHasNeispuoAccessDefault`,
            undefined,
        );
        await queryRunner.query(`ALTER TABLE azure_temp.UsersArchived DROP COLUMN HasNeispuoAccess`, undefined);
    }
}
