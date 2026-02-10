import { MigrationInterface, QueryRunner } from 'typeorm';

export class EmptyMigration1634632901693 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('ALTER TABLE core.Person ADD SchoolBooksCodesID VARCHAR(10) NULL', undefined);
        await queryRunner.query(
            'ALTER TABLE core.RelativeChild ADD HasAccess INT CONSTRAINT HasAccessDefaultConstraint  DEFAULT 0',
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('ALTER TABLE core.Person DROP COLUMN SchoolBooksCodesID', undefined);
        await queryRunner.query('ALTER TABLE core.RelativeChild DROP CONSTRAINT HasAccessDefaultConstraint', undefined);
        await queryRunner.query('ALTER TABLE core.RelativeChild DROP COLUMN HasAccess', undefined);
    }
}
