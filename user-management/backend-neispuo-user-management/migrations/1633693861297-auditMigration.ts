import { MigrationInterface, QueryRunner } from 'typeorm';

// eslint-disable-next-line @typescript-eslint/naming-convention
export class auditMigration1633693861297 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('CREATE SCHEMA audit;', undefined);

        await queryRunner.query(
            `CREATE TABLE [audit].[RoleAssignment](
                [RoleAssignmentID] [int] NOT NULL IDENTITY(1,1) ,
                [AssignedTo] [int] NOT NULL,
                [CreatedBy] [int] NULL,
                [SysRole] [int] NOT NULL,
                [CreatedOn] [datetime] NOT NULL,
                [Action] [nvarchar](256) NOT NULL,
                [InstitutionID] [int] NULL,
             CONSTRAINT [PK_RoleAssignment] PRIMARY KEY CLUSTERED 
            (
                [RoleAssignmentID] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
            ) ON [PRIMARY]`,
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

    public async down(queryRunner: QueryRunner): Promise<void> {
        //@TODO i have a feeling this will not work!!!! Need to test
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

        await queryRunner.query('DROP TABLE [audit].[RoleAssignment];', undefined);

        await queryRunner.query('DROP SCHEMA audit;', undefined);
    }
}
