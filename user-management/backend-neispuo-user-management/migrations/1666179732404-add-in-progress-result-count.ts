import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddInProgressResultCount1666179732404 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `ALTER TABLE azure_temp.Classes ADD InProgressResultCount INT CONSTRAINT AzureTempCInPResCountDefault DEFAULT 0 WITH VALUES`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE azure_temp.Users ADD InProgressResultCount INT CONSTRAINT AzureTempUInPResCountDefault DEFAULT 0 WITH VALUES`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE azure_temp.Enrollments ADD InProgressResultCount INT CONSTRAINT AzureTempEInPResCountDefault DEFAULT 0 WITH VALUES`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE azure_temp.Organizations ADD InProgressResultCount INT CONSTRAINT AzureTempOInPResCountDefault DEFAULT 0 WITH VALUES`,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `ALTER TABLE azure_temp.Classes DROP CONSTRAINT AzureTempCInPResCountDefault`,
            undefined,
        );
        await queryRunner.query(`ALTER TABLE azure_temp.Users DROP CONSTRAINT AzureTempUInPResCountDefault`, undefined);
        await queryRunner.query(
            `ALTER TABLE azure_temp.Enrollments DROP CONSTRAINT AzureTempEInPResCountDefault`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE azure_temp.Organizations DROP CONSTRAINT AzureTempOInPResCountDefault`,
            undefined,
        );
        await queryRunner.query(`ALTER TABLE azure_temp.Classes DROP COLUMN InProgressResultCount;`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.Users DROP COLUMN InProgressResultCount;`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.Enrollments DROP COLUMN InProgressResultCount;`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.Organizations DROP COLUMN InProgressResultCount;`, undefined);
    }
}
