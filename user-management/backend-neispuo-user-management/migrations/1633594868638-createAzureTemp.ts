import { MigrationInterface, QueryRunner } from 'typeorm';

// eslint-disable-next-line @typescript-eslint/naming-convention
export class createAzureTemp1633594868638 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            'ALTER TABLE core.SysUser ADD isAzureSynced INT NOT NULL CONSTRAINT isAzureSyncedNotNull DEFAULT 0 ',
            undefined,
        );

        await queryRunner.query('CREATE SCHEMA azure_temp;', undefined);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('ALTER TABLE core.SysUser DROP CONSTRAINT isAzureSyncedNotNull', undefined);
        await queryRunner.query(`ALTER TABLE core.SysUser DROP COLUMN isAzureSynced;`, undefined);
        await queryRunner.query('DROP SCHEMA azure_temp;', undefined);
    }
}
