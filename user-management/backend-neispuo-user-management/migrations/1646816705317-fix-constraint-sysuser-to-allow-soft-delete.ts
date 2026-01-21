import { MigrationInterface, QueryRunner } from 'typeorm';

export class FixConstraintSysUserToAllowSoftDelete1646816705317 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`ALTER TABLE [core].[SysUser] DROP CONSTRAINT OneToOneSysUserPerson`, undefined);
        await queryRunner.query(`SET ANSI_PADDING ON`, undefined);
        await queryRunner.query(`ALTER TABLE [core].[SysUser] DROP CONSTRAINT UQ_SysUser_Username`, undefined);
        await queryRunner.query(
            `CREATE UNIQUE INDEX uqOneToOneSysUserPersonByCondition ON [core].[SysUser](PersonID) WHERE DeletedOn IS NULL`,
            undefined,
        );
        await queryRunner.query(
            `CREATE UNIQUE INDEX uqSysUserUsername ON [core].[SysUser](UserName) where DeletedOn IS NULL`,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`DROP INDEX uqOneToOneSysUserPersonByCondition ON [core].[SysUser]`, undefined);
        await queryRunner.query(`DROP INDEX uqSysUserUsername ON [core].[SysUser]`, undefined);
        await queryRunner.query(
            `ALTER TABLE [core].[SysUser] ADD CONSTRAINT UQ_SysUser_Username UNIQUE(Username)`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE [core].[SysUser] ADD CONSTRAINT OneToOneSysUserPerson UNIQUE(PersonID)`,
            undefined,
        );
        await queryRunner.query(`SET ANSI_PADDING OFF`, undefined);
    }
}
