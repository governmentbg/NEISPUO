import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddTemporaryColumnSysUserType1656591024677 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        /**
         * Add temporary column to annotate if the user is External/Internal
         * External (created outside of NESIPUO) = 0,
         * Institution = 1
         * Student/Teacher = 2
         *
         * This is required in order to address invalid records in the Person table
         */
        await queryRunner.query('ALTER TABLE core.Person ADD SysUserType INT DEFAULT NULL;', undefined);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('ALTER TABLE core.Person DROP COLUMN SysUserType;', undefined);
    }
}
