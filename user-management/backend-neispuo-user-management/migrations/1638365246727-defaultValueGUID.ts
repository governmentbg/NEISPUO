import { MigrationInterface, QueryRunner } from 'typeorm';

// eslint-disable-next-line @typescript-eslint/naming-convention
export class defaultValueGUID1638365246727 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `ALTER TABLE azure_temp.Users ADD CONSTRAINT DEFAULT_VALUE_GUID_USER DEFAULT newid() FOR GUID`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE azure_temp.Enrollments ADD CONSTRAINT DEFAULT_VALUE_GUID_ENROLLMENTS DEFAULT newid() FOR GUID`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE azure_temp.Classes ADD CONSTRAINT DEFAULT_VALUE_GUID_CLASSES DEFAULT newid() FOR GUID`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE azure_temp.Organizations ADD CONSTRAINT DEFAULT_VALUE_GUID_ORGANIZATIONS DEFAULT newid() FOR GUID`,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('ALTER TABLE azure_temp.Users DROP CONSTRAINT DEFAULT_VALUE_GUID_USER', undefined);
        await queryRunner.query(
            'ALTER TABLE azure_temp.Enrollments DROP CONSTRAINT DEFAULT_VALUE_GUID_ENROLLMENTS',
            undefined,
        );
        await queryRunner.query('ALTER TABLE azure_temp.Classes DROP CONSTRAINT DEFAULT_VALUE_GUID_CLASSES', undefined);
        await queryRunner.query(
            'ALTER TABLE azure_temp.Organizations DROP CONSTRAINT DEFAULT_VALUE_GUID_ORGANIZATIONS',
            undefined,
        );
    }
}
