import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddPersonIdToAzureUsers1644222040394 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`ALTER TABLE azure_temp.Users ADD PersonID INT NULL`, undefined);

        await queryRunner.query(`ALTER TABLE azure_temp.UsersArchived ADD PersonID INT NULL`, undefined);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`ALTER TABLE azure_temp.Users DROP COLUMN PersonID`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.UsersArchived DROP COLUMN PersonID`, undefined);
    }
}
