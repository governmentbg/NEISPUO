import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddDeletedOnToSysUser1639385293344 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            'ALTER TABLE core.SysUser ADD DeletedOn DATETIME2 CONSTRAINT DeletedOnSysUserDefault DEFAULT NULL',
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('ALTER TABLE core.SysUser DROP CONSTRAINT DeletedOnSysUserDefault', undefined);
        await queryRunner.query('ALTER TABLE core.SysUser DROP COLUMN DeletedOn', undefined);
    }
}
