import { MigrationInterface, QueryRunner } from 'typeorm';

export class RefactorRoleAudit1636362617937 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `ALTER TABLE audit.RoleAssignment DROP CONSTRAINT FK_RoleAssignment_SysUser1;`,
            undefined,
        );

        await queryRunner.query(
            `ALTER TABLE audit.RoleAssignment DROP CONSTRAINT FK_RoleAssignment_SysUser;`,
            undefined,
        );

        await queryRunner.query(
            `ALTER TABLE audit.RoleAssignment DROP CONSTRAINT FK_RoleAssignment_SysRole;`,
            undefined,
        );

        await queryRunner.query(
            `ALTER TABLE audit.RoleAssignment DROP CONSTRAINT FK_RoleAssignment_Institution;`,
            undefined,
        );

        await queryRunner.query(
            `EXEC sp_RENAME 'audit.RoleAssignment.CreatedBy', 'CreatedBySysUserID', 'COLUMN';`,
            undefined,
        );

        await queryRunner.query(
            `EXEC sp_RENAME 'audit.RoleAssignment.AssignedTo', 'AssignedToSysUserID', 'COLUMN';`,
            undefined,
        );

        await queryRunner.query(`EXEC sp_RENAME 'audit.RoleAssignment.SysRole', 'SysRoleID', 'COLUMN';`, undefined);

        await queryRunner.query(
            `
        ALTER TABLE audit.RoleAssignment 
        ADD 
            AssignedToSysUsername [nvarchar](256) NULL,
            CreatedBySysUsername [nvarchar](256) NULL,
            SysRoleName [nvarchar](256) NULL;
      `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
      ALTER TABLE audit.RoleAssignment DROP COLUMN SysRoleName;
      `,
            undefined,
        );
        await queryRunner.query(
            `
      ALTER TABLE audit.RoleAssignment DROP COLUMN CreatedBySysUsername;
      `,
            undefined,
        );
        await queryRunner.query(
            `
      ALTER TABLE audit.RoleAssignment DROP COLUMN AssignedToSysUsername;
      `,
            undefined,
        );

        await queryRunner.query(`EXEC sp_RENAME 'audit.RoleAssignment.SysRoleID', 'SysRole', 'COLUMN';`, undefined);

        await queryRunner.query(
            `EXEC sp_RENAME 'audit.RoleAssignment.AssignedToSysUserID', 'AssignedTo', 'COLUMN';`,
            undefined,
        );

        await queryRunner.query(
            `EXEC sp_RENAME 'audit.RoleAssignment.CreatedBySysUserID', 'CreatedBy', 'COLUMN';`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE [audit].[RoleAssignment]  WITH CHECK ADD  CONSTRAINT [FK_RoleAssignment_Institution] FOREIGN KEY([InstitutionID])
            REFERENCES [core].[Institution] ([InstitutionID])`,
            undefined,
        );

        await queryRunner.query(
            `ALTER TABLE [audit].[RoleAssignment] CHECK CONSTRAINT [FK_RoleAssignment_Institution]`,
            undefined,
        );

        await queryRunner.query(
            `ALTER TABLE [audit].[RoleAssignment]  WITH CHECK ADD  CONSTRAINT [FK_RoleAssignment_SysRole] FOREIGN KEY([SysRole])
            REFERENCES [core].[SysRole] ([SysRoleID])`,
            undefined,
        );

        await queryRunner.query(
            `ALTER TABLE [audit].[RoleAssignment] CHECK CONSTRAINT [FK_RoleAssignment_SysRole]`,
            undefined,
        );

        await queryRunner.query(
            `ALTER TABLE [audit].[RoleAssignment]  WITH CHECK ADD  CONSTRAINT [FK_RoleAssignment_SysUser] FOREIGN KEY([AssignedTo])
            REFERENCES [core].[SysUser] ([SysUserID])`,
            undefined,
        );

        await queryRunner.query(
            `ALTER TABLE [audit].[RoleAssignment] CHECK CONSTRAINT [FK_RoleAssignment_SysUser]`,
            undefined,
        );

        await queryRunner.query(
            `ALTER TABLE [audit].[RoleAssignment]  WITH CHECK ADD  CONSTRAINT [FK_RoleAssignment_SysUser1] FOREIGN KEY([AssignedTo])
            REFERENCES [core].[SysUser] ([SysUserID])`,
            undefined,
        );

        await queryRunner.query(
            `ALTER TABLE [audit].[RoleAssignment] CHECK CONSTRAINT [FK_RoleAssignment_SysUser1]`,
            undefined,
        );
    }
}
